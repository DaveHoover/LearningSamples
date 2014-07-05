using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuildManager.Data;
using System.Diagnostics;

namespace BuildManager.Model
{
    public class JobSetInformation
    {

        public const int LogSize = 300;

        public string LogFileName { get; set; }

        public JobResult[] Results { get; set; }

        public Configuration[] Jobs { get; set; }

        public Int32 Index { get; set; }

        public bool IsSuccessful { get; set; }

        public bool IsBuild { get; set; }

        public bool IsRunning { get; set; }

        public Process CommandProcess { get; set; }

        public string ErrorInformation { get; set; }

    }
}
