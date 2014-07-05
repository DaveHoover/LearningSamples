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
using System.Xml.Serialization;

namespace BuildManager
{

    /// <summary>
    /// 
    /// </summary>
    public class DailyBuildFullResults
    {
        public DateTime BuildStartTime { get; set; }

        public string PublishShare { get; set; }

        public string PublishLogShare { get; set; }

        public string PublishSourceShare { get; set; }

        public string WpdtVersion { get; set; }

        public string YdrVersion { get; set; }

        public SingleOperationResults EnlistmentSync = new SingleOperationResults("EnlistmentSync");

        public SingleOperationResults UninstallWdpt = new SingleOperationResults("UninstallWpdt");

        public SingleOperationResults InstallWdpt = new SingleOperationResults("InstallWpdt");

        public SingleOperationResults ModifyWdpt = new SingleOperationResults("ModifyWpdt");

        public SingleOperationResults UninstallYdr = new SingleOperationResults("UninstallYdr");

        public SingleOperationResults InstallYdr = new SingleOperationResults("InstallYdr");

        public SingleOperationResults PublishToReleaseShare = new SingleOperationResults("PublishToReleaseShare");

        public SingleOperationResults EmailBuildResults = new SingleOperationResults("EmailBuildResults");

        public BuildTargetResults[] BuildTargets;


        /// <summary>
        /// Initializes a new instance of the <see cref="DailyBuildFullResults"/> class.
        /// </summary>
        public DailyBuildFullResults()
        {

        }

        /// <summary>
        /// Resets the specified configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public void Reset(BuildControlParameters configuration)
        {
            this.EnlistmentSync.Reset();
            this.UninstallWdpt.Reset();
            this.InstallWdpt.Reset();
            this.ModifyWdpt.Reset();
            this.UninstallYdr.Reset();
            this.InstallYdr.Reset();
            this.PublishToReleaseShare.Reset();
            this.EmailBuildResults.Reset();
            this.BuildTargets = null;
            this.InitializeBuildTargetResults(configuration);
            this.EnlistmentSync.RequestedToRun = configuration.SyncEnlistment;
            this.UninstallWdpt.RequestedToRun = configuration.UpdateWpdt;
            this.InstallWdpt.RequestedToRun = configuration.UpdateWpdt;
            this.UninstallYdr.RequestedToRun = configuration.UpdateYdr;
            this.InstallYdr.RequestedToRun = configuration.UpdateYdr;
            if (configuration.UpdateWpdt)
            {
                this.ModifyWdpt.RequestedToRun = configuration.InstallOnW2k8;
            }
            else
            {
                this.ModifyWdpt.RequestedToRun = false;
            }
            this.PublishToReleaseShare.RequestedToRun = true;
            this.EmailBuildResults.RequestedToRun = configuration.EmailResults;
        }


        public void UpdateWpdtOpsForInstalledVersion(BuildControlParameters configuration)
        {
           
        }


        /// <summary>
        /// Initializes the build target results.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public void InitializeBuildTargetResults(BuildControlParameters configuration)
        {
            BuildTargets = new BuildTargetResults[configuration.Projects.Length];
            for (Int32 i = 0; i < configuration.Projects.Length; i++)
            {
                BuildTargets[i] = new BuildTargetResults();
                BuildTargets[i].ProjectTargetName = configuration.Projects[i].ProjectPath;
                BuildTargets[i].Reset(configuration);
            }
        }

        /// <summary>
        /// HTML string for the fields in the loop block of the generated report.
        /// </summary>
        public string FormatHtmlInLoop()
        {
            return "<td width=126 valign=top style='width:75.4pt;border-top:none;border-left:none;border-bottom:solid black 1.0pt;border-right:solid black 1.0pt;background:#D9D9D9;padding:0in 0in 0in 0in'><p class=MsoNormal><b><span style='color:#{0}'>{1}</span><o:p></o:p></b></p></td>";
        }

        /// <summary>
        /// HTML string for the static fields of the generated report.
        /// </summary>
        public string FormatHtmlStatic()
        {
            return "<b><span style='font-size:12.0pt;font-family:\"Times New Roman\",\"serif\";color:#1F497D'>{0}</span></b><span style='font-size:12.0pt;font-family:\"Times New Roman\",\"serif\";color:#1F497D'> = </span><b><span style='font-size:12.0pt;font-family:\"Times New Roman\",\"serif\";color:#{1}'>{2}</span></b><span style='font-size:12.0pt;font-family:\"Times New Roman\",\"serif\";color:#1F497D'><o:p></o:p></span></p>";
        }

