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
        private List<Polygon> regions = new List<Polygon>();


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
                canvasMap.Children.Clear();
                canvasMap.Children.Add(imageMap);
                UpdateSpritesByCombatantEntries(spriteSheet.Heroes, spriteHeroes, lstHeroes);
                UpdateSpritesByCombatantEntries(spriteSheet.Monsters, spriteMonsters, lstMonsters);
                UpdateRegionsByEntries(spriteSheet.Regions, regions);
            }
        }

        private void UpdateSpritesByCombatantEntries(List<CombatantEntry> combatantEntries, List<Image> listToUpdate, ListBox listBoxToUpdate)
        {
            //Clear existing images
            //foreach (Image image in listToUpdate)
            //{
            //    canvasMap.Children.Remove(image);
            //    image.Source = null;
            //}
            listToUpdate.Clear();
            //Update images and list
            List<CombatantListBoxItemTemplate> items = new List<CombatantListBoxItemTemplate>();
            if (combatantEntries != null)
            {
                foreach (var x in combatantEntries)
                {
                    Image image = new Image { Source = x.Bitmap };
                    image.MouseLeftButtonUp += imageCombatant_MouseLeftButtonUp;
                    Canvas.SetLeft(image, x.Xcord * TILE_SIZE_IN_PIXELS);
                    Canvas.SetTop(image, x.Ycord * TILE_SIZE_IN_PIXELS);
                    canvasMap.Children.Add(image);
                    listToUpdate.Add(image);
                    items.Add(new CombatantListBoxItemTemplate(x.ToString(), x, image));
                }
            }
            listBoxToUpdate.ItemsSource = items;

        }

        private void UpdateRegionsByEntries(List<RegionEntry> regionEntries, List<Polygon> listToUpdate)
        {
            if (regionEntries != null)
            {
                regions.Clear();
                foreach (RegionEntry regionEntry in regionEntries)
                {
                    Polygon polygon = new Polygon();
                    foreach (Point point in regionEntry.Points)
                    {
                        polygon.Points.Add( new Point(point.X * TILE_SIZE_IN_PIXELS, point.Y * TILE_SIZE_IN_PIXELS));
                    }
                    Brush brushColor = MapHardCodes.GetRegionColor(listToUpdate.Count);
                    polygon.Stroke = brushColor;
                    polygon.Fill = brushColor;
                    polygon.StrokeThickness = 4;
                    polygon.Opacity = .25;
                    canvasMap.Children.Add(polygon);
                    listToUpdate.Add(polygon);
                }
            }
        }

        //------------------------------------------------------------------------------------------
        // Combatant Selection
        //------------------------------------------------------------------------------------------

        private void CombatantList_SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                //foreach (CombatantListBoxItemTemplate x in e.RemovedItems)
                //{
                //    x.Image.Effect = null;
                //}
                //rectCursor.Visibility = Visibility.Collapsed;
            }
            if (e.AddedItems.Count > 0)
            {
                //DropShadowEffect dropShadow = new DropShadowEffect()
                //{
                //    Color = Colors.White,
                //    ShadowDepth = 0,
                //    BlurRadius = 8,
                //    RenderingBias = RenderingBias.Performance,
                //};
                //foreach (CombatantListBoxItemTemplate x in e.AddedItems)
                //{
                //    x.Image.Effect = dropShadow;
                //}

                //CombatantListBoxItemTemplate first = (CombatantListBoxItemTemplate)(e.AddedItems[0]);
                //Canvas.SetLeft(rectCursor, first.Combatant.Xcord * TILE_SIZE_IN_PIXELS - 2);
                //Canvas.SetTop(rectCursor, first.Combatant.Ycord * TILE_SIZE_IN_PIXELS - 2);
                //rectCursor.Visibility = Visibility.Visible;

                SelectCombatant(((CombatantListBoxItemTemplate)e.AddedItems[0]).Image);
            }
        }

        private void imageCombatant_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Update the lists
            lstHeroes.SelectedItem = null;
            int index = spriteHeroes.IndexOf((Image)sender);
            if (index >= 0)
            {
                lstHeroes.SelectedIndex = index;
            }
            else
            {
                //lstHeroes.SelectedIndex = -1;
            }

            lstMonsters.SelectedItem = null;
            index = spriteMonsters.IndexOf((Image)sender);
            if (index >= 0)
            {
                lstMonsters.SelectedIndex = index;
            }
            else
            {
                //lstHeroes.SelectedIndex = -1;
            }
            //No need to call the SelectCombatant because that will be called automatically by the lists during the selection event.
            //SelectCombatant((Image)sender);
        }

        private void SelectCombatant(Image image)
        {
            //Select image
            if (image == null)
            {
                rectCursor.Visibility = Visibility.Collapsed;
            }
            else
            {
                Canvas.SetLeft(rectCursor, Canvas.GetLeft(image) - 2);
                Canvas.SetTop(rectCursor, Canvas.GetTop(image) - 2);
                rectCursor.Visibility = Visibility.Visible;
            }
        }

    }
}