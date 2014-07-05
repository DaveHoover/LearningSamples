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
using System.Threading;

namespace DailyBuildSource
{
    class Program
    {

        /// <summary>
        /// Program configuration file name.
        /// </summary>
        public const string MainConfigurationFile = "DailyBuildConfiguration.xml";

        /// <summary>
        /// File name for general execution status
        /// </summary>
        public const string ExecutionLogFile = "ExecutionStatus.log";

        /// <summary>
        /// File name for email summary log information that will be used to 
        /// create the email summary
        /// </summary>
        public const string EmailSummaryLogFile = "EmailSummary.log";


        /// <summary>
        /// Mains the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        static void Main(string[] args)
        {

            DailyBuildFullResults results = new DailyBuildFullResults();
            BuildControlParameters configuration = null;
            try
            {
                if (File.Exists(MainConfigurationFile))
                {
                    Console.Write("Reading Configuration File .... ");
                    configuration = BuildControlParameters.LoadConfiguration(MainConfigurationFile);
                    Console.WriteLine(" Success");

                }
                else
                {
                    Console.WriteLine("Fail. No Configuration file found, creating sample starting file...");
                    configuration = new BuildControlParameters();
                    configuration.SampleInit();
                    BuildControlParameters.SaveConfiguration(configuration, MainConfigurationFile);
                    Console.WriteLine("Sample file created ");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to read Configuration File. Program will exit. " +
                    "See log file");
                Thread.Sleep(5000);
                ProgramExecutionLog.AddEntry("Failed to read Configuration File. Error was " +
                    e.Message);
                ProgramExecutionLog.AddEntry("Required configuration file is " +
                    MainConfigurationFile + " and in the same directory");

                string[] logEntries = ProgramExecutionLog.FullLogContents();
                File.WriteAllLines(ExecutionLogFile, logEntries);
                Environment.Exit(1);
            }
            configuration.UpdateWpdtLkgLink();
            results.Reset(configuration);
            results.UpdateWpdtOpsForInstalledVersion(configuration);

            CheckForWpdtTestCaseFiles(configuration);

            ExecutionStatus status = GeneralFileOperations.ReadProgramStatusFile();

            ParseCommandLineArguments(args, status);

            WpdtOpsUninstall(configuration, results);

            if (configuration.SyncEnlistment)
            {
                SourceCodeOps.SyncEnlistment(configuration, results);
            }

            WpdtOpsInstall(configuration, results);

            UpdateYdr(configuration, results);

            SourceCodeOps.CreateBuildScriptAndExecute(configuration, results);

            WriteSummaryResults(configuration, results);
            GeneralFileOperations.WriteProgramStatusFile(ExecutionStatus.Idle);
        }

        /// <summary>
        /// Parses the command line arguments.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <param name="status">The status.</param>
        private static void ParseCommandLineArguments(string[] args, ExecutionStatus status)
        {
            if (args.Length > 0)
            {
                if (args[0].ToLower() == "autologon")
                {
                    if (status == ExecutionStatus.UninstallWdpt)
                    {
                        return;
                    }
                    Environment.Exit(0);
                }
            }
        }

        /// <summary>
        /// Checks for WPDT test case files.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        private static void CheckForWpdtTestCaseFiles(BuildControlParameters configuration)
        {
            if (configuration.UpdateWpdt)
            {
                string uninstallTestCase = Path.Combine(configuration.SwanTestCasePath,
                    configuration.WpdtUninstallTestCase);
                string installTestCase = Path.Combine(configuration.SwanTestCasePath,
                    configuration.WpdtInstallTestCase);
                bool exitProgram = false;
                if (!File.Exists(uninstallTestCase))
                {
                    exitProgram = true;
                    string error1 = "Wpddt uninstall Test Case File " + uninstallTestCase +
                        " not found. Program will exit";

                    Console.WriteLine(error1);
                    ProgramExecutionLog.AddEntry(error1);
                    Thread.Sleep(2000);
                }
                if (!File.Exists(installTestCase))
                {
                    exitProgram = true;
                    string error2 = "Wpddt install Test Case File " + installTestCase +
                        " not found. Program will exit";
                    Console.WriteLine(error2);
                    ProgramExecutionLog.AddEntry(error2);
                    Thread.Sleep(2000);
                }
                if (exitProgram)
                {
                    Environment.Exit(1);
                }
            }
        }


