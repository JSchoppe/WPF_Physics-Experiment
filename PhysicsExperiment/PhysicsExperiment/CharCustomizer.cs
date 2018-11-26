using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace PhysicsExperiment
{
    public static class CharCustomizer
    {
        public static Bitmap[] skins;
        public static Bitmap[] faces;
        public static Bitmap[] hairs;
        public static Bitmap[] pants;
        public static Bitmap[] shirts;
        public static Bitmap[] shoes;

        public static void Initiate(string spritePath)
        {
            skins = ArrayFromBitmapFolder(spritePath + "/Skins");
            faces = ArrayFromBitmapFolder(spritePath + "/Faces");
            hairs = ArrayFromBitmapFolder(spritePath + "/Hairs");
            pants = ArrayFromBitmapFolder(spritePath + "/Pants");
            shirts = ArrayFromBitmapFolder(spritePath + "/Shirts");
            shoes = ArrayFromBitmapFolder(spritePath + "/Shoes");
        }

        private static Bitmap[] ArrayFromBitmapFolder(string folderPath)
        {
            string[] files = Directory.GetFiles(folderPath);
            Bitmap[] returnArray = new Bitmap[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                returnArray[i] = new Bitmap(files[i]);
            }

            return returnArray;
        }
    }
}