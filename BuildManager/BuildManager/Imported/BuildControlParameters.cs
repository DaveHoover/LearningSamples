//
// Copyright (c) Microsoft Corporation.  All rights reserved.
//
//
// Use of this source code is subject to the terms of the Microsoft
// premium shared source license agreement under which you licensed
// this source code. If you did not accept the terms of the license
// agreement, you are not authorized to use this source code.
// For the terms of the license, please see the license agreement
// signed by you and Microsoft.
// THE SOURCE CODE IS PROVIDED "AS IS", WITH NO WARRANTIES OR INDEMNITIES.
//
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;
using Microsoft.WindowsAPICodePack.Shell;


namespace BuildManager
{

    public enum BuildTypeEnum
    {
        None,
        Daily,
        Lkg
    }

    public enum ExecutionStatus
    {
        Idle,
        Waiting,
        UninstallWdpt,
        InstallWdpt,
        SynEnlistment,
        Running
    }

    public enum YdrBranch
    {
        Mainline,
        Wm7_AppPlatform,
        Wm7_AppPlatform_DevDiv
    }


    public class BuildControlParameters
    {
        /// <summary>
        /// Split string for creating an array of output from command line programs.
        /// </summary>
        public const string CommandLineProgLineSplit = "\r\n";

        public const string LoggingRelativePath = "Logging";

        public const string BuildRelativePath = "BuildResults";

        public const string WpdtRelativePath = "WpdtLkg";

        public const string SwanRelativePath = "Swan";

        public const string SwanTestCasesRelativePath = "SwanTestCases";


        public const string ProgramStatusFileName = "ExecutionStatus.txt";

        public const string SwanControllerExeName = "Controller.exe";

        public const string FileToModifyForW2k8 = "baseline.dat";

        public const string WpdtLkgRoot = @"\\winphonelabs\securestorage\Projects\All\AppDev\Drops\LKG\WPDT.lnk";

        //public const string WpdtLkgRoot = @"\\wpdtselfhost\Drops\PublicPreview-LKG5\WPDT\30319.84\enu\vm\exp\cd";

        public const string ReleaseShare = @"\\build\release\HeroApps";

        public const string SignedXapOrigFileExtension = ".old";

        public const string LicenseNameAddition = "_License.xml";

        public const string ApplicationParentDefaultFolderName = "HeroApps";

        public const string YdrGuid = "{B86149D3-18A2-41FD-A153-60AF944E47FE}";

        public const string YdrRootFolder = @"\\build\Release\DeveloperTools\YDR";

        public const string YdrLatestBuildFolderFile = "setbld.bat";

        public const string YdrInstallPathFromBranchRoot =
            @"drop\ApTools\SdkSetup\out10.0\WindowsPhoneDeveloperResources.msi";

        public const Int32 DefaultInstallTimeMinutes = 15;

        public string ApplicationParentFolderName { get; set; }

        public bool SyncEnlistment { get; set; }

        public bool InstallOnW2k8 { get; set; }

        public bool UpdateWpdt { get; set; }

        public bool UpdateYdr { get; set; }

        public YdrBranch YdrBranchSelection { get; set; }

        public bool UpdateDailyBuildVersion { get; set; }


        public bool EmailResults { get; set; }

        public bool UpdateTestTreeXaps { get; set; }

        public Int32 WpdtInstallationTimeMinutes { get; set; }

        public string WpdtInstallationProductId { get; set; }

        public string WpdtInstallTestCase { get; set; }

        public string WpdtUninstallTestCase { get; set; }


        public string ReleaseShareRoot { get; set; }

        public string EmailAlias { get; set; }

        public string EmailFromAlias { get; set; }

        public BuildTypeEnum BuildType { get; set; }

        public bool ArchiveSource { get; set; }

        public BuildProjectControlParameters[] Projects { get; set; }

        [XmlIgnore]
        public bool SourceDepotOnlineMode { get; set; }

        [XmlIgnore]
        public bool AbortCurrentOperation { get; set; }


        [XmlIgnore]
        public string WpdtLkgSourcePath { get; set; }

        [XmlIgnore]
        public string LogPath { get; set; }

        [XmlIgnore]
        public string BuildOutputPath { get; set; }

        [XmlIgnore]
        public string PathForSourceDepotProgram { get; set; }

        [XmlIgnore]
        public string WpdtLkgTargetPath { get; set; }

        [XmlIgnore]
        public string SwanPath { get; set; }

        [XmlIgnore]
        public string SwanTestCasePath { get; set; }

        [XmlIgnore]
        public string MsBuildPath { get; set; }



        /// <summary>
        /// Backing store for directories to exclude
        /// </summary>
        private Collection<string> directoriesToExclude = new Collection<string>();

