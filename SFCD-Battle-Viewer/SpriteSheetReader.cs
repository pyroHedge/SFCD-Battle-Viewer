using PyroPatchViewer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace SFCD_Battle_Viewer
{

    internal class SpriteSheet
    {
        const string pathSpriteset = @"\battle\spriteset.bin";

        public List<CombatantEntry> Heroes { get; set; } = new List<CombatantEntry>();
        public List<CombatantEntry> Monsters { get; set; } = new List<CombatantEntry>();
        public List<RegionEntry> Regions { get; set; } = new List<RegionEntry>();
        public List<PointEntry> Points { get; set; } = new List<PointEntry>();

        public SpriteSheet() { }
        public SpriteSheet(string pathBase) { populateLists(pathBase); }

        public void populateLists(string pathBase)
        {
            if (Path.Exists(pathBase + pathSpriteset))
            {
                BinaryFile file = new BinaryFile(pathBase + pathSpriteset);
                int heroCount = ByteConversion.ByteToInt(file.Data[0]);
                int monsterCount = ByteConversion.ByteToInt(file.Data[1]);
                int regionCount = ByteConversion.ByteToInt(file.Data[2]);
                int pointCount = ByteConversion.ByteToInt(file.Data[3]);

                int n = 4;
                //Heroes
                Heroes.Clear();
                for (int i = 0; i < heroCount; i++)
                {
                    Heroes.Add(new CombatantEntry(file.GetByteChunk(n, 12), pathBase, false));
                    n += 12;
                }
                //Monsters
                Monsters.Clear();
                for (int i = 0; i < monsterCount; i++)
                {
                    Monsters.Add(new CombatantEntry(file.GetByteChunk(n, 12), pathBase, true));
                    n += 12;
                }
                //Regions
                Regions.Clear();
                for (int i = 0; i < regionCount; i++)
                {
                    Regions.Add(new RegionEntry(file.GetByteChunk(n, 12)));
                    n += 12;
                }
                //Points
                Points.Clear();
                for (int i = 0; i < pointCount; i++)
                {
                    Points.Add(new PointEntry(file.GetByteChunk(n, 2)));
                    n += 2;
                }
            }
        }
    }


    public class CombatantEntry
    {
        //1  Unit ID
        //2  X Coordinate(starting position on map)
        //3  Y Coordinate(starting position on map)
        //4  AI Code(0 to F)
        //5  Item condition
        //6  Item Value(7F is empty or not used)
        //7  Special AI 1 (FF is default/blank)
        //8  Map Region 1 (0F if blank or not used)
        //9  Special AI 2 (FF is default/blank)
        //10 Map Region 2 (0F if blank or not used)
        //11 ?
        //12 Continuously respawn = 01; Region-triggered spawn = 02(00 is default / not used)

        const string pathMapSprite = @"\mapsprites\mapsprite000-2-0.png"; //replace 000 with the sprite #

        public int UnitId { get; set; }
        public int Xcord { get; set; }
        public int Ycord { get; set; }
        public int AiCode { get; set; }
        public int ItemCondition { get; set; }
        public int Item { get; set; }
        public int SpecialAi1 { get; set; }
        public int TriggerRegion1 { get; set; }
        public int SpecialAi2 { get; set; }
        public int TriggerRegion2 { get; set; }
        public int Unknown { get; set; }
        public int SpawnCode { get; set; }
        public BitmapImage Bitmap { get; set; } = null;

        public CombatantEntry() { }

        public CombatantEntry(ByteChunk byteChunk, string pathBase, bool isMonster)
        {
            PopulateFromByteChunk(byteChunk, pathBase, isMonster);
        }

        public override string ToString()
        {
            return UnitId.ToString() + "," + Xcord.ToString() + "," + Ycord.ToString() + "," + AiCode.ToString();
        }

        public void PopulateFromByteChunk(ByteChunk byteChunk, string pathBase, bool isMonster = false)
        {
            UnitId = ByteConversion.ByteToInt(byteChunk.Bytes[0]);
            Xcord = ByteConversion.ByteToInt(byteChunk.Bytes[1]);
            Ycord = ByteConversion.ByteToInt(byteChunk.Bytes[2]);
            AiCode = ByteConversion.ByteToInt(byteChunk.Bytes[3]);
            ItemCondition = ByteConversion.ByteToInt(byteChunk.Bytes[4]);
            Item = ByteConversion.ByteToInt(byteChunk.Bytes[5]);
            SpecialAi1 = ByteConversion.ByteToInt(byteChunk.Bytes[6]);
            TriggerRegion1 = ByteConversion.ByteToInt(byteChunk.Bytes[7]);
            SpecialAi2 = ByteConversion.ByteToInt(byteChunk.Bytes[8]);
            TriggerRegion2 = ByteConversion.ByteToInt(byteChunk.Bytes[9]);
            Unknown = ByteConversion.ByteToInt(byteChunk.Bytes[10]);
            SpawnCode = ByteConversion.ByteToInt(byteChunk.Bytes[11]);

            int spriteId = MapHardCodes.GetSpriteId(isMonster ? UnitId + 32 : UnitId);
            string path = pathBase + pathMapSprite.Replace("000", spriteId.ToString("000"));
            if (System.IO.Path.Exists(path))
            {
                Bitmap = new BitmapImage(new Uri(path));
            }
        }

    }


    public class RegionEntry
    {
        //Points in this order:
        //upper left
        //bottom left
        //bottom right
        //upper right

        public int Type { get; set; }
        public PointCollection Points { get; set; } = new PointCollection();

        public RegionEntry() { }

        public RegionEntry(ByteChunk byteChunk)
        {
            PopulateFromByteChunk(byteChunk);
        }

        public void PopulateFromByteChunk(ByteChunk byteChunk)
        {
            Type = ByteConversion.ByteToInt(byteChunk.Bytes[0]);
            Points.Clear();
            for (int i = 2; i < 10; i += 2)
            {
                Points.Add(new Point(ByteConversion.ByteToInt(byteChunk.Bytes[i]), ByteConversion.ByteToInt(byteChunk.Bytes[i + 1])));
            }
        }
    }

    public class PointEntry
    {
        public int Xcord { get; set; }
        public int Ycord { get; set; }

        public PointEntry() { }

        public PointEntry(ByteChunk byteChunk)
        {
            PopulateFromByteChunk(byteChunk);
        }

        public void PopulateFromByteChunk(ByteChunk byteChunk)
        {
            Xcord = ByteConversion.ByteToInt(byteChunk.Bytes[0]);
            Ycord = ByteConversion.ByteToInt(byteChunk.Bytes[1]);
        }
    }

    public class MapHardCodes
    { 
        static public int GetSpriteId(int combatantId) => combatantId switch
        {
            89 => 32+39, //battle 1
            90 => 32 + 3,
            91 => 32 + 5,
            92 => 32 + 39,
            93 => 32 + 33,
            94 => 32 + 6,
            95 => 32 + 43,
            96 => 32 + 8,
            97 => 32 + 34,
            88 => 79, //battle 10
            98 => 32 + 41, //battle 11
            99 => 32 + 13, //battle 12
            100 => 32 + 12, //battle 13
            102 => 32 + 20, //battle 18
            79 => 88, //battle 20
            _ => combatantId,
        };


        static public Brush GetRegionColor(int regionId) => regionId switch
        {
            0 => new SolidColorBrush(Color.FromRgb(51,34,136)),
            1 => new SolidColorBrush(Color.FromRgb(17, 119, 51)),
            2 => new SolidColorBrush(Color.FromRgb(221, 204, 119)),
            3 => new SolidColorBrush(Color.FromRgb(204, 102, 119)),
            4 => new SolidColorBrush(Color.FromRgb(136, 204, 238)),
            5 => new SolidColorBrush(Color.FromRgb(170, 68, 153)),
            6 => new SolidColorBrush(Color.FromRgb(68, 170, 153)),
            7 => new SolidColorBrush(Color.FromRgb(136, 34, 85)),
            8 => new SolidColorBrush(Color.FromRgb(51, 34, 136)),
            9 => new SolidColorBrush(Color.FromRgb(51, 34, 136)),
            10 => new SolidColorBrush(Color.FromRgb(51, 34, 136)),
            11 => new SolidColorBrush(Color.FromRgb(51, 34, 136)),
            12 => new SolidColorBrush(Color.FromRgb(51, 34, 136)),
            13 => new SolidColorBrush(Color.FromRgb(51, 34, 136)),
            14 => new SolidColorBrush(Color.FromRgb(51, 34, 136)),
            15 => new SolidColorBrush(Color.FromRgb(51, 34, 136)),
            _ => Brushes.Brown,
        };
    }
}
