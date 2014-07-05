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

    using System;

    /// <summary>
    /// This class will define and supply a method to raise a status event from operations
    /// to inform subscribers of the state of the operation. 
    /// Status is completed events out of the total
    /// </summary>
    public sealed class OperationNotify
    {

        /// <summary>
        /// Remove public constructor for static method only class
        /// </summary>
        private OperationNotify()
        {
        }

        /// <summary>
        /// This event will be raised in the context of the thread that created
        /// the instance of this class. Clients can subscribe to this event
        /// to get notified of the completion of the audio command
        /// </summary>
        public static event EventHandler<ProgressNotificationEventArgs> OPStatusEvent;


        /// <summary>
        /// Helper function to raise the event for the progress notification back to an 
        /// interested party
        /// </summary>
        /// <param name="sender">Object calling this routine</param>
        /// <param name="totalEvents">Total number of events to be processed</param>
        /// <param name="completedEvents">How many events have been completed</param>
        public static void StatusUpdate(object sender, Int32 totalEvents, Int32 completedEvents)
        {
            if (OPStatusEvent != null)
            {
                OperationStatus status = new OperationStatus(totalEvents, completedEvents);
                ProgressNotificationEventArgs args = new ProgressNotificationEventArgs(status);
                OPStatusEvent(sender, args);
            }
        }



    }
}
