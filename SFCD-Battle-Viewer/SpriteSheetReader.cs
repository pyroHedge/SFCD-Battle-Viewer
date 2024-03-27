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
using System.Windows.Media.Imaging;

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

            int spriteId = isMonster ? UnitId + 32 : UnitId;
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
        public List<Point> Points { get; set; } = new List<Point>();

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
}
