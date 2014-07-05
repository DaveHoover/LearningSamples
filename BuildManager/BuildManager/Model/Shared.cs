using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuildManager.Data;

namespace BuildManager.Model
{
    public class Shared
    {

        public static EnlistmentDC EnlistmentDataContext {get; private set;}

        static Shared()
        {

            EnlistmentDataContext = new EnlistmentDC(@"Data\Enlistment.sdf");
        }


        public static void ResetDataContext()
        {
            EnlistmentDataContext.Dispose();
            EnlistmentDataContext = null;
            EnlistmentDataContext = new EnlistmentDC(@"Data\Enlistment.sdf");
        }

    }
}
