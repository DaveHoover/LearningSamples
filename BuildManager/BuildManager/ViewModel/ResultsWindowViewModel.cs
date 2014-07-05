using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuildManager.Model;
using BuildManager.Data;
using System.ComponentModel;

namespace BuildManager.ViewModel
{
    public class ResultsWindowViewModel : INotifyPropertyChanged
    {

        public EnlistmentDC dc = Shared.EnlistmentDataContext;

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Standard pattern for data binding and notifications.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        /// Notify subscribers of a change in the property
        /// </summary>
        /// <param name="propertyName">Name of the property to signal there has been a changed</param>
        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
                this.PropertyChanged(this, args);
            }
        }



    }
}
