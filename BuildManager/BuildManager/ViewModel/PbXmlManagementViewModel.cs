using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuildManager.Data;
using BuildManager.Model;
using System.Windows;


namespace BuildManager.ViewModel
{
    public class PbXmlManagementViewModel : DbTableViewModelBase
    {
        public IEnumerable<BuildXml> DbTable { get; set; }

        public BuildXml CurrentItemToEdit { get; set; }

        public PbXmlManagementViewModel(string tableName)
            : base(tableName)
        {

        }

        internal override void AddItem()
        {
            short maxId = (from c in dc.BuildXmls select c.FileID).Max();

            BuildXml en = new BuildXml { FileID = (short)(maxId + 1), FilePath = @"C:\MyPbXml.xml" };
            dc.BuildXmls.InsertOnSubmit(en);
            base.AddItem();
        }

        internal override void ModifyItem(string pathValue)
        {
            BuildXml en = (from c in dc.BuildXmls
                           where c.FileID == CurrentItemToEdit.FileID
                           select c).First();
            en.FilePath = pathValue;
            dc.SubmitChanges();
        }

        internal override void DeleteItem()
        {

            try
            {
                if (this.CurrentItemToEdit != null)
                {
                    dc.BuildXmls.DeleteOnSubmit(CurrentItemToEdit);
                    dc.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }


        }

        internal override void QueryAllDbTable()
        {

            DbTable = from c in dc.BuildXmls select c;
        }
    }
}

