using System.IO;
using System.Diagnostics;

namespace projectgen
{

    /// <summary>
    /// General application helper functions (graceful exists, file operations)
    /// </summary>
    class ApplicationHelper
    {

        /// <summary>
        /// All currently avaialable "graceful" exit states
        /// </summary>
        public enum ExitStates
        {
            CLEAN,

            NO_BARE_PATH,
            INVALID_BARE_PATH
        }


        /// <summary>
        /// Last state the application was in
        /// </summary>
        public static ExitStates LastState;


        /// <summary>
        /// Initiate a graceful shutdown with the state provided
        /// </summary>
        /// <param name="state">ExitState to use</param>
        /// <returns>Returns the ExitState supplied in the first parameter</returns>
        public static ExitStates ExitWithState(ExitStates state)
        {
            LastState = state;
            Debug.WriteLine("");
            Debug.WriteLine(string.Format("########## Exiting with code: {0} ##########", state));
            Debug.WriteLine("");
            return LastState;
        }


        /// <summary>
        /// Helper function to recursively copy a directory and all its files
        /// </summary>
        /// <param name="source">Path to source folder</param>
        /// <param name="destination">Path to destination folder.</param>
        /// <exception cref="DirectoryNotFoundException">Source directory does not exist or could not be found:  + source</exception>
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
