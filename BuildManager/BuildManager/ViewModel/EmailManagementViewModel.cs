using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuildManager.Data;
using BuildManager.Model;
using System.Windows;

namespace BuildManager.ViewModel
{
    public class EmailManagementViewModel : DbTableViewModelBase
    {

        public IEnumerable<Email> DbTable { get; set; }

        public Email CurrentItemToEdit { get; set; }

        public EmailManagementViewModel(string tableName)
            : base(tableName)
        {
        }

        internal override void AddItem()
        {
            short maxId = (from c in dc.Emails select c.EmailID).Max();

            Email en = new Email { EmailID = (short)(maxId + 1), EmailAccount = "NewEmail", Primary = true, UserID = 1 };
            dc.Emails.InsertOnSubmit(en);
            base.AddItem();
        }

        internal override void ModifyItem(string pathValue)
        {
            Email en = (from c in dc.Emails
                        where c.EmailID == CurrentItemToEdit.EmailID
                        select c).First();
            en.EmailAccount = pathValue;
            dc.SubmitChanges();
        }

        internal override void DeleteItem()
        {

            try
            {
                if (this.CurrentItemToEdit != null)
                {
                    dc.Emails.DeleteOnSubmit(CurrentItemToEdit);
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

            DbTable = from c in dc.Emails select c;
        }
    }
}
