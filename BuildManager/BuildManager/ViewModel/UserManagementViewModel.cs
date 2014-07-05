using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuildManager.Data;
using BuildManager.Model;
using System.Windows;


namespace BuildManager.ViewModel
{
    public class UserManagementViewModel : DbTableViewModelBase
    {

        public IEnumerable<User> DbTable { get; set; }

        public User CurrentItemToEdit { get; set; }


        public UserManagementViewModel(string tableName)
            : base(tableName)
        {
        }

        internal override void AddItem()
        {
            short maxId = (from c in dc.Users select c.UserID).Max();

            User en = new User { UserID = (short)(maxId + 1), FirstName = "John", LastName = "Doe" };
            dc.Users.InsertOnSubmit(en);
            base.AddItem();
        }

        internal override void ModifyItem(string pathValue)
        {
            User en = (from c in dc.Users
                       where c.UserID == CurrentItemToEdit.UserID
                       select c).First();
            en.FirstName = pathValue;
            dc.SubmitChanges();
        }

        internal void ModifyItemLastName(string newValue)
        {
            User en = (from c in dc.Users
                       where c.UserID == CurrentItemToEdit.UserID
                       select c).First();
            en.LastName = newValue;
            dc.SubmitChanges();
        }



        internal override void DeleteItem()
        {

            try
            {
                if (this.CurrentItemToEdit != null)
                {
                    dc.Users.DeleteOnSubmit(CurrentItemToEdit);
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

            DbTable = from c in dc.Users select c;
        }
    }
}

