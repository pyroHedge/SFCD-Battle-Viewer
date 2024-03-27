using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SFCD_Battle_Viewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string testDir = @"C:\Users\adams\Documents\GitHub\SFCD-Battle-Viewer\sfcd-assets";
        const string pathMapImage = @"\map\1-layout.png";
        const int TILE_SIZE_IN_PIXELS = 24; //how large the map tiles are at full size

        public ObservableCollection<string> BankList { get; set; } = new ObservableCollection<string>();

        private List<string> BankDirs = new List<string>();
        private SpriteSheet spriteSheet = new SpriteSheet();
        private List<Image> spriteHeroes = new List<Image>();
        private List<Image> spriteMonsters = new List<Image>();


        public MainWindow()
        {
            PopulateBankList(testDir);
            InitializeComponent();
        }

        public bool PopulateBankList(string directoryPath)
        {
            BankDirs = Directory.GetDirectories(directoryPath).ToList();
            BankList = new ObservableCollection<string>(BankDirs.Select(d => new DirectoryInfo(d).Name));
            return BankList.Count > 0;
        }


        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                string basePath = BankDirs[((ListBox)sender).SelectedIndex];
                string imagePath = basePath + pathMapImage;
                if (System.IO.Path.Exists(imagePath))
                {
                    //ImageBrush imageBrush = new ImageBrush()
                    //{
                    //    ImageSource = new BitmapImage(new Uri(imagePath, UriKind.Relative)),
                    //    TileMode = TileMode.None,
                    //    Stretch = Stretch.None,
                    //};
                    //canvasMap.Background = imageBrush;
                    imageMap.Source = new BitmapImage(new Uri(imagePath));
                }
                else
                {
                    //canvasMap.Background = null;
                    imageMap.Source = null;
                }

                spriteSheet = new SpriteSheet(basePath);
                UpdateSprites();
            }
        }

        public void UpdateSprites()
        {
            if (spriteSheet != null)
            {
                UpdateSpritesByCombatantEntries(spriteSheet.Heroes, spriteHeroes, lstHeroes);
                UpdateSpritesByCombatantEntries(spriteSheet.Monsters, spriteMonsters, lstMonsters);
            }
        }

        private void UpdateSpritesByCombatantEntries(List<CombatantEntry> combatantEntries, List<Image> listToUpdate, ListBox listBoxToUpdate)
        {
            //Clear existing images
            {
                //canvasMap.Children.Clear();
                //canvasMap.Children.Add(imageMap);
                foreach (Image image in listToUpdate)
                {
                    canvasMap.Children.Remove(image);
                    image.Source = null;
                }
                for (int i = 0; i < listToUpdate.Count; i++)
                {
                    //canvasMap.Children.Remove((Image)listToUpdate[i]);
                    //listToUpdate[i].Source = null;
                }
                listToUpdate.Clear();
            }
            //Update images and list
            List<CombatantListBoxItemTemplate> items = new List<CombatantListBoxItemTemplate>();
            if (combatantEntries != null)
            {
                foreach (var x in combatantEntries)
                {
                    Image image = new Image { Source = x.Bitmap };
                    Canvas.SetLeft(image, x.Xcord * TILE_SIZE_IN_PIXELS);
                    Canvas.SetTop(image, x.Ycord * TILE_SIZE_IN_PIXELS);
                    canvasMap.Children.Add(image);
                    listToUpdate.Add(image);
                    items.Add(new CombatantListBoxItemTemplate(x.ToString(), x, image));
                }
            }
            listBoxToUpdate.ItemsSource = items;

        }

        private void lstHeroes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                foreach (CombatantListBoxItemTemplate x in e.RemovedItems)
                {
                    x.Image.Effect = null;
                }
            }
            if (e.AddedItems.Count > 0)
            {
                DropShadowEffect dropShadow = new DropShadowEffect()
                {
                    Color = Colors.White,
                    ShadowDepth = 0,
                    BlurRadius= 8,
                    RenderingBias = RenderingBias.Performance,
                };
                foreach (CombatantListBoxItemTemplate x in e.AddedItems)
                {
                    x.Image.Effect = dropShadow;
                }
            }
        }
    }
}