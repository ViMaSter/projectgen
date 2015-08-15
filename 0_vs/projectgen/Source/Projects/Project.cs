using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace projectgen.Projects
{
    struct ProjectJSON
    {
        public string name;
        public char mnemonic;
    }
    class Project : IHasPath
    {
#region IHasPath implementation
        public string GetAbsolutePath()
        {
            return ProgramData.BareProjectRoot + FolderName + "/";
        }
#endregion

        public string FolderName;
        public string DisplayName;
        public char Mnemonic;

        public List<Component> Components;
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
                        if (!new FileInfo(string.Format("{0}{1}/{2}", targetLocation, Components[index].FolderName, location.File)).Exists)
                        {
                            Console.WriteLine("Invalid replace-instruction!");
                        }
                        else
                        {
                            string text = File.ReadAllText(Components[index].GetAbsolutePath() + location.File);
                            text = text.Replace(location.Wildcard, rule.ReplaceWith);
                            File.WriteAllText(Components[index].GetAbsolutePath() + location.File, text);
                        }
                    }
                }
                ApplicationHelper.DirectoryCopy(tempPath, targetLocation);
                Directory.Delete(tempPath, true);
            }
        }
    }
}
