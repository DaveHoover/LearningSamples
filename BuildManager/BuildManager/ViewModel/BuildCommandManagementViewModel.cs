using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuildManager.Data;
using BuildManager.Model;
using System.Windows;


namespace BuildManager.ViewModel
{
    public class BuildCommandManagementViewModel : DbTableViewModelBase
    {

        public IEnumerable<Command> DbTable { get; set; }

        public Command CurrentItemToEdit { get; set; }


        public BuildCommandManagementViewModel(string tableName)
            : base(tableName)
        {
        }

        internal override void AddItem()
        {
            short maxId = (from c in dc.Commands select c.CommandID).Max();

            Command en = new Command { CommandID = (short)(maxId + 1), CommandLine = "NewCommand" };
            dc.Commands.InsertOnSubmit(en);
            base.AddItem();
        }

        internal override void ModifyItem(string pathValue)
        {
            Command en = (from c in dc.Commands
                          where c.CommandID == CurrentItemToEdit.CommandID
                           select c).First();
            en.CommandLine = pathValue;
            dc.SubmitChanges();
        }

        internal  void ModifyItemCommand(string newValue)
        {
            Command en = (from c in dc.Commands
                          where c.CommandID == CurrentItemToEdit.CommandID
                          select c).First();
            en.Command1 = newValue;
            dc.SubmitChanges();
        }



        internal override void DeleteItem()
        {
            try
            {
                if (this.CurrentItemToEdit != null)
                {
                    dc.Commands.DeleteOnSubmit(CurrentItemToEdit);
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

            DbTable = from c in dc.Commands select c;
        }
    }
}
