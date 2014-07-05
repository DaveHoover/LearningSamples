using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;


namespace BuildManager.Model
{
    public class TestInformation
    {
        public const Int32 NumberOfSupportedPages = 16;
        public const Int32 NumberOfTestsPerPage = 8;
        public string AppDescription { get; set; }
        public TestPage [] Pages { get; set; }

        public TestInformation()
        {
            this.AppDescription = "L2S Common Scenarios";
            this.Pages = new TestPage[NumberOfSupportedPages];
            for (Int32 i = 0; i < NumberOfSupportedPages; i++)
            {
                this.Pages[i] = new TestPage("Test Page " + (i + 1).ToString());
            }
        }

        public static void Serialize(TestInformation t , string fileName)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(TestInformation));
            // To write to a file, create a StreamWriter object.
            StreamWriter myWriter = new StreamWriter(fileName);
            mySerializer.Serialize(myWriter, t);
            myWriter.Close();
        }

        public static TestInformation DeSerialize(string fileName)
        {

            TestInformation ret = null;

            try
            {
                XmlSerializer mySerializer = new XmlSerializer(typeof(TestInformation));
                // To read the file, create a FileStream.
                FileStream myFileStream = new FileStream(fileName, FileMode.Open);
                // Call the Deserialize method and cast to the object type.
                ret = (TestInformation)mySerializer.Deserialize(myFileStream);
                myFileStream.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception at LoadFromString: " + e.ToString());
            }
            return ret;

        }

    }
}
