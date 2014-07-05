using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuildManager.Data;
using BuildManager.Helpers;
using BuildManager.Model;

namespace BuildManager.ViewModel
{
    public class DbTableViewModelBase
    {


        protected EnlistmentDC dc = Shared.EnlistmentDataContext;

        public string TableName { get; set; }


        public DbTableViewModelBase(string tableName)
        {

            this.TableName = tableName;

        }

        internal virtual void AddItem()
        {
            dc.SubmitChanges();
            this.QueryAllDbTable();
        }

        internal virtual void ModifyItem(string pathValue)
        {
        }

        internal virtual void DeleteItem()
        {

        }

        internal virtual void QueryAllDbTable()
        {

        }

    }
}
