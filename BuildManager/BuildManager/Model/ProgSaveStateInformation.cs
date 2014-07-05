using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace BuildManager.Model
{



    public class ProgSaveStateInformation
    {

        public const string ProgConfigFileName = "BuildManagerConfig.xml";

        public const Int32 NumberOfDefaultQuerySettings = 4;

        public bool         ScheduledStart { get; set; }
        public DateTime     ScheduledStartTime { get; set; }
        public string       SingleEnlistmentPath { get; set; }
        public string       SingleEnlistmentSku { get; set; }
        public bool         BuildTestTree { get; set; }
        public  QueryParameters [] Query { get; set;}
        public short[]      ConfigurationIDs { get; set; }
        public ProcessPriorityClass ProcessPriority { get; set; }



        public static void Serialize(ProgSaveStateInformation t, string fileName)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(ProgSaveStateInformation));
            // To write to a file, create a StreamWriter object.
            StreamWriter myWriter = new StreamWriter(fileName);
            mySerializer.Serialize(myWriter, t);
            myWriter.Close();
        }

        public static ProgSaveStateInformation DeSerialize(string fileName)
        {

            ProgSaveStateInformation ret = null;
            try
            {
                XmlSerializer mySerializer = new XmlSerializer(typeof(ProgSaveStateInformation));
                // To read the file, create a FileStream.
                FileStream myFileStream = new FileStream(fileName, FileMode.Open);
                // Call the Deserialize method and cast to the object type.
                ret = (ProgSaveStateInformation)mySerializer.Deserialize(myFileStream);
                myFileStream.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception loading Config file: " + e.Message);
                ret = CreateDefault();
            }
            return ret;
        }

        public static ProgSaveStateInformation CreateDefault()
        {
            ProgSaveStateInformation p = new ProgSaveStateInformation();
            p.ScheduledStart = false;
            p.ScheduledStartTime = DateTime.Now;
            p.SingleEnlistmentPath = String.Empty;
            p.SingleEnlistmentSku = String.Empty;
            p.Query = new QueryParameters[NumberOfDefaultQuerySettings];
            for (Int32 i = 0; i < NumberOfDefaultQuerySettings; i++)
            {
                p.Query[i] = new QueryParameters();
                p.Query[i].TableSelection = String.Empty;
                p.Query[i].FieldItemSelection = String.Empty;
            }
            return p;
        }
    }
}
