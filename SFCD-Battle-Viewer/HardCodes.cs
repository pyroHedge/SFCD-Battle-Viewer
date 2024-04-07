using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SFCD_Battle_Viewer
{
    internal class HardCodes
    {
        static public int GetSpriteId(int combatantId, int book)
        {
            if (book == 1)
            {
                return GetSpriteIdBook1(combatantId);
            }
            else if (book == 2)
            {
                return GetSpriteIdBook2(combatantId);
            }
            else
            {
                return combatantId;
            }
        }


        static public int GetSpriteIdBook1(int combatantId) => combatantId switch
        {
            89 => 32 + 39, //battle 1
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

        static public int GetSpriteIdBook2(int combatantId) => combatantId switch
        {
            32 + 47 => 32 + 2, //battle 1
            32 + 48 => 32 + 3, //battle 2 - I have to skip this one because 48 is a valid combatantId later
            32 + 49 => 32 + 5,
            32 + 50 => 32 + 32, //battle 4
            32 + 78 => 14,      //battle 4
            32 + 51 => 32 + 39,
            32 + 52 => 32 + 6,
            32 + 53 => 32 + 34,
            32 + 54 => 79,
            32 + 55 => 32 + 40, //battle 9
            32 + 82 => 32 + 34, //battle 9
            32 + 56 => 32 + 9,
            32 + 57 => 32 + 44, //battle 11+14
            32 + 75 => 32 + 24, //battle 11
            32 + 58 => 32 + 41,
            32 + 59 => 32 + 10, //batle 13
            32 + 80 => 32 + 34, //batle 13
            32 + 60 => 32 + 41,
            32 + 61 => 32 + 13, //battle 15
            32 + 79 => 39,      //battle 15
            32 + 62 => 32 + 28, //battle 16
            32 + 81 => 32 + 25, //battle 16
            32 + 83 => 32 + 44, //battle 16
            32 + 63 => 32 + 12,
            32 + 64 => 80,      //battle 18
            32 + 76 => 32 + 35, //battle 18
            32 + 65 => 32 + 14, //battle 19
            32 + 77 => 32 + 41, //battle 19
            32 + 66 => 32 + 42, //battle 20
            32 + 74 => 87,      //battle 20
            32 + 67 => 81,
            32 + 68 => 82,
            32 + 69 => 83,
            32 + 70 => 110,     //battle 24
            32 + 71 => 84,      //battle 24
            32 + 72 => 85,      //battle 24
            32 + 73 => 86,      //battle 24
            _ => combatantId,
        };

        static public Brush GetRegionColor(int regionId) => regionId switch
        {
            0 => new SolidColorBrush(Color.FromRgb(51, 34, 136)), //blue
            1 => new SolidColorBrush(Color.FromRgb(170, 68, 153)), //purple
            2 => new SolidColorBrush(Color.FromRgb(221, 204, 119)), //yellow
            3 => new SolidColorBrush(Color.FromRgb(204, 102, 119)), //pink
            4 => new SolidColorBrush(Color.FromRgb(136, 204, 238)), //white-blue
            5 => new SolidColorBrush(Color.FromRgb(136, 34, 85)),
            6 => new SolidColorBrush(Color.FromRgb(68, 170, 153)),
            7 => new SolidColorBrush(Color.FromRgb(17, 119, 51)), //green
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

        static public BankDefinition GetBankDefinition(string bankName) => bankName switch
        {
            "BANKD00" => new BankDefinition(bankName, "Book 1 - Intro", 1, 0),
            "BANKD01" => new BankDefinition(bankName, "Book 1 - Battle 1", 1, 1),
            "BANKD02" => new BankDefinition(bankName, "Book 1 - Battle 2", 1, 2),
            "BANKD03" => new BankDefinition(bankName, "Book 1 - Battle 3", 1, 3),
            "BANKD04" => new BankDefinition(bankName, "Book 1 - Battle 4", 1, 4),
            "BANKD05" => new BankDefinition(bankName, "Book 1 - Battle 5", 1, 5),
            "BANKD06" => new BankDefinition(bankName, "Book 1 - Battle 6", 1, 6),
            "BANKD07" => new BankDefinition(bankName, "Book 1 - Battle 7", 1, 7),
            "BANKD08" => new BankDefinition(bankName, "Book 1 - Battle 8", 1, 8),
            "BANKD09" => new BankDefinition(bankName, "Book 1 - Battle 9", 1, 9),
            "BANKD0A" => new BankDefinition(bankName, "Book 1 - Battle 10", 1, 10),
            "BANKD0B" => new BankDefinition(bankName, "Book 1 - Battle 11", 1, 11),
            "BANKD0C" => new BankDefinition(bankName, "Book 1 - Battle 12", 1, 12),
            "BANKD0D" => new BankDefinition(bankName, "Book 1 - Battle 13", 1, 13),
            "BANKD0E" => new BankDefinition(bankName, "Book 1 - Battle 14", 1, 14),
            "BANKD0F" => new BankDefinition(bankName, "Book 1 - Battle 15", 1, 15),
            "BANKD10" => new BankDefinition(bankName, "Book 1 - Battle 16", 1, 16),
            "BANKD11" => new BankDefinition(bankName, "Book 1 - Battle 17", 1, 17),
            "BANKD12" => new BankDefinition(bankName, "Book 1 - Battle 18", 1, 18),
            "BANKD13" => new BankDefinition(bankName, "Book 1 - Battle 19", 1, 19),
            "BANKD14" => new BankDefinition(bankName, "Book 1 - Battle 20", 1, 20),
            "BANKD15" => new BankDefinition(bankName, "Book 1 - Battle 21", 1, 21),
            "BANKD16" => new BankDefinition(bankName, "Book 1 - Battle 22", 1, 22),
            "BANKD17" => new BankDefinition(bankName, "Book 1 - Dummy", 1, 0),
            "BANKD18" => new BankDefinition(bankName, "Book 1 - Dummy", 1, 0),
            "BANKD19" => new BankDefinition(bankName, "Book 1 - Dummy", 1, 0),
            "BANKD1A" => new BankDefinition(bankName, "Book 1 - Dummy", 1, 0),
            "BANKD1B" => new BankDefinition(bankName, "Book 1 - Dummy", 1, 0),
            "BANKD1C" => new BankDefinition(bankName, "Book 1 - Dummy", 1, 0),
            "BANKD1D" => new BankDefinition(bankName, "Book 1 - Dummy", 1, 0),
            "BANKD1E" => new BankDefinition(bankName, "Book 1 - Dummy", 1, 0),
            "BANKD1F" => new BankDefinition(bankName, "Book 1 - Dummy", 1, 0),
            "BANKD20" => new BankDefinition(bankName, "Book 2 - Intro", 2, 0),
            "BANKD21" => new BankDefinition(bankName, "Book 2 - Battle 1", 2, 1),
            "BANKD22" => new BankDefinition(bankName, "Book 2 - Battle 2", 2, 2),
            "BANKD23" => new BankDefinition(bankName, "Book 2 - Battle 3", 2, 3),
            "BANKD24" => new BankDefinition(bankName, "Book 2 - Battle 4", 2, 4),
            "BANKD25" => new BankDefinition(bankName, "Book 2 - Battle 5", 2, 5),
            "BANKD26" => new BankDefinition(bankName, "Book 2 - Battle 6", 2, 6),
            "BANKD27" => new BankDefinition(bankName, "Book 2 - Battle 7", 2, 7),
            "BANKD28" => new BankDefinition(bankName, "Book 2 - Battle 8", 2, 8),
            "BANKD29" => new BankDefinition(bankName, "Book 2 - Battle 9", 2, 9),
            "BANKD2A" => new BankDefinition(bankName, "Book 2 - Battle 10", 2, 10),
            "BANKD2B" => new BankDefinition(bankName, "Book 2 - Battle 11", 2, 11),
            "BANKD2C" => new BankDefinition(bankName, "Book 2 - Battle 12", 2, 12),
            "BANKD2D" => new BankDefinition(bankName, "Book 2 - Battle 13", 2, 13),
            "BANKD2E" => new BankDefinition(bankName, "Book 2 - Battle 14", 2, 14),
            "BANKD2F" => new BankDefinition(bankName, "Book 2 - Battle 15", 2, 15),
            "BANKD30" => new BankDefinition(bankName, "Book 2 - Battle 16", 2, 16),
            "BANKD31" => new BankDefinition(bankName, "Book 2 - Battle 17", 2, 17),
            "BANKD32" => new BankDefinition(bankName, "Book 2 - Battle 18", 2, 18),
            "BANKD33" => new BankDefinition(bankName, "Book 2 - Battle 19", 2, 19),
            "BANKD34" => new BankDefinition(bankName, "Book 2 - Battle 20", 2, 20),
            "BANKD35" => new BankDefinition(bankName, "Book 2 - Battle 21", 2, 21),
            "BANKD36" => new BankDefinition(bankName, "Book 2 - Battle 22", 2, 22),
            "BANKD37" => new BankDefinition(bankName, "Book 2 - Battle 23", 2, 23),
            "BANKD38" => new BankDefinition(bankName, "Book 2 - Battle 24", 2, 24),
            "BANKD39" => new BankDefinition(bankName, "Book 2 - Credits", 2, 0),
            "BANKD3A" => new BankDefinition(bankName, "Book 3 - Credits", 3, 0),
            "BANKD3B" => new BankDefinition(bankName, "Book 4 - Credits", 4, 0),
            "BANKD3C" => new BankDefinition(bankName, "Book 0 - Dummy", 0, 0),
            "BANKD3D" => new BankDefinition(bankName, "Book 0 - Dummy", 0, 0),
            "BANKD3E" => new BankDefinition(bankName, "Book 0 - Dummy", 0, 0),
            "BANKD3F" => new BankDefinition(bankName, "Book 0 - Dummy", 0, 0),
            "BANKD40" => new BankDefinition(bankName, "Book 3 - Intro", 3, 0),
            "BANKD41" => new BankDefinition(bankName, "Book 3 - Battle 1", 3, 1),
            "BANKD42" => new BankDefinition(bankName, "Book 3 - Battle 2", 3, 2),
            "BANKD43" => new BankDefinition(bankName, "Book 3 - Battle 3", 3, 3),
            "BANKD44" => new BankDefinition(bankName, "Book 3 - Battle 4", 3, 4),
            "BANKD45" => new BankDefinition(bankName, "Book 3 - Battle 5", 3, 5),
            "BANKD46" => new BankDefinition(bankName, "Book 3 - Battle 6", 3, 6),
            "BANKD47" => new BankDefinition(bankName, "Book 4 - Intro", 4, 0),
            "BANKD48" => new BankDefinition(bankName, "Book 4 - Battle 1", 4, 1),
            _ => new BankDefinition(bankName, "Unknown", 0, 0),
        };


        public enum AiCommand
        {
            Heal,
            Attack,
            BuffDebuff,
            Move,
            SpecialMove,
            Stay,
        }

        static public List<AiCommand> GetAiCommands(int ai)
        {
            List<AiCommand> result = new List<AiCommand>();
            switch (ai)
            {
                case 0:
                    result.Add(AiCommand.Heal);
                    result.Add(AiCommand.BuffDebuff);
                    result.Add(AiCommand.Attack);
                    result.Add(AiCommand.Move);
                    result.Add(AiCommand.Stay);
                    break;

                case 1:
                    result.Add(AiCommand.SpecialMove);
                    result.Add(AiCommand.Heal);
                    result.Add(AiCommand.BuffDebuff);
                    result.Add(AiCommand.Attack);
                    result.Add(AiCommand.Move);
                    result.Add(AiCommand.Stay);
                    break;

                case 2 - 5:
                    result.Add(AiCommand.Heal);
                    result.Add(AiCommand.SpecialMove);
                    result.Add(AiCommand.BuffDebuff);
                    result.Add(AiCommand.Attack);
                    result.Add(AiCommand.Move);
                    result.Add(AiCommand.Stay);
                    break;

                case 6:
                    result.Add(AiCommand.Attack);
                    result.Add(AiCommand.Heal);
                    result.Add(AiCommand.BuffDebuff);
                    result.Add(AiCommand.Move);
                    result.Add(AiCommand.Stay);
                    break;

                case 7:
                    result.Add(AiCommand.SpecialMove);
                    result.Add(AiCommand.Attack);
                    result.Add(AiCommand.Heal);
                    result.Add(AiCommand.BuffDebuff);
                    result.Add(AiCommand.Move);
                    result.Add(AiCommand.Stay);
                    break;

                case 8:
                    result.Add(AiCommand.Attack);
                    result.Add(AiCommand.Heal);
                    result.Add(AiCommand.BuffDebuff);
                    result.Add(AiCommand.Stay);
                    break;

                case 9:
                case 13 - 15:
                    result.Add(AiCommand.SpecialMove);
                    result.Add(AiCommand.Attack);
                    result.Add(AiCommand.Heal);
                    result.Add(AiCommand.BuffDebuff);
                    result.Add(AiCommand.SpecialMove);
                    result.Add(AiCommand.Move);
                    result.Add(AiCommand.Stay);
                    break;

                case 10 - 11:
                    result.Add(AiCommand.Stay);
                    break;

                case 12:
                    result.Add(AiCommand.SpecialMove);
                    result.Add(AiCommand.BuffDebuff);
                    result.Add(AiCommand.Attack);
                    result.Add(AiCommand.Heal);
                    result.Add(AiCommand.Move);
                    result.Add(AiCommand.Stay);
                    break;
            }
            return result;
        }
    }

    public class BankDefinition
    {
        public string BankName { get; set; }
        public string Text { get; set; }
        public int Book { get; set; }
        public int Battle { get; set; }
        public string DirectoryPath { get; set; }
        public BankDefinition(string bankName, string text, int book, int battle)
        {
            BankName = bankName;
            Text = text;
            Book = book;
            Battle = battle;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
