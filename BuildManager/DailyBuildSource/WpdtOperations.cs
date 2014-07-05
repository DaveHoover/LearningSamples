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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Win32;

namespace DailyBuildSource
{
    public class WpdtOperations
    {

        /// <summary>
        /// Section to look for to modify the install information
        /// </summary>
        public const string InstallSection = "[gencomp7788]";

        /// <summary>
        /// First entry to modify for installation on W2k8
        /// </summary>
        public const string InstallLhs = "InstallOnLHS";

        /// <summary>
        /// Second entry to modify for installation of W2k8
        /// </summary>
        public const string InstallServer = "InstallOnWin7Server";


        public const string InstallationFileCheckName = "InstalledProg.txt";

        public const string InstallationCheckProgramName = "msiinv.exe";

        public const string MsiExec = "msiexec.exe";

        // These 2 keys are under HKLM
        public const string Wpdt64InstallRegKey =
            @"SOFTWARE\Wow6432Node\Microsoft\VPDExpress\10.0\SplashInfo";

        public const string Wpdt32InstallRegKey =
            @"SOFTWARE\Microsoft\VPDExpress\10.0\SplashInfo";

        public const string WpdtInstallVersionProperty = "EnvVersion";

        /// <summary>
        /// Look for a string of the type 30319.91
        /// The build number can grow to 4 digits.
        /// </summary>
        public const string WpdtVersionPattern = @"\d{5}.\d{2,4}";


        private static bool wpdtVersionLogged = false;



        /// <summary>
        /// Uninstalls the ydr.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public static void UninstallYdr(BuildControlParameters configuration)
        {
            if (IsYdrInstalled(configuration))
            {
                Console.Write("Beginning uninstall of YDR... ");
                try
                {
                    string arguments = @"/x" + BuildControlParameters.YdrGuid + @" /qb /quiet";
                    ProcessInformation process = ProcessOperations.RunProcess(MsiExec, Application.StartupPath,
                        arguments, true);
                    Console.WriteLine("Completed!");
                }
                catch (Exception e)
                {
                    Console.WriteLine(" Failed to uninstall YDR!");
                    ProgramExecutionLog.AddEntry(
                        "Failed to uninstall Ydr. Error message was " + e.Message);
                }
            }
        }


        /// <summary>
        /// Installs the ydr.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="results">The results.</param>
        public static void InstallYdr(
            BuildControlParameters configuration,
            DailyBuildFullResults results)
        {
            if (IsYdrInstalled(configuration))
            {
                UninstallWdpt(configuration);
            }
            Console.WriteLine("Beginning install of Ydr for branch " + configuration.YdrBranchSelection + " ...");
            string buildFolderFile = "";
            string setupBranchRoot = "";
            switch (configuration.YdrBranchSelection)
            {
                case YdrBranch.Mainline:
                    buildFolderFile = File.ReadAllText(
                        Path.Combine(BuildControlParameters.YdrRootFolder,
                        BuildControlParameters.YdrLatestBuildFolderFile));
                    setupBranchRoot = Path.Combine(BuildControlParameters.YdrRootFolder,
                        YdrBuildFolder(buildFolderFile));
                    break;
                case YdrBranch.Wm7_AppPlatform:
                case YdrBranch.Wm7_AppPlatform_DevDiv:
                    string branchRoot = Path.Combine(BuildControlParameters.YdrRootFolder,
                        configuration.YdrBranchSelection.ToString());
                    buildFolderFile = File.ReadAllText(
                        Path.Combine(branchRoot,
                        BuildControlParameters.YdrLatestBuildFolderFile));
                    setupBranchRoot = Path.Combine(branchRoot,
                       YdrBuildFolder(buildFolderFile));
                    break;
            }
            string fullInstallPath = Path.Combine(setupBranchRoot,
                BuildControlParameters.YdrInstallPathFromBranchRoot);
            Console.WriteLine("Ydr Installation Path = " + fullInstallPath);
            try
            {
                string arguments = @"";
                ProcessInformation process = ProcessOperations.RunProcess(fullInstallPath, Application.StartupPath,
                    arguments, true);
                results.InstallYdr.Success = true;
                results.YdrVersion = "Branch: " + configuration.YdrBranchSelection.ToString() +
                    " Build " + YdrVersion(YdrBuildFolder(buildFolderFile)).ToString();
                ;
                Console.WriteLine("Ydr Install Completed!");
            }
            catch (Exception e)
            {
                string errorMessage = "Failed to install Ydr. Error message was " + e.Message;
                ProgramExecutionLog.AddEntry(errorMessage);
                results.InstallYdr.SetResults(false, errorMessage);
            }


        }

