using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace LinkListTest
{
	/// <summary>
	/// This class is a small test class and implementation for the Microsoft
	/// questionnaire 
	/// The assignment was  as follows
	/// Write a function that would:    return the 5th   element from the end in a 
	/// singly linked list of integers, in one pass, and then provide a set of test 
	/// cases against that function.
	/// 
	/// The function written to support this is RetrieveData.
	/// A 2nd implementation is supported that takes advantage of some of the more
	/// powerful .NET capabilities. This function is RetrieveDataNet.
	///  The capabilities in RetrieveDataNet would not be available without custom coding
	/// in a non .NET language.
    /// Test for GitHub
	/// 
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
      private System.Windows.Forms.Button btnShortList;
      private System.Windows.Forms.Button btnLongList;

      private const Int32 SHORT_LIST_SIZE       = 4;
      private const Int32 LONG_LIST_SIZE        = 17;

      private ListDictionary      m_List         = new ListDictionary();
      private System.Windows.Forms.TextBox textBox1;
      private System.Windows.Forms.TextBox tbRetVal;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label tbIndexSel;
      private System.Windows.Forms.TextBox tbIndexToRet;
      private System.Windows.Forms.Label lblError;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.TextBox tbRetNet;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
         this.btnShortList = new System.Windows.Forms.Button();
         this.btnLongList = new System.Windows.Forms.Button();
         this.textBox1 = new System.Windows.Forms.TextBox();
         this.tbRetVal = new System.Windows.Forms.TextBox();
         this.label1 = new System.Windows.Forms.Label();
         this.tbIndexSel = new System.Windows.Forms.Label();
         this.tbIndexToRet = new System.Windows.Forms.TextBox();
         this.lblError = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.tbRetNet = new System.Windows.Forms.TextBox();
         this.SuspendLayout();
         // 
         // btnShortList
         // 
         this.btnShortList.Location = new System.Drawing.Point(32, 24);
         this.btnShortList.Name = "btnShortList";
         this.btnShortList.TabIndex = 0;
         this.btnShortList.Text = "&Short List";
         this.btnShortList.Click += new System.EventHandler(this.btnShortList_Click);
         // 
         // btnLongList
         // 
         this.btnLongList.Location = new System.Drawing.Point(32, 64);
         this.btnLongList.Name = "btnLongList";
         this.btnLongList.TabIndex = 1;
         this.btnLongList.Text = "&Long List";
         this.btnLongList.Click += new System.EventHandler(this.btnLongList_Click);
         // 
         // textBox1
         // 
         this.textBox1.Location = new System.Drawing.Point(304, 24);
         this.textBox1.Multiline = true;
         this.textBox1.Name = "textBox1";
         this.textBox1.Size = new System.Drawing.Size(136, 360);
         this.textBox1.TabIndex = 2;
         this.textBox1.Text = "textBox1";
         // 
         // tbRetVal
         // 
         this.tbRetVal.Location = new System.Drawing.Point(192, 144);
         this.tbRetVal.Name = "tbRetVal";
         this.tbRetVal.TabIndex = 3;
         this.tbRetVal.Text = "";
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(16, 144);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(152, 23);
         this.label1.TabIndex = 4;
         this.label1.Text = "Returned Value";
         // 
         // tbIndexSel
         // 
         this.tbIndexSel.Location = new System.Drawing.Point(32, 96);
         this.tbIndexSel.Name = "tbIndexSel";
         this.tbIndexSel.Size = new System.Drawing.Size(144, 40);
         this.tbIndexSel.TabIndex = 5;
         this.tbIndexSel.Text = "&Index From End To Return. 1 selects the last element in the list";
         // 
         // tbIndexToRet
         // 
         this.tbIndexToRet.Location = new System.Drawing.Point(192, 104);
         this.tbIndexToRet.Name = "tbIndexToRet";
         this.tbIndexToRet.TabIndex = 6;
         this.tbIndexToRet.Text = "";
         // 
         // lblError
         // 
         this.lblError.ForeColor = System.Drawing.Color.Brown;
         this.lblError.Location = new System.Drawing.Point(16, 256);
         this.lblError.Name = "lblError";
         this.lblError.Size = new System.Drawing.Size(272, 128);
         this.lblError.TabIndex = 7;
         this.lblError.Visible = false;
         // 
         // label2
         // 
         this.label2.Location = new System.Drawing.Point(16, 168);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(152, 23);
         this.label2.TabIndex = 9;
         this.label2.Text = "Returned Value .NET tools";
         // 
         // tbRetNet
         // 
         this.tbRetNet.Location = new System.Drawing.Point(192, 168);
         this.tbRetNet.Name = "tbRetNet";
         this.tbRetNet.TabIndex = 8;
         this.tbRetNet.Text = "";
         // 
         // Form1
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(488, 406);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.tbRetNet);
         this.Controls.Add(this.lblError);
         this.Controls.Add(this.tbIndexToRet);
         this.Controls.Add(this.tbIndexSel);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.tbRetVal);
         this.Controls.Add(this.textBox1);
         this.Controls.Add(this.btnLongList);
         this.Controls.Add(this.btnShortList);
         this.Name = "Form1";
         this.Text = "Form1";
         this.Load += new System.EventHandler(this.FormLoad);
         this.ResumeLayout(false);

      }
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

      private void FormLoad(object sender, System.EventArgs e)
      {
         this.tbIndexToRet.Text = "5";
      
      }
      #region Test Functions. btnShortList_Click/btnLongList_Click
      /// <summary>
      /// Test case for a list that is shorter than the value that is 
      /// desired to return.
      /// This function will initialize the list, and then call the function
      /// to retrieve the specified data element.
      /// The element from the end of the list is specified in a text box 
      /// control. 
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void btnShortList_Click(object sender, System.EventArgs e)
      {
         TestRetrieve ( SHORT_LIST_SIZE );
      }

      /// <summary>
      /// Test case for a list that is long enough so that we can access the
      /// desired value to return.
      /// This function will initialize the list, and then call the function
      /// to retrieve the specified data element.
      /// The element from the end of the list is specified in a text box 
      /// control. 
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void btnLongList_Click(object sender, System.EventArgs e)
      {
         TestRetrieve ( LONG_LIST_SIZE );
      }
      #endregion
      #region TestRetrieve
      /// <summary>
      /// Test function to initialize a linnked list data structure, and retrieve
      /// an element from the list
      /// </summary>
      /// <param name="ListSize">Size of the linked list</param>
      private void TestRetrieve ( Int32 ListSize )
      {
         Int32 RetData;
         InitList ( ListSize);
         this.lblError.Visible = false;
         this.tbRetNet.Text   = "";
         this.tbRetVal.Text   = "";
         try
         {
            RetData = RetrieveData( Convert.ToInt32(this.tbIndexToRet.Text) );
            RetData = RetrieveDataNet( Convert.ToInt32(this.tbIndexToRet.Text) );
         }
         catch ( Exception e1 )
         {
            this.lblError.Text = e1.ToString();
            this.lblError.Visible = true;
         }
      }
      #endregion
      #region InitList
      /// <summary>
      /// This function will initialize a .NET data structure with a set of 
      /// integer data that we will access as a linked list type of data 
      /// structure.
      /// We will store the square of the array index as the data value for 
      /// the element.
      /// In addition, for test purposes, we will create a string [] which will
      /// allow us to easily display the values entered into the list.
      /// In this way, it is easy to verify that are algorithm is working 
      /// correctly and returning the correct value.
      /// </summary>
      /// <param name="Size"></param>
      private void InitList ( Int32 Size )
      {
         // String entry list
         ArrayList List    = new ArrayList();
         string Entry;
         Int32 Value;
         // Clear linked list data structure
         m_List.Clear();
         // initialize the list for the number of elements specified.
         for ( Int32 i = 0; i < Size; i++ )
         {
            Value = i * i;
            m_List.Add ( i , Value );
            Entry = "Index " + i.ToString() + " Value = " + 
                     Value.ToString();
            List.Add ( Entry);

         }
         string [] Str = ( string [] ) List.ToArray( typeof (string));
         this.textBox1.Lines = Str;
         this.textBox1.Update();
      }
      #endregion
      #region RetrieveData
      /// <summary>
      /// This function will retrieve an element from a linked list data structure
      /// The requirements were to search the list in one pass, and retrieve an 
      /// element a certain number of entries from the end of the list.
      /// I chose to use a .NET queue data structure to store the elements so
      /// that I would have easy access to the required data element at the end
      /// of the list.
      /// Cases that could occur.
      /// 1) The size of the linked list is too small to find the correct entry
      /// 2) The list is large enough and we should retrieve the element.
      /// Note: ****
      /// I assumed here that we should search through the list without using
      /// some of the nicer .NET features. I could have used the .Count property
      /// of the ListDictionary, which would have made it much easier to access the
      /// correct element.
      /// I will supply an implementation that uses some of the .NET features
      /// of this data structure as well.
      /// </summary>
      /// <param name="ElementIndexFromEnd">1 based index from the end 
      /// of the list. 1 here would select the last element in the list.</param>
      /// <returns>Data element from the linked list that is the 
      /// ElementIndexFromEnd entry </returns>
      private Int32 RetrieveData ( Int32 ElementIndexFromEnd )
      {
         if ( ElementIndexFromEnd <= 0 )
         {
            throw new Exception 
               ( "Invalid selected index. Index was " + ElementIndexFromEnd.ToString() );
         }
         // Keep track of the size of the list
         Int32 ListSize = 0;
         DictionaryEntry Dict;
         // Data element retrieved from the linked list
         Int32 Data  = 0;
         // Flag to know when we successfully retrieved a data element from the list
         bool Success   ;
         // Data structure to store the last items from the list
         Queue Q  = new Queue();
         // Initialize the linked list enumerator for accessing the list
         IDictionaryEnumerator Enum = m_List.GetEnumerator();
         Enum.Reset();
         Enum.MoveNext();
         // Cycle through all elements of the list
         // List size will be the number of entries in the list at the end
         // of the do loop.
         do
         {
            Dict = (DictionaryEntry)Enum.Current;
            Data = (Int32)Dict.Value;
            Q.Enqueue ( Data );
            /* If we have stored one more than the required number of 
             * elements from the end, remove an element from the queue
             */
            if ( Q.Count == ElementIndexFromEnd + 1 )
            {
               Q.Dequeue();
            }
            Success = Enum.MoveNext();
            ListSize++;
         }
         while ( Success );
         /* Check to see if the linked list was long enough so that we can
          * retrieve the correct element from the list.
          */
         if ( ListSize >= ElementIndexFromEnd )
         {
            // Get correct data item from the .NET queue
            Int32 RetData = (Int32) Q.Dequeue();
            // Push the data to a Windows form text box.
            this.tbRetVal.Text = RetData.ToString();
            return RetData;
         }
         else
         {
            throw new Exception ( "List size was not long enough");
         }
      }
      #endregion
      #region RetrieveDataNet

      /// <summary>
      /// This function will perform the same operation as the RetrieveData function,
      /// but uses the advanced .NET capabilities to access the list data structure
      /// to get the array of values, and then access the desired element directly.
      /// </summary>
      /// <param name="ElementIndexFromEnd">1 based index for element from the end
      /// of the list. A value of 1 gives us the last element</param>
      /// <returns>Data ElementIndexFromEnd from the end of the list, 1 gives the
      /// last element of the list 0 is not a valid selection</returns>
      private Int32 RetrieveDataNet ( Int32 ElementIndexFromEnd )
      {
         if ( ElementIndexFromEnd <= 0 )
         {
            throw new Exception 
               ( "Invalid selected index. Index was " + ElementIndexFromEnd.ToString() );
         }
         ICollection Col   = m_List.Values;
         // Get the size of the linked list
         Int32 Size        = Col.Count;
         // Create an array of integers for the size of the linked list data structure
         Int32 [] Values   = new Int32[Size];
         // Copy the elements from the linked list to the array
         Col.CopyTo ( Values , 0 );
         // Check to see if the linked list was long enough.
         if ( Values.Length < ElementIndexFromEnd )
         {
            throw new Exception ( "List size was not long enough. Caught in .NET retrieve");
         }
         else
         {
            /* Select the index of the correct item. Since length is one longer than 
             * the array, and the ElementIndexFromEnd is 1 based, we see that if we
             * want the last element of the array, ElementIndexfromEnd being one would
             * give us the correct value
             */
            Int32 RetData = Values[Values.Length - ElementIndexFromEnd];
            this.tbRetNet.Text = RetData.ToString();
            return RetData;
         }
      }
      #endregion
	}
}
