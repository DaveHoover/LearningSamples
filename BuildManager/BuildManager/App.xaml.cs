using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.IO;


namespace BuildManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static string StartupPath
        {
            get
            {
                string fileName = Process.GetCurrentProcess().MainModule.FileName;
                return Path.GetDirectoryName(fileName);
            }
        }
    }
}
