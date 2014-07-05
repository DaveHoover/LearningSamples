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

namespace BuildManager
{
    public class SingleOperationResults
    {
        public const Int32 OpMaxLength = 15;
        public string OperationName { get; set; }
        public bool RequestedToRun { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public SingleOperationResults()
        {
            this.Success = false;
            this.ErrorMessage = "";
            this.RequestedToRun = true;
        }

        public SingleOperationResults(string operationName)
        {
            this.OperationName = operationName;
            this.Success = false;
            this.ErrorMessage = "";
            this.RequestedToRun = true;
        }

        public SingleOperationResults(
            string operationName,
            bool success,
            string errorMessage)
        {
            this.OperationName = operationName;
            this.Success = success;
            this.ErrorMessage = errorMessage;
            this.RequestedToRun = true;
        }

        public void SetResults(
            bool success,
            string errorMessage)
        {
            this.Success = success;
            this.ErrorMessage = errorMessage;
        }


        public void Reset()
        {
            this.Success = false;
            this.RequestedToRun = true;
            this.ErrorMessage = "";
        }

        public void StatusToNotRun()
        {
            this.Success = true;
            this.RequestedToRun = false;
            this.ErrorMessage = "";
        }

        public override string ToString()
        {
            if (RequestedToRun)
            {
                StringBuilder b = new StringBuilder();
                if (OperationName.Length < OpMaxLength)
                {
                    for (Int32 i = OperationName.Length; i < OpMaxLength; i++)
                    {
                        b.Append(" ");
                    }
                }
                string r = OperationName + ":" + b.ToString() +
                    " Status = " + Success.ToString()
                    + (String.IsNullOrEmpty(ErrorMessage) ? "" :
                      " Error message " + ErrorMessage);
                return r;
            }
            else
            {
                return "";
            }
        }
    }
}
