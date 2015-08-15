using System.IO;
using System.Diagnostics;

namespace projectgen
{
    class ApplicationHelper
    {
        public enum States
        {
            CLEAN,

            NO_BARE_PATH,
            INVALID_BARE_PATH
        }

        public static States LastState;

        public static States ExitWithState(States state)
        {
            LastState = state;
            Debug.WriteLine("");
            Debug.WriteLine(string.Format("########## Exiting with code: {0} ##########", state));
            Debug.WriteLine("");
            return LastState;
        }

        // Based of https://msdn.microsoft.com/en-us/library/bb762914.aspx
        public static void DirectoryCopy(string source, string destination) {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(source);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + source);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destination, file.Name);
                file.CopyTo(temppath, true);
            }

            // Copy subdirectories and their contents to new location. 
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destination, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath);
            }
        }
    }
}
