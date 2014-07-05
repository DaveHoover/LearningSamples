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
using System.Text;

namespace DailyBuildSource
{

    /// <summary>
    /// 
    /// </summary>
    public class BuildTargetResults
    {

        public const Int32 ProjectMaxLength = 20;
        /// <summary>
        /// Start time for the build
        /// </summary>
        public DateTime BuildStartTime { get; set; }

        /// <summary>
        /// Name of the project folder 
        /// </summary>
        public string ProjectTargetName { get; set; }

        /// <summary>
        /// Link to published build log
        /// </summary>
        public string BuildLogFileLink { get; set; }

        /// <summary>
        /// True if all operations are successful
        /// </summary>
        public bool OperationSuccessful
        {
            get
            {
                return Build.Success && ModifyVersion.Success && RevertVersion.Success &&
                    SignXap.Success && LicenseXap.Success;
            }
        }

        /// <summary>
        /// Build status
        /// </summary>
        public SingleOperationResults Build = new SingleOperationResults("Build");

        /// <summary>
        /// Modify assembly version status
        /// </summary>
        public SingleOperationResults ModifyVersion = new SingleOperationResults("ModifyVersion");

        /// <summary>
        /// Revert assemlby version status
        /// </summary>
        public SingleOperationResults RevertVersion = new SingleOperationResults("RevertVersion");

        /// <summary>
        /// SignXap status
        /// </summary>
        public SingleOperationResults SignXap = new SingleOperationResults("SignXap");

        /// <summary>
        /// LicenseXap Status
        /// </summary>
        public SingleOperationResults LicenseXap = new SingleOperationResults("LicenseXap");

        /// <summary>
        /// Default constructor
        /// </summary>
        public BuildTargetResults()
        {

        }


        public void Reset(BuildControlParameters configuration)
        {
            Build.Reset();
            ModifyVersion.Reset();
            RevertVersion.Reset();
            SignXap.Reset();
            LicenseXap.Reset();

            if (configuration.BuildType == BuildTypeEnum.None)
            {
                Build.StatusToNotRun();
                ModifyVersion.StatusToNotRun();
                RevertVersion.StatusToNotRun();
                SignXap.StatusToNotRun();
                LicenseXap.StatusToNotRun();
            }
            if (!configuration.UpdateDailyBuildVersion)
            {
                ModifyVersion.StatusToNotRun();
                RevertVersion.StatusToNotRun();
            }

        }


        /// <summary>
        /// Returns a summary of the operation results
        /// </summary>
        /// <returns>
        /// Summary of results 
        /// </returns>
        public override string ToString()
        {
            string r;
            if (OperationSuccessful)
            {
                r = "Build of " + ProjectTargetName + " Successful!";
            }
            else
            {
                StringBuilder b = new StringBuilder();
                if (ProjectTargetName.Length < ProjectMaxLength)
                {
                    for (Int32 i = ProjectTargetName.Length; i < ProjectMaxLength; i++)
                    {
                        b.Append(" ");
                    }
                }
                r = ProjectTargetName + ": " + b.ToString() +
                    Build.ToString() + " " +
                    ModifyVersion.ToString() + " " +
                    RevertVersion.ToString() + " " +
                    SignXap.ToString() + " " +
                    LicenseXap.ToString();
            }
            return r;
        }
    }
}
