using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuildManager.Data;
using BuildManager.Helpers;
using BuildManager.Model;
using System.Diagnostics;

namespace BuildManager.ViewModel
{
    public class EnlistmentManagementWindowViewModel : DbTableViewModelBase
    {
        public IEnumerable<Enlistment1> DbTable { get; set; }

        public Enlistment1 CurrentItemToEdit { get; set; }



        public EnlistmentManagementWindowViewModel(string tableName)
            :base(tableName)
        {
        }

        internal override void AddItem()
        {
            short maxId  = (from c in dc.Enlistments select c.Id).Max();

            Enlistment1 en = new Enlistment1 { Id = (short)(maxId + 1), Path = "NewPath" };
            dc.Enlistments.InsertOnSubmit(en);
            base.AddItem();
        }

        internal override void ModifyItem(string pathValue)
        {
            Enlistment1 en = (from c in dc.Enlistments
                             where c.Id == CurrentItemToEdit.Id
                             select c).First();
            en.Path = pathValue;
            dc.SubmitChanges();
        }

        internal override void DeleteItem()
        {
            if (this.CurrentItemToEdit != null)
            {
                dc.Enlistments.DeleteOnSubmit(CurrentItemToEdit);
                dc.SubmitChanges();
            }
        }

        internal override void QueryAllDbTable()
        {

            DbTable = from c in dc.Enlistments select c;

        }

    }
}
