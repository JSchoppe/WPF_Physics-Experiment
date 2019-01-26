using System.IO;
using System.Drawing;

namespace PhysicsExperiment
{
    /// <summary>Contains information for the customizer</summary>
    public static class CharCustomizer
    {
        // Storage for the cutomization options.
        public static Bitmap[] skins;
        public static Bitmap[] faces;
        public static Bitmap[] hairs;
        public static Bitmap[] pants;
        public static Bitmap[] shirts;
        public static Bitmap[] shoes;

        // Run this to populate the options from the resources folder.
        public static void Initiate(string spritePath)
        {
            skins = ArrayFromBitmapFolder(spritePath + "/Skins");
            faces = ArrayFromBitmapFolder(spritePath + "/Faces");
            hairs = ArrayFromBitmapFolder(spritePath + "/Hairs");
            pants = ArrayFromBitmapFolder(spritePath + "/Pants");
            shirts = ArrayFromBitmapFolder(spritePath + "/Shirts");
            shoes = ArrayFromBitmapFolder(spritePath + "/Shoes");
        }

        // Pull bitmaps from a folder location.
        private static Bitmap[] ArrayFromBitmapFolder(string folderPath)
        {
            // Get the files from the directory.
            string[] files = Directory.GetFiles(folderPath);

            // Generate a new bitmap array to return.
            Bitmap[] returnArray = new Bitmap[files.Length];

            // Add each file to the return array.
            for (int i = 0; i < files.Length; i++)
            {
                // TODO add sanity check to make sure files are of PNG type.
                returnArray[i] = new Bitmap(files[i]);
            }

            // Return the array to populate.
            return returnArray;
        }
    }
}