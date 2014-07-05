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

namespace DailyBuildSource
{

    using System;
    using System.Diagnostics;

    /// <summary>
    /// This class will hold information related to starting up a process.
    /// </summary>
    public class ProcessInformation
    {
        /// <summary>
        /// Reference to the ProcessStartInfo object that was used to start the process
        /// </summary>
        public ProcessStartInfo ProcessStartReference { get; private set; }

        /// <summary>
        /// Handle to the process that was created. Will be null if we waited for the 
        /// process to exit
        /// </summary>
        public Process ProcessHandle { get; private set; }

        /// <summary>
        /// Output from the process standard output stream.
        /// Null if we don't wait for the process to exit
        /// </summary>
        private string[] standardOutput;

        /// <summary>
        /// Output from the process standard output stream.
        /// Null if we don't wait for the process to exit
        /// </summary>
        public string[] GetStandardOutput()
        {
            if (this.standardOutput != null)
            {
                return this.standardOutput;
            }
            else
            {
                string[] lines = new string[1];
                return lines;
            }

        }

        /// <summary>
        /// Output from the process error stream. Null if we don't wait for the process to exit
        /// </summary>
        private string[] errorOutput;


        /// <summary>
        /// Output from the process error stream. Null if we don't wait for the process to exit
        /// </summary>
        public string[] GetErrorOutput()
        {
            if (this.errorOutput != null)
            {
                return this.errorOutput;
            }
            else
            {
                string[] lines = new string[1];
                return lines;
            }

        }

        /// <summary>
        /// Exit code from the process, if we waited for the process to exit. 
        /// Will be set to zero otherwise
        /// </summary>
        public Int32 ExitCode { get; private set; }


        /// <summary>
        /// Constructor to use when we don't wait for the process to exit
        /// </summary>
        /// <param name="startInfo">Process start info information that was used to 
        /// start the process</param>
        /// <param name="handle">Process handle for the created process</param>
        public ProcessInformation(ProcessStartInfo startInfo, Process handle)
        {
            this.ProcessStartReference = startInfo;
            this.ProcessHandle = handle;
            this.ExitCode = 0;
        }

        /// <summary>
        /// Constructor to use when we wait for the process to exit.
        /// </summary>
        /// <param name="startInfo">Process start info information that was used to 
        /// start the process</param>
        /// <param name="handle">Process handle for the created process. Normally null 
        /// in this constructor, as the process has probably exited</param>
        /// <param name="standardOutput">Output from the standard output stream, if any</param>
        /// <param name="errorOutput">Output from the standard error stream, if any</param>
        /// <param name="exitCode">Exit code for the process</param>
        public ProcessInformation(
           ProcessStartInfo startInfo,
           Process handle,
           string[] standardOutput,
           string[] errorOutput,
           Int32 exitCode)
        {
            this.ProcessStartReference = startInfo;
            this.ProcessHandle = handle;
            this.standardOutput = standardOutput;
            this.errorOutput = errorOutput;
            this.ExitCode = exitCode;
        }
    }
}
