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

namespace BuildManager
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using Microsoft.Win32;
    using BuildManager.Model;



    /// <summary>
    /// This class will contain some common process operations to assist in starting up a 
    /// process where we are to wait for it to exit, as well as to start and return.
    /// </summary>
    public sealed class ProcessOperations
    {

        private static int numOutputLines = 0;

        public static MainWindow mainWindow { get; set; }

        /// <summary>
        /// Remove constructor from static only method class
        /// </summary>
        private ProcessOperations()
        {
        }

        /// <summary>
        /// Call sd edit command on the given input file
        /// </summary>
        /// <param name="configuration">Main program configuration</param>
        /// <param name="fileName">File name to have the sd command execute on</param>
        /// <returns>ProcessInformation, so the caller can look at the return code etc</returns>
        public static ProcessInformation SDEdit(
           BuildControlParameters configuration,
           string fileName)
        {
            string arguments = "edit " + Path.GetFileName(fileName);
            ProcessInformation procInfo = SDCommand(configuration, fileName, arguments);
            return procInfo;
        }

        /// <summary>
        /// Call sd edit command on the given input file
        /// </summary>
        /// <param name="configuration">Main program configuration</param>
        /// <param name="fileName">File name to have the sd command execute on</param>
        /// <returns>ProcessInformation, so the caller can look at the return code etc</returns>
        public static ProcessInformation SDAdd(
           BuildControlParameters configuration,
           string fileName)
        {
            string arguments = "add " + fileName;
            ProcessInformation procInfo = SDCommand(configuration, fileName, arguments);
            return procInfo;
        }

        /// <summary>
        /// Call sd sync command for the enlistment
        /// </summary>
        /// <param name="configuration">Main program configuration</param>
        /// <param name="fileName">File name to have the sd command execute on</param>
        /// <returns>ProcessInformation, so the caller can look at the return code etc</returns>
        public static ProcessInformation SDSync(
           BuildControlParameters configuration)
        {
            string arguments = "sync ";
            ProcessInformation procInfo = SDCommand(configuration, ".", arguments);
            return procInfo;
        }

        /// <summary>
        /// Call sd Revert command on the given input file
        /// </summary>
        /// <param name="configuration">Main program configuration</param>
        /// <param name="fileName">File name to have the sd command execute on</param>
        /// <returns>ProcessInformation, so the caller can look at the return code etc</returns>
        public static ProcessInformation SDRevert(
           BuildControlParameters configuration,
           string fileName)
        {
            string arguments = "revert " + fileName;
            ProcessInformation procInfo = SDCommand(configuration, fileName, arguments);
            return procInfo;
        }




        /// <summary>
        /// Execute a sd command
        /// </summary>
        /// <param name="configuration">Main Program configuration</param>
        /// <param name="fileName">File name to perform the SD command on</param>
        /// <param name="arguments">Filled out argument string that will contain the sd command as 
        /// well as the file to operate on.</param>
        /// <returns></returns>
        private static ProcessInformation SDCommand(
           BuildControlParameters configuration,
           string fileName,
           string arguments)
        {
            string progName = Path.Combine(configuration.PathForSourceDepotProgram, "sd.exe");
            string workingDirectory = Path.GetDirectoryName(fileName);
            ProcessInformation procInfo = RunProcess(progName, workingDirectory, arguments, true);
            return procInfo;
        }


        /// <summary>
        /// Startup a process
        /// </summary>
        /// <param name="processName">Name of the Program to start</param>
        /// <param name="workingDirectory">Working directory for the program</param>
        /// <param name="arguments">Arguments to pass to the program</param>
        /// <param name="waitForExit">true if we are to wait for the process to exit</param>
        /// <returns>ProcessInformation class is returned with all relevant data for the 
        /// process. Much more data is returned if we are to wait for the program to exit.</returns>
        public static ProcessInformation RunProcess(
           string processName,
           string workingDirectory,
           string arguments,
           bool waitForExit)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.CreateNoWindow = true;
            //processStartInfo.CreateNoWindow = false;
            processStartInfo.FileName = processName;
            processStartInfo.WorkingDirectory = workingDirectory;
            processStartInfo.Arguments = arguments;
            ProcessInformation returnInfo;
            if (waitForExit)
            {
                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.RedirectStandardError = true;
                Process buildProcess = Process.Start(processStartInfo);
                buildProcess.PriorityClass = ApplicationState.ProgramSavedState.ProcessPriority;
                mainWindow.viewModel.RunningJobs.CommandProcess = buildProcess;
                // Read the standard output of the spawned process.

#if Synchronous
                string output = contents.StandardOutput.ReadToEnd();
                string errorOutputInfo = contents.StandardError.ReadToEnd();
                string[] splitArray = new string[] { BuildControlParameters.CommandLineProgLineSplit };
                string[] standardOutput = null;
                if (output != null)
                {
                    standardOutput = output.Split(splitArray, StringSplitOptions.None);
                }
                string[] errorOutput = null;
                if (errorOutputInfo != null)
                {
                    errorOutput = errorOutputInfo.Split(splitArray, StringSplitOptions.None);
                }
                contents.WaitForExit();
                returnInfo = new ProcessInformation(processStartInfo,
                    null, standardOutput, errorOutput, (Int32)contents.ExitCode);

#else
                numOutputLines = 0;
                // Set our event handler to asynchronously read the sort output.
                buildProcess.OutputDataReceived += new DataReceivedEventHandler(SortOutputHandler);
                buildProcess.BeginOutputReadLine();
                buildProcess.BeginErrorReadLine();
                buildProcess.WaitForExit();
                try
                {
                buildProcess.CancelErrorRead();
                buildProcess.CancelOutputRead();
                returnInfo = new ProcessInformation(processStartInfo,
                    null, null, null, (Int32)buildProcess.ExitCode);
                buildProcess.Dispose();

                }
                catch (Exception)
                {
                    returnInfo = null;
                }
                mainWindow.viewModel.RunningJobs.CommandProcess = null;
#endif

            }
            else
            {
                Process contents = Process.Start(processStartInfo);
                returnInfo = new ProcessInformation(processStartInfo, contents);
            }
            return returnInfo;
        }

           private static void SortOutputHandler(object sendingProcess, 
            DataReceivedEventArgs outLine)
        {
            //Action<string> log = delegate(string s) 
            //{
            //    AddLogEntry(s); 
            //};
            //Action<string> log = s => AddLogEntry(s);

            Func<string, int, bool> log = ((s, l) =>
                {
                    lock (mainWindow.viewModel.Log.lockObject)
                    {
                        // Just show the last number of lines for a build log.
                        if (mainWindow.viewModel.Log.Count > JobSetInformation.LogSize)
                        {
                            mainWindow.viewModel.Log.RemoveAt(0);
                        }
                        mainWindow.viewModel.Log.Add("[" + l.ToString() + "] - " + s);
                        mainWindow.ScrollJobDetailsBoxToEnd();
                    }
                    return true;
                });

            // Collect the sort command output.
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                numOutputLines++;
                // Add the text to the collected output.
                //Debug.WriteLine("[" + numOutputLines.ToString() + "] - " + outLine.Data);
                mainWindow.Dispatcher.BeginInvoke(log , new object[] { outLine.Data , numOutputLines });
            }
        }



        public static void KillSetupProcesses()
        {
            Process[] fullList = Process.GetProcesses();
            for (Int32 i = 0; i < fullList.Length; i++)
            {
                if (fullList[i].ProcessName.ToLower().Contains("setup"))
                {
                    fullList[i].Kill();
                }
            }
        }

        public static void KillSwanProcesses()
        {
            Process[] fullList = Process.GetProcesses();
            for (Int32 i = 0; i < fullList.Length; i++)
            {
                if (fullList[i].ProcessName.ToLower().Contains("controller"))
                {
                    fullList[i].Kill();
                }
            }

            //try
            //{

            //    Registry.LocalMachine.DeleteSubKeyTree(@"Software\Wow6432Node\Swan");
            //}
            //catch (Exception e)
            //{


            //}


            using (RegistryKey hklm = Registry.LocalMachine)
            {
                if (Is64Bit())
                {
                    try
                    {

                        using (RegistryKey wow64 = hklm.OpenSubKey(@"SOFTWARE\Wow6432Node", true))
                        {
                            string[] keyNames = wow64.GetSubKeyNames();
                            for (Int32 i = 0; i < keyNames.Length; i++)
                            {
                                if (keyNames[i].ToLower() == "swan")
                                {
                                    wow64.DeleteSubKeyTree("Swan");
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        ProgramExecutionLog.AddEntry(
                          "64 bit system. Failed to delete  Swan  subkey Error was " +
                          e.Message);
                    }
                }
                else
                {
                    try
                    {
                        using (RegistryKey software = hklm.OpenSubKey(@"SOFTWARE", true))
                        {
                            string[] keyNames = software.GetSubKeyNames();
                            for (Int32 i = 0; i < keyNames.Length; i++)
                            {
                                if (keyNames[i].ToLower() == "swan")
                                {
                                    software.DeleteSubKeyTree("Swan");
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        ProgramExecutionLog.AddEntry(
                          "32 bit system. Failed to delete  Swan  subkey Error was " +
                           e.Message);
                    }
                }
            }

        }

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool lpSystemInfo);

        internal static bool Is64Bit()
        {
            if (IntPtr.Size == 8 || (IntPtr.Size == 4 && Is32BitProcessOn64BitProcessor()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static bool Is32BitProcessOn64BitProcessor()
        {
            bool retVal;

            IsWow64Process(Process.GetCurrentProcess().Handle, out retVal);

            return retVal;
        }


    }
}
