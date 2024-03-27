using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace SFCD_Battle_Viewer
{
    public class CombatantListBoxItemTemplate
    {
        public string Text { get; set; }
        public CombatantEntry Combatant { get; set; }
        //public BitmapImage Sprite { get; set; }
        public Image Image { get; set; }

        public CombatantListBoxItemTemplate() { }

        public CombatantListBoxItemTemplate(string text, CombatantEntry combatant, Image image)
        {
            Text = text;
            Combatant = combatant;
            Image = image;
        }   
    }
}
