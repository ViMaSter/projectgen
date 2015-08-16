using System;
using System.Collections.Generic;
using System.IO;

namespace projectgen
{

    /// <summary>
    /// Container for globally used variables
    /// 
    /// Run Init() with the path to the folder containing
    /// the setups supplied by this tool
    /// </summary>
    class ProgramData
    {

        /// <summary>
        /// Root folder of all "bare projects" that are served
        /// </summary>
        public static string BareProjectRoot;

        /// <summary>
        /// Command line argument number 2 can be a target location.
        /// The path will be saved here.
        /// 
        /// If this variable is "", the tool will ask for a target
        /// location before initiating the copy
        /// </summary>
        public static string SuppliedTargetLocation = "";

        /// <summary>
        /// Target location when copying a project
        /// </summary>
        public static string TargetLocation = "";

        /// <summary>
        /// List of all successfully parsed projects
        /// </summary>
        public static List<Projects.Project> ProjectList;

        public static void Init(string bareProjectRoot)
        {
            BareProjectRoot = bareProjectRoot;
            ProjectList = new List<Projects.Project>();

            foreach (string directory in Directory.GetDirectories(BareProjectRoot))
            {
                ProjectList.Add(new Projects.Project(new DirectoryInfo(directory).Name));
            }
        }
    }


    /// <summary>
    /// Class that manages the console interface (view)
    /// </summary>
    class ProgramInterface
    {
        /// <summary>
        /// Current project selected to be copied
        /// </summary>
        public static Projects.Project CurrentProject;

        #region Interface states
        /// <summary>
        /// Each state has an associated function that is called,
        /// until another state is set here
        /// 
        /// Also used to gracefully terminate the program (ProgramInterface.Exit)
        /// </summary>
        public static State NextState = State.None;

        /// <summary>
        /// Possible states the interface can be in
        /// </summary>
        public enum State {
            None,
            MainMenu,
            ProjectSelector,
            ComponentSelector,
            ReplacementFiller,
            CopyProject,

            Exit
        }

        public static void MainMenu()
        {
            Console.Clear();
            Console.WriteLine("Available options:");
            Console.WriteLine("1: Create project");
            Console.WriteLine("");
            if (ProgramData.SuppliedTargetLocation != "")
            {
                Console.WriteLine("c: Clear target location");
                Console.Write("   Currently set to: ");
                Console.Write(ProgramData.SuppliedTargetLocation);
                Console.WriteLine("");
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("");
            }
            Console.WriteLine("");
            Console.WriteLine("←: Exit");

            while (!Console.KeyAvailable) { }
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            switch (keyInfo.Key)
            {
                case System.ConsoleKey.C:
                    ProgramData.SuppliedTargetLocation = "";
                    break;
                case System.ConsoleKey.D1:
                    NextState = State.ProjectSelector;
                    break;
                case System.ConsoleKey.Backspace:
                case System.ConsoleKey.LeftArrow:
                case System.ConsoleKey.X:
                    NextState = State.Exit;
                    break;
            }
        }

        public static void ProjectSelector()
        {
            Console.Clear();
            Console.WriteLine("Available options:");

            foreach (Projects.Project prj in ProgramData.ProjectList)
            {
                Console.WriteLine(string.Format("{0}: {1}", prj.Mnemonic, prj.DisplayName));
            }
            // Itt prjs

            Console.WriteLine("");
            Console.WriteLine("←: Exit");

            while (!Console.KeyAvailable) { }
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            foreach (Projects.Project prj in ProgramData.ProjectList)
            {
                if (prj.Mnemonic == keyInfo.KeyChar)
                {
                    CurrentProject = prj;
                    NextState = State.ComponentSelector;
                    return;
                }
            }

            switch (keyInfo.Key)
            {
                case System.ConsoleKey.Backspace:
                case System.ConsoleKey.LeftArrow:
                case System.ConsoleKey.X:
                    NextState = State.MainMenu;
                    break;
            }
        }

        public static void ComponentSelector()
        {
            Console.Clear();
            Console.WriteLine("Available options:");

            for (int i = 0; i < CurrentProject.Components.Count; i++)
            {
                Console.WriteLine(string.Format("{0} [{1}]: {2}", (char)('0' + (i+1)), (CurrentProject.ActiveComponents.Contains(i) ? 'x' : ' '), CurrentProject.Components[i].DisplayName));
            }

            Console.WriteLine("");
            Console.WriteLine("←: Exit");

            while (!Console.KeyAvailable) { }
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            for (int i = 0; i < CurrentProject.Components.Count; i++)
            {
                if (keyInfo.KeyChar == '0' + (i + 1))
                {
                    if (CurrentProject.ActiveComponents.Contains(i))
                    {
                        CurrentProject.ActiveComponents.Remove(i);
                        return;
                    }
                    else
                    {
                        CurrentProject.ActiveComponents.Add(i);
                        return;
                    }
                }
            }

            switch (keyInfo.Key)
            {
                case System.ConsoleKey.Enter:
                    if (CurrentProject.ActiveComponents.Count > 0)
                    {
                        NextState = State.ReplacementFiller;
                    }
                    else
                    {
                        NextState = State.ProjectSelector;
                    }
                    break;
                case System.ConsoleKey.Backspace:
                case System.ConsoleKey.LeftArrow:
                case System.ConsoleKey.X:
                    NextState = State.ProjectSelector;
                    break;
            }
        }