        /// <summary>
        /// Ydrs the build folder.
        /// </summary>
        /// <param name="buildFolderFileContents">The build folder file contents.</param>
        /// <returns></returns>
        private static string YdrBuildFolder(string buildFolderFileContents)
        {
            Int32 startIndex = buildFolderFileContents.IndexOf('=');
            string buildFolder = buildFolderFileContents.Substring(startIndex + 1);
            buildFolder = buildFolder.Trim();
            return buildFolder;
        }

        /// <summary>
        /// Ydrs the version.
        /// </summary>
        /// <param name="ydrFolder">The ydr folder.</param>
        /// <returns></returns>
        private static Int32 YdrVersion(string ydrFolder)
        {
            Int32 version = 0;

            try
            {
                string[] folderParts = ydrFolder.Split('.');
                if (folderParts.Length >= 2)
                {
                    Int32.TryParse(folderParts[0], out version);
                }
            }
            catch (Exception e)
            {
                ProgramExecutionLog.AddEntry("Failed to determine Ydr version from build " +
                    "folder. Error was " + e.Message);
            }
            return version;
        }





        /// <summary>
        /// Determines the WDPT installed version.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public static string DetermineWdptInstalledVersion(BuildControlParameters configuration)
        {
            string wpdtVersion = "";
            if (!IsWpdtInstalled(configuration))
            {
                return wpdtVersion;
            }

            using (RegistryKey hklm = Registry.LocalMachine)
            {
                string version = "";
                if (ProcessOperations.Is64Bit())
                {
                    try
                    {

                        using (RegistryKey wow64 =
                            hklm.OpenSubKey(Wpdt64InstallRegKey, true))
                        {
                            version = (string)wow64.GetValue(WpdtInstallVersionProperty);
                        }
                    }
                    catch (Exception e)
                    {
                        ProgramExecutionLog.AddEntry(
                          "64 bit system. Failed to access Wpdt Version Key/value. Error was  " +
                          e.Message);
                    }
                }
                else
                {
                    try
                    {
                        using (RegistryKey software =
                            hklm.OpenSubKey(Wpdt32InstallRegKey, true))
                        {
                            version = (string)software.GetValue(WpdtInstallVersionProperty);
                        }
                    }
                    catch (Exception e)
                    {
                        ProgramExecutionLog.AddEntry(
                          "32 bit system. Failed to access Wpdt Version Key/value. Error was " +
                           e.Message);
                    }
                }
                if (!String.IsNullOrEmpty(version))
                {
                    Regex ntMatch = new Regex(WpdtVersionPattern,
                                      RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.RightToLeft);
                    Match m = ntMatch.Match(version);
                    if (m.Success)
                    {
                        wpdtVersion = m.Value;
                    }
                }
                if (!wpdtVersionLogged)
                {
                    wpdtVersionLogged = true;
                    Console.WriteLine("Wpdt installed version was " + wpdtVersion);
                    ProgramExecutionLog.AddEntry("Wpdt installed version was " + wpdtVersion);
                }
                return wpdtVersion;
            }

        }


