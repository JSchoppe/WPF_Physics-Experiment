using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;

// System params are used to position the window based on available work area.
using static System.Windows.SystemParameters;

namespace PhysicsExperiment
{
    /// <summary>Interaction logic for MainWindow.xaml</summary>
    public partial class MainWindow : Window
    {
        // Constructor.
        public MainWindow()
        {
            InitializeComponent();
        }

        // The path of the resources directory.
        private DirectoryInfo resourcesPath;

        // Ints that store the current selected options.
        // Possible TODO: Generalize this system to be a two dimensional array that
        // generates tabs for folders in player sprite components.
        private int currentSkin = 0;
        private int currentFace = 0;
        private int currentHair = 0;
        private int currentPants = 0;
        private int currentShirt = 0;
        private int currentShoes = 0;

        // Stores the HSV slider values for each tab of the customizer.
        private HSVAdjustment skinAdjust = new HSVAdjustment();
        private HSVAdjustment faceAdjust = new HSVAdjustment();
        private HSVAdjustment hairAdjust = new HSVAdjustment();
        private HSVAdjustment pantsAdjust = new HSVAdjustment();
        private HSVAdjustment shirtAdjust = new HSVAdjustment();
        private HSVAdjustment shoesAdjust = new HSVAdjustment();

        // This will run immediately after the window controls have initialized.
        private void StartupWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Set the window properties.
            Title = "Character Customizer";

            // Set the bitmap scaling mode to NN, since it is pixel art.
            VisualBitmapScalingMode = BitmapScalingMode.NearestNeighbor;

            // Determine how many pixels are being added by window border.
            // This allows us to convert between window-size and client-size.
            double BorderAddedX = Width - MainGrid.ActualWidth;
            double BorderAddedY = Height - MainGrid.ActualHeight;

            // Currently the program restricts itself to at most 2/3 of the work area.
            int screenWidth = (int)(2.0 / 3.0 * WorkArea.Width);
            int screenHeight = (int)(2.0 / 3.0 * WorkArea.Height);

            // Round down to even(so window can be centered).
            screenWidth -= screenWidth % 2;
            screenHeight -= screenHeight % 2;

            // Calculate the maximum client areas possible in both directions.
            int maxClientX = (int)(screenWidth - BorderAddedX);
            int maxClientY = (int)(screenHeight - BorderAddedY);

            // Which client maximum is more restrictive? (Given x = 2y fixed ratio).
            if (2 * maxClientY > maxClientX)
            {
                // The maximum X is more restrictive.
                StartupWindow.Width = maxClientX + BorderAddedX;
                StartupWindow.Height = (maxClientX / 2) + BorderAddedY;
            }
            else
            {
                // The maximum Y is more restrictive.
                StartupWindow.Width = (maxClientY * 2) + BorderAddedX;
                StartupWindow.Height = maxClientY + BorderAddedY;
            }


            // TODO research whether the following methodology would work in
            // a packaged project.

            // Get the directory that Game.cs is in.
            resourcesPath = new DirectoryInfo(".");

            // Cycle up through the files until the parent is the solution title.
            while (resourcesPath.Parent.Name != "PhysicsExperiment")
            {
                resourcesPath = resourcesPath.Parent;
            }

            // Get the resources folder.
            resourcesPath = new DirectoryInfo(resourcesPath.Parent.FullName + "/Resources");

            // Run code to populate the character customizer.
            PopulateCustomizer();
        }

        // Generates image controls for the customizable options.
        private void PopulateCustomizer()
        {
            // Tell the customizer to pull the relevant player sprite files into its arrays.
            CharCustomizer.Initiate(resourcesPath + "/PlayerSpriteComponents");

            // Populate the grids with the customizer arrays and bind events to them.
            PopulatePage(HairGrid, CharCustomizer.hairs, HairOption_Click);
            PopulatePage(FaceGrid, CharCustomizer.faces, FaceOption_Click);
            PopulatePage(SkinGrid, CharCustomizer.skins, SkinOption_Click);
            PopulatePage(PantsGrid, CharCustomizer.pants, PantsOption_Click);
            PopulatePage(ShirtGrid, CharCustomizer.shirts, ShirtOption_Click);
            PopulatePage(ShoesGrid, CharCustomizer.shoes, ShoesOption_Click);

            // Generate the initial avatar.
            UpdateAvatar();
        }

        // Updates the image control for the avatar.
        private void UpdateAvatar()
        {
            // Generate a bitmap by stacking the various selected layers.
            System.Drawing.Bitmap avatar = ImageTools.MergeDown(new System.Drawing.Bitmap[] {
                CharCustomizer.skins[currentSkin],
                CharCustomizer.shoes[currentShoes],
                CharCustomizer.faces[currentFace],
                CharCustomizer.hairs[currentHair],
                CharCustomizer.pants[currentPants],
                CharCustomizer.shirts[currentShirt]
            });

            // Place the new avatar into memory and set it as the control source.
            Avatar.Source = ImageTools.BitmapToImageSource(avatar);
        }

