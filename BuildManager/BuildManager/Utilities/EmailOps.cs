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
    using System.Net.Mail;

    /// <summary>
    /// This class will send email addresses inside the corpnet environment.
    /// We must have a valid account to send the mail from for authentication.
    /// 
    /// </summary>
    public sealed class EmailOpsOrig
    {

        /// <summary>
        /// Address to send mail to corporate machines.
        /// </summary>
        public const string SmtpServer = "smtphost.redmond.corp.microsoft.com";

        /// <summary>
        /// Lock out instance constructor
        /// </summary>
        private EmailOpsOrig()
        {
        }

        /// <summary>
        /// Test function to send a mail message in corpnet.
        /// </summary>
        /// <param name="senderEmailAddress">Sender's email address</param>
        /// <param name="toEmailAddress">Recipient's email address</param>
        /// <param name="subject">Email Subject</param>
        /// <param name="body">Email Body</param>
        public static void SendMailtoExchangeTest(
           string senderEmailAddress,
           string toEmailAddress,
           string subject,
           string body
           )
        {
            SmtpClient client = new SmtpClient(SmtpServer);
            client.UseDefaultCredentials = true;
            MailMessage message = new MailMessage(senderEmailAddress, toEmailAddress);
            if (body == null)
            {
                message.Body = "This is a test e-mail message sent by an application. ";
            }
            else
            {
                message.Body = body;
            }
            message.Subject = subject;
            client.Send(message);
        }


        /// <summary>
        /// This function will send email messages for either a success or failed work item.
        /// The checking will be done whether we should send the mail messages, and mail
        /// will be sent for either success or fail
        /// </summary>
        public static void SendMailtoExchange(
            string senderEmailAddress,
            BuildControlParameters configuration,
            DailyBuildFullResults results)
        {

            if (configuration.EmailResults)
            {
                MailMessage message;
                SmtpClient client;
                String subject;
                try
                {
                    client = new SmtpClient(SmtpServer);
                    message = new MailMessage();
                    message.IsBodyHtml = true;
                    client.UseDefaultCredentials = true;
                    MailAddress from = new MailAddress(senderEmailAddress + @"@microsoft.com");
                    message.From = from;
                    subject = "Hero Apps Daily Build Status for: " + DateTime.Now.ToString("MM/dd/yyyy");
                    message.To.Add(new MailAddress(configuration.EmailAlias + @"@microsoft.com"));
                    message.Subject = subject;
                }
                catch (Exception e)
                {
                    ProgramExecutionLog.AddEntry("Email SMTP, send address or to address failure." + e.Message);
                    return;
                }

                try
                {
                    // No point sending if there are none in the email list 
                    if (message.To.Count > 0)
                    {
                        message.Body += "\n\n";
                        message.Body = results.MakeHeroAppsReport(configuration);
                        client.Send(message);
                        Debug.WriteLine("Sent WI email with subject: " + message.Subject);
                    }
                }
                catch (SmtpFailedRecipientException e)
                {
                    ProgramExecutionLog.AddEntry("Partial email send failure. " + e.Message);
                }
                catch (ArgumentException e)
                {
                    ProgramExecutionLog.AddEntry("Email send failure. " + e.Message);

                }
                catch (InvalidOperationException e)
                {
                    ProgramExecutionLog.AddEntry("Email send failure. " + e.Message);
                }
                catch (SmtpException e)
                {
                    ProgramExecutionLog.AddEntry("Email send failure. " + e.Message);
                }
            }
        }
    }
}

