using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace PhysicsExperiment
{
    public class RuntimeDataManager
    {
        // The whole path for the app data folder for this application.
        private string appDataFolder;

        // The name of the folder in app data.
        private readonly string folderName = "JS_CharCustomizerData";

        public RuntimeDataManager()
        {
            // Get the path to appdata plus this applications folder name.
            appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/" + folderName;

            // If the directory doesn't exist, create it.
            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }
        }

        // Code example for saving bitmaps, answered by Jay T(https://stackoverflow.com/users/5844387/jay-t)
        // https://stackoverflow.com/questions/35804375/how-do-i-save-a-bitmapimage-from-memory-into-a-file-in-wpf-c

        /// <summary>Writes a PNG file to the appdata folder</summary>
        /// <param name="toWrite">The bitmapImage to write</param>
        /// <param name="name">The name for the file(will overwrite)</param>
        public void WriteData(BitmapImage toWrite, string name)
        {
            // Create an encoder to write the file.
            BitmapEncoder encoder = new PngBitmapEncoder();

            // Add the passed image as the first(and only) frame to be written.
            encoder.Frames.Add(BitmapFrame.Create(toWrite));

            // Create a files stream for this operation.
            using (FileStream stream = new FileStream(appDataFolder + "/" + name + ".png", FileMode.Create))
            {
                // Write to the stream using the png bitmap encoder.
                encoder.Save(stream);
            }
        }

        public BitmapImage LoadImage(string name)
        {
            Bitmap bitmap = new Bitmap(appDataFolder + "/" + name);

            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}