        /// <summary>
        /// WPDTs the ops uninstall.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="results">The results.</param>
        private static void WpdtOpsUninstall(
            BuildControlParameters configuration,
            DailyBuildFullResults results)
        {
            ExecutionStatus status = GeneralFileOperations.ReadProgramStatusFile();
            results.WpdtVersion = WpdtOperations.DetermineWdptInstalledVersion(configuration);
            results.YdrVersion = "Same as Wpdt";
            if (status != ExecutionStatus.UninstallWdpt &&
                 configuration.UpdateWpdt)
            {
                if (WpdtOperations.IsWpdtInstalled(configuration))
                {
                    if (!WpdtOperations.IsInstalledWpdtVersionSameAsLkg(configuration))
                    {
                        WpdtOperations.UninstallWdpt(configuration);
                        // Just show some time on the console before we exit
                        for (Int32 i = 0; i < 10; i++)
                        {
                            Console.Write(".");
                            Thread.Sleep(1000);
                        }
                        Console.WriteLine("Exiting process, waiting for Wpdt Uninstall to complete and reboot");
                        // Just show some more time on the console
                        for (Int32 i = 0; i < 10; i++)
                        {
                            Console.Write(".");
                            Thread.Sleep(1000);
                        }
                        Environment.Exit(0);
                    }
                }
                else
                {
                    Console.WriteLine("Requested to Uninstall Wpdt, but it was not installed!");
                    ProgramExecutionLog.AddEntry(
                    "Requested to uninstall Wpdt, but it was not installed");
                }
            }
            if (status == ExecutionStatus.UninstallWdpt)
            {
                results.UninstallWdpt.SetResults(true, "");
                ProcessOperations.KillSwanProcesses();
                Console.WriteLine("Continuing build process after reboot from Wpdt uninstall ...");
                GeneralFileOperations.WriteProgramStatusFile(ExecutionStatus.Running);
            }
        }


        /// <summary>
        /// WPDTs the ops install.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="results">The results.</param>
        private static void WpdtOpsInstall(
            BuildControlParameters configuration,
            DailyBuildFullResults results)
        {
            if (configuration.UpdateWpdt &&
                !WpdtOperations.IsInstalledWpdtVersionSameAsLkg(configuration))
            {
                WpdtOperations.PrepareWpdtTargetDirectory(configuration, results);
                if (configuration.InstallOnW2k8)
                {
                    WpdtOperations.ModifyInstallForW2k8(configuration, results);
                }
                WpdtOperations.InstallWdpt(configuration, results);
            }
        }

        /// <summary>
        /// Updates the ydr.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="results">The results.</param>
        private static void UpdateYdr(
             BuildControlParameters configuration,
             DailyBuildFullResults results)
        {
            if (configuration.UpdateYdr)
            {
                WpdtOperations.UninstallYdr(configuration);
                WpdtOperations.InstallYdr(configuration, results);
            }
        }



        /// <summary>
        /// Writes the summary results.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="results">The results.</param>
        private static void WriteSummaryResults(
            BuildControlParameters configuration,
            DailyBuildFullResults results)
        {
            // Summary results via writing execution logs to output share.
            GeneralFileOperations.WriteProgramStatusFile(ExecutionStatus.Idle);
            string[] logEntries = ProgramExecutionLog.FullLogContents();
            string[] emailSummary = results.GenerateResultsSummary().ToArray<string>();
            File.WriteAllLines(ExecutionLogFile, logEntries);
            File.WriteAllLines(EmailSummaryLogFile, emailSummary);
            if (!String.IsNullOrEmpty(results.PublishLogShare))
            {
                File.WriteAllLines(Path.Combine(results.PublishLogShare, ExecutionLogFile), logEntries);
                File.WriteAllLines(Path.Combine(results.PublishLogShare, EmailSummaryLogFile), emailSummary);
            }
        }
    }
}
