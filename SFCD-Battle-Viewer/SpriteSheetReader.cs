using PyroPatchViewer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
                    Heroes.Add(new CombatantEntry(file.GetByteChunk(n, 12), pathBase, true));
                    n += 12;
                }
                //Monsters
                Monsters.Clear();
                for (int i = 0; i < monsterCount; i++)
                {
                    Monsters.Add(new CombatantEntry(file.GetByteChunk(n, 12), pathBase, false));
                    n += 12;
                }
                //Regions
                Regions.Clear();
                for (int i = 0; i < regionCount; i++)
                {
                    Regions.Add(new RegionEntry(file.GetByteChunk(n, 12), i));
                    n += 12;
                }
                //Points
                Points.Clear();
                for (int i = 0; i < pointCount; i++)
                {
                    Points.Add(new PointEntry(file.GetByteChunk(n, 2), i));
                    n += 2;
                }
            }
        }
    }


    public class CombatantEntry
    {
        public enum SpecialMoveType
        {
            ForceMember,
            Point = 2,
            Monster = 4,
            None = 7
        }

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
        public int TriggerRegion1 { get; set; }
        public int TriggerRegion2 { get; set; }
        public byte SpecialAi1 { get; set; }
        public SpecialMoveType SpecialMove1 { get; set; }
        public int SpecialMoveTarget1 { get; set; }
        public byte SpecialAi2 { get; set; }
        public SpecialMoveType SpecialMove2 { get; set; }
        public int SpecialMoveTarget2 { get; set; }
        public int Unknown { get; set; }
        public int SpawnCode { get; set; }
        public bool IsHero { get; set; }
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

        public void PopulateFromByteChunk(ByteChunk byteChunk, string pathBase, bool isHero = false)
        {

            UnitId = ByteConversion.ByteToInt(byteChunk.Bytes[0]);
            Xcord = ByteConversion.ByteToInt(byteChunk.Bytes[1]);
            Ycord = ByteConversion.ByteToInt(byteChunk.Bytes[2]);
            AiCode = ByteConversion.ByteToInt(byteChunk.Bytes[3]);
            ItemCondition = ByteConversion.ByteToInt(byteChunk.Bytes[4]);
            Item = ByteConversion.ByteToInt(byteChunk.Bytes[5]);
            SpecialAi1 = byteChunk.Bytes[6];
            SpecialMove1 = GetSpecialMove(SpecialAi1);
            SpecialMoveTarget1 = GetSpecialMoveTarget(SpecialAi1);
            TriggerRegion1 = ByteConversion.ByteToInt(byteChunk.Bytes[7]);
            SpecialAi2 = byteChunk.Bytes[8];
            SpecialMove2 = GetSpecialMove(SpecialAi2);
            SpecialMoveTarget2 = GetSpecialMoveTarget(SpecialAi2);
            TriggerRegion2 = ByteConversion.ByteToInt(byteChunk.Bytes[9]);
            Unknown = ByteConversion.ByteToInt(byteChunk.Bytes[10]);
            SpawnCode = ByteConversion.ByteToInt(byteChunk.Bytes[11]);

            IsHero = isHero;

            int spriteId = HardCodes.GetSpriteId(isHero ? UnitId : UnitId + 32);
            string path = pathBase + pathMapSprite.Replace("000", spriteId.ToString("000"));
            if (System.IO.Path.Exists(path))
            {
                Bitmap = new BitmapImage(new Uri(path));
            }
        }

        private static SpecialMoveType GetSpecialMove(byte specialAi)
        {
            BitArray bitArray = new BitArray(new byte[] { specialAi });
            int[] array = new int[1];
            bitArray.RightShift(5).CopyTo(array, 0);
            return (SpecialMoveType)array[0];
        }

        private static int GetSpecialMoveTarget(byte specialAi)
        {
            BitArray bitArray = new BitArray(new byte[] { specialAi });
            bitArray[5] = false;
            bitArray[6] = false;
            bitArray[7] = false;
            int[] array = new int[1];
            bitArray.CopyTo(array, 0);
            return array[0];
        }

        private static string GetBitString(byte value)
        {
            BitArray bitArray = new BitArray(new byte[] { value });
            StringBuilder sb = new StringBuilder();
            foreach (bool x in bitArray)
            {
                sb.Append(x ? "1" : "0");
            }
            return sb.ToString();
        }

        public List<CombatantData> GetCombatantData()
        {
            List<CombatantData> result = new List<CombatantData>();

            var properties = this.GetType().GetProperties();
            foreach (var p in properties)
            {
                string name = p.Name;
                var value = p.GetValue(this, null);

                if (value == null)
                {
                    value = "";
                }
                if (value.GetType() == typeof(int) || value.GetType() == typeof(string) || value.GetType() == typeof(SpecialMoveType))
                {
                    result.Add(new CombatantData(name, value.ToString()));
                }
                else if (value.GetType() == typeof(byte))
                {
                    result.Add(new CombatantData(name, GetBitString((byte)value)));
                }
            }

            return result;
        }

        /// <summary>
        /// Get a long-form description of how the monster's AI functions.
        /// </summary>
        /// <returns></returns>
        public string GetAiDescription()
        {
            StringBuilder sb = new StringBuilder();

            //Trigger region
            sb.AppendLine("AI TRIGGERS");
            sb.AppendLine();
            sb.AppendLine(GetAiTriggers());

            //AI Code Summary
            sb.AppendLine("AI SUMMARY");
            sb.AppendLine();
            sb.AppendLine(GetAiSummary());

            //AI Code Description
            sb.AppendLine("AI DETAILS");
            sb.AppendLine();
            sb.AppendLine(GetAiDetail());

            return sb.ToString();
        }

        /// <summary>
        /// Print the full AI description to a text block with text formatting.
        /// </summary>
        /// <param name="textBlock"></param>
        public void PrintAiDescription(TextBlock textBlock)
        {
            //Trigger region
            textBlock.Inlines.Add(new Run("Trigger Rules") { TextDecorations = TextDecorations.Underline, FontWeight = FontWeights.Bold });
            textBlock.Inlines.Add(GetAiTriggers() );

            //AI Code Summary


            //AI Code Description
        }

        public string GetAiTriggers()
        {
            if (IsHero)
            {
                return "Human controlled.";
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                if (TriggerRegion1 < 15)
                {
                    if (TriggerRegion2 < 15)
                    {
                        sb.AppendLine(string.Format("Do not move until a force member is within region {0} or region {1} at the start of a round.", TriggerRegion1, TriggerRegion2));
                    }
                    else
                    {
                        sb.AppendLine(string.Format("Do not move until a force member is within region {0} at the start of a round.", TriggerRegion1));
                    }
                }
                else
                {
                    if (TriggerRegion2 < 15)
                    {
                        sb.AppendLine(string.Format("Do not move until a force member is within region {0} at the start of a round.", TriggerRegion2));
                    }
                    else
                    {
                        sb.AppendLine("Always active.");
                    }
                }
                sb.AppendLine();
                return sb.ToString();
            }
        }

        public string GetAiSummary()
        {
            if (IsHero)
            {
                return "Human controlled.";
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (HardCodes.AiCommand command in HardCodes.GetAiCommands(AiCode))
                {
                    sb.AppendLine(command.ToString());
                }
                sb.AppendLine();
                return sb.ToString();
            }
        }

        public string GetAiDetail()
        {
            if (IsHero)
            {
                return "Human controlled.";
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                int n = 0;
                foreach (HardCodes.AiCommand command in HardCodes.GetAiCommands(AiCode))
                {
                    n++;
                    switch (command)
                    {
                        case HardCodes.AiCommand.Heal:
                            sb.AppendLine("Check " + n + ": If targets reachable by highest level heal spell have 66% or less remaining health, heal them. Prioritizes leaders, then based upon move type. Will target multiple if it includes the highest priority targets.");
                            break;
                        case HardCodes.AiCommand.Attack:
                            sb.AppendLine("Check " + n + ": Attack physically or with magic, if targets are in range.");
                            break;
                        case HardCodes.AiCommand.BuffDebuff:
                            sb.AppendLine("Check " + n + ": Buff or debuff, if targets are in range. Dispel requires two targets with MP. Muddle 2 requires three targets. Boost 2 requires two targets.");
                            break;
                        case HardCodes.AiCommand.Move:
                            sb.AppendLine("Check " + n + ": Move two spaces closer to the force, if possible.");
                            break;
                        case HardCodes.AiCommand.SpecialMove:
                            sb.AppendLine(string.Format("Check " + n + ": Move towards {0} {1} with maximum movement (if necessary). Will attack or use magic if targets are within a 9x9 grid centered on the monster at the start of the turn.", SpecialMove1.ToString(), SpecialMoveTarget1));
                            if (SpecialMove1 == SpecialMoveType.Point)
                            {
                                sb.Append(" Exception is if the monster starts the turn on the desired point. In that case, this check is skipped.");
                            }
                            break;
                        case HardCodes.AiCommand.Stay:
                            sb.AppendLine("Check " + n + ": Stay in place and do not move.");
                            break;
                    }
                    sb.AppendLine();
                }
                return sb.ToString();
            }
        }
    }

    public class CombatantData
    {
        public string Stat { get; set; }
        public string Value { get; set; }

        public CombatantData() { }
        public CombatantData(string stat, string value) { Stat = stat; Value = value; }
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
        public int Id { get; set; }

        public RegionEntry() { }

        public RegionEntry(ByteChunk byteChunk, int id)
        {
            PopulateFromByteChunk(byteChunk);
            Id = id;
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

        public override string ToString()
        {
            return "Region " + Id;
        }
    }

    public class PointEntry
    {
        public int Xcord { get; set; }
        public int Ycord { get; set; }
        public int Id { get; set; }

        public PointEntry() { }

        public PointEntry(ByteChunk byteChunk, int id)
        {
            PopulateFromByteChunk(byteChunk);
            Id = id;
        }

        public void PopulateFromByteChunk(ByteChunk byteChunk)
        {
            Xcord = ByteConversion.ByteToInt(byteChunk.Bytes[0]);
            Ycord = ByteConversion.ByteToInt(byteChunk.Bytes[1]);
        }

        public override string ToString()
        {
            return "Point " + Id;
        }
    }
}
