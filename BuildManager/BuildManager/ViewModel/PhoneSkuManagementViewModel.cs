using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuildManager.Data;
using BuildManager.Model;
using System.Windows;


namespace BuildManager.ViewModel
{
    public class PhoneSkuManagementViewModel : DbTableViewModelBase
    {
        public IEnumerable<PhoneSKU> DbTable { get; set; }

        public PhoneSKU CurrentItemToEdit { get; set; }

        public PhoneSkuManagementViewModel(string tableName)
            : base(tableName)
        {

        }

        internal override void AddItem()
        {
            short maxId = (from c in dc.PhoneSKUs select c.ID).Max();

            PhoneSKU en = new PhoneSKU { ID = (short)(maxId + 1) , Name = "PhoneSku" };
            dc.PhoneSKUs.InsertOnSubmit(en);
            base.AddItem();
        }

        internal override void ModifyItem(string pathValue)
        {
            PhoneSKU en = (from c in dc.PhoneSKUs
                              where c.ID == CurrentItemToEdit.ID
                              select c).First();
            en.Name = pathValue;
            dc.SubmitChanges();
        }

        internal override void DeleteItem()
        {
            try
            {
                if (this.CurrentItemToEdit != null)
                {
                    dc.PhoneSKUs.DeleteOnSubmit(CurrentItemToEdit);
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

            DbTable = from c in dc.PhoneSKUs select c;
        }
    }
}
