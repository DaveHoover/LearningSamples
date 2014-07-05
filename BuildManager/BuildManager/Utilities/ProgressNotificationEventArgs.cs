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

    /// <summary>
    /// Power Monitor Marshal event argument class
    /// </summary
    public class ProgressNotificationEventArgs : EventArgs
    {

        /// <summary>
        /// Message to be marshalling back to the UI
        /// </summary>
        public OperationStatus Status { get; set; }

        /// <summary>
        /// Constructor to initialize the status of the operation.
        /// </summary>
        /// <param name="status">Status information to pass to the requested party</param>
        public ProgressNotificationEventArgs(OperationStatus status)
        {
            this.Status = status;
        }
    }
}

