using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace projectgen.Projects
{
    #region Raw JSON container
    struct ProjectJSON
    {
        public string name;
        public char mnemonic;
    }
    #endregion

    /// <summary>
    /// A project is a set of components, that can be copied
    /// </summary>
    class Project : IHasPath
    {
#region IHasPath implementation
        public string GetAbsolutePath()
        {
            return ProgramData.BareProjectRoot + FolderName + "/";
        }
#endregion

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
        /// Mnemonic that allows for easier selection using the console window
        /// </summary>
        public char Mnemonic;


        /// <summary>
        /// List of all possible components
        /// </summary>
        public List<Component> Components;

        /// <summary>
        /// List of index of Components-enties currently selected for copying
        /// </summary>
        public List<int> ActiveComponents;

        public Project(string folderName)
        {
            FolderName = folderName;

            Components = new List<Component>();
            ActiveComponents = new List<int>();

            try
            {
                ProjectJSON RawJSON = JsonConvert.DeserializeObject<ProjectJSON>(File.ReadAllText(GetAbsolutePath() + "description.json"));
                DisplayName = RawJSON.name;
                Mnemonic = RawJSON.mnemonic;
            }
            catch (System.IO.FileNotFoundException e)
            {
                DisplayName = FolderName;
                Mnemonic = FolderName[0];
            }

            foreach (string directory in Directory.GetDirectories(GetAbsolutePath()).OrderBy(d => d))
            {
                Components.Add(new Projects.Component(this, new DirectoryInfo(directory).Name));
            }
        }

        /// <summary>
        /// Clears every ReplacementRule of this object
        /// </summary>
        public void Clear()
        {
            foreach (Component component in Components)
            {
                foreach (ReplacementRule rule in component.ReplacementRules.Values)
                {
                    rule.Clear();
                }
            }
        }

        /// <summary>
        /// Starts a copy of the project to the specified file location
        /// </summary>
        /// <param name="targetLocation">Path to the destination folder</param>
        public void Copy(string targetLocation)
        {
            foreach (int index in ActiveComponents)
            {
                string tempPath = targetLocation + Components[index].FolderName + "/";
                ApplicationHelper.DirectoryCopy(Components[index].GetAbsolutePath(), tempPath);
                File.Delete(tempPath + "description.json");
                foreach (ReplacementRule rule in Components[index].ReplacementRules.Values)
                {
                    foreach (ReplacementLocation location in rule.ReplacementLocations)
                    {
                        if (location.InFileName)
                        {
                            string filename = tempPath + location.File;
                            File.Move(filename, filename.Replace(location.Wildcard, rule.ReplaceWith));
                        }
                        else
                        {
                            if (!new FileInfo(string.Format("{0}{1}/{2}", targetLocation, Components[index].FolderName, location.File)).Exists)
                            {
                                Console.WriteLine("Invalid replace-instruction!");
                            }
                            else
                            {
                                string text = File.ReadAllText(tempPath + location.File);
                                text = text.Replace(location.Wildcard, rule.ReplaceWith);
                                File.WriteAllText(tempPath + location.File, text);
                            }
                        }
                    }
                }
                ApplicationHelper.DirectoryCopy(tempPath, targetLocation);
                Directory.Delete(tempPath, true);
            }
        }
    }
}
