using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;

using static PhysicsExperiment.ImageTools;
using static System.Windows.SystemParameters;

namespace PhysicsExperiment
{
    /// <summary>Interaction logic for MainWindow.xaml</summary>
    public partial class MainWindow : Window
    {
        // The path of the resources directory.
        private DirectoryInfo resourcesPath;

        // Ints that store the current selected options.
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

        // Default constructor.
        public MainWindow()
        {
            InitializeComponent();
        }

        // This will run immediately after the window controls have initialized.
        private void StartupWindow_Loaded(object sender, RoutedEventArgs e)
        {
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

            // Which client maximum is more restrictive? (Given x = y fixed ratio).
            if (maxClientY > maxClientX)
            {
                // The maximum X is more restrictive.
                StartupWindow.Width = maxClientX + BorderAddedX;
                StartupWindow.Height = maxClientX + BorderAddedY;
            }
            else
            {
                // The maximum Y is more restrictive.
                StartupWindow.Width = maxClientY + BorderAddedX;
                StartupWindow.Height = maxClientY + BorderAddedY;
            }

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
            System.Drawing.Bitmap avatar = MergeDown(new System.Drawing.Bitmap[] {
                AdjustHSV(CharCustomizer.skins[currentSkin], skinAdjust.hue, skinAdjust.sat, skinAdjust.val),
                AdjustHSV(CharCustomizer.shoes[currentShoes], shoesAdjust.hue, shoesAdjust.sat, shoesAdjust.val),
                AdjustHSV(CharCustomizer.faces[currentFace], faceAdjust.hue, faceAdjust.sat, faceAdjust.val),
                AdjustHSV(CharCustomizer.hairs[currentHair], hairAdjust.hue, hairAdjust.sat, hairAdjust.val),
                AdjustHSV(CharCustomizer.pants[currentPants], pantsAdjust.hue, pantsAdjust.sat, pantsAdjust.val),
                AdjustHSV(CharCustomizer.shirts[currentShirt], shirtAdjust.hue, shirtAdjust.sat, shirtAdjust.val)
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
                HueSlider.Value = hairAdjust.hue;
                SatSlider.Value = hairAdjust.sat;
                ValSlider.Value = hairAdjust.val;
            }
            else if (CharacterTabs.SelectedItem == FaceTab)
            {
                HueSlider.Value = faceAdjust.hue;
                SatSlider.Value = faceAdjust.sat;
                ValSlider.Value = faceAdjust.val;
            }
            else if (CharacterTabs.SelectedItem == SkinTab)
            {
                HueSlider.Value = skinAdjust.hue;
                SatSlider.Value = skinAdjust.sat;
                ValSlider.Value = skinAdjust.val;
            }
            else if (CharacterTabs.SelectedItem == ShirtTab)
            {
                HueSlider.Value = shirtAdjust.hue;
                SatSlider.Value = shirtAdjust.sat;
                ValSlider.Value = shirtAdjust.val;
            }
            else if (CharacterTabs.SelectedItem == PantsTab)
            {
                HueSlider.Value = pantsAdjust.hue;
                SatSlider.Value = pantsAdjust.sat;
                ValSlider.Value = pantsAdjust.val;
            }
            else // ShoesTab.
            {
                HueSlider.Value = shoesAdjust.hue;
                SatSlider.Value = shoesAdjust.sat;
                ValSlider.Value = shoesAdjust.val;
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
            public int hue = 0;    //[-180, +180]
            public double sat = 0; //[-1, +1]
            public double val = 0; //[-1, +1]
        }

        // Bools that track whether the sliders are being dragged.
        // This prevents color from recalculating every tick of the slide.
        private bool hueSliding = false;
        private bool satSliding = false;
        private bool valSliding = false;

        // Hue Slider:
        // Set sliding to true.
        private void HueSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            hueSliding = true;
        }

        // Set sliding to false and update the colors.
        private void HueSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            hueSliding = false;
            UpdateHue();
        }

        // Update the colors if the change wasn't in a drag.
        private void HueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!hueSliding)
            {
                UpdateHue();
            }
        }

        // Update the hue and redraw character.
        private void UpdateHue()
        {
            // Based on the tab, update slider values. Then apply adjustments.
            if (CharacterTabs.SelectedItem == HairTab)
            {
                hairAdjust.hue = (int)HueSlider.Value;
            }
            else if (CharacterTabs.SelectedItem == FaceTab)
            {
                faceAdjust.hue = (int)HueSlider.Value;
            }
            else if (CharacterTabs.SelectedItem == SkinTab)
            {
                skinAdjust.hue = (int)HueSlider.Value;
            }
            else if (CharacterTabs.SelectedItem == ShirtTab)
            {
                shirtAdjust.hue = (int)HueSlider.Value;
            }
            else if (CharacterTabs.SelectedItem == PantsTab)
            {
                pantsAdjust.hue = (int)HueSlider.Value;
            }
            else // ShoesTab.
            {
                shoesAdjust.hue = (int)HueSlider.Value;
            }

            // Redraw the avatar.
            UpdateAvatar();
        }

        // Sat Slider:
        // Set sliding to true.
        private void SatSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            satSliding = true;
        }

        // Set sliding to false and update the colors.
        private void SatSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            satSliding = false;
            UpdateSat();
        }

        // Update the colors if the change wasn't in a drag.
        private void SatSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!satSliding)
            {
                UpdateSat();
            }
        }

        // Update the saturation and redraw character.
        private void UpdateSat()
        {
            // Based on the tab, update slider values. Then apply adjustments.
            if (CharacterTabs.SelectedItem == HairTab)
            {
                hairAdjust.sat = SatSlider.Value;
            }
            else if (CharacterTabs.SelectedItem == FaceTab)
            {
                faceAdjust.sat = SatSlider.Value;
            }
            else if (CharacterTabs.SelectedItem == SkinTab)
            {
                skinAdjust.sat = SatSlider.Value;
            }
            else if (CharacterTabs.SelectedItem == ShirtTab)
            {
                shirtAdjust.sat = SatSlider.Value;
            }
            else if (CharacterTabs.SelectedItem == PantsTab)
            {
                pantsAdjust.sat = SatSlider.Value;
            }
            else // ShoesTab.
            {
                shoesAdjust.sat = SatSlider.Value;
            }

            // Redraw the avatar.
            UpdateAvatar();
        }

        // Val Slider:
        // Set sliding to true.
        private void ValSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            valSliding = true;
        }

        // Set sliding to false and update the colors.
        private void ValSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            valSliding = false;
            UpdateVal();
        }

        // Update the colors if the change wasn't in a drag.
        private void ValSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!valSliding)
            {
                UpdateVal();
            }
        }

        // Update the value and redraw character.
        private void UpdateVal()
        {
            // Based on the tab, update slider values. Then apply adjustments.
            if (CharacterTabs.SelectedItem == HairTab)
            {
                hairAdjust.val = ValSlider.Value;
            }
            else if (CharacterTabs.SelectedItem == FaceTab)
            {
                faceAdjust.val = ValSlider.Value;
            }
            else if (CharacterTabs.SelectedItem == SkinTab)
            {
                skinAdjust.val = ValSlider.Value;
            }
            else if (CharacterTabs.SelectedItem == ShirtTab)
            {
                shirtAdjust.val = ValSlider.Value;
            }
            else if (CharacterTabs.SelectedItem == PantsTab)
            {
                pantsAdjust.val = ValSlider.Value;
            }
            else // ShoesTab.
            {
                shoesAdjust.val = ValSlider.Value;
            }

            // Redraw the avatar.
            UpdateAvatar();
        }
    }
}