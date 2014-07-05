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

namespace BuildManager.Model
{
    public class TestPage
    {
        public string PageTitle { get; set; }
        public TestDescription[] Tests { get; set; }

        public TestPage()
        {
            this.InitializeTestInformation(string.Empty);
        }

        public TestPage(string pageTitle)
        {
            this.PageTitle = pageTitle;
            this.InitializeTestInformation(pageTitle);

        }

        private void InitializeTestInformation(string pageTitle)
        {
            this.Tests = new TestDescription[TestInformation.NumberOfTestsPerPage];
            for (Int32 i = 0; i < TestInformation.NumberOfTestsPerPage; i++)
            {
                this.Tests[i] = new TestDescription("Test " + (i + 1).ToString());
            }
        }
    }

}
