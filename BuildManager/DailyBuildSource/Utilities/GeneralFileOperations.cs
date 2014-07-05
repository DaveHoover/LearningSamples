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
    using System.IO;

    /// <summary>
    /// General file support functions to be used by all file 
    /// support types.
    /// </summary>
    public sealed class GeneralFileOperations
    {
        /// <summary>
        /// Remove public constructor
        /// </summary>
        private GeneralFileOperations()
        {
        }


        public static void WriteProgramStatusFile(ExecutionStatus status)
        {
            string statusFile = Path.Combine(Application.StartupPath, BuildControlParameters.ProgramStatusFileName);
            if (File.Exists(statusFile))
            {
                File.Delete(statusFile);
            }
            File.WriteAllText(statusFile, status.ToString());
        }

        public static ExecutionStatus ReadProgramStatusFile()
        {

            string statusFile = Path.Combine(Application.StartupPath, BuildControlParameters.ProgramStatusFileName);
            ExecutionStatus status = ExecutionStatus.Idle;
            if (File.Exists(statusFile))
            {

                string fileContents = File.ReadAllText(statusFile);
                try
                {
                    status = (ExecutionStatus)Enum.Parse(typeof(ExecutionStatus), fileContents);
                }
                catch (Exception)
                {
                }
            }
            return status;
        }


        /// <summary>
        /// Helper function that will check to see if a file has a read only 
        /// attribute set. If it does, then the attribute is removed.
        /// </summary>
        /// <param name="configuration">Main program configuration</param>
        /// <param name="fullFileName">File Name to check for read only attribute</param>
        public static void RemoveReadOnlyAttribute(
           BuildControlParameters configuration,
           string fullFileName)
        {
            FileInfo f = new FileInfo(fullFileName);

            if ((f.Attributes & FileAttributes.ReadOnly) != 0)
            {
                if (configuration.SourceDepotOnlineMode)
                {
                    ProcessOperations.SDEdit(configuration, fullFileName);
                }
                else
                {
                    f.Attributes = f.Attributes & ~FileAttributes.ReadOnly;
                }
            }
        }


        public static void CreateDirectory(string directoryPath)
        {
            try
            {
                Directory.CreateDirectory(directoryPath);
            }
            catch (Exception e)
            {
                ProgramExecutionLog.AddEntry(
                              "Failed to create directory " + directoryPath + " Exception information was " +
                            e.ToString());
            }
        }



        /// <summary>
        /// Helper function to delete a directory and all subdirectories
        /// </summary>
        /// <param name="directoryPath">Directory to delete</param>
        public static void DeleteDirectory(string directoryPath)
        {
            try
            {
                ClearAttributes(directoryPath);

                Directory.Delete(directoryPath, true);
            }
            catch (IOException e1)
            {
                ProgramExecutionLog.AddEntry(
                    "Failed to delete directory " + directoryPath + " Exception information was " +
                  e1.ToString());
            }
            catch (UnauthorizedAccessException e2)
            {
                ProgramExecutionLog.AddEntry(
                   "Failed to delete directory " + directoryPath + " Exception information was " +
                 e2.ToString());
            }

        }


        public static void DeleteFile(string fileName)
        {
            try
            {
                File.Delete(fileName);
            }
            catch (Exception e)
            {
                ProgramExecutionLog.AddEntry(
               "Failed to delete file " + fileName + " Exception information was " +
                      e.ToString());
            }
        }

        public static void RenameFile(string sourceFileName, string targetFileName)
        {
            try
            {
                File.Move(sourceFileName, targetFileName);
            }
            catch (Exception e)
            {
                ProgramExecutionLog.AddEntry(
             "Failed to rename file " + sourceFileName + " to " + targetFileName + " Exception information was " +
                    e.ToString());
            }

        }

        public static void ClearAttributes(string currentDir)
        {
            if (Directory.Exists(currentDir))
            {
                string[] subDirs = Directory.GetDirectories(currentDir);
                foreach (string dir in subDirs)
                {
                    ClearAttributes(dir);
                }
                string[] files = files = Directory.GetFiles(currentDir);
                foreach (string file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                }
            }
        }

        /// <summary>
        /// Helper function to create a file folder if one does not exist
        /// </summary>
        /// <param name="path">Folder path to check if it exists and to create</param>
        public static void CheckForFolderAndCreate(string path)
        {
            DirectoryInfo d = new DirectoryInfo(path);
            if (!d.Exists)
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