        public static void ReplacementFiller()
        {
            for (int i = 0; i < CurrentProject.ActiveComponents.Count; i++)
            {
                foreach (KeyValuePair<string, Projects.ReplacementRule> item in CurrentProject.Components[CurrentProject.ActiveComponents[i]].ReplacementRules)
                {
                    Console.Clear();
                    Console.WriteLine("Set up required variables");
                    Console.WriteLine(string.Format("Component:   \"{0}\"", CurrentProject.Components[CurrentProject.ActiveComponents[i]].DisplayName));
                    Console.WriteLine(string.Format("Key:         \"{0}\"", item.Key));
                    Console.WriteLine("");
                    Console.WriteLine("Enter value:   (Leave blank to go back one component)");
                    string answer = Console.ReadLine();
                    if (answer == "")
                    {
                        if (i == 0)
                        {
                            NextState = State.ComponentSelector;
                            return;
                        }
                        else
                        {
                            i--;
                            return;
                        }
                    }
                    else
                    {
                        item.Value.SetValue(answer);
                    }

                    Console.WriteLine("");
                    Console.WriteLine("←: Exit");
                }
            }

            // start copying
            NextState = State.CopyProject;
        }

        public static void CopyProject()
        {
            Console.Clear();

            // Immediately start, if we supplied a target location as second parameter
            if (ProgramData.SuppliedTargetLocation != "")
            {
                ProgramData.TargetLocation = ProgramData.SuppliedTargetLocation;
            }
            else
            {
                Console.WriteLine("Absolute target path (WITH trailing slash):");
                ProgramData.TargetLocation = Console.ReadLine();
                if (ProgramData.TargetLocation == "")
                {
                    NextState = State.ComponentSelector;
                    return;
                }

                if (!Path.IsPathRooted(ProgramData.TargetLocation))
                {
                    return;
                }
            }

            Console.Clear();
            Console.WriteLine("Target location: " + ProgramData.TargetLocation);
            Console.WriteLine("Project: " + CurrentProject.DisplayName);
            Console.Write("Selected components: ");
            for (int i = 0; i < CurrentProject.ActiveComponents.Count; i++ )
            {
                if (i != 0)
                {
                    Console.Write(", ");
                }

                Console.Write(CurrentProject.Components[CurrentProject.ActiveComponents[i]].DisplayName);
            }

            Console.WriteLine("");
            Console.WriteLine("");

            Console.WriteLine("Copying files...");

            Console.WriteLine("");
            CurrentProject.Copy(ProgramData.TargetLocation);

            Console.WriteLine("");
            Console.WriteLine("Success! Press any key!");
            Console.ReadLine();
            NextState = State.MainMenu;
        }
        #endregion
    }


    /// <summary>
    /// Class for main entry point
    /// </summary>
    class Program
    {

        /// <summary>
        /// Main entry point
        /// </summary>
        /// <param name="args">Command line arguments
        /// 
        /// Only the first one is used: Absolute path to the root-directory of bare projects
        /// </param>
        static void Main(string[] args)
        {
            if (args.Length >= 1)
            {
                if (args.Length >= 2)
                {
                    if (Path.IsPathRooted(args[1]))
                    {
                        ProgramData.SuppliedTargetLocation = args[1];
                    }
                }

                if (Directory.Exists(args[0])) {
                    ProgramData.Init(args[0]);
                    ProgramInterface.NextState = ProgramInterface.State.MainMenu;

                    while (ProgramInterface.NextState != ProgramInterface.State.Exit)
                    {
                        switch (ProgramInterface.NextState)
                        {
                            case ProgramInterface.State.MainMenu:
                                ProgramInterface.MainMenu();
                                break;
                            case ProgramInterface.State.ProjectSelector:
                                ProgramInterface.ProjectSelector();
                                break;
                            case ProgramInterface.State.ComponentSelector:
                                ProgramInterface.ComponentSelector();
                                break;
                            case ProgramInterface.State.ReplacementFiller:
                                ProgramInterface.ReplacementFiller();
                                break;
                            case ProgramInterface.State.CopyProject:
                                ProgramInterface.CopyProject();
                                break;
                        }
                    }
                    ApplicationHelper.ExitWithState(ApplicationHelper.ExitStates.CLEAN);
                }
                else
                {
                    ApplicationHelper.ExitWithState(ApplicationHelper.ExitStates.INVALID_BARE_PATH);
                    return;
                }
            }
            else
            {
                ApplicationHelper.ExitWithState(ApplicationHelper.ExitStates.NO_BARE_PATH);
                return;
            }
        }


    }
}
