using System;
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace projectgen.Projects
{
    class ReplaceJSON
    {
        public string file;
        public string wildcard;
    }
    class FieldJSON
    {
        public string name;
        public List<ReplaceJSON> replace;
    }
    class ComponentJSON
    {
        public string name;
        public List<FieldJSON> fields;
    }

    class Component : IHasPath
    {
#region IHasPath implementation
        public string GetAbsolutePath()
        {
            return AssociatedProject.GetAbsolutePath() + FolderName + "/";
        }
#endregion

        // Required to build absolute paths
        public Project AssociatedProject;

        public string FolderName;
        public string DisplayName;

        // Key: Visual name
        // Value: List of replacement rules
        public Dictionary<string, ReplacementRule> ReplacementRules;

        public Component(Project associatedProject, string folderName)
        {
            AssociatedProject = associatedProject;
            FolderName = folderName;
            DisplayName = "";

            ReplacementRules = new Dictionary<string, ReplacementRule>();

            try
            {
                ComponentJSON RawJSON = JsonConvert.DeserializeObject<ComponentJSON>(File.ReadAllText(GetAbsolutePath() + "description.json"));
                //DisplayName = RawJSON.name;
                DisplayName = RawJSON.name;
                foreach (FieldJSON field in RawJSON.fields)
                {
                    ReplacementRules.Add(field.name, new ReplacementRule(this));
                    foreach (ReplaceJSON replace in field.replace)
                    {
                        ReplacementRules[field.name].ReplacementLocations.Add(new ReplacementLocation(replace.file, replace.wildcard));
                    }
                }
            }
            catch (Exception e) { }

            if (DisplayName == "")
            {
              DisplayName = FolderName;
            }
        }
    }
}
