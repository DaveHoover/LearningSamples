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

namespace BuildManager.Helpers
{

   using System;
   using System.Collections.Generic;
   using System.Linq;
   using System.Text;
   using System.Net;
   using System.Net.Mail;
   using System.Net.Mime;
   using System.Threading;
   using System.ComponentModel;
   using System.Globalization;
   using System.Diagnostics;

   /// <summary>
   /// This class will send email addresses inside the corpnet environment.
   /// We must have a valid account to send the mail from for authentication.
   /// 
   /// </summary>
   public sealed class EmailOps
   {

      /// <summary>
      /// Address to send mail to corporate machines.
      /// </summary>
      public const string SmtpServer = "smtphost.redmond.corp.microsoft.com";

      /// <summary>
      /// Lock out instance constructor
      /// </summary>
      private EmailOps()
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
            string senderEmailAddress ,
            string toEmailAddress ,
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
      /// <param name="senderEmailAddress">Account the mail will be sent from</param>
      /// <param name="jobId">Job ID</param>
      /// <param name="jobName">Job Name</param>
      /// <param name="workItem">WorkItem containing the information for sending email </param>
      /// <param name="wiStatus">Status of the WI, if the WI or singe file op timed out</param>
         public static void SendMailtoExchange( 
             string senderEmailAddress ,
             string receiverEmailAddress ,
             string subject ,
             List<string> body)
         {
               MailMessage message;
               SmtpClient client;
               try
               {
                  client = new SmtpClient(SmtpServer);
                  message = new MailMessage();
                  client.UseDefaultCredentials = true;
                  MailAddress from = new MailAddress(senderEmailAddress);
                  message.From = from;
                  message.To.Add(new MailAddress(receiverEmailAddress));
               }
               catch (Exception e)
               {
                   Debug.WriteLine("Failed to create email message. Error was " + e.Message);
                  return;
               }
               message.Subject = subject;
               try
               {
                   foreach (string s in body)
                   {
                       message.Body += s + "\n";
                   }
                     client.Send(message);
                 Debug.WriteLine("Sent WI email with subject: " + message.Subject);
               }
               catch (SmtpFailedRecipientException e)
               {
                   Debug.WriteLine("Mail Error. Exception was " + e.Message);
               }
               catch (ArgumentException e)
               {
                   Debug.WriteLine("Mail Error. Exception was " + e.Message);
               }
               catch (InvalidOperationException e)
               {
                   Debug.WriteLine("Mail Error. Exception was " + e.Message);
               }
               catch (SmtpException e)
               {
                   Debug.WriteLine("Mail Error. Exception was " + e.Message);
               }
         }
      }
   }