        /// <summary>
        /// Creates the build report that gets sent out to habr@microsoft.com.
        /// </summary>
        public string MakeHeroAppsReport(BuildControlParameters configuration)
        {
            string passTextColor = "00B050";
            string failTextColor = "C0504D";
            string blackTextColor = "190707";

            string sourceDir = configuration.ReleaseShareRoot;

            //Gets all log files in the dated directory 
            string[] fileEntries = Directory.GetFiles(PublishLogShare, "*.log", SearchOption.AllDirectories);

            string WPDT = "WPDT";
            string wpdtVersion = string.Format(FormatHtmlStatic(), WPDT, passTextColor, configuration.WpdtLkgSourcePath);

            string YDR = "YDR Version";
            string ydrVersion = string.Format(FormatHtmlStatic(), YDR, blackTextColor, YdrVersion);

            //Set email body static header
            string emailHeader = "<html xmlns:v=\"urn:schemas-microsoft-com:vml\" " +
            "xmlns:o=\"urn:schemas-microsoft-com:office:office\" " +
            "xmlns:w=\"urn:schemas-microsoft-com:office:word\" " +
            "xmlns:m=\"http://schemas.microsoft.com/office/2004/12/omml\" " +
            "xmlns=\"http://www.w3.org/TR/REC-html40\"><head><META HTTP-EQUIV=\"Content-Type\" " +
            "CONTENT=\"text/html; charset=us-ascii\"><meta name=Generator content=\"Microsoft Word 14 (filtered medium)\"><style><!--/* Font Definitions */@font-face	{font-family:Calibri;	panose-1:2 15 5 2 2 2 4 3 2 4;}@font-face	{font-family:Tahoma;	panose-1:2 11 6 4 3 5 4 4 2 4;}/* Style Definitions */p.MsoNormal, li.MsoNormal, div.MsoNormal	{margin:0in;	margin-bottom:.0001pt;	font-size:11.0pt;	font-family:\"Calibri\",\"sans-serif\";}a:link, span.MsoHyperlink	{mso-style-priority:99;	color:blue;	text-decoration:underline;}a:visited, span.MsoHyperlinkFollowed	{mso-style-priority:99;	color:purple;	text-decoration:underline;}span.EmailStyle17	{mso-style-type:personal;	font-family:\"Calibri\",\"sans-serif\";	color:windowtext;}span.EmailStyle18	{mso-style-type:personal-reply;	font-family:\"Calibri\",\"sans-serif\";	color:#1F497D;}.MsoChpDefault	{mso-style-type:export-only;	font-size:10.0pt;}@page WordSection1	{size:8.5in 11.0in;	margin:1.0in 1.0in 1.0in 1.0in;}div.WordSection1	{page:WordSection1;}--></style><!--[if gte mso 9]><xml><o:shapedefaults v:ext=\"edit\" spidmax=\"1026\" /></xml><![endif]--><!--[if gte mso 9]><xml><o:shapelayout v:ext=\"edit\"><o:idmap v:ext=\"edit\" data=\"1\" /></o:shapelayout></xml><![endif]--></head>";

            //Build Machine name
            string dailyBuildMachine = "Build Machine";
            string machineName = System.Environment.MachineName;
            string buildMachine = string.Format(FormatHtmlStatic(), dailyBuildMachine, blackTextColor, machineName);

            //Set build Location in HTML
            string dailyBuildLocation = "Build Location";
            string buildLocation = string.Format(FormatHtmlStatic(), dailyBuildLocation, blackTextColor, PublishShare);


            string enlistmentSync = "";
            string syncdEnlistment = "Enlistment Sync";
            if (configuration.SyncEnlistment)
            {
                if (EnlistmentSync.Success)
                {
                    string enlistmentSuccess = "PASS";
                    enlistmentSync = string.Format(FormatHtmlStatic(), syncdEnlistment, passTextColor, enlistmentSuccess);
                }
                else
                {
                    string enlistmentFail = "FAIL";
                    enlistmentSync = string.Format(FormatHtmlStatic(), syncdEnlistment, failTextColor, enlistmentFail);
                }
            }

            string wpdtStatus = "";
            string wpdtTitle = "Uninstall and Reinstall of WPDT";
            if (configuration.UpdateWpdt)
            {
                    string wpdtLkgNoChange = "Machine has current LKG installed";
                    wpdtStatus = string.Format(FormatHtmlStatic(), wpdtTitle, blackTextColor, wpdtLkgNoChange);
            }

            //Setup table formatting in HTML         
            string tableHeader = "<body lang=EN-US link=blue vlink=purple><div class=WordSection1><table class=MsoNormalTable border=0 cellspacing=0 cellpadding=0 width=781 style='width:468.45pt;border-collapse:collapse'><tr><td width=285 style='width:171.2pt;border:solid black 1.0pt;background:#D9D9D9;padding:0in 5.4pt 0in 5.4pt'><p class=MsoNormal><b><span style='color:#00B0F0'>Hero App<o:p></o:p></span></b></p></td><td width=117 style='width:70.05pt;border:solid black 1.0pt;border-left:none;background:#D9D9D9;padding:0in 5.4pt 0in 5.4pt'><p class=MsoNormal><b><span style='color:#00B0F0'>Result <o:p></o:p></span></b></p></td><td width=126 valign=top style='width:75.4pt;border:solid black 1.0pt;border-left:none;background:#D9D9D9;padding:0in 0in 0in 0in'><p class=MsoNormal><b><span style='color:#00B0F0'>Sign XAP<o:p></o:p></span></b></p></td><td width=126 valign=top style='width:75.4pt;border:solid black 1.0pt;border-left:none;background:#D9D9D9;padding:0in 0in 0in 0in'><p class=MsoNormal><b><span style='color:#00B0F0'>License XAP<o:p></o:p></span></b></p></td><td width=126 style='width:75.4pt;border-top:solid black 1.0pt;border-left:none;border-bottom:solid black 1.0pt;border-right:none;background:#D9D9D9;padding:0in 0in 0in 0in'><p class=MsoNormal><b><span style='color:#00B0F0'>Logs<o:p></o:p></span></b></p></td>";


            //Sets the static members of the email body
            string completeMailBody = emailHeader + buildMachine + buildLocation + enlistmentSync + wpdtStatus + wpdtVersion + ydrVersion + tableHeader;



            //This is the loop that builds the report. I generates all the information to feed to the email body in HTML
            foreach (BuildTargetResults target in BuildTargets)
            {

                string fileNameString = target.ProjectTargetName;
                string tempFileNameString = string.Format("<tr><td width=173 style='width:103.75pt;border:solid black 1.0pt;border-top:none;background:#D9D9D9;padding:0in 5.4pt 0in 5.4pt'><p class=MsoNormal><b>{0}<o:p></o:p></b></p></td>", fileNameString);
                completeMailBody = completeMailBody + tempFileNameString;


                if (target.Build.Success)
                {

                    string pass = "PASS";
                    string tempPass = string.Format(FormatHtmlInLoop(), passTextColor, pass);
                    completeMailBody = completeMailBody + tempPass;
                }
                else
                {

                    //App was NOT successfully built.
                    string fail = "FAIL";
                    string tempFail = string.Format(FormatHtmlInLoop(), failTextColor, fail);
                    completeMailBody = completeMailBody + tempFail;
                }
                if (target.SignXap.Success)
                {
                    //App was signed 
                    string xapSignTrue = "PASS";
                    string tempXapSignTrue = string.Format(FormatHtmlInLoop(), passTextColor, xapSignTrue);
                    completeMailBody = completeMailBody + tempXapSignTrue;
                }
                else
                {

                    string xapSignFalse = "FAIL";
                    string tempXapSignFalse = string.Format(FormatHtmlInLoop(), failTextColor, xapSignFalse);
                    completeMailBody = completeMailBody + tempXapSignFalse;

                }

                if (target.LicenseXap.Success)
                {
                    //App was signed 
                    string xapLicenseTrue = "PASS";
                    string tempXapLicenseTrue = string.Format(FormatHtmlInLoop(), passTextColor, xapLicenseTrue);
                    completeMailBody = completeMailBody + tempXapLicenseTrue;
                }
                else
                {
                    string xapLicenseFalse = "FAIL";
                    string tempXapSignFalse = string.Format(FormatHtmlInLoop(), failTextColor, xapLicenseFalse);
                    completeMailBody = completeMailBody + tempXapSignFalse;
                }

                //Sets the log location for all the apps
                string logLocation = string.Format("<td width=76 style='width:45.7pt;border-top:none;border-left:none;border-bottom:solid black 1.0pt;border-right:solid black 1.0pt;background:#D9D9D9;padding:0in 5.4pt 0in 5.4pt'><p class=MsoNormal><b><a href=\"{0}\">LOG</a><o:p></o:p></b></p></td></tr><tr>",
                    PublishLogShare);
                completeMailBody = completeMailBody + logLocation;
            }
            return completeMailBody;
        }



