using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Diagnostics;

namespace BuildManager.Helpers
{
    public class FileSys
    {

        public static bool IsValidEnlistmentRoot ( string path)
        {
            try
            {
                if ( Directory.Exists(path))
                {
                    string sdIniFile = Path.Combine(path, "sd.ini");
                    if (File.Exists(sdIniFile))
                    {
                        return true;
                    }
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("Enlistment path was not valid. Path was " + path  +
                    " Error was " + e.Message);
            }
            return false;
        }
    }
}
