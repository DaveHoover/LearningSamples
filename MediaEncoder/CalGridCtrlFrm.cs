using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using C1.Win.C1FlexGrid;

namespace MediaEncoder
{



	/// <summary>
	/// Summary description for CalGridCtrlFrm.
	/// </summary>
	public class CalGridCtrlFrm : System.Windows.Forms.UserControl
	{
      public delegate void GridBeforeEditDelegate ( object sender , RowColEventArgs e );
      public event         GridBeforeEditDelegate OnBeforeEditEvent;
      public event         EventHandler           OnEnterCellEvent;
      public event         GridBeforeEditDelegate OnAfterEditEvent;

      public  C1.Win.C1FlexGrid.C1FlexGrid   fg;
      private GridLogSetupStruct             m_FgCfg  = new GridLogSetupStruct();
      private bool                           m_GridUpdateInProgress = false;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

      /// <summary>
      /// This class is class that encapsulates FlexGrid behavior to allow
      /// this to be used for severa purposes in the Audio test program.
      /// Since this is a user control, we can then add this control to
      /// Windows forms objects.
      /// </summary>
		public CalGridCtrlFrm()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
         fg.FocusRect      = C1.Win.C1FlexGrid.FocusRectEnum.Light;
         Column Data = fg.Cols[1];
         Column Name = fg.Cols[0];
         Data.TextAlign = TextAlignEnum.LeftCenter;
         Name.TextAlign = TextAlignEnum.LeftCenter;
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
         this.fg = new C1.Win.C1FlexGrid.C1FlexGrid();
         ((System.ComponentModel.ISupportInitialize)(this.fg)).BeginInit();
         this.SuspendLayout();
         // 
         // fg
         // 
         this.fg.AutoClipboard = true;
         this.fg.ColumnInfo = @"4,0,0,0,0,75,Columns:0{Width:300;Caption:""File Name"";DataType:System.String;TextAlign:LeftCenter;}	1{Width:250;Caption:""Title"";DataType:System.String;TextAlign:LeftCenter;ImageAlign:CenterCenter;}	2{Width:250;Caption:""Description"";DataType:System.String;TextAlign:LeftCenter;}	3{Width:75;Caption:""Author"";DataType:System.String;TextAlign:LeftCenter;}	";
         this.fg.Dock = System.Windows.Forms.DockStyle.Fill;
         this.fg.ExtendLastCol = true;
         this.fg.Location = new System.Drawing.Point(0, 0);
         this.fg.Name = "fg";
         this.fg.Rows.Count = 30;
         this.fg.Rows.DefaultSize = 17;
         this.fg.Size = new System.Drawing.Size(900, 208);
         this.fg.StyleInfo = @"Normal{Font:Microsoft Sans Serif, 8.25pt;}	Alternate{}	Fixed{BackColor:Control;ForeColor:ControlText;Border:Flat,1,ControlDark,Both;}	Highlight{BackColor:Highlight;ForeColor:HighlightText;}	Focus{BackColor:235, 255, 255;}	Editor{}	Search{BackColor:Highlight;ForeColor:HighlightText;}	Frozen{BackColor:Beige;}	NewRow{}	EmptyArea{BackColor:AppWorkspace;Border:Flat,1,ControlDarkDark,Both;}	GrandTotal{BackColor:Black;ForeColor:White;}	Subtotal0{BackColor:ControlDarkDark;ForeColor:White;}	Subtotal1{BackColor:ControlDarkDark;ForeColor:White;}	Subtotal2{BackColor:ControlDarkDark;ForeColor:White;}	Subtotal3{BackColor:ControlDarkDark;ForeColor:White;}	Subtotal4{BackColor:ControlDarkDark;ForeColor:White;}	Subtotal5{BackColor:ControlDarkDark;ForeColor:White;}	";
         this.fg.TabIndex = 0;
         this.fg.AfterEdit += new C1.Win.C1FlexGrid.RowColEventHandler(this.AfterEditEvent);
         this.fg.Leave += new System.EventHandler(this.FocusLeaveEvent);
         this.fg.EnterCell += new System.EventHandler(this.EnterCellEvent);
         this.fg.Enter += new System.EventHandler(this.FocusEnterEvent);
         this.fg.StartEdit += new C1.Win.C1FlexGrid.RowColEventHandler(this.StartEditEvent);
         this.fg.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDownEvent);
         // 
         // CalGridCtrlFrm
         // 
         this.Controls.Add(this.fg);
         this.Name = "CalGridCtrlFrm";
         this.Size = new System.Drawing.Size(900, 208);
         ((System.ComponentModel.ISupportInitialize)(this.fg)).EndInit();
         this.ResumeLayout(false);

      }
		#endregion

      #region FlexGrid Event Handlers Focus, In/Out, Key Down, Edit...

      /// <summary>
      /// This handler will be triggered when focus enter the control. 
      /// This allows us to give a visual queue that this control has 
      /// focus
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void FocusEnterEvent(object sender, System.EventArgs e)
      {
         C1FlexGrid fg = (C1FlexGrid) sender;
         fg.FocusRect = C1.Win.C1FlexGrid.FocusRectEnum.Heavy;
      
      }

      /// <summary>
      /// This function is triggered when control leaves the control.
      /// This allows us to restore data types for the columns as well
      /// as to restor any visual queues.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void FocusLeaveEvent(object sender, System.EventArgs e)
      {
         C1FlexGrid fg = (C1FlexGrid) sender;
         fg.FocusRect = C1.Win.C1FlexGrid.FocusRectEnum.Light;
         // Restore to normal default data type for data element column
         fg.Cols[1].DataType  = typeof(System.String);
      
      }

      /// <summary>
      /// This was a test event handler that could be used to capture certain 
      /// keystrokes. In this case, we are trapping a Ctrl C to be used to 
      /// copy the contents of the control to the clipboard
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void KeyDownEvent(object sender, System.Windows.Forms.KeyEventArgs e)
      {
         C1FlexGrid fg = ( C1FlexGrid ) sender;
         if ( e.Control )
         {
//            if ( e.KeyCode == Keys.C )
//            {
//               Clipboard.SetDataObject ( fg.Clip );
//               e.Handled   = true;
//            }
         }
         e.Handled   = false;
      }

      /// <summary>
      /// This function will be triggered when we start editing the cell. An event
      /// is raised in this function which allows a derived object to subscribe
      /// to the event and perform a desired handler before the control enters 
      /// edit mode
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void StartEditEvent(object sender, C1.Win.C1FlexGrid.RowColEventArgs e)
      {
         if ( !m_GridUpdateInProgress )
         {
            if ( OnBeforeEditEvent != null )
            {
               OnBeforeEditEvent ( sender , e );
            }
         }
      }

      /// <summary>
      /// This function will be triggered after an edit event is fired. An event
      /// will be raised which can be subsribed to so that additional processing
      /// can be performed.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void AfterEditEvent(object sender, C1.Win.C1FlexGrid.RowColEventArgs e)
      {
         if ( !m_GridUpdateInProgress )
         {
            if ( OnAfterEditEvent != null )
            {
               OnAfterEditEvent ( sender , e );
            }
         }
      }

      /// <summary>
      /// This function will be triggered when a cell receives focus in the 
      /// flexgrid control Again, we raise an event to allow the parent to
      /// perform a desired processing when the cell is entered
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void EnterCellEvent(object sender, System.EventArgs e)
      {
         if ( !m_GridUpdateInProgress )
         {
            if ( OnEnterCellEvent != null )
            {
               OnEnterCellEvent ( sender , e );
            }
         }
      }
      #endregion

      #region Original Splintercat code usage examples
