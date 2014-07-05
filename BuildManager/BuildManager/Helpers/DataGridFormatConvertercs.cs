using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace BuildManager.Helpers
{

    public class DataGridFormatConverters : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type of data expected by 
        /// the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            System.Globalization.CultureInfo culture)
        {
            string separator = value as string;
            Thickness margin;
            if (separator == null)
            {

                // margin offest for all other '.' number format decimal separator
                margin = new Thickness(47, -39, 0, 0);
                return margin;
            }

            // these margins are for a font size of 80 with the default font

            if (separator.CompareTo(",") == 0)
            {
                // margin offest for the french ',' number format decimal separator
                margin = new Thickness(47, -40, 0, 0);
            }
            else
            {
                // margin offest for all other '.' number format decimal separator
                margin = new Thickness(47, -39, 0, 0);
            }

            return margin;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  
        /// This method is called only in 
        /// <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The type of data expected by 
        /// the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