        /// <summary>
        /// Determines whether [is installed WPDT version same as LKG] [the specified configuration].
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// 	<c>true</c> if [is installed WPDT version same as LKG] [the specified configuration]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInstalledWpdtVersionSameAsLkg(BuildControlParameters configuration)
        {
            string currentInstalledWpdtVersion = DetermineWdptInstalledVersion(configuration);
            if (!String.IsNullOrEmpty(currentInstalledWpdtVersion))
            {
                return configuration.WpdtLkgSourcePath.ToLower().Contains(currentInstalledWpdtVersion.ToLower());
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Uninstalls the WDPT.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public static void UninstallWdpt(BuildControlParameters configuration)
        {
            ProcessOperations.KillSwanProcesses();
            UninstallYdr(configuration);

            Console.WriteLine("Beginning uninstall of Wpdt ...");
            GeneralFileOperations.WriteProgramStatusFile(ExecutionStatus.UninstallWdpt);
            string swanController = Path.Combine(configuration.SwanPath, BuildControlParameters.SwanControllerExeName);
            string swanArguments = "/f " + Path.Combine(configuration.SwanTestCasePath, configuration.WpdtUninstallTestCase);

            ProcessOperations.RunProcess(swanController, configuration.SwanTestCasePath, swanArguments, false);
        }

        /// <summary>
        /// Installs the WDPT.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="results">The results.</param>
        public static void InstallWdpt(
            BuildControlParameters configuration,
            DailyBuildFullResults results)
        {
            ProcessOperations.KillSwanProcesses();
            Console.WriteLine("Starting Wpdt installation ....");
            GeneralFileOperations.WriteProgramStatusFile(ExecutionStatus.InstallWdpt);

            string swanController = Path.Combine(configuration.SwanPath, BuildControlParameters.SwanControllerExeName);
            string swanArguments = "/f " + Path.Combine(configuration.SwanTestCasePath, configuration.WpdtInstallTestCase);
            Console.WriteLine("SwanController = " + swanController);
            Console.WriteLine("SwanArguments = " + swanArguments);

            ProcessOperations.RunProcess(swanController, configuration.SwanTestCasePath, swanArguments, false);
            Console.WriteLine("Waiting " + configuration.WpdtInstallationTimeMinutes.ToString() +
                " minutes for install to complete");
            bool wpdtInstalled = false;
            for (Int32 i = 0; i < configuration.WpdtInstallationTimeMinutes; i++)
            {
                for (Int32 j = 0; j < 12; j++)
                {
                    Thread.Sleep(5000);
                    Console.Write(".");
                }
                // Wait one more minute after we detect the product is installed to be sure 
                // of any final cleanup required.
                if (wpdtInstalled)
                {
                    break;
                }
                Console.WriteLine("");
                Console.WriteLine("Waited " + (i + 1).ToString() + " minutes");
            }
            wpdtInstalled = IsWpdtInstalled(configuration);
            if (wpdtInstalled)
            {
                results.InstallWdpt.SetResults(true, "");
                Console.WriteLine("Completed Install of Wpdt!");
            }
            else
            {
                results.InstallWdpt.SetResults(false, "Wpdt did not install int the alloted time!");
                Console.WriteLine("Wpdt did not install int the alloted time!");
            }
            ProcessOperations.KillSwanProcesses();
            ProcessOperations.KillSetupProcesses();
            results.WpdtVersion = WpdtOperations.DetermineWdptInstalledVersion(configuration);
            results.YdrVersion = "Same as Wpdt";
        }

        /// <summary>
        /// Modifies the install for W2K8.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="results">The results.</param>
        public static void ModifyInstallForW2k8(
            BuildControlParameters configuration,
            DailyBuildFullResults results)
        {
            Console.WriteLine("Modifying Installation for W2k8 ....");
            try
            {
                string fileName = Path.Combine(configuration.WpdtLkgTargetPath, BuildControlParameters.FileToModifyForW2k8);
                File.SetAttributes(fileName, FileAttributes.Normal);
                string[] fileContents = File.ReadAllLines(fileName);
                bool foundSection = false;
                bool foundInstallOnLHS = false;
                bool foundInstallonWin7Server = false;
                for (Int32 i = 0; i < fileContents.Length; i++)
                {
                    if (!foundSection)
                    {
                        if (fileContents[i].Contains(InstallSection))
                        {
                            foundSection = true;
                            continue;
                        }
                    }
                    else
                    {
                        if (fileContents[i].ToLower().Contains(InstallLhs.ToLower()))
                        {
                            fileContents[i] = InstallLhs + "=0";
                            foundInstallOnLHS = true;
                            continue;
                        }
                        if (fileContents[i].ToLower().Contains(InstallServer.ToLower()))
                        {
                            fileContents[i] = InstallServer + "=0";
                            foundInstallonWin7Server = true;
                            break;
                        }
                    }
                }
                if (foundSection && foundInstallOnLHS && foundInstallonWin7Server)
                {
                    File.WriteAllLines(fileName + ".tmp", fileContents);
                    GeneralFileOperations.DeleteFile(fileName);
                    GeneralFileOperations.RenameFile(fileName + ".tmp", fileName);
                    Console.WriteLine("Successfully completed modifying the installation for W2k8!");
                }
                else
                {
                    ProgramExecutionLog.AddEntry(
                             "Failed to find expected settings to enable W2k8 install. This may or not be a problem");
                    results.ModifyWdpt.SetResults(false,
                        "Failed to find expected settings to enable W2k8 install. This may or not be a problem");
                }
            }
            catch (Exception e)
            {
                ProgramExecutionLog.AddEntry(
                             "Failed to modify for W2k8 install. This may or not be a problem");
                results.ModifyWdpt.SetResults(false, e.Message);
            }
        }

        /// <summary>
        /// Helper function to initialize a test set of files for development
        /// </summary>
        /// <param name="configuration">Main program configuration</param>
        /// <returns>Destination folder path. To use for other operations</returns>
        public static string PrepareWpdtTargetDirectory(
            BuildControlParameters configuration,
            DailyBuildFullResults results)
        {
            string sourcePath = configuration.WpdtLkgSourcePath;
            string destinationPath = configuration.WpdtLkgTargetPath;
            if (Directory.Exists(destinationPath))
            {
                GeneralFileOperations.DeleteDirectory(destinationPath);
            }
            CopyWpdtLkgToTargetDirectory(sourcePath, destinationPath, results);
            return destinationPath;
        }



        /// <summary>
        /// Helper function to copy test data from an archive location to a test file location.
        /// </summary>
        /// <param name="sourceFolderName">Source folder name</param>
        /// <param name="targetFolderName">Destination folder name</param>
        public static void CopyWpdtLkgToTargetDirectory(
            string sourceFolderName,
            string targetFolderName,
             DailyBuildFullResults results)
        {
            DirectoryInfo dirInfo;
            DirectoryInfo[] directories;
            string fileNameOnly = "";
            string sourceFileName = "";
            string destinationFileName = "";

            try
            {
                dirInfo = new DirectoryInfo(sourceFolderName);
                directories = dirInfo.GetDirectories();
                GeneralFileOperations.CheckForFolderAndCreate(targetFolderName);
                FileInfo[] f = dirInfo.GetFiles("*.*");
                for (Int32 i = 0; i < f.Length; i++)
                {
                    fileNameOnly = f[i].Name;
                    sourceFileName = Path.Combine(sourceFolderName, fileNameOnly);
                    destinationFileName = Path.Combine(targetFolderName, fileNameOnly);
                    File.Copy(sourceFileName, destinationFileName);
                }
            }
            catch (IOException e)
            {
                ProgramExecutionLog.AddEntry(
                    "Failed to read file information from source folder. Operation canceled");
                results.InstallWdpt.SetResults(false, e.Message);
                return;
            }
            if (directories.Length > 0)
            {
                foreach (DirectoryInfo dinfo in directories)
                {
                    string newDirInfo = dinfo.FullName.Replace(sourceFolderName, "");
                    string newTargetFolder = targetFolderName + newDirInfo;
                    CopyWpdtLkgToTargetDirectory(dinfo.FullName, newTargetFolder, results);
                }
            }
        }

        /// <summary>
        /// Determines whether [is WPDT installed] [the specified configuration].
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// 	<c>true</c> if [is WPDT installed] [the specified configuration]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsWpdtInstalled(BuildControlParameters configuration)
        {
            string outputFile = Path.Combine(Application.StartupPath, InstallationFileCheckName);
            string app = Path.Combine(Application.StartupPath, InstallationCheckProgramName);
            try
            {
                string arguments = @"/p";
                ProcessInformation process = ProcessOperations.RunProcess(app, Application.StartupPath,
                    arguments, true);
                string[] contents = process.GetStandardOutput();
                if (contents != null)
                {
                    var wpdtInstalled = (from installId in contents
                                         where installId.Contains(configuration.WpdtInstallationProductId)
                                         select installId).Count();
                    return wpdtInstalled > 0 ? true : false;
                }
                else
                {
                    ProgramExecutionLog.AddEntry(
                        "No output was returned from the msiinv.exe program");
                    return false;
                }
            }
            catch (Exception e)
            {
                ProgramExecutionLog.AddEntry(
                        "Failure checking for Installation of Wpdt. Error was " + e.Message);
            }
            return false;
        }

        /// <summary>
        /// Determines whether [is ydr installed] [the specified configuration].
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// 	<c>true</c> if [is ydr installed] [the specified configuration]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsYdrInstalled(BuildControlParameters configuration)
        {
            string outputFile = Path.Combine(Application.StartupPath, InstallationFileCheckName);
            string app = Path.Combine(Application.StartupPath, InstallationCheckProgramName);
            try
            {
                string arguments = @"/p";
                ProcessInformation process = ProcessOperations.RunProcess(app, Application.StartupPath,
                    arguments, true);
                string[] contents = process.GetStandardOutput();
                if (contents != null)
                {
                    var ydrInstalled = (from installId in contents
                                        where installId.Contains(BuildControlParameters.YdrGuid)
                                        select installId).Count();
                    return ydrInstalled > 0 ? true : false;
                }
                else
                {
                    ProgramExecutionLog.AddEntry(
                        "No output was returned from the msiinv.exe program");
                    return false;
                }
            }
            catch (Exception e)
            {
                ProgramExecutionLog.AddEntry(
                        "Failure checking for Installation of Wpdt. Error was " + e.Message);
            }
            return false;

        }

    }
}