        /// <summary>
        /// Generates the results summary.
        /// </summary>
        /// <returns></returns>
        public List<string> GenerateResultsSummary()
        {
            List<string> r = new List<string>();
            r.Add("WpdtVersion = " + WpdtVersion);
            r.Add("YdrVersion = " + YdrVersion);
            r.Add("PublishShare = " + (String.IsNullOrEmpty(PublishShare) ? "" : PublishShare));
            r.Add("PublishLogShare = " + (String.IsNullOrEmpty(PublishLogShare) ? "" : PublishLogShare));
            r.Add("PublishSourceShare = " + (String.IsNullOrEmpty(PublishSourceShare) ? "" : PublishSourceShare));
            r.Add(EnlistmentSync.ToString());
            r.Add(UninstallWdpt.ToString());
            r.Add(InstallWdpt.ToString());
            r.Add(ModifyWdpt.ToString());
            for (Int32 i = 0; i < BuildTargets.Length; i++)
            {
                r.Add(BuildTargets[i].ToString());
            }
            return r;
        }

        #region SaveConfiguration/LoadConfiguration 2 Overloads)



        /// <summary>
        /// Save the program configuration to disk
        /// </summary>
        /// <param name="config">Configuration settings</param>
        /// <param name="directoryForSave">Directory where to save the file</param>
        /// <param name="fileName">File name to use for the saving of the file</param>
        public static void SaveConfiguration(
           DailyBuildFullResults config,
           string directoryForSave,
           string fileName)
        {
            string fullPath = Path.Combine(directoryForSave, fileName);

            WriteConfiguration(config, fullPath);
        }