//      public void CfgDlg ( ScpiLogInfo Dat  )
//      {
//         fg.Rows.Count++;
//         fg.Row   = fg.Rows.Count - 1;
//         DisplayScpiLogInfo ( fg.Rows.Count - 1 , Dat  );
//      }
//
//      public void CfgDlg ( Queue In  )
//      {
//         m_GridUpdateInProgress = true;
//         fg.SuspendLayout();
//         ScpiLogInfo [] Dat = new ScpiLogInfo [ In.Count];
//         In.CopyTo ( Dat , 0 );
//         fg.Rows.Count = Dat.Length + 1;
//         fg.Focus();
//         fg.Col = 1;
//         fg.Cols[1].TextAlign = TextAlignEnum.LeftCenter;
//         for ( Int32 i = 0; i < Dat.Length; i++ )
//         {
//            fg.Cols[1].TextAlign = TextAlignEnum.LeftCenter;
//            fg.Row   = i + 1;
//            DisplayScpiLogInfo ( i + 1 , Dat[i]  );
//         }
//         fg.ResumeLayout();
//         m_GridUpdateInProgress = false;
//      }
//
//      private void DisplayScpiLogInfo ( Int32 RowIndex , ScpiLogInfo Dat  )
//      {
//         ScpiTransactionEventArgs Info = Dat.Info;
//         fg.SetData  ( RowIndex , 0 , Dat.Interface.ToString() );
//         string Cmd;
//         string ErrStr;
//         Dat.CreateReportStrings ( out Cmd , out ErrStr );
//         fg.SetData  ( RowIndex , 1 , Dat.SequenceNumber );
//         fg.SetData  ( RowIndex , 2 , Cmd );
//         if ( ErrStr != null )
//         {
//            fg.SetData  ( RowIndex , 3 , ErrStr);
//         }
//         fg.Row = RowIndex;
//      }