        /// <summary>
        /// Directories to exclude
        /// </summary>
        [XmlIgnore]
        public Collection<string> DirectoriesToExclude
        {
            get
            {
                return this.directoriesToExclude;
            }
        }

        /// <summary>
        /// Backing store for FilesToExclude
        /// </summary>
        private Collection<string> filesToExclude = new Collection<string>();

        /// <summary>
        /// Directories to exclude
        /// </summary>
        [XmlIgnore]
        public Collection<string> FilesToExclude
        {
            get
            {
                return this.filesToExclude;
            }

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildControlParameters"/> class.
        /// </summary>
        public BuildControlParameters()
        {
            DirectoryInfo current = new DirectoryInfo(App.StartupPath);
            DirectoryInfo parent = current.Parent;
            this.SourceDepotOnlineMode = true;
            this.LogPath = Path.Combine(parent.FullName, LoggingRelativePath);
            this.ReleaseShareRoot = Path.Combine(parent.FullName, "Release");
            this.BuildOutputPath = Path.Combine(parent.FullName, BuildRelativePath);
            this.PathForSourceDepotProgram = App.StartupPath;
            this.WpdtLkgTargetPath = Path.Combine(parent.FullName, WpdtRelativePath);
            this.SwanPath = Path.Combine(parent.FullName, SwanRelativePath);
            this.SwanTestCasePath = Path.Combine(parent.FullName, SwanTestCasesRelativePath);
            this.UpdateWpdtLkgLink();
            this.WpdtInstallationTimeMinutes = DefaultInstallTimeMinutes;
        }


        /// <summary>
        /// Updates the WPDT LKG link.
        /// </summary>
        public void UpdateWpdtLkgLink()
        {
            this.WpdtLkgSourcePath = WpdtLkgRoot;
            var symlink = ShellObject.FromParsingName(WpdtLkgRoot);
            this.WpdtLkgSourcePath = symlink.Properties.System.Link.TargetParsingPath.Value;
            if (String.IsNullOrEmpty(this.WpdtLkgSourcePath))
            {
                this.WpdtLkgSourcePath = WpdtLkgRoot;
            }
        }


        public void SampleInit()
        {
            DirectoryInfo current = new DirectoryInfo(App.StartupPath);
            DirectoryInfo parent = current.Parent;
            this.AbortCurrentOperation = false;
            this.EmailAlias = "yamasvn";
            this.EmailFromAlias = "yamasvn";
            this.EmailResults = false;
            this.SyncEnlistment = true;
            this.UpdateWpdtLkgLink();


            this.InstallOnW2k8 = true;
            this.WpdtInstallationTimeMinutes = DefaultInstallTimeMinutes;
            this.WpdtInstallationProductId = "Microsoft Visual Studio 2010 Express for Windows Phone  Beta - ENU";
            this.UpdateWpdt = true;
            this.MsBuildPath = Path.Combine(System.Environment.GetEnvironmentVariable("ProgramFiles"),
                @"Microsoft Visual Studio 10.0\VC");
            this.BuildType = BuildTypeEnum.Daily;
            this.UpdateDailyBuildVersion = true;
            this.SourceDepotOnlineMode = true;

            this.Projects = new BuildProjectControlParameters[9];
            this.Projects[0] = new BuildProjectControlParameters(
                BuildProjectControl.Solution, "1.0.0.0", "Level",
                "level.sln",
                @"level.csproj",
                @"bin\Release\Level.xap",
                @"Properties\AssemblyInfo.cs");
            this.Projects[1] = new BuildProjectControlParameters(
                BuildProjectControl.Solution, "1.0.0.0", "MapControlSdk",
                "MapControlSdk.sln",
                "MapControlSdk.csproj",
                @"bin\Release\MapControlSdk.xap",
                @"Properties\AssemblyInfo.cs");
            this.Projects[2] = new BuildProjectControlParameters(
              BuildProjectControl.Solution, "1.0.0.0", "NYTReader",
              "NYTReader.sln",
              @"NYTReader\NYTReader.csproj",
              @"NYTReader\Bin\Release\NYTReader.xap",
              @"NYTReader\Properties\AssemblyInfo.cs");
            this.Projects[3] = new BuildProjectControlParameters(
              BuildProjectControl.Project, "1.0.0.0", "ShoppingList",
              @"ShoppingListPhone\ShoppingListPhone.sln",
              @"ShoppingListPhone\ShoppingListPhone.csproj",
              @"ShoppingListPhone\Bin\Release\ShoppingList.xap",
              @"ShoppingListPhone\Properties\AssemblyInfo.cs");
            this.Projects[4] = new BuildProjectControlParameters(
            BuildProjectControl.Project, "1.0.0.0", "Stocks",
            "Stocks.sln",
            @"Stocks\Stocks.csproj",
            @"Stocks\Bin\Release\Stocks.xap",
            @"Stocks\Properties\AssemblyInfo.cs");

            this.Projects[5] = new BuildProjectControlParameters(
           BuildProjectControl.Project, "1.0.0.0", "translator2",
           "Translator.sln",
           @"Translator\Translator.csproj",
           @"Translator\Bin\Release\Translator.xap",
           @"Translator\Properties\AssemblyInfo.cs");

            this.Projects[6] = new BuildProjectControlParameters(
           BuildProjectControl.Project, "1.0.0.0", "UnitConverter2",
           @"UnitConverter2.sln",
           @"UnitConverter2\UnitConverter2.csproj",
           @"UnitConverter2\Bin\Release\Convert.xap",
           @"UnitConverter2\Properties\AssemblyInfo.cs");

            this.Projects[7] = new BuildProjectControlParameters(
           BuildProjectControl.Solution, "1.0.0.0", "Unite",
           @"Unite\Unite.sln",
           @"Unite\UniteGame\UniteGame.csproj",
           @"Unite\UniteGame\Bin\Release\UniteGame.xap",
           @"Unite\UniteGame\Properties\AssemblyInfo.cs");

            this.Projects[8] = new BuildProjectControlParameters(
        BuildProjectControl.Project, "1.0.0.0", "Weather",
            @"Weather.sln",
            @"Weather\Weather.csproj",
            @"Weather\Bin\Release\Weather.xap",
            @"Weather\Properties\AssemblyInfo.cs");
        }

        #region SaveConfiguration/LoadConfiguration 2 Overloads)



        /// <summary>
        /// Save the program configuration to disk
        /// </summary>
        /// <param name="config">Configuration settings</param>
        /// <param name="directoryForSave">Directory where to save the file</param>
        /// <param name="fileName">File name to use for the saving of the file</param>
        public static void SaveConfiguration(
           BuildControlParameters config,
           string directoryForSave,
           string fileName)
        {
            string fullPath = Path.Combine(directoryForSave, fileName);

            WriteConfiguration(config, fullPath);
        }

        /// <summary>
        /// Write to config file
        /// </summary>
        /// <param name="config">BDC config settings</param>
        /// <param name="fileName">Config file</param>
        private static void WriteConfiguration(BuildControlParameters config, string fileName)
        {
            try
            {
                XmlSerializer mySerializer = new XmlSerializer(typeof(BuildControlParameters));
                // To write to a file, create a StreamWriter object.
                StreamWriter myWriter = new StreamWriter(fileName);
                mySerializer.Serialize(myWriter, config);
                myWriter.Close();
            }
            catch (Exception e)
            {
                ProgramExecutionLog.AddEntry("Configuration file " + fileName +
                                  " could not be written to disk! Error was " + e.Message);
            }
        }

        /// <summary>
        /// Save the config
        /// </summary>
        /// <param name="config">BDC config settings</param>
        /// <param name="fileName">Config filename</param>
        public static void SaveConfiguration(BuildControlParameters config, string fileName)
        {
            WriteConfiguration(config, fileName);
        }


        /// <summary>
        /// Load the measurement configuration with a supplied filename. This will
        /// normally be called as part of the command line options where the 
        /// file name is supplied in the command arguments      
        /// </summary>
        /// <param name="fileName">Config file name</param>
        /// <param name="showExceptionDialog">True if we want to show the user an exception dialog
        /// If false, the exception will not be caught.</param>
        /// <returns>BDC Settings</returns>
        public static BuildControlParameters LoadConfiguration(string fileName)
        {
            BuildControlParameters ret = null;
            try
            {
                XmlSerializer mySerializer = new XmlSerializer(typeof(BuildControlParameters));
                // To read the file, create a FileStream.
                FileStream myFileStream = new FileStream(fileName, FileMode.Open);
                // Call the Deserialize method and cast to the object type.
                ret = (BuildControlParameters)mySerializer.Deserialize(myFileStream);
                myFileStream.Close();
            }
            catch (FileNotFoundException e)
            {
                ProgramExecutionLog.AddEntry("Configuration file " + fileName +
                    " was not found! Error was " + e.Message);
            }
            if (ret != null)
            {
                // For backwards compatibility, set the default to the HeroApps build 
                // folder if this is not specified in the configuration file.
                if (String.IsNullOrEmpty(ret.ApplicationParentFolderName))
                {
                    ret.ApplicationParentFolderName = ApplicationParentDefaultFolderName;
                }
            }
            return ret;
        }

        #endregion
    }
}
