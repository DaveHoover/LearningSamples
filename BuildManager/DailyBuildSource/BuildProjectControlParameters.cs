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

    public enum BuildProjectControl
    {
        None,
        Solution,
        Project
    }

    public class BuildProjectControlParameters
    {
        public string ProjectPath { get; set; }
        public string SolutionPathAndNameFromProjectRoot { get; set; }
        public string XapPathAndNameFromProjectRoot { get; set; }
        public string ProjectPathAndNameFromProjectRoot { get; set; }
        public string AssemblyFilePathAndNameFromProjectRoot { get; set; }
        public BuildProjectControl BuildConfiguration { get; set; }
        public string LkgVersion { get; set; }


        public BuildProjectControlParameters()
        {

        }


        public BuildProjectControlParameters(
            BuildProjectControl buildConfiguration,
            string lkgVersion,
            string projectPath,
            string solutionPathAndName,
            string projectPathAndNameFromSolution,
            string xapPathAndNameFromSolution,
            string assemblyFilePath
            )
        {
            this.ProjectPath = projectPath;
            this.SolutionPathAndNameFromProjectRoot = solutionPathAndName;
            this.XapPathAndNameFromProjectRoot = xapPathAndNameFromSolution;
            this.ProjectPathAndNameFromProjectRoot = projectPathAndNameFromSolution;
            this.AssemblyFilePathAndNameFromProjectRoot = assemblyFilePath;
            this.BuildConfiguration = buildConfiguration;
            this.LkgVersion = lkgVersion;
        }


    }
}