//    #region OrigMeas Interface Functions
//
//      private void CfgDlgLength ( Int32 iLength )
//      {
//         m_FgCfg.iTotalLength = iLength;
//         fg.Rows.Count        = iLength + 1;
//      }
//      private Int32 CfgDlgPartial ( Int32 iStartIndex , MemberInfo[] Info , 
//         object[] od )
//      {
//         return CfgDlgData ( iStartIndex , Info , od );
//      }
//
//
//      private  void CfgDlgLabels ( string sCol0 , string sCol1 )
//      {
//         m_FgCfg.sCol0Label   = sCol0;
//         m_FgCfg.sCol1Label   = sCol1;
//         fg.SetData ( 0 , 0 , sCol0 );
//         fg.SetData ( 0 , 1 , sCol1 );
//      }
//
//      private  void ResultsDlgCfg ( ArrayList sParmNames, ArrayList sParmValues )
//      {
//         Int32 i = 1;
//         for ( Int32 j = 0; j < sParmNames.Count; j++ )
//         {
//            fg.SetData ( i , 0 , (string)sParmNames[j] );
//            fg.SetData ( i , 1 , (string)sParmValues[j]);
//            i++;
//         }
//
//      }
//
//
//      private void CfgDlgSingle ( ParmReqLimitCheckInfo Info , Int32 MuiIndex )
//      {
//         /*
//          * Since we now have the cell enter event triggered, moving the 
//          * cursor will now allow the data type to be changed correctly
//          */
//         fg.Row = MuiIndex + 1;
//         if ( Info.Dat.GetType() == typeof (System.Boolean) )
//         {
//            MuiLogBool Tmp;
//            bool    TmpBool = (bool)Info.Dat;
//            if ( TmpBool )
//               Tmp = MuiLogBool.TRUE;
//            else
//               Tmp = MuiLogBool.FALSE;
//            fg.SetData ( MuiIndex + 1 , 1 , Tmp );
//         }
//         else
//         {
//            fg.SetData ( MuiIndex + 1 , 1 , Info.Dat );
//         }
//         fg.SetData ( MuiIndex + 1 , 2 , Info.ErrInfo.Msg ); 
//      }
//
//
//      private void CfgDlg ( LCParmInfo [] Dat )
//      {
//         m_GridUpdateInProgress = true;
//         fg.SuspendLayout();
//         fg.Rows.Count = Dat.Length + 1;
//         fg.Focus();
//         fg.Col = 1;
//         fg.Cols[1].TextAlign = TextAlignEnum.LeftCenter;
//         for ( Int32 i = 0; i < Dat.Length; i++ )
//         {
//            fg.Cols[1].TextAlign = TextAlignEnum.LeftCenter;
//            if ( Dat[i].myType == typeof (System.Boolean) )
//            {
//               bool Tmp = (bool)Dat[i].Dat;
//               MuiLogBool Mui;
//               if ( Tmp)
//                  Mui = MuiLogBool.TRUE;
//               else
//                  Mui = MuiLogBool.FALSE;
//
//               fg.Cols[1].DataType = typeof ( MuiLogBool);
//               fg.Row   = i + 1;
//               fg.SetData ( i + 1 , 0 , Dat[i].Name );
//               fg.SetData ( i + 1 , 1 , Mui );
//               fg.SetData ( i + 1 , 2 , "" );
//               fg.SetData ( i + 1 , 3 , Dat[i].Index);
//
//            }
//            else
//            {
//               fg.Cols[1].DataType = Dat[i].myType;
//               fg.Row   = i + 1;
//               fg.SetData ( i + 1 , 0 , Dat[i].Name );
//               fg.SetData ( i + 1 , 1 , Dat[i].Dat );
//               fg.SetData ( i + 1 , 2 , "" );
//               fg.SetData ( i + 1 , 3 , Dat[i].Index);
//
//            }
//         }
//         fg.ResumeLayout();
//         m_GridUpdateInProgress = false;
//         fg.Row = 1;
//      }
//
//      private void CfgDlg (MemberInfo[] Info , object[] od)
//      {
//         fg.Rows.Count = Info.Length + 1;
//         CfgDlgData ( 1 , Info , od );
//      }
//      private Int32 CfgDlgData ( Int32 iStartIndex , MemberInfo[] Info , 
//                                 object[] od)
//      {
//         Int32 i           = iStartIndex;
//         Int32 iInfoIndex  = 0;
//         float []  fArray = null;
//         string sArrayName = null;
//
//         foreach ( object Dat in od )
//         {
//            if ( Dat.GetType() != typeof ( float []  ) )
//            {
//               fg.SetData ( i , 0 , Info[iInfoIndex].Name);
//               iInfoIndex++;
//               if ( Dat.GetType() != typeof ( string))
//               {
//                  fg.SetData ( i , 1 , Dat.ToString());
//               }
//               else
//               {
//                  fg.SetData ( i , 1 , Dat);
//               }
//               i++;
//            }
//            else
//            {
//               fArray = (float [] ) Dat;
//               // Since od has one entry in the list for an array, 
//               // add 1 less than the array count
//               fg.Rows.Count += fArray.Length - 1;
//               for ( Int32 j = 0; j < fArray.Length; j++ )
//               {
//                  sArrayName = Info[iInfoIndex].Name + j.ToString();
//                  fg.SetData ( i , 0 , sArrayName);
//                  fg.SetData ( i , 1 , fArray[j].ToString());
//                  i++;
//               }
//               iInfoIndex++;
//            }
//         }
//         return i;
//      }
//      private Int32 ReadDlgPartial ( Int32 iStartIndex , MemberInfo[] Info , 
//                                    object[] od )
//      {
//      return ReadDlgData ( iStartIndex , Info , od );
//      }
//      private void ReadDlg (MemberInfo[] Info , object[] od)
//      {
//         ReadDlgData ( 1 , Info , od );
//
//      }
//      private Int32 ReadDlgData ( Int32 iStartIndex , MemberInfo[] Info , 
//                                  object[] od )
//      {
//         Int32 i           = iStartIndex;
//         Int32 iIndex      = 0;
//         float []  fArray = null;
//         foreach ( object Dat in od )
//         {
//            if ( Dat.GetType() != typeof ( float []  ) )
//            {
//               #region Check System Types
//               if ( Dat.GetType() == typeof ( System.Int32 ) )
//               {
//                  od[iIndex] = Convert.ToInt32(fg.GetData ( i , 1 ));
//               }
//               if ( Dat.GetType() == typeof ( System.Single ) )
//               {
//                  od[iIndex] = Convert.ToSingle(fg.GetData ( i , 1 ));
//               }
//               if ( Dat.GetType() == typeof ( System.String ) )
//               {
//                  od[iIndex] = fg.GetData ( i , 1 );
//               }
//               if ( Dat.GetType() == typeof ( System.Boolean ) )
//               {
//                  od[iIndex] = Convert.ToBoolean(fg.GetData ( i , 1 ));
//               }
//               if ( Dat.GetType() == typeof ( System.UInt32 ) )
//               {
//                  od[iIndex] = Convert.ToUInt32( fg.GetData( i , 1 ) );
//               }
//               #endregion
//
//               #region CheckEnumerationType
//               if ( (Dat.GetType()).IsEnum )
//               {
//                  Type myType = Dat.GetType();
//                  string StringDat  = fg.GetData( i , 1 ).ToString();
//                  od[iIndex] = Enum.Parse( myType, StringDat );
//               }
//               #endregion
//               iIndex++;
//               i++;
//            }
//            else
//            {
//               fArray = (float [] ) Dat;
//               for ( Int32 j = 0; j < fArray.Length; j++ )
//               {
//                  fArray[j] = Convert.ToSingle(fg.GetData ( i , 1 ));
//                  i++;
//               }
//               iIndex++;
//            }
//         }
//         return i;
//      }
      #endregion
	}
   #region GridLogSetupStruct

   public struct GridLogSetupStruct
   {
      public   Int32    iTotalLength;
      public   string   sCol0Label;
      public   string   sCol1Label;
   }
   #endregion

}
