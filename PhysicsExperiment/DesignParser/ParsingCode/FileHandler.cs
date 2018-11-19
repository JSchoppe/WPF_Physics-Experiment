using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DesignParser.ParsingCode
{
    class FileHandler
    {
        // Get the local directory.
        DirectoryInfo local = new DirectoryInfo(".");

        /// <summary>Creates the file handler and checks for paths</summary>
        public FileHandler()
        {
            
        }

        //private DirectoryInfo SearchUp(DirectoryInfo fromDirectory, string toDirectory)
        //{
        //    DirectoryInfo currentDirectory = fromDirectory;
        //    while (currentDirectory.Name != toDirectory)
        //    {
        //        if (currentDirectory.Parent != null)
        //        {

        //        }
        //    }
        //}
    }
}