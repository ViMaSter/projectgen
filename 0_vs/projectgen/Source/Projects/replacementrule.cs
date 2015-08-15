using System.Collections.Generic;

namespace projectgen.Projects
{
    class ReplacementLocation
    {
        public string File;
        public string Wildcard;

        public ReplacementLocation(string file, string wildcard)
        {
            File = file;
            Wildcard = wildcard;
        }
    }

    class ReplacementRule
    {
        public Component AssociatedComponent;
        public string ReplaceWith;
        public List<ReplacementLocation> ReplacementLocations;

        public ReplacementRule(Component associatedComponent)
        {
            AssociatedComponent = associatedComponent;
            ReplacementLocations = new List<ReplacementLocation>();

            ReplaceWith = "";
        }

        public void SetValue(string replaceWith)
        {
            ReplaceWith = replaceWith;
        }

        public void Clear()
        {
            ReplaceWith = "";
        }
    }
}