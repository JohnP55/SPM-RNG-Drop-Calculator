using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPM_RNG_Drop_Calculator
{
    public struct DropItem : IEquatable<DropItem>
    {
        public int ItemId;
        public int Chance;

        public DropItem(int itemId, int chance)
        {
            ItemId = itemId;
            Chance = chance;
        }

        public bool Equals(DropItem other)
        {
            return ItemId == other.ItemId && Chance == other.Chance;
        }

        public static bool operator==(DropItem p1, DropItem p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator!=(DropItem p1, DropItem p2)
        {
            return !(p1 == p2);
        }

        public override string ToString()
        {
            return $"ItemId: {ItemId}, Chance: {Chance}";
        }
    }
    public class Enemy
    {
        public static List<Enemy>? Enemies { get; private set; }
        public static void InitializeEnemies(string ramdumpPath, uint p_npcTribes, uint p_npcEnemyTemplates)
        {
            p_npcEnemyTemplates &= 0xffffff;
            p_npcTribes &= 0xffffff;

            Enemies = new List<Enemy>();

            const int NPCTRIBE_SIZE = 0x68;
            const int NPCTRIBE_DROPITEM_CHANCE = 0x4e;
            const int NPCTRIBE_DROPITEM_LIST_PTR = 0x50;

            const int NB_NPC_ENEMY_TEMPLATES = 434;
            const int NPCENEMYTEMPLATE_SIZE = 0x68;
            const int NPCENEMYTEMPLATE_TRIBE_ID = 0x14;

            const int DROPITEM_SIZE = 0x8;

            byte[] ramdump = File.ReadAllBytes(ramdumpPath);

            for (int i = 0; i < NB_NPC_ENEMY_TEMPLATES; i++)
            {
                int tribeId = Utils.ReadInt32(ramdump, (int)(p_npcEnemyTemplates + i * NPCENEMYTEMPLATE_SIZE + NPCENEMYTEMPLATE_TRIBE_ID));
                int dropItemChance = Utils.ReadInt16(ramdump, (int)(p_npcTribes + tribeId * NPCTRIBE_SIZE + NPCTRIBE_DROPITEM_CHANCE));
                int p_dropItemList = Utils.ReadInt32(ramdump, (int)(p_npcTribes + tribeId * NPCTRIBE_SIZE + NPCTRIBE_DROPITEM_LIST_PTR)) & 0xffffff;

                List<DropItem> dropItemList = new List<DropItem>();
                int itemId = -1;
                int chance = -1;

                for (int j = 0; itemId != 0; j += DROPITEM_SIZE)
                {
                    itemId = Utils.ReadInt32(ramdump, p_dropItemList + j);
                    chance = Utils.ReadInt32(ramdump, p_dropItemList + j + 4);
                    dropItemList.Add(new DropItem(itemId, chance));
                }
                Enemies.Add(new Enemy(i, dropItemList, dropItemChance));
            }
        }
        
        public static List<Enemy> EnemiesFromSetupFile(string setupFilePath)
        {
            Debug.Assert(Enemies is not null, "Enemy data list was not initialized.");

            byte[] setupData = File.ReadAllBytes(setupFilePath);

            const int SETUP_NB_ENEMIES = 100;
            const int SETUP_ENEMY_LIST_OFFSET = 0x4;
            const int SETUPENEMY_SIZE = 0x70;
            const int SETUPENEMY_ID_OFFSET = 0xC;

            List<Enemy> enemies = new List<Enemy>();

            for (int i = 0; i < SETUP_NB_ENEMIES; i++)
            {
                int templateId = Utils.ReadInt32(setupData, SETUP_ENEMY_LIST_OFFSET + i * SETUPENEMY_SIZE + SETUPENEMY_ID_OFFSET);
                if (templateId == 0) break; // Technically template id 0 is a real template but 1- it's unused and 2- this is literally how the game knows the list is over so ratio

                enemies.Add(Enemies[templateId]);
            }

            return enemies;
        }

        public int TemplateId { get; set; }
        public List<DropItem> DropItemList { get; } = new List<DropItem>();
        public int DropItemChance { get; set; }

        public Enemy(int templateId, IEnumerable<DropItem> dropItemList, int dropItemChance)
        {
            TemplateId = templateId;
            DropItemList.AddRange(dropItemList);
            DropItemChance = dropItemChance;
        }

        public int GetDrop()
        {
            int dropItemId = 0;

            if (DropItemChance != 0 && SpmSystem.irand(99) < DropItemChance && DropItemList.Count > 0)
            {
                int chanceTotal = 0;
                int[] idTable = new int[12];
                int index_max = 0;

                for (int i = 0; i < 12; i++)
                {
                    idTable[i] = DropItemList[i].ItemId;
                    chanceTotal += DropItemList[i].Chance;
                    index_max = i;
                    if (DropItemList[i].ItemId == 0) break;
                }

                if (!(index_max < 12))
                {
                    throw new Exception("index<12: ドロップアイテムリストのリスト数オーバー");
                }

                int result = SpmSystem.irand(chanceTotal - 1);

                int dropId = 0;

                if (index_max > 0)
                {
                    for (dropId = 0; dropId < index_max; dropId++)
                    {
                        result -= DropItemList[dropId].Chance;
                        if (result < 0) break;
                    }
                }
                dropItemId = idTable[dropId];
            }

            return dropItemId;
        }
    }
}
