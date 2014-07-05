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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;

namespace BuildManager
{
    public class SourceCodeOps
    {

        public const string RevisionFileStringBase = "AssemblyFileVersion";

        public const string LicenseSubDirectory = "Preinstall_License";

        public const string InstallXapsOnDevice = "InstallXapsOnDevice.cmd";

        public const string UninstallXapsFromDevice = "UninstallXapsFromDevice.cmd";

        public const string RelativePathToWpapp = @"..\..\BuildScripts\";

        public const string WpappProgramPathAndName = @"Wpapp\Wpapp.exe";
                
        public const string LogsFolder = "logs";

        public const string SourceArchiveFolder = "Source";

        public const string SignedXapFolder = "Signed";

        public const string UnsignedXapFolder = "UnSigned";

        public const string SourceArchiveName = "SourceArchive.zip";

        /// <summary>
        /// Syncs the enlistment.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="results">The results.</param>
        public static void SyncEnlistment(
            BuildControlParameters configuration,
            DailyBuildFullResults results)
        {
            Console.Write("Syncing Enlistment ... ");
            GeneralFileOperations.WriteProgramStatusFile(ExecutionStatus.SynEnlistment);
            try
            {
                ProcessInformation process = ProcessOperations.SDSync(configuration);
                CheckForErrorFromSDOperation(process, results, results.EnlistmentSync);
                Console.WriteLine("Success!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Fail!");
                results.EnlistmentSync.SetResults(false, e.Message);
                ProgramExecutionLog.AddEntry(
                                  "Failed to sync enlistment " +
                                  " Error was " + e.Message);
            }
            Console.WriteLine("Enlistment Sync Status = " + results.EnlistmentSync.ToString());
        }

        /// <summary>
        /// Creates the date time format string.
        /// </summary>
        /// <param name="currentTime">The current time.</param>
        /// <returns></returns>
        public static string CreateDateTimeFormatString(DateTime currentTime)
        {
            string r = currentTime.Year.ToString("d4");
            r += currentTime.Month.ToString("d2");
            r += currentTime.Day.ToString("d2");
            r += currentTime.Hour.ToString("d2");
            r += currentTime.Minute.ToString("d2");
            return r;
        }

        /// <summary>
        /// This function was added as a defensive measure where we saw a couple of times
        /// a file was created for what should have been a directory. This caused the 
        /// build scripts to fail in a very non obvious way.
        /// 
        /// </summary>
        /// <param name="path"></param>
        private static void DeleteIntermediateDirectory(string path)
        {
            if (File.Exists(path))
            {
                GeneralFileOperations.DeleteFile(path);
            }
            if (Directory.Exists(path))
            {
                GeneralFileOperations.DeleteDirectory(path);
            }
        }

