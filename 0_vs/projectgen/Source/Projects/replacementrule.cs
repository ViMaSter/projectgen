using System.Collections.Generic;

namespace projectgen.Projects
{

    /// <summary>
    /// A wildcard to replace in a certain file
    /// </summary>
    class ReplacementLocation
    {

        /// <summary>
        /// Path to the file (relative to the component root!)
        /// </summary>
        public string File;

        /// <summary>
        /// Wildcard to replace
        /// </summary>
        public string Wildcard;

        /// <summary>
        /// Whether or not the wildcard is located in the filename
        /// or the files content
        /// </summary>
        public bool InFileName;

        public ReplacementLocation(string file, string wildcard, bool inFileName)
        {
            File = file;
            Wildcard = wildcard;
            InFileName = inFileName;
        }
    }

    /// <summary>
    /// Association of locations for a replacement and their new value
    /// </summary>
    class ReplacementRule
    {

        /// <summary>
        /// The associated component
        /// </summary>
        public Component AssociatedComponent;

        /// <summary>
        /// New value to replace the wildcard with
        /// </summary>
        public string ReplaceWith;


        /// <summary>
        /// All occurences where the replacement has to take place
        /// </summary>
        public List<ReplacementLocation> ReplacementLocations;

        public ReplacementRule(Component associatedComponent)
        {
            AssociatedComponent = associatedComponent;
            ReplacementLocations = new List<ReplacementLocation>();

            ReplaceWith = "";
        }


        /// <summary>
        /// Sets the value to replace with
        /// </summary>
        /// <param name="replaceWith">Value to replace with</param>
        public void SetValue(string replaceWith)
        {
            ReplaceWith = replaceWith;
        }

        /// <summary>
        /// Clears the value to replace with.
        /// </summary>
        public void Clear()
        {
            ReplaceWith = "";
        }
    }
}