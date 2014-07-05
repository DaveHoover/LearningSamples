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
    /// Class to hold the status of the current running operation
    /// </summary>
    public class OperationStatus
    {
        /// <summary>
        /// The total number of events we will process, if we know it
        /// </summary>
        public Int32 TotalNumberOfEvents { get; set; }

        /// <summary>
        /// The number of events that have been completed
        /// </summary>
        public Int32 CompletedEvents { get; set; }

        /// <summary>
        /// Initialize the object
        /// </summary>
        /// <param name="totalNumberOfEvents">Total number of events that we have to process</param>
        /// <param name="completedEvents">How many events have been completed</param>
        public OperationStatus(Int32 totalNumberOfEvents, Int32 completedEvents)
        {
            this.TotalNumberOfEvents = totalNumberOfEvents;
            this.CompletedEvents = completedEvents;
        }
    }
}