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
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;



    /// <summary>
    /// This class will walk the selected tree and create lists of information that can be used 
    /// for later processing.
    /// 
    /// </summary>
    public sealed class DirFileDiscovery
    {

        /// <summary>
        /// Remove the private constructor
        /// </summary>
        private DirFileDiscovery()
        {
        }

        /// <summary>
        /// This function was from a previous application, but collected information
        /// related to files.
        /// This is left here for possible use going forward
        /// </summary>
        /// <param name="directoryPath">Directory to enumerate</param>
        /// <param name="recursive">True if we are to process directories recursively</param>
        /// <param name="applyExclusions">True if we are to apply exclusions</param>
        /// <param name="configuration">Main configuration settings</param>
        /// <param name="results">Results for walking the tree object</param>
        public static void FindDirFileInfo(
           string directoryPath,
           bool recursive,
           bool applyExclusions,
           BuildControlParameters configuration,
           TotalDirectoryFileStats results)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                DirectoryInfo[] directories = directoryInfo.GetDirectories();
                results.TotalNumberOfDirectories += directories.Length;
                FileInfo[] fileSet = directoryInfo.GetFiles();
                results.TotalNumberOfFiles += fileSet.Length;
                foreach (DirectoryInfo d in directories)
                {
                    if (configuration.AbortCurrentOperation)
                    {
                        return;
                    }
                    results.AllDirectories.Add(d.FullName);
                    bool excludeDirectory = false;
                    if (applyExclusions)
                    {
                        foreach (string exclude in configuration.DirectoriesToExclude)
                        {
                            if (d.Name.ToLower(CultureInfo.CurrentCulture) == exclude.ToLower(CultureInfo.CurrentCulture))
                            {
                                results.TotalNumberOfExcludedDirectories++;
                                excludeDirectory = true;
                                break;
                            }
                        }
                    }
                    if (!excludeDirectory)
                    {
                        results.AllSelectedDirectories.Add(d.FullName);
                        results.TotalNumberOfSelectedDirectories++;
                    }

                }
                foreach (FileInfo f in fileSet)
                {
                    results.AllFiles.Add(f.FullName);
                    bool excludeFile = false;
                    if (applyExclusions)
                    {
                        foreach (string exclude in configuration.FilesToExclude)
                        {
                            if (MatchFiles(exclude, f.Name))
                            {
                                results.TotalNumberOfExcludedFiles++;
                                excludeFile = true;
                                break;
                            }
                        }
                    }
                    if (!excludeFile)
                    {
                        results.AllSelectedFiles.Add(f.FullName);
                        results.TotalNumberOfSelectedFiles++;
                    }
                }

                if (directories.Length > 0)
                {
                    foreach (DirectoryInfo info in directories)
                    {
                        if (results.AllSelectedDirectories.Contains(info.FullName))
                        {
                            FindDirFileInfo(info.FullName, recursive, applyExclusions, configuration, results);
                        }
                    }
                }
            }
            catch (IOException)
            {
                results.DirectoryFileExceptions.Add("Parent Directory Name is " + directoryPath);
            }
        }

        /// <summary>
        /// This function will take a list of input strings and will match according to the 
        /// directory file specification entered. This is to mirror a list of files that we 
        /// got from a zip file and be able to select a subset just like we do on the file 
        /// system.
        /// </summary>
        /// <param name="selection">Actual file selection *.pkg for instance</param>
        /// <param name="fileName">File name to compare against</param>
        /// <returns>true if this file is a match, false otherwise</returns>
        private static bool MatchFiles(
           string selection,
           string fileName)
        {
            // Convert the file selection specification to the appropriate Regex maching expression.
            // I don't support all cases, but handle the standard file system wild cards.
            string regexMatch = ConvertFileSelectToRegEx(selection);
            Regex dirMatch = new Regex(regexMatch,
              RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.RightToLeft);
            Match m = dirMatch.Match(fileName);
            return m.Success;
        }

        /// <summary>
        /// Helper function to take a directory specification in the form of something like
        /// *.pkg.* and expand to a regular expression query.
        /// Right now only * and . are support as special characters. ? is not supported
        /// </summary>
        /// <param name="fileSelect">File selector</param>
        /// <returns>Regex expression</returns>
        private static string ConvertFileSelectToRegEx(string fileSelect)
        {
            string regex = fileSelect.Replace(".", @"\.");
            regex = regex.Replace("*", ".*");
            return regex;
        }

    }
}