        /// <summary>
        /// Builds the batch file.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="results">The results.</param>
        /// <returns></returns>
        private static string BuildBatchFile(
            BuildControlParameters configuration,
            DailyBuildFullResults results)
        {
            results.BuildStartTime = DateTime.Now;
            string progFiles = System.Environment.GetEnvironmentVariable("ProgramFiles");
            string envVarSetup = Path.Combine(progFiles, @"Microsoft Visual Studio 10.0\VC\vcvarsall.bat");
            DeleteIntermediateDirectory(configuration.BuildOutputPath);
            DeleteIntermediateDirectory(configuration.LogPath);

            List<string> buildScript = new List<string>();

            buildScript.Add("@echo off");
            buildScript.Add("call " + "\"" + envVarSetup + "\"" + " x86");
            buildScript.Add("set ScriptDir=%~d0%~p0");
            buildScript.Add("set LogDir=" + configuration.LogPath);
            buildScript.Add("set BuildDir=" + configuration.BuildOutputPath);
            buildScript.Add("set UnsignedXapDir=" + Path.Combine(configuration.BuildOutputPath, UnsignedXapFolder));
            buildScript.Add("rd %LogDir% /s /q");
            buildScript.Add("md %LogDir%");
            buildScript.Add("rd %BuildDir% /s /q");
            buildScript.Add("md %BuildDir%");
            buildScript.Add("md %UnsignedXapDir%");
            buildScript.Add("copy XapUpdater.* %BuildDir%");

            string heroAppSource = FindHeroAppSourceFolder(configuration);
            for (Int32 i = 0; i < configuration.Projects.Length; i++)
            {
                if (configuration.Projects[i].BuildConfiguration != BuildProjectControl.None)
                {
                    StringBuilder b = new StringBuilder();
                    BuildProjectControlParameters p = configuration.Projects[i];
                    string appFolder = Path.Combine(heroAppSource, p.ProjectPath);
                    string solutionName = Path.GetFileName(p.SolutionPathAndNameFromProjectRoot);
                    string projectName = Path.GetFileName(p.ProjectPathAndNameFromProjectRoot);
                    string solution = Path.Combine(appFolder, p.SolutionPathAndNameFromProjectRoot);
                    string project = Path.Combine(appFolder, p.ProjectPathAndNameFromProjectRoot);
                    string outputDirectory = Path.Combine(appFolder,
                        Path.GetDirectoryName(p.XapPathAndNameFromProjectRoot));
                    string buildtarget = p.BuildConfiguration == BuildProjectControl.Solution ?
                                solution : project;
                    string logFileName = Path.GetFileName(buildtarget) + ".log";
                    string xapFileName = Path.GetFileName(p.XapPathAndNameFromProjectRoot);
                    string signLogFileName      = xapFileName + "_sign" + ".log";
                    string licenseLogFileName   = xapFileName + "_license" + ".log";
                    results.BuildTargets[i].BuildLogFileLink = Path.Combine(configuration.LogPath, logFileName);

                    buildScript.Add("echo Building " + solutionName);
                    buildScript.Add("del " + outputDirectory + @"\* /q");
                    buildScript.Add("msbuild " + buildtarget + @" /t:Rebuild /p:Configuration=Release >%LogDir%\" + logFileName + " 2>&1");
                    buildScript.Add(@"IF %ERRORLEVEL% NEQ 0 echo " + solutionName + " build failed");
                    buildScript.Add("copy " + outputDirectory + @"\*.xap" + " %BuildDir%");
                    buildScript.Add("copy " + outputDirectory + @"\*.xap" + " %UnsignedXapDir%");
                    buildScript.Add("cd %BuildDir%");
                    buildScript.Add("XapUpdater.exe -xap " + xapFileName + @" >%LogDir%\" + signLogFileName);
                    buildScript.Add("XapUpdater.exe -generatepreinstalllicense " + xapFileName + @" >%LogDir%\" + licenseLogFileName);
                    buildScript.Add("cd %ScriptDir%");
                }
            }
            try
            {
                File.WriteAllLines("BuildHeroApps.cmd", buildScript.ToArray<string>());
            }
            catch (Exception e)
            {
                ProgramExecutionLog.AddEntry(
                                                 "Failed to write build commannd file" +
                                                 " Error was " + e.Message);
            }
            return "BuildHeroApps.cmd";
        }

        /// <summary>
        /// Finds the hero app source folder.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        private static string FindHeroAppSourceFolder(BuildControlParameters configuration)
        {
            try
            {
                DirectoryInfo current = new DirectoryInfo(App.StartupPath);
                DirectoryInfo heroAppSource = current;
                do
                {
                    heroAppSource = heroAppSource.Parent;
                }
                while (heroAppSource.Name.ToLower() !=
                    configuration.ApplicationParentFolderName.ToLower());
                return heroAppSource.FullName;
            }

            catch (Exception e)
            {
                ProgramExecutionLog.AddEntry(
                 "Failed to find Application parent source folder" +
                 " Error was " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// Checks for error from SD operation.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="results">The results.</param>
        /// <param name="opToLog">The op to log.</param>
        private static void CheckForErrorFromSDOperation(
            ProcessInformation process,
            DailyBuildFullResults results,
            SingleOperationResults opToLog
             )
        {
            string[] errorOutput = process.GetErrorOutput();
            if (errorOutput != null)
            {
                if (errorOutput.Length > 1)
                {
                    // Ignore message where the enlistment is up to date.
                    if (errorOutput[0].ToLower().Contains("File(s) up-to-date".ToLower()))
                    {
                        opToLog.Success = true;
                        return;
                    }
                    opToLog.Success = false;
                    for (Int32 j = 0; j < errorOutput.Length; j++)
                    {
                        opToLog.ErrorMessage += errorOutput[j] + " ";
                    }
                    ProgramExecutionLog.AddEntry(
                       "Source Depot message was: " + opToLog.ErrorMessage +
                       " . Severity not determined");
                    return;
                }
            }
            opToLog.Success = true;
        }

        /// <summary>
        /// Modifies the build version files.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="results">The results.</param>
        /// <returns></returns>
        private static bool ModifyBuildVersionFiles(
            BuildControlParameters configuration,
            DailyBuildFullResults results)
        {
            bool allCompletedWithSuccess = true;
            string heroAppSource = FindHeroAppSourceFolder(configuration);
            if (!String.IsNullOrEmpty(heroAppSource))
            {
                DateTime currentTime = DateTime.Now;
                for (Int32 i = 0; i < configuration.Projects.Length; i++)
                {
                    results.BuildTargets[i].BuildStartTime = currentTime;
                    string appFolder = Path.Combine(heroAppSource, configuration.Projects[i].ProjectPath);
                    string assemblyFile = Path.Combine(appFolder, configuration.Projects[i].AssemblyFilePathAndNameFromProjectRoot);
                    if (File.Exists(assemblyFile))
                    {
                        try
                        {
                            ProcessInformation process = ProcessOperations.SDEdit(configuration, assemblyFile);
                            CheckForErrorFromSDOperation(process, results, results.BuildTargets[i].ModifyVersion);
                            string[] assemblyFileContents = File.ReadAllLines(assemblyFile);
                            for (Int32 j = 0; j < assemblyFileContents.Length; j++)
                            {
                                if (assemblyFileContents[j].ToLower().Contains(RevisionFileStringBase.ToLower()))
                                {
                                    string v = assemblyFileContents[j];
                                    string[] versions = v.Split('.');
                                    if (versions.Length >= 3)
                                    {
                                        versions[2] = CreateDateTimeFormatString(currentTime);
                                    }
                                    v = "";
                                    for (Int32 k = 0; k < versions.Length; k++)
                                    {
                                        v += versions[k] + ".";
                                    }

                                    assemblyFileContents[j] = v.TrimEnd('.');
                                    break;
                                }
                            }
                            File.WriteAllLines(assemblyFile, assemblyFileContents);
                            results.BuildTargets[i].ModifyVersion.Success = true;
                        }
                        catch (Exception e)
                        {
                            results.BuildTargets[i].ModifyVersion.SetResults(false, e.Message);
                            ProgramExecutionLog.AddEntry(
                                    "Failed to modify  file " + assemblyFile + " Exception information was " +
                                e.ToString());
                            allCompletedWithSuccess = false;
                        }
                    }
                    else
                    {
                        string errorMessage = "Assembly File did not exist. Path was " +
                            assemblyFile;
                        results.BuildTargets[i].ModifyVersion.SetResults(false, 
                            errorMessage );
                        ProgramExecutionLog.AddEntry( errorMessage);
                        allCompletedWithSuccess = false;
                    }
                }
            }
            else
            {
                return false;
            }
            return allCompletedWithSuccess;
        }

        /// <summary>
        /// Reverts the build version files.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="results">The results.</param>
        private static void RevertBuildVersionFiles(
            BuildControlParameters configuration,
            DailyBuildFullResults results)
        {
            string heroAppSource = FindHeroAppSourceFolder(configuration);
            for (Int32 i = 0; i < configuration.Projects.Length; i++)
            {
                string assemblyFile = "";
                try
                {
                    string appFolder = Path.Combine(heroAppSource, configuration.Projects[i].ProjectPath);
                    assemblyFile = Path.Combine(appFolder, configuration.Projects[i].AssemblyFilePathAndNameFromProjectRoot);
                    ProcessInformation process = ProcessOperations.SDRevert(configuration, assemblyFile);
                    CheckForErrorFromSDOperation(process, results, results.BuildTargets[i].RevertVersion);
                    results.BuildTargets[i].RevertVersion.Success = true;
                }
                catch (Exception e)
                {
                    ProgramExecutionLog.AddEntry(
                                               "Failed to revert  file " + assemblyFile + " Exception information was " +
                                           e.ToString());
                    results.BuildTargets[i].RevertVersion.SetResults(false, e.Message);
                }
            }
        }


        /// <summary>
        /// Validates the build results.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="results">The results.</param>
        private static void ValidateBuildResults(
            BuildControlParameters configuration,
            DailyBuildFullResults results)
        {
            Console.Write("Beginning validation of Build target results... ");
            string heroAppSource = FindHeroAppSourceFolder(configuration);
            string outputPath = configuration.BuildOutputPath;
            string licensePath = Path.Combine(outputPath, LicenseSubDirectory);
            List<string> installScript = new List<string>();
            List<string> uninstallScript = new List<string>();
            string quote = "\"";
            string deviceList = quote + @"%ProgramFiles%\Zune\updatewp.exe" + quote + " /list";
            installScript.Add(deviceList);
            uninstallScript.Add(deviceList);
            
            for (Int32 i = 0; i < configuration.Projects.Length; i++)
            {
                try
                {
                    string xapName = Path.GetFileName(configuration.Projects[i].XapPathAndNameFromProjectRoot);
                    string xapNameNoExt = Path.GetFileNameWithoutExtension(configuration.Projects[i].XapPathAndNameFromProjectRoot);
                    string xapNameBeforeSign = xapName + BuildControlParameters.SignedXapOrigFileExtension;
                    string licenseName = xapNameNoExt + BuildControlParameters.LicenseNameAddition;
                    results.BuildTargets[i].Build.Success = File.Exists(Path.Combine(configuration.BuildOutputPath, xapName));
                    results.BuildTargets[i].SignXap.Success = File.Exists(Path.Combine(configuration.BuildOutputPath, xapNameBeforeSign));
                    results.BuildTargets[i].LicenseXap.Success = File.Exists(Path.Combine(licensePath, licenseName));
                    installScript.Add(RelativePathToWpapp + WpappProgramPathAndName     
                        + " i " + Path.Combine(configuration.BuildOutputPath, xapName));
                    uninstallScript.Add(RelativePathToWpapp + WpappProgramPathAndName   
                        + " u " + Path.Combine(configuration.BuildOutputPath, xapName));
                }
                catch (Exception e)
                {
                    ProgramExecutionLog.AddEntry(
                   "Failed to validate output files for build target " + configuration.Projects[i].ProjectPath +
                   " Error was " + e.Message);
                    Console.WriteLine("Failed!");
                    Console.WriteLine("For project " + configuration.Projects[i].ProjectPath +
                        " Error was " + e.Message);
                }
            }
            try
            {
                File.WriteAllLines(InstallXapsOnDevice, installScript.ToArray<string>());
            }
            catch (Exception e)
            {
                Console.WriteLine ( "Failed to write " + InstallXapsOnDevice  + " File. See log");
                ProgramExecutionLog.AddEntry(
                  "Failed to write " + InstallXapsOnDevice + " File. " +
                  " Error was " + e.Message);

            }
            try
            {
                File.WriteAllLines(UninstallXapsFromDevice, uninstallScript.ToArray<string>());
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to write " + UninstallXapsFromDevice + " File. See log");
                ProgramExecutionLog.AddEntry(
                  "Failed to write " + UninstallXapsFromDevice + " File. " +
                  " Error was " + e.Message);

            }
            Console.WriteLine("Completed!");
        }

        /// <summary>
        /// Publishes the build output.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="results">The results.</param>
        private static void PublishBuildOutput(
                        BuildControlParameters configuration,
            DailyBuildFullResults results)
        {
            Console.WriteLine("Publishing results to release share ...");
            string dateFolder = CreateDateTimeFormatString(results.BuildStartTime);
            results.PublishToReleaseShare.Success = true;
            try
            {
                string publishShare = Path.Combine(configuration.ReleaseShareRoot, dateFolder);
                string publishLogShare = Path.Combine(publishShare, LogsFolder);
                string publishSourceShare = Path.Combine(publishShare, SourceArchiveFolder);
                string signedXapFolder = Path.Combine(publishShare, SignedXapFolder);
                string unsignedXapFolder = Path.Combine(publishShare, UnsignedXapFolder);
                results.PublishShare = publishShare;
                results.PublishLogShare = publishLogShare;
                results.PublishSourceShare = publishSourceShare;
                GeneralFileOperations.CreateDirectory(publishShare);
                GeneralFileOperations.CreateDirectory(signedXapFolder);
                GeneralFileOperations.CreateDirectory(unsignedXapFolder);
                GeneralFileOperations.CreateDirectory(publishLogShare);
                GeneralFileOperations.CreateDirectory(publishSourceShare);
                string[] files = null;
                // Copy Signed xaps and license files
                try
                {
                    files = Directory.GetFiles(configuration.BuildOutputPath);
                    foreach (string file in files)
                    {
                        try
                        {
                            string extension = Path.GetExtension(file);
                            if (extension == ".xap" ||
                                 file.ToLower().Contains("license"))
                            {
                                string targetFile = 
                                    Path.Combine(signedXapFolder, Path.GetFileName(file));
                                File.Copy(file, targetFile);
                            }
                        }
                        catch (Exception e)
                        {
                            ProgramExecutionLog.AddEntry(
                            "Failed to copy result file " + file + " Error message: " + e.Message);
                            results.PublishToReleaseShare.SetResults(false, e.Message);
                        }
                    }
                }
                catch (Exception e1)
                {
                    ProgramExecutionLog.AddEntry(
                        "Failed to access directory " + configuration.BuildOutputPath +
                        " Error message: " + e1.Message);
                    results.PublishToReleaseShare.SetResults(false, e1.Message);
                }

                // Copy unigned xaps
                try
                {
                    files = Directory.GetFiles(
                        Path.Combine(configuration.BuildOutputPath, UnsignedXapFolder));
                    foreach (string file in files)
                    {
                        try
                        {
                            string extension = Path.GetExtension(file);
                            if (extension == ".xap")
                            {
                                string targetFile = 
                                    Path.Combine(unsignedXapFolder, Path.GetFileName(file));
                                File.Copy(file, targetFile);
                            }
                        }
                        catch (Exception e)
                        {
                            ProgramExecutionLog.AddEntry(
                            "Failed to copy result file " + file + " Error message: " + e.Message);
                            results.PublishToReleaseShare.SetResults(false, e.Message);
                        }
                    }
                }
                catch (Exception e1)
                {
                    ProgramExecutionLog.AddEntry(
                        "Failed to access directory " + configuration.BuildOutputPath +
                         " " + UnsignedXapFolder +
                        " Error message: " + e1.Message);
                    results.PublishToReleaseShare.SetResults(false, e1.Message);
                }

                // Copy License Files
                try
                {
                    files = Directory.GetFiles(
                        Path.Combine(configuration.BuildOutputPath, LicenseSubDirectory));
                    foreach (string file in files)
                    {
                        try
                        {
                            string extension = Path.GetExtension(file);
                            if (extension == ".xap" ||
                                 file.ToLower().Contains("license"))
                            {
                                string targetFile = 
                                    Path.Combine(signedXapFolder, Path.GetFileName(file));
                                File.Copy(file, targetFile);
                            }
                        }
                        catch (Exception e)
                        {
                            ProgramExecutionLog.AddEntry(
                            "Failed to copy result file " + file + " Error message: " + e.Message);
                            results.PublishToReleaseShare.SetResults(false, e.Message);
                        }
                    }
                }
                catch (Exception e1)
                {
                    ProgramExecutionLog.AddEntry(
                        "Failed to access directory " + configuration.BuildOutputPath +
                         " " + LicenseSubDirectory +
                        " Error message: " + e1.Message);
                    results.PublishToReleaseShare.SetResults(false, e1.Message);
                }

                // Copy Log Files
                try
                {
                    files = Directory.GetFiles(configuration.LogPath);
                    foreach (string file in files)
                    {
                        try
                        {
                            string logTargetFile = 
                                Path.Combine(publishLogShare, Path.GetFileName(file));
                            File.Copy(file, logTargetFile);
                        }
                        catch (Exception e)
                        {
                            ProgramExecutionLog.AddEntry(
                             "Failed to copy result file " + file + " Error message: " + e.Message);
                            results.PublishToReleaseShare.SetResults(false, e.Message);
                        }
                    }
                }
                catch (Exception e1)
                {
                    ProgramExecutionLog.AddEntry(
                        "Failed to access directory " + configuration.LogPath +
                        " Error message: " + e1.Message);
                    results.PublishToReleaseShare.SetResults(false, e1.Message);
                }

                for (Int32 i = 0; i < results.BuildTargets.Length; i++)
                {
                    results.BuildTargets[i].BuildLogFileLink =
                        Path.Combine(publishLogShare,
                        Path.GetFileName(results.BuildTargets[i].BuildLogFileLink));
                }
            }
            catch (Exception e)
            {
                ProgramExecutionLog.AddEntry(
                                       "General fail during Publish" + "Error message: " + e.Message);
                results.PublishToReleaseShare.SetResults(false, e.Message);
            }
            if (configuration.ArchiveSource)
            {
                ZipSourceFiles(configuration, results, results.PublishSourceShare);
            }
            Console.WriteLine("Finish publish operation. Overall status was " +
                results.PublishToReleaseShare.Success.ToString());
        }

        /// <summary>
        /// Sends the result via email.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="results">The results.</param>
        private static void SendResultViaEmail(
            BuildControlParameters configuration,
            DailyBuildFullResults results)
        {
            Console.WriteLine("Emailing results ....");
            EmailOpsOrig.SendMailtoExchange(configuration.EmailFromAlias, configuration, results);
            Console.WriteLine("Completed mailing of results");
        }


        /// <summary>
        /// Creates the build script and execute.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="results">The results.</param>
        public static void CreateBuildScriptAndExecute(
            BuildControlParameters configuration,
            DailyBuildFullResults results)
        {
            string heroAppSource = FindHeroAppSourceFolder(configuration);
            if (String.IsNullOrEmpty(heroAppSource))
            {
                Console.WriteLine("Could not find Application Parent Folder. Aborting ...");
                ProgramExecutionLog.AddEntry("Could not find Application Parent Folder. Aborting ...");
                return;
            }

            Console.Write("Starting Build/Sign/License Process... ");
            GeneralFileOperations.WriteProgramStatusFile(ExecutionStatus.Running);
            if (configuration.BuildType == BuildTypeEnum.Daily &&
                 configuration.UpdateDailyBuildVersion)
            {
                bool allCompletedWithSuccess = ModifyBuildVersionFiles(configuration, results);
                Console.WriteLine("Completed Modifying Revision Files. Overall status was " +
                    allCompletedWithSuccess.ToString());
            }
            else
            {
                foreach (BuildTargetResults t in results.BuildTargets)
                {
                    t.ModifyVersion.RequestedToRun = false;
                }
            }
            Console.WriteLine("Completed!");
            Console.Write("Beginning to create batch file for build/sign/license operations ... ");
            string commandFile = BuildBatchFile(configuration, results);
            Console.WriteLine("Completed. Running batch file ....");
            ProcessOperations.RunProcess(commandFile, App.StartupPath, "", true);
            Console.WriteLine("Batch File completed. Status not reported here");
            ValidateBuildResults(configuration, results);
            PublishBuildOutput(configuration, results);
            if (configuration.BuildType == BuildTypeEnum.Daily &&
                            configuration.UpdateDailyBuildVersion)
            {
                RevertBuildVersionFiles(configuration, results);
            }

            if (configuration.EmailResults)
            {
                SendResultViaEmail(configuration, results);
            }
        }

        /// <summary>
        /// Zips the source files.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="results">The results.</param>
        /// <param name="sourceDirectory">The source directory.</param>
        public static void ZipSourceFiles(
            BuildControlParameters configuration,
            DailyBuildFullResults results,
            string sourceDirectory)
        {
            string archive = Path.Combine(sourceDirectory, SourceArchiveName);
            try
            {
                string heroAppSource = FindHeroAppSourceFolder(configuration);
                using (ZipFile zip = new ZipFile(archive))
                {
                    for (Int32 i = 0; i < configuration.Projects.Length; i++)
                    {
                        if (configuration.Projects[i].BuildConfiguration != BuildProjectControl.None)
                        {
                            string appFolder = Path.Combine(heroAppSource, configuration.Projects[i].ProjectPath);
                            zip.AddDirectory(appFolder, configuration.Projects[i].ProjectPath);
                        }
                    }
                    zip.Save();
                }
            }
            catch (Exception e)
            {
                ProgramExecutionLog.AddEntry(
                 "Failed to archive source files" +
                  " Error was " + e.Message);
            }
        }

    }
}
