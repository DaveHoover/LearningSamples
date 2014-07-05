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
    using System.Collections.ObjectModel;

    /// <summary>
    /// This class will hold the results of walking the selected directory tree.
    /// This will hold the total statistics of files selected, and directories selected, as well
    /// as the results of looking for byte order marks in files, copyright failures, and contents.oak
    /// failures.
    /// </summary>
    public class TotalDirectoryFileStats
    {
        /// <summary>
        /// Total number of directories in the selected tree. All directories are inclded
        /// </summary>
        public Int32 TotalNumberOfDirectories { get; set; }

        /// <summary>
        /// All selected directories, after applying the requested exclusions
        /// </summary>
        public Int32 TotalNumberOfSelectedDirectories { get; set; }

        /// <summary>
        /// Number of directories that were excluded
        /// </summary>
        public Int32 TotalNumberOfExcludedDirectories { get; set; }

        /// <summary>
        /// Total number of files in the selected tree
        /// </summary>
        public Int32 TotalNumberOfFiles { get; set; }

        /// <summary>
        /// Total number of files in the selected tree after applying exclusions
        /// </summary>
        public Int32 TotalNumberOfSelectedFiles { get; set; }

        /// <summary>
        /// Number of files excluded
        /// </summary>
        public Int32 TotalNumberOfExcludedFiles { get; set; }

        /// <summary>
        /// Number of files with byte order marks
        /// </summary>
        public Int32 TotalNumberOfFilesContainingByteOrderMarks { get; set; }

        /// <summary>
        /// Number of directories where there was a contents.oak failure
        /// </summary>
        public Int32 TotalNumberOfContentsOakFailures { get; set; }

        /// <summary>
        /// Total number of copyright failures
        /// </summary>
        public Int32 TotalNumberOfCopyrightFailures { get; set; }

        /// <summary>
        /// Backing store for the AllDirectories property
        /// </summary>
        private Collection<string> allDirectories = new Collection<string>();

        /// <summary>
        /// List of all directories in the tree
        /// </summary>
        public Collection<string> AllDirectories
        {
            get
            {
                return this.allDirectories;
            }
        }

        /// <summary>
        /// Backing store for the AllFiles collection
        /// </summary>
        private Collection<string> allFiles = new Collection<string>();

        /// <summary>
        /// All files in the selected directory tree
        /// </summary>
        public Collection<string> AllFiles
        {
            get
            {
                return this.allFiles;
            }
        }

        /// <summary>
        /// Backing store for the AllSelectedDirectories property
        /// </summary>
        private Collection<string> allSelectedDirectories = new Collection<string>();

        /// <summary>
        /// List of all selected directories after applying exclusions
        /// </summary>
        public Collection<string> AllSelectedDirectories
        {
            get
            {
                return this.allSelectedDirectories;
            }
        }

        /// <summary>
        /// Backing store for the AllSelectedFiles property
        /// </summary>
        private Collection<string> allSelectedFiles = new Collection<string>();

        /// <summary>
        /// All selected files copyright checking
        /// </summary>
        public Collection<string> AllSelectedFiles
        {
            get
            {
                return this.allSelectedFiles;
            }
        }

        /// <summary>
        /// Backing store for the FilesWithByteOrderMarks property
        /// </summary>
        private Collection<string> filesWithByteOrderMarks = new Collection<string>();

        /// <summary>
        /// List of files that have ByteOrderMarks which can cause style cop to fail
        /// </summary>
        public Collection<string> FilesWithByteOrderMarks
        {
            get
            {
                return this.filesWithByteOrderMarks;
            }
        }


        /// <summary>
        /// Backing store for the FileWithCoyrightFailure property
        /// </summary>
        private Collection<string> filesWithCopyrightFailure = new Collection<string>();

        /// <summary>
        /// List of files that failed the copyright check
        /// </summary>
        public Collection<string> FilesWithCopyrightFailure
        {
            get
            {
                return this.filesWithCopyrightFailure;
            }
        }

        /// <summary>
        /// Backing store for the DirectoriesWithContentsOakFailure
        /// </summary>
        private Collection<string> directoriesWithContentsOakFailure = new Collection<string>();

        /// <summary>
        /// List of directories with contents.oak failures
        /// </summary>
        public Collection<string> DirectoriesWithContentsOakFailure
        {
            get
            {
                return this.directoriesWithContentsOakFailure;
            }
        }

        /// <summary>
        /// Backing store for the DirectoriesWithContentsOakFailure
        /// </summary>
        private Collection<string> directoryFileExceptions = new Collection<string>();

        /// <summary>
        /// List of directories and or files  where an exception was caught when we were trying to access
        /// the directory/files
        /// </summary>
        public Collection<string> DirectoryFileExceptions
        {
            get
            {
                return this.directoryFileExceptions;
            }
        }
        /// <summary>
        /// Backing store for the DirectoriesWithContentsUpdateFailures
        /// </summary>
        private Collection<string> directoriesWithContentsUpdateFailures = new Collection<string>();

        /// <summary>
        /// List of directories and or files  where an exception was caught when we were trying to update the
        /// contents.oak files. This means a manual update will be required.
        /// </summary>
        public Collection<string> DirectoriesWithContentsUpdateFailures
        {
            get
            {
                return this.directoriesWithContentsUpdateFailures;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TotalDirectoryFileStats()
        {

        }

        /// <summary>
        /// Clear all settings for the object;
        /// </summary>
        public void Initialize()
        {
            this.TotalNumberOfDirectories = 0;
            this.TotalNumberOfSelectedDirectories = 0;
            this.TotalNumberOfExcludedDirectories = 0;
            this.TotalNumberOfFiles = 0;
            this.TotalNumberOfSelectedFiles = 0;
            this.TotalNumberOfExcludedFiles = 0;

            this.allDirectories.Clear();
            this.allFiles.Clear();
            this.allSelectedDirectories.Clear();
            this.allSelectedFiles.Clear();
            this.directoryFileExceptions.Clear();

            this.InitializeByteOrderMarkResults();
            this.InitializeContentsOakFailureResults();
            this.InitializeCopyrightFailureResults();
        }

        /// <summary>
        /// Initialize the reults for the byte order mark check
        /// </summary>
        public void InitializeByteOrderMarkResults()
        {
            this.TotalNumberOfFilesContainingByteOrderMarks = 0;
            this.filesWithByteOrderMarks.Clear();
        }

        /// <summary>
        /// Initialize the Contents.oak Failure results parameters
        /// </summary>
        public void InitializeContentsOakFailureResults()
        {
            this.TotalNumberOfContentsOakFailures = 0;
            this.directoriesWithContentsOakFailure.Clear();
            this.directoriesWithContentsUpdateFailures.Clear();
        }

        /// <summary>
        /// Initialize the results for the copyright check
        /// </summary>
        public void InitializeCopyrightFailureResults()
        {
            this.TotalNumberOfCopyrightFailures = 0;
            this.filesWithCopyrightFailure.Clear();

        }



    }
}
