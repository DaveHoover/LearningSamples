using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Controls;
using System.Collections;
using BuildManager.Model;
using BuildManager.Data;

namespace BuildManager.Helpers
{
    class WpfDataGridSort : IComparer   
    {
        public WpfDataGridSort(ListSortDirection direction, DataGridColumn column)     
        {           
            Direction = direction; 
            Column = column;      
        }         
        public ListSortDirection Direction  {  get;  private set;  } 

        public DataGridColumn Column        {  get;  private set;  }

        int StringCompare(string s1, string s2)
        {            
            if (Direction == ListSortDirection.Ascending) 
            {
            return s1.CompareTo(s2); 
            }
            return s2.CompareTo(s1); 
        }      
  
        int IComparer.Compare(object X, object Y) 
        {
            string str1 = string.Empty;
            string str2 = string.Empty;
            short int1;
            short int2;
            switch ((string)Column.Header) 
            {
                case "ConfigID":
                    int1 = ((Configuration)(X)).ID;
                    int2 = ((Configuration)(Y)).ID;
                    if (Direction == ListSortDirection.Ascending)
                    {
                        return int1.CompareTo(int2);
                    }
                    else
                    {
                        return int2.CompareTo(int1);
                    }
                        
                        
                case "UserID":
                    int1 = (short)((Configuration)(X)).UserID;
                    int2 = (short)((Configuration)(Y)).UserID;
                    if (Direction == ListSortDirection.Ascending)
                    {
                        return int1.CompareTo(int2);
                    }
                    else
                    {
                        return int2.CompareTo(int1);
                    }

                    //str1 = ((Employee)X).Name;
                    //str2 = ((Employee)Y).Name; 
                    //return StringCompare(str1, str2);
                // ... do same for other columns 
            }           
            return 0;       
        }    
    }  
}