        /// <summary>
        /// Write to config file
        /// </summary>
        /// <param name="config">BDC config settings</param>
        /// <param name="fileName">Config file</param>
        private static void WriteConfiguration(DailyBuildFullResults config, string fileName)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(DailyBuildFullResults));
            // To write to a file, create a StreamWriter object.
            StreamWriter myWriter = new StreamWriter(fileName);
            mySerializer.Serialize(myWriter, config);
            myWriter.Close();
        }

        /// <summary>
        /// Save the config
        /// </summary>
        /// <param name="config">BDC config settings</param>
        /// <param name="fileName">Config filename</param>
        public static void SaveConfiguration(DailyBuildFullResults config, string fileName)
        {
            WriteConfiguration(config, fileName);
        }


        /// <summary>
        /// Load the measurement configuration with a supplied filename. This will
        /// normally be called as part of the command line options where the 
        /// file name is supplied in the command arguments      
        /// </summary>
        /// <param name="fileName">Config file name</param>
        /// <param name="showExceptionDialog">True if we want to show the user an exception dialog
        /// If false, the exception will not be caught.</param>
        /// <returns>BDC Settings</returns>
        public static DailyBuildFullResults LoadConfiguration(string fileName, bool showExceptionDialog)
        {
            DailyBuildFullResults ret = null;
            try
            {
                XmlSerializer mySerializer = new XmlSerializer(typeof(DailyBuildFullResults));
                // To read the file, create a FileStream.
                FileStream myFileStream = new FileStream(fileName, FileMode.Open);
                // Call the Deserialize method and cast to the object type.
                ret = (DailyBuildFullResults)mySerializer.Deserialize(myFileStream);
                myFileStream.Close();
            }
            catch (FileNotFoundException)
            {
                if (showExceptionDialog)
                {

                }
                else
                {
                    throw;
                }
            }
            return ret;
        }

        #endregion
    }
}
