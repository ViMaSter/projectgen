using System;
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace projectgen.Projects
{
    #region Raw JSON container
    class ReplaceJSON
    {
        public string file;
        public string wildcard;
        public bool inFileName;
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
    #endregion

    /// <summary>
    /// A component is a part of a project that can be selected for copying
    /// </summary>
    class Component : IHasPath
    {
#region IHasPath implementation
        public string GetAbsolutePath()
        {
            return AssociatedProject.GetAbsolutePath() + FolderName + "/";
        }
#endregion


        /// <summary>
        /// The associated project
        /// 
        /// Required to build absolute paths
        /// </summary>
        public Project AssociatedProject;


        /// <summary>
        /// Name of the folder in the project-root that contains this component
        /// </summary>
        public string FolderName;

        /// <summary>
        /// Nicer display name of this component
        /// 
        /// Can be set via the description.json-file
        /// </summary>
        public string DisplayName;


        /// <summary>
        /// A set of replacement rules
        /// 
        /// Key: Display name of the variable that's being replaced
        /// Value: Link to a ReplacementRule-object
        /// </summary>
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
                        ReplacementRules[field.name].ReplacementLocations.Add(new ReplacementLocation(replace.file, replace.wildcard, replace.inFileName));
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
