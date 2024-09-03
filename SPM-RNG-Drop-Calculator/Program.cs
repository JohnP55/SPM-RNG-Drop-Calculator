using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace SPM_RNG_Drop_Calculator
{
    class Program
    {
        enum Item
        {
            Wildcard = -1,
            NULL,
            Fire_Burst = 0x41,
            Shroom_Shake = 0x50,
            Catch_Card = 0x57,
            Hot_Sauce = 0x6d
        }
        static Dictionary<int, string> enemyNames = new Dictionary<int, string>();

        static Dictionary<int, string> items = new Dictionary<int, string>
        {
            { 0x0000, "NULL" },
            { 0x0041, "Fire Burst" },
            { 0x0042, "Ice Storm" },
            { 0x0043, "Thunder Rage" },
            { 0x0044, "Shooting Star" },
            { 0x0045, "POW Block" },
            { 0x0046, "Shell Shock" },
            { 0x0047, "Gold Bar" },
            { 0x0048, "Gold Bar x3" },
            { 0x0049, "Block Block" },
            { 0x004A, "Courage Shell" },
            { 0x004B, "Mighty Tonic" },
            { 0x004C, "Volt Shroom" },
            { 0x004D, "Ghost Shroom" },
            { 0x004E, "Sleepy Sleep" },
            { 0x004F, "Stop Watch" },
            { 0x0050, "Shroom Shake" },
            { 0x0051, "Super Shroom Shake" },
            { 0x0052, "Ultra Shroom Shake" },
            { 0x0053, "Dried Shroom" },
            { 0x0054, "Life Shroom" },
            { 0x0055, "Long-Last Shake" },
            { 0x0056, "Mystery Box" },
            { 0x0057, "Catch Card" },
            { 0x0058, "Catch Card SP" },
            { 0x0059, "HP Plus" },
            { 0x005A, "Power Plus" },
            { 0x005B, "Blue Apple" },
            { 0x005C, "Yellow Apple" },
            { 0x005D, "Red Apple" },
            { 0x005E, "Pink Apple" },
            { 0x005F, "Black Apple" },
            { 0x0060, "Star Medal" },
            { 0x0061, "Gold Medal" },
            { 0x0062, "Poison Shroom" },
            { 0x0063, "Slimy Shroom" },
            { 0x0064, "Peachy Peach" },
            { 0x0065, "Keel Mango" },
            { 0x0066, "Primordial Fruit" },
            { 0x0067, "Golden Leaf" },
            { 0x0068, "Turtley Leaf" },
            { 0x0069, "Cake Mix" },
            { 0x006A, "Whacka Bump" },
            { 0x006B, "Horsetail" },
            { 0x006C, "Fresh Pasta Bunch" },
            { 0x006D, "Hot Sauce" },
            { 0x006E, "Inky Sauce" },
            { 0x006F, "Dayzee Tear" },
            { 0x0070, "Sap Soup" },
            { 0x0071, "Bone-In Cut" },
            { 0x0072, "Fresh Veggie" },
            { 0x0073, "Smelly Herb" },
            { 0x0074, "Honey Jar" },
            { 0x0075, "Power Steak" },
            { 0x0076, "Big Egg" },
            { 0x0077, "Mild Cocoa Bean" },
            { 0x0078, "Sweet Choco-bar" },
            { 0x0079, "Shroom Choco-bar" },
            { 0x007A, "Golden Choco-bar" },
            { 0x007B, "Gradual Syrup" },
            { 0x007C, "Dayzee Syrup" },
            { 0x007D, "Slimy Extract" },
            { 0x007E, "Fried Shroom Plate" },
            { 0x007F, "Roast Shroom Dish" },
            { 0x0080, "Shroom Steak" },
            { 0x0081, "Honey Shroom" },
            { 0x0082, "Honey Super" },
            { 0x0083, "Shroom Broth" },
            { 0x0084, "Mushroom Crepe" },
            { 0x0085, "Shroom Cake" },
            { 0x0086, "Chocolate Cake" },
            { 0x0087, "Heartful Cake" },
            { 0x0088, "Mousse" },
            { 0x0089, "Peach Tart" },
            { 0x008A, "Horsetail Tart" },
            { 0x008B, "Sweet Cookie Snack" },
            { 0x008C, "Koopa Dumpling" },
            { 0x008D, "Sap Muffin" },
            { 0x008E, "Town Special" },
            { 0x008F, "Mango Pudding" },
            { 0x0090, "Love Pudding" },
            { 0x0091, "Couple's Cake" },
            { 0x0092, "Fruit Parfait" },
            { 0x0093, "Snow Cone" },
            { 0x0094, "Snow Bunny" },
            { 0x0095, "Berry Snow Bunny" },
            { 0x0096, "Honey Candy" },
            { 0x0097, "Electro Pop" },
            { 0x0098, "Herb Tea" },
            { 0x0099, "Koopa Tea" },
            { 0x009A, "Spaghetti Plate" },
            { 0x009B, "Spicy Pasta Dish" },
            { 0x009C, "Ink Pasta Dish" },
            { 0x009D, "Koopasta Dish" },
            { 0x009E, "Love Noodle Dish" },
            { 0x009F, "Fried Egg" },
            { 0x00A0, "Egg Bomb" },
            { 0x00A1, "Dyllis Dynamite" },
            { 0x00A2, "Spit Roast" },
            { 0x00A3, "Meteor Meal" },
            { 0x00A4, "Omelette Plate" },
            { 0x00A5, "Spicy Soup" },
            { 0x00A6, "Hot Dog" },
            { 0x00A7, "Healthy Salad" },
            { 0x00A8, "Dyllis Dinner" },
            { 0x00A9, "Dyllis Special" },
            { 0x00AA, "Dyllis Deluxe" },
            { 0x00AB, "Space Food" },
            { 0x00AC, "Emergency Ration" },
            { 0x00AD, "Dangerous Delight" },
            { 0x00AE, "Trial Stew" },
            { 0x00AF, "Mistake" },
            { 0x00B0, "Mistake (Sleepy Sheep)" },
            { 0x00B1, "Warm Cocoa" },
            { 0x00B2, "Odd Dinner" },
            { 0x00B3, "Inky Soup" },
            { 0x00B4, "Gingerbread House" },
            { 0x00B5, "Volcano Shroom" },
            { 0x00B6, "Koopa Pilaf" },
            { 0x00B7, "Spicy Dinner" },
            { 0x00B8, "Shroom Pudding" },
            { 0x00B9, "Heavy Meal" },
            { 0x00BA, "Primordial Dinner" },
            { 0x00BB, "Gorgeous Steak" },
            { 0x00BC, "Golden Meal" },
            { 0x00BD, "Luxurious Set" },
            { 0x00BE, "Roast Whacka Bump" },
            { 0x00BF, "Dyllis Breakfast" },
            { 0x00C0, "Dyllis Lunch" },
            { 0x00C1, "Sky Juice" },
            { 0x00C2, "Stamina Juice" },
            { 0x00C3, "Choco Pasta Dish" },
            { 0x00C4, "Shroom Delicacy" },
            { 0x00C5, "Roast Horsetail" },
            { 0x00C6, "Sap Syrup" },
            { 0x00C7, "Hamburger" },
            { 0x00C8, "Peach Juice" },
            { 0x00C9, "Standard Chocolate" },
            { 0x00CA, "Fruity Cake" },
            { 0x00CB, "Fruity Hamburger" },
            { 0x00CC, "Fruity Punch" },
            { 0x00CD, "Fruity Shroom" },
            { 0x00CE, "Block Meal" },
            { 0x00CF, "Veggie Set" },
            { 0x00D0, "Weird Extract" },
            { 0x00D1, "Awesome Snack" },
            { 0x00D2, "Mango Juice" },
            { 0x00D3, "Meat Pasta Dish" },
            { 0x00D4, "Mixed Shake" },
            { 0x00D5, "Miracle Dinner" },
            { 0x00D6, "Megaton Dinner" },
            { 0x00D7, "Lovely Chocolate" },
        };

        static bool CompareItemLists(List<int> drops, List<int> intendedDrops)
        {
            List<int> copyDrops = drops.ToList();
            foreach(var drop in intendedDrops)
            {
                if (!copyDrops.Contains(drop))
                    return false;
                else
                    copyDrops.Remove(drop);
            }

            return true;
        }

        static void PrintDrops(List<Enemy> enemies)
        {
            Console.WriteLine($"Random Seed: 0x{SpmSystem.randomSeed:x}");

            Console.WriteLine("Output Drops:");
            foreach (var enemy in enemies)
                Console.WriteLine($"{enemy.TemplateId}: {enemy.GetDrop()}");
        }

        static long GetSpecificDrops(List<Enemy> enemies, List<int> intendedDrops)
        {
            List<int> drops = new List<int>(intendedDrops.Count);

            uint backupSeed;
            long increments = -1;
            do
            {
                increments++;
                SpmSystem._rand_advance();

                // Backup current seed
                backupSeed = SpmSystem.randomSeed;

                // Clear Drop List
                drops.Clear();

                foreach (var enemy in enemies)
                {
                    drops.Add(enemy.GetDrop());
                }

                // Restore seed
                SpmSystem.randomSeed = backupSeed;

                if (increments > uint.MaxValue)
                {
                    throw new Exception("Ran out of RNG values");
                }

            } while (!CompareItemLists(drops, intendedDrops));

            return increments;
        }

        static void GetMostDrops(List<Enemy> enemies, int intendedItem)
        {
            List<int> drops = new List<int>(enemies.Count);

            uint backupSeed;
            long increments = -1;

            (uint seed, int count) = (0, 0);
            do
            {
                increments++;
                SpmSystem._rand_advance();

                // Backup current seed
                backupSeed = SpmSystem.randomSeed;

                // Clear Drop List
                drops.Clear();

                foreach (var enemy in enemies)
                {
                    drops.Add(enemy.GetDrop());
                }

                // Restore seed
                SpmSystem.randomSeed = backupSeed;

                Func<int, bool> pred;
                if (intendedItem == (int)Item.Wildcard)
                    // Go absolutely primal
                    pred = x => x != 0;
                else
                    pred = x => x == intendedItem;


                int itemCount = drops.Count(pred);
                if (itemCount > count)
                {
                    seed = SpmSystem.randomSeed;
                    count = itemCount;
                    Console.WriteLine($"New Seed Found: 0x{seed:x} with {count} drops");
                }

            } while (increments <= 0x100000000);

            SpmSystem.randomSeed = seed;
            PrintDrops(enemies);
            Console.WriteLine($"Count: {count}");
        }
        static void DumpEnemyData()
        {
            (string name, string path, uint p_npcTribes, uint p_npcEnemyTemplates)[] defs = new (string, string, uint, uint)[]
            {
                ("PAL", "T:\\spm-tas\\evt-disassembler-master\\pal.raw", 0x8043bf30, 0x80449888),
                ("NTSC-U Revision 0", "T:\\spm-tas\\evt-disassembler-master\\us0.raw", 0x803fc290, 0x80409be8),
                ("NTSC-U Revision 1", "T:\\spm-tas\\evt-disassembler-master\\us1.raw", 0x803fd5f0, 0x8040af48),
                ("NTSC-U Revision 2", "T:\\spm-tas\\evt-disassembler-master\\us2.raw", 0x803fd7d0, 0x8040b128),
                ("NTSC-J Revision 0", "T:\\spm-tas\\evt-disassembler-master\\jp0.raw", 0x803d14f0, 0x803dee48),
                ("NTSC-J Revision 1", "T:\\spm-tas\\evt-disassembler-master\\jp1.raw", 0x803d2670, 0x803dffc8),
                ("NTSC-K", "T:\\spm-tas\\evt-disassembler-master\\kr0.raw", 0x8046d1a8, 0x8047ab00),
            };

            foreach(var def in defs)
            {
                Console.WriteLine($"\nDumping {def.name}");
                Enemy.InitializeEnemies(def.path, def.p_npcTribes, def.p_npcEnemyTemplates);
                foreach(var enemy in Enemy.Enemies!)
                {
                    Console.WriteLine($"Template {enemy.TemplateId}:");
                    enemyNames.TryGetValue(enemy.TemplateId, out string? name);
                    Console.WriteLine($"    Name: {name ?? "(No name)"}");
                    Console.WriteLine($"    DropItemChance: {enemy.DropItemChance}");
                    Console.WriteLine($"    DropItemList:");
                    foreach (var e in enemy.DropItemList)
                    {
                        Console.WriteLine($"        {items[e.ItemId]}: {e.Chance}");
                    }
                }
            }
        }

        static uint[] GetNextRNGSeeds(List<Enemy> enemies, List<int> intendedDrops, int n, uint incrementsOffset=0 /*index for example */)
        {
            List<uint> seeds = new();
            long totalIncrements = incrementsOffset;
            for (int i = 0; i < n; i++)
            {
                totalIncrements += GetSpecificDrops(enemies, intendedDrops);
                //PrintDrops(enemies);
                Console.WriteLine($"Index: {totalIncrements}");
                seeds.Add(SpmSystem.randomSeed); totalIncrements++;
                //SpmSystem._rand_advance();
            }
            return seeds.ToArray();
        }

        static void Main(string[] args)
        {
            //var _data = JsonConvert.DeserializeObject<List<qqq>>(File.ReadAllText("Enemies.json")) ?? throw new FileNotFoundException();
            //foreach (var e in _data)
            //    enemyNames.Add(e.ID, e.Name);
            //DumpEnemyData();
            //return;

            // Initialize RNG seed
            SpmSystem.randomSeed = 0x9ff844c8;
            
            // Initialize Enemy module
            Enemy.InitializeEnemies("T:\\spm-tas\\evt-disassembler-master\\jp0.raw", 0x803d14f0, 0x803dee48);
            
            // Initialize enemy list
            List<Enemy> enemies = Enemy.EnemiesFromSetupFile("Z:\\Wii Emulation\\Games\\wj0orkdir\\files\\setup\\he1_01.dat");

            // Initialize intended drop list
            List<int> intendedDrops = new List<int>()
            {
                (int)Item.Catch_Card,
                (int)Item.Catch_Card,
                (int)Item.Catch_Card
            };

            int count = 1;
            foreach(var seed in GetNextRNGSeeds(enemies, intendedDrops, count, 0))
            {
                Console.WriteLine($"Seed: {seed:x}");
            }
            //GetMostDrops(enemies, (int)Item.Fire_Burst);
        }
    }
}