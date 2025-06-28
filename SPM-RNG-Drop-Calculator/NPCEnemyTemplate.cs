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
        public s32 ItemId;
        public s32 Chance;

        public DropItem(s32 itemId, s32 chance)
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
        public static void InitializeEnemies(string ramdumpPath, u32 p_npcTribes, u32 p_npcEnemyTemplates)
        {
            p_npcEnemyTemplates &= 0xffffff;
            p_npcTribes &= 0xffffff;

            Enemies = new List<Enemy>();

            const s32 NPCTRIBE_SIZE = 0x68;
            const s32 NPCTRIBE_DROPITEM_CHANCE = 0x4e;
            const s32 NPCTRIBE_DROPITEM_LIST_PTR = 0x50;

            const s32 NB_NPC_ENEMY_TEMPLATES = 434;
            const s32 NPCENEMYTEMPLATE_SIZE = 0x68;
            const s32 NPCENEMYTEMPLATE_TRIBE_ID = 0x14;

            const s32 DROPITEM_SIZE = 0x8;

            u8[] ramdump = File.ReadAllBytes(ramdumpPath);

            for (s32 i = 0; i < NB_NPC_ENEMY_TEMPLATES; i++)
            {
                s32 tribeId = Utils.ReadS32(ramdump, (s32)(p_npcEnemyTemplates + i * NPCENEMYTEMPLATE_SIZE + NPCENEMYTEMPLATE_TRIBE_ID));
                s32 dropItemChance = Utils.ReadS16(ramdump, (s32)(p_npcTribes + tribeId * NPCTRIBE_SIZE + NPCTRIBE_DROPITEM_CHANCE));
                s32 p_dropItemList = Utils.ReadS32(ramdump, (s32)(p_npcTribes + tribeId * NPCTRIBE_SIZE + NPCTRIBE_DROPITEM_LIST_PTR)) & 0xffffff;

                List<DropItem> dropItemList = new List<DropItem>();
                s32 itemId = -1;
                s32 chance = -1;

                for (s32 j = 0; itemId != 0; j += DROPITEM_SIZE)
                {
                    itemId = Utils.ReadS32(ramdump, p_dropItemList + j);
                    chance = Utils.ReadS32(ramdump, p_dropItemList + j + 4);
                    dropItemList.Add(new DropItem(itemId, chance));
                }
                Enemies.Add(new Enemy(i, dropItemList, dropItemChance));
            }
        }
        
        public static List<Enemy> EnemiesFromSetupFile(string setupFilePath)
        {
            Debug.Assert(Enemies is not null, "Enemy data list was not initialized.");

            u8[] setupData = File.ReadAllBytes(setupFilePath);

            const s32 SETUP_NB_ENEMIES = 100;
            const s32 SETUP_ENEMY_LIST_OFFSET = 0x4;
            const s32 SETUPENEMY_SIZE = 0x70;
            const s32 SETUPENEMY_ID_OFFSET = 0xC;

            List<Enemy> enemies = new List<Enemy>();

            for (s32 i = 0; i < SETUP_NB_ENEMIES; i++)
            {
                s32 templateId = Utils.ReadS32(setupData, SETUP_ENEMY_LIST_OFFSET + i * SETUPENEMY_SIZE + SETUPENEMY_ID_OFFSET);
                if (templateId == 0) break; // Technically template id 0 is a real template but 1- it's unused and 2- this is literally how the game knows the list is over so ratio

                enemies.Add(Enemies[templateId]);
            }

            return enemies;
        }

        public s32 TemplateId { get; set; }
        public List<DropItem> DropItemList { get; } = new List<DropItem>();
        public s32 DropItemChance { get; set; }

        public Enemy(s32 templateId, IEnumerable<DropItem> dropItemList, s32 dropItemChance)
        {
            TemplateId = templateId;
            DropItemList.AddRange(dropItemList);
            DropItemChance = dropItemChance;
        }

        public s32 GetDrop()
        {
            s32 dropItemId = 0;

            if (DropItemChance != 0 && SpmSystem.irand(99) < DropItemChance && DropItemList.Count > 0)
            {
                s32 chanceTotal = 0;
                s32[] idTable = new s32[12];
                s32 index_max = 0;

                for (s32 i = 0; i < 12; i++)
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

                s32 result = SpmSystem.irand(chanceTotal - 1);

                s32 dropId = 0;

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
