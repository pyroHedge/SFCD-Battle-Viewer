using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SFCD_Battle_Viewer
{
    internal class HardCodes
    {
        static public int GetSpriteId(int combatantId) => combatantId switch
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
}
