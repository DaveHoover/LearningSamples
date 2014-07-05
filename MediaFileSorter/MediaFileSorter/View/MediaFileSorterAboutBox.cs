using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Text;

namespace MediaFileSorter.View
{
    partial class MediaFileSorterAboutBox : Form
    {
        public MediaFileSorterAboutBox()
        {
            InitializeComponent();
            this.Text = String.Format("About {0}", AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;
            this.labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
            this.labelCopyright.Text = AssemblyCopyright;
            this.labelCompanyName.Text = AssemblyCompany;
            this.textBoxDescription.Text = AssemblyDescription;
            StringBuilder b = new StringBuilder();
            b.Append("Program Usage: \r\n");
            b.Append("This program is a simple media file manager. \r\n");
            b.Append("It is designed to allow modification of a input set of files \r\n");
            b.Append("and modify the file names to allow better numerical sorting or \r\n");
            b.Append("changing the names of files that are created by a digital camera. \r\n");
            b.Append("\r\n");
            b.Append("Common usage: \r\n");
            b.Append("  1) Select the input folder where the media files are located \r\n");
            b.Append("  2) Select a target folder where the media files will be changed/copied \r\n");
            b.Append("     Note: The Target folder can be the same as the source foler. \r\n");
            b.Append("           I would recommend that you make a copy of the source files \r\n");
            b.Append("           before starting so you will not lose your originals in case\r\n");
            b.Append("           or problems\r\n");
            b.Append(" 3) Select either recursive or just the directory you are pointed to \r\n");
            b.Append(" 4) If you want to reload the list of files for the source/ target folders \r\n");
            b.Append("    press the Rescan button. \r\n");
            b.Append(" 5) If you select items in the source list, to clear the selection \r\n");
            b.Append("    press the Clear Selection button\r\n");
            b.Append(" 6) You can operate on a subset of the items for Search/Replace and\r\n");
            b.Append("    create a numerical sorted list from Raw images. To do this, \r\n");
            b.Append("    Select a set of file entries in the source list box and then \r\n");
            b.Append("    click the Update Names button in the proper control area\r\n");
            b.Append(" \r\n");
            b.Append(" Fix Numerical sorting problems \r\n");
            b.Append(" 1) Press the Update Names button in the top part of the Window. \r\n");
            b.Append("    The modified files will be shown in the right pane\r\n");
            b.Append(" \r\n");
            b.Append(" Create a Numerical Sorted List from a set of Raw Camera images\r\n");
            b.Append(" 1) Enter the prefix for the file names\r\n");
            b.Append(" 2) Enter the Main string for the file names\r\n");
            b.Append(" 3) If you want all items changed, make sure no items are selected \r\n");
            b.Append("    in the source list box. You can select a set of items and only\r\n");
            b.Append("    change the selected items.\r\n");
            b.Append("    Example: Files are named imageDatexxx.jpg (where xxx is a number) \r\n");
            b.Append("             If Prefix is Begin and  main is  MainPart, the files will \r\n");
            b.Append("             be named BeginxxxMainPart.jpg \r\n");
            b.Append("\r\n");
            b.Append("Search/Replace to modify File names\r\n");
            b.Append("1) Select the set of items to change, or all if non are selected \r\n");
            b.Append("2) Select the Source string and target string to replace in the file names\r\n");
            b.Append("3) Press Update names in the Search/Replace control section \r\n");
            b.Append("\r\n");
            this.textBoxDescription.Text = b.ToString();

        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion
    }
}
