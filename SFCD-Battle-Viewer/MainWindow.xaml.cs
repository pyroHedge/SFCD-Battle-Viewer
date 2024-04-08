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
        const string pathAssets = @"sfcd-assets";
        const string pathMapImage = @"\map\1-layout.png";
        const int TILE_SIZE_IN_PIXELS = 24; //how large the map tiles are at full size

        public ObservableCollection<BankDefinition> BankList { get; set; } = new ObservableCollection<BankDefinition>();
        private string[] BankDirs;
        private SpriteSheet spriteSheet = new SpriteSheet();
        private List<Image> spriteHeroes = new List<Image>();
        private List<Image> spriteMonsters = new List<Image>();
        private List<Polygon> regions = new List<Polygon>();
        private List<Rectangle> points = new List<Rectangle>();


        public MainWindow()
        {
            string dir = Environment.ProcessPath; //this is the best method of finding the .exe location now -- it was only available in .NET Core 6.0 or later
            dir = System.IO.Path.GetDirectoryName(dir);
            dir = System.IO.Path.Combine(dir, pathAssets); 
            if (System.IO.Path.Exists(dir))
            {
                PopulateBankList(dir);
                InitializeComponent();
            }
            else
            {
                MessageBox.Show("No \"sfcd-assets\" folder found in " + dir + "\n\n Please place the \"sfcd-assets\" folder in the same directory as the executable.", "SFCD Battle Viewer", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                System.Windows.Application.Current.Shutdown();
            }
        }

        public bool PopulateBankList(string directoryPath)
        {
            BankDirs = Directory.GetDirectories(directoryPath);
            BankList.Clear();
            foreach (var dir in BankDirs)
            {
                BankDefinition temp = HardCodes.GetBankDefinition(new DirectoryInfo(dir).Name);
                temp.DirectoryPath = dir;
                BankList.Add(temp);
            }
            //BankList = new ObservableCollection<string>(BankDirs.Select(d => new DirectoryInfo(d).Name));
            return BankList.Count > 0;
        }


        //------------------------------------------------------------------------------------------
        // Battle Selection
        //------------------------------------------------------------------------------------------

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

        //------------------------------------------------------------------------------------------
        // Update Sprites and Lists
        //------------------------------------------------------------------------------------------

        public void UpdateSprites()
        {
            if (spriteSheet != null)
            {
                canvasMap.Children.Clear();
                canvasMap.Children.Add(imageMap);
                UpdateRegionsByEntries(spriteSheet.Regions, regions, lstRegions);
                UpdatePointsByEntries(spriteSheet.Points, points, lstPoints);
                UpdateSpritesByCombatantEntries(spriteSheet.Heroes, spriteHeroes, lstHeroes);
                UpdateSpritesByCombatantEntries(spriteSheet.Monsters, spriteMonsters, lstMonsters);
                canvasMap.Children.Add(rectCursor);
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

        private void UpdateRegionsByEntries(List<RegionEntry> regionEntries, List<Polygon> listToUpdate, ListBox listBoxToUpdate)
        {
            listBoxToUpdate.ItemsSource = regionEntries;
            listToUpdate.Clear();
            foreach (RegionEntry regionEntry in regionEntries)
            {
                Polygon polygon1 = new Polygon();
                Polygon polygon2 = new Polygon();
                foreach (Point point in regionEntry.Points)
                {
                    polygon1.Points.Add(new Point(point.X * TILE_SIZE_IN_PIXELS + TILE_SIZE_IN_PIXELS / 2, point.Y * TILE_SIZE_IN_PIXELS + TILE_SIZE_IN_PIXELS / 2));
                    polygon2.Points.Add(new Point(point.X * TILE_SIZE_IN_PIXELS + TILE_SIZE_IN_PIXELS / 2, point.Y * TILE_SIZE_IN_PIXELS + TILE_SIZE_IN_PIXELS / 2));
                }
                Brush brushColor = HardCodes.GetRegionColor(listToUpdate.Count / 2);
                polygon1.Fill = brushColor;
                polygon1.StrokeThickness = 4;
                polygon1.Opacity = .25;
                polygon1.Visibility = Visibility.Collapsed;
                canvasMap.Children.Add(polygon1);
                listToUpdate.Add(polygon1);

                polygon2.Stroke = brushColor;
                polygon2.Fill = null;
                polygon2.StrokeThickness = 4;
                polygon2.Visibility = Visibility.Collapsed;
                canvasMap.Children.Add(polygon2);
                listToUpdate.Add(polygon2);
            }
        }

        private void UpdatePointsByEntries(List<PointEntry> pointEntries, List<Rectangle> listToUpdate, ListBox listBoxToUpdate)
        {
            listBoxToUpdate.ItemsSource = pointEntries;
            listToUpdate.Clear();
            foreach (PointEntry pointEntry in pointEntries)
            {
                Rectangle rect = new Rectangle();
                Brush brushColor = HardCodes.GetRegionColor(listToUpdate.Count);
                rect.Fill = brushColor;
                rect.Stroke = brushColor;
                rect.StrokeThickness = 4;
                rect.Visibility = Visibility.Collapsed;
                rect.Width = TILE_SIZE_IN_PIXELS;
                rect.Height = TILE_SIZE_IN_PIXELS;
                Canvas.SetLeft(rect, pointEntry.Xcord * TILE_SIZE_IN_PIXELS);
                Canvas.SetTop(rect, pointEntry.Ycord * TILE_SIZE_IN_PIXELS);
                canvasMap.Children.Add(rect);
                listToUpdate.Add(rect);
            }
        }

        //------------------------------------------------------------------------------------------
        // Combatant Selection
        //------------------------------------------------------------------------------------------

        private void CombatantList_SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                RemoveCombatantSelectionVisuals();
                lstRegions.SelectedIndex = -1;
                lstPoints.SelectedIndex = -1;
                rectCursor.Visibility = Visibility.Collapsed;
                gridCombatant.ItemsSource = new List<CombatantData>();
                textAiDescription.Text = string.Empty;
            }
            if (e.AddedItems.Count > 0)
            {
                SelectCombatant((CombatantListBoxItemTemplate)e.AddedItems[0]);
            }
        }

        private void imageCombatant_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Update the lists
            int index = spriteHeroes.IndexOf((Image)sender);
            if (index >= 0)
            {
                if (lstHeroes.SelectedIndex != -1 && lstHeroes.SelectedIndex == index)
                {
                    lstHeroes.SelectedIndex = -1;
                }
                else
                {
                    lstHeroes.SelectedItem = null;
                    lstHeroes.SelectedIndex = index;
                }
            }

            index = spriteMonsters.IndexOf((Image)sender);
            if (index >= 0)
            {
                if (lstMonsters.SelectedIndex != -1 && lstMonsters.SelectedIndex == index)
                {
                    lstMonsters.SelectedIndex = -1;
                }
                else
                {
                    lstMonsters.SelectedItem = null;
                    lstMonsters.SelectedIndex = index;
                }
            }
        }

        private void SelectCombatant(CombatantListBoxItemTemplate item)
        {
            RemoveCombatantSelectionVisuals();

            Image image = item.Image;
            CombatantEntry combatant = item.Combatant;

            //Select image
            Canvas.SetLeft(rectCursor, Canvas.GetLeft(image) - 2);
            Canvas.SetTop(rectCursor, Canvas.GetTop(image) - 2);
            rectCursor.Visibility = Visibility.Visible;

            //Select regions
            lstRegions.UnselectAll();
            if (combatant.IsHero == false)
            {
                if (combatant.TriggerRegion1 < 15)
                {
                    lstRegions.SelectedItems.Add(lstRegions.Items[combatant.TriggerRegion1]);
                }
                if (combatant.TriggerRegion2 < 15)
                {
                    lstRegions.SelectedItems.Add(lstRegions.Items[combatant.TriggerRegion2]);
                }
            }

            //Select points
            lstPoints.UnselectAll();
            if (combatant.IsHero == false)
            {
                if (combatant.SpecialMove1 == CombatantEntry.SpecialMoveType.Point)
                {
                    lstPoints.SelectedItems.Add(lstPoints.Items[combatant.SpecialMoveTarget1]);
                }
                if (combatant.SpecialMove2 == CombatantEntry.SpecialMoveType.Point)
                {
                    lstPoints.SelectedItems.Add(lstPoints.Items[combatant.SpecialMoveTarget2]);
                }
            }

            //Highlight force members
            if (combatant.IsHero == false)
            {
                if (combatant.SpecialMove1 == CombatantEntry.SpecialMoveType.ForceMember)
                {
                    AddShadow(((CombatantListBoxItemTemplate)lstHeroes.Items[combatant.SpecialMoveTarget1]).Image);
                }
                if (combatant.SpecialMove2 == CombatantEntry.SpecialMoveType.ForceMember)
                {
                    AddShadow(((CombatantListBoxItemTemplate)lstHeroes.Items[combatant.SpecialMoveTarget2]).Image);
                }
            }

            //Highlight monsters
            if (combatant.IsHero == false)
            {
                if (combatant.SpecialMove1 == CombatantEntry.SpecialMoveType.Monster)
                {
                    AddShadow(((CombatantListBoxItemTemplate)lstMonsters.Items[combatant.SpecialMoveTarget1]).Image);
                }
                if (combatant.SpecialMove2 == CombatantEntry.SpecialMoveType.Monster)
                {
                    AddShadow(((CombatantListBoxItemTemplate)lstMonsters.Items[combatant.SpecialMoveTarget2]).Image);
                }
            }

            //Show combatant data
            gridCombatant.ItemsSource = item.Combatant.GetCombatantData();
            //textAiDescription.Text = item.Combatant.GetAiDescription();
            textAiDescription.Text = string.Empty;
            item.Combatant.PrintAiDescription(textAiDescription);
        }

        //Remove all selection visuals
        private void RemoveCombatantSelectionVisuals()
        {
            //Cursor
            rectCursor.Visibility = Visibility.Collapsed;
            //Hero effect
            foreach (CombatantListBoxItemTemplate x in lstHeroes.Items)
            {
                x.Image.Effect = null;
            }
            //Monster effect
            foreach (CombatantListBoxItemTemplate x in lstMonsters.Items)
            {
                x.Image.Effect = null;
            }
        }

        //Add a standard drop shadow effect to targets of the special AI move
        private void AddShadow(Image image)
        {
            DropShadowEffect dropShadow = new DropShadowEffect()
            {
                Color = Colors.Black,
                ShadowDepth = 4,
                BlurRadius = 4,
                RenderingBias = RenderingBias.Performance,
            };
            image.Effect = dropShadow;
        }


        //------------------------------------------------------------------------------------------
        // Region Selection
        //------------------------------------------------------------------------------------------
        private void lstRegions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (RegionEntry x in e.RemovedItems)
            {
                regions[x.Id * 2].Visibility = Visibility.Collapsed;
                regions[x.Id * 2 + 1].Visibility = Visibility.Collapsed;
            }
            foreach (RegionEntry x in e.AddedItems)
            {
                regions[x.Id * 2].Visibility = Visibility.Visible;
                regions[x.Id * 2 + 1].Visibility = Visibility.Visible;
            }
        }


        //------------------------------------------------------------------------------------------
        // Point Selection
        //------------------------------------------------------------------------------------------
        private void lstPoints_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (PointEntry x in e.RemovedItems)
            {
                points[x.Id].Visibility = Visibility.Collapsed;
            }
            foreach (PointEntry x in e.AddedItems)
            {
                points[x.Id].Visibility = Visibility.Visible;
            }
        }
    }
}