        // Passing methods as args: https://stackoverflow.com/questions/917551/func-delegate-with-no-return-type
        // Thanks to Jason: https://stackoverflow.com/users/100902/jason

        // Procedure to create all the image controls for one page of the customizer.
        private void PopulatePage(Grid toGrid, System.Drawing.Bitmap[] customMaps, Action<object, RoutedEventArgs> clickEvent)
        {
            // Variables for populating the controls for each tab in the customizer:
            double columnActualWidth = HairGrid.ActualWidth / 3; // Pixel width of the column.
            int currentColumn = 0; // Which of the three columns to place this control in.
            int currentItem = 0; // The current index of the item(gets tagged in the control name).
            double[] columnTravel = new double[3]; // The current vertical travel in each column.

            // Foreach bitmap:
            foreach (System.Drawing.Bitmap bmp in customMaps)
            {
                // Create a new image control.
                Image image = new Image();

                // Generate the icon for this option:
                // Crop the passed bitmap, then add a one pixel margin on each edge.
                System.Drawing.Bitmap cropped = ImageTools.AddMargin(ImageTools.CropByAlpha(bmp));

                // Set the source to be a cropped version of the map with one pixel added margin.
                image.Source = ImageTools.BitmapToImageSource(cropped);

                // Set the control to show the clicking hand cursor when mouse is over.
                image.ForceCursor = true;
                image.Cursor = Cursors.Hand;

                // Position this image as a child to the hair grid.
                toGrid.Children.Add(image);

                // Set the column to the currentColumn.
                Grid.SetColumn(image, currentColumn);

                // Set the alignment to fill the column and be positioned vertically from the top.
                image.VerticalAlignment = VerticalAlignment.Top;
                image.HorizontalAlignment = HorizontalAlignment.Stretch;

                // Translate the image down as the columns are cycled.
                image.Margin = new Thickness(0, columnTravel[currentColumn], 0, 0);
                columnTravel[currentColumn] += columnActualWidth * cropped.Height / cropped.Width;

                // Name the item using _ as a delimiter.
                image.Name = "item_" + currentItem;

                // Bind click to the passed method.
                image.MouseDown += new MouseButtonEventHandler(clickEvent);

                // Increment the index of the current item.
                currentItem++;
                // Update the column position(cycles 0-1-2).
                currentColumn++;
                currentColumn %= 3;
            }
        }

        // Event that happens when a customization option is clicked.
        private void HairOption_Click(object sender, RoutedEventArgs e)
        {
            // Cast the sender as an image control.
            Image image = (Image)sender;

            // Use split to get the number that was assigned to this image.
            currentHair = Convert.ToInt32(image.Name.Split('_')[1]);

            // Update the avatar image.
            UpdateAvatar();
        }

        // Event that happens when a customization option is clicked.
        private void FaceOption_Click(object sender, RoutedEventArgs e)
        {
            Image image = (Image)sender;
            currentFace = Convert.ToInt32(image.Name.Split('_')[1]);
            UpdateAvatar();
        }

        // Event that happens when a customization option is clicked.
        private void SkinOption_Click(object sender, RoutedEventArgs e)
        {
            Image image = (Image)sender;
            currentSkin = Convert.ToInt32(image.Name.Split('_')[1]);
            UpdateAvatar();
        }

        // Event that happens when a customization option is clicked.
        private void PantsOption_Click(object sender, RoutedEventArgs e)
        {
            Image image = (Image)sender;
            currentPants = Convert.ToInt32(image.Name.Split('_')[1]);
            UpdateAvatar();
        }

        // Event that happens when a customization option is clicked.
        private void ShirtOption_Click(object sender, RoutedEventArgs e)
        {
            Image image = (Image)sender;
            currentShirt = Convert.ToInt32(image.Name.Split('_')[1]);
            UpdateAvatar();
        }

        // Event that happens when a customization option is clicked.
        private void ShoesOption_Click(object sender, RoutedEventArgs e)
        {
            Image image = (Image)sender;
            currentShoes = Convert.ToInt32(image.Name.Split('_')[1]);
            UpdateAvatar();
        }

        // Updates the HSV slider values when switching tabs.
        private void CharacterTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // When the tab has changed, set the sliders to the appropriate positions.
            if (CharacterTabs.SelectedItem == HairTab)
            {
                HueSlider.Value = hairAdjust.hueAdjust;
                SatSlider.Value = hairAdjust.satAdjust;
                ValSlider.Value = hairAdjust.valAdjust;
            }
            else if (CharacterTabs.SelectedItem == FaceTab)
            {
                HueSlider.Value = faceAdjust.hueAdjust;
                SatSlider.Value = faceAdjust.satAdjust;
                ValSlider.Value = faceAdjust.valAdjust;
            }
            else if (CharacterTabs.SelectedItem == SkinTab)
            {
                HueSlider.Value = skinAdjust.hueAdjust;
                SatSlider.Value = skinAdjust.satAdjust;
                ValSlider.Value = skinAdjust.valAdjust;
            }
            else if (CharacterTabs.SelectedItem == ShirtTab)
            {
                HueSlider.Value = shirtAdjust.hueAdjust;
                SatSlider.Value = shirtAdjust.satAdjust;
                ValSlider.Value = shirtAdjust.valAdjust;
            }
            else if (CharacterTabs.SelectedItem == PantsTab)
            {
                HueSlider.Value = pantsAdjust.hueAdjust;
                SatSlider.Value = pantsAdjust.satAdjust;
                ValSlider.Value = pantsAdjust.valAdjust;
            }
            else // ShoesTab.
            {
                HueSlider.Value = shoesAdjust.hueAdjust;
                SatSlider.Value = shoesAdjust.satAdjust;
                ValSlider.Value = shoesAdjust.valAdjust;
            }
        }

        // Closes the window when the quit button is clicked.
        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Define a class to save adjustment values for each customization tab.
        private class HSVAdjustment
        {
            public int hueAdjust = 0;    //[-180, +180]
            public double satAdjust = 0; //[-1, +1]
            public double valAdjust = 0; //[-1, +1]
        }

        // Redraw options when the hue slider value changes.
        private void HueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Based on the tab, update slider values. Then apply adjustments.
            if (CharacterTabs.SelectedItem == HairTab)
            {
                hairAdjust.hueAdjust = (int)HueSlider.Value;
            }
            else if (CharacterTabs.SelectedItem == FaceTab)
            {
                faceAdjust.hueAdjust = (int)HueSlider.Value;
            }
            else if (CharacterTabs.SelectedItem == SkinTab)
            {
                skinAdjust.hueAdjust = (int)HueSlider.Value;
            }
            else if (CharacterTabs.SelectedItem == ShirtTab)
            {
                shirtAdjust.hueAdjust = (int)HueSlider.Value;
            }
            else if (CharacterTabs.SelectedItem == PantsTab)
            {
                pantsAdjust.hueAdjust = (int)HueSlider.Value;
            }
            else // ShoesTab.
            {
                shoesAdjust.hueAdjust = (int)HueSlider.Value;
            }

            // Redraw the avatar.
            UpdateAvatar();
        }

        // Redraw options when the saturation slider value changes.
        private void SatSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Based on the tab, update slider values. Then apply adjustments.
            if (CharacterTabs.SelectedItem == HairTab)
            {
                hairAdjust.satAdjust = SatSlider.Value;
            }
            else if (CharacterTabs.SelectedItem == FaceTab)
            {
                faceAdjust.satAdjust = SatSlider.Value;
            }
            else if (CharacterTabs.SelectedItem == SkinTab)
            {
                skinAdjust.satAdjust = SatSlider.Value;
            }
            else if (CharacterTabs.SelectedItem == ShirtTab)
            {
                shirtAdjust.satAdjust = SatSlider.Value;
            }
            else if (CharacterTabs.SelectedItem == PantsTab)
            {
                pantsAdjust.satAdjust = SatSlider.Value;
            }
            else // ShoesTab.
            {
                shoesAdjust.satAdjust = SatSlider.Value;
            }

            // Redraw the avatar.
            UpdateAvatar();
        }

        // Redraw options when the value slider value changes.
        private void ValSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Based on the tab, update slider values. Then apply adjustments.
            if (CharacterTabs.SelectedItem == HairTab)
            {
                hairAdjust.valAdjust = ValSlider.Value;
            }
            else if (CharacterTabs.SelectedItem == FaceTab)
            {
                faceAdjust.valAdjust = ValSlider.Value;
            }
            else if (CharacterTabs.SelectedItem == SkinTab)
            {
                skinAdjust.valAdjust = ValSlider.Value;
            }
            else if (CharacterTabs.SelectedItem == ShirtTab)
            {
                shirtAdjust.valAdjust = ValSlider.Value;
            }
            else if (CharacterTabs.SelectedItem == PantsTab)
            {
                pantsAdjust.valAdjust = ValSlider.Value;
            }
            else // ShoesTab.
            {
                shoesAdjust.valAdjust = ValSlider.Value;
            }

            // Redraw the avatar.
            UpdateAvatar();
        }
    }
}