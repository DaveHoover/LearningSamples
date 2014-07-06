using System;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using WMEncoderLib;

namespace MediaEncoder
{
	/// <summary>
	/// Summary description for MultipleFileFrm.
	/// </summary>
	public class MultipleFileFrm : System.Windows.Forms.Form
	{
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.TextBox tbOutputDirectory;
      //private CalGridCtrlFrm fgCtrl;
      private System.Windows.Forms.Button btnEncodeAll;
      private System.Windows.Forms.Button btnEncodeLossless;
      private System.Windows.Forms.Label lblError;
      private System.Windows.Forms.Label lblStatus;
      private System.Windows.Forms.Button btnAbort;

      private EncoderInfo []             m_EncodingSourceInfo    = null;
      private Int32                      m_CurrentIndex          = 0;
      private bool                       m_AbortRequested         = false;



      private EncodingProfileEnum         m_CurrentProfile;
      private EncodingProfileEnum         m_StartingProfile;
      private bool                        m_EncodingDone;
      private WMEncoder                   m_Encoder;
      private EncoderInfo                 m_CurrentInfo;
      private string                      VbcLowProfile        = "VBC_LowAudioQuality";
      private string                      VbcHighProfile       = "VBC_HighAudioQuality";
      private string                      VbcLosslessProfile   = "VBC_Lossless";
      private DataGridView dataGridView1;
      private DataGridViewTextBoxColumn Column1;
      private DataGridViewTextBoxColumn Column2;
      private DataGridViewTextBoxColumn Column3;
      private DataGridViewTextBoxColumn Column4;
      


      
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MultipleFileFrm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         //fgCtrl.OnBeforeEditEvent += new CalGridCtrlFrm.GridBeforeEditDelegate ( BeforeEdit );
         //fgCtrl.OnAfterEditEvent  += new CalGridCtrlFrm.GridBeforeEditDelegate ( AfterEdit );
         //fgCtrl.OnEnterCellEvent  += new EventHandler ( EnterCell );
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
               //fgCtrl.OnBeforeEditEvent -= new CalGridCtrlFrm.GridBeforeEditDelegate ( BeforeEdit );
               //fgCtrl.OnAfterEditEvent  -= new CalGridCtrlFrm.GridBeforeEditDelegate ( AfterEdit );
               //fgCtrl.OnEnterCellEvent  -= new EventHandler ( EnterCell );

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
         this.btnOK = new System.Windows.Forms.Button();
         this.btnCancel = new System.Windows.Forms.Button();
         this.label2 = new System.Windows.Forms.Label();
         this.tbOutputDirectory = new System.Windows.Forms.TextBox();
         this.btnEncodeAll = new System.Windows.Forms.Button();
         this.btnEncodeLossless = new System.Windows.Forms.Button();
         this.lblError = new System.Windows.Forms.Label();
         this.lblStatus = new System.Windows.Forms.Label();
         this.btnAbort = new System.Windows.Forms.Button();
         this.dataGridView1 = new System.Windows.Forms.DataGridView();
         this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
         this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
         this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
         this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
         ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
         this.SuspendLayout();
         // 
         // btnOK
         // 
         this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.btnOK.Location = new System.Drawing.Point(880, 32);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(75, 23);
         this.btnOK.TabIndex = 0;
         this.btnOK.Text = "&Ok";
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // btnCancel
         // 
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(880, 64);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(75, 23);
         this.btnCancel.TabIndex = 1;
         this.btnCancel.Text = "&Cancel";
         this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(24, 24);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(84, 13);
         this.label2.TabIndex = 4;
         this.label2.Text = "&Output Directory";
         // 
         // tbOutputDirectory
         // 
         this.tbOutputDirectory.Location = new System.Drawing.Point(120, 24);
         this.tbOutputDirectory.Name = "tbOutputDirectory";
         this.tbOutputDirectory.Size = new System.Drawing.Size(424, 20);
         this.tbOutputDirectory.TabIndex = 5;
         // 
         // btnEncodeAll
         // 
         this.btnEncodeAll.Location = new System.Drawing.Point(728, 32);
         this.btnEncodeAll.Name = "btnEncodeAll";
         this.btnEncodeAll.Size = new System.Drawing.Size(136, 23);
         this.btnEncodeAll.TabIndex = 7;
         this.btnEncodeAll.Text = "&Encode All";
         this.btnEncodeAll.Click += new System.EventHandler(this.btnEncodeAll_Click);
         // 
         // btnEncodeLossless
         // 
         this.btnEncodeLossless.Location = new System.Drawing.Point(728, 64);
         this.btnEncodeLossless.Name = "btnEncodeLossless";
         this.btnEncodeLossless.Size = new System.Drawing.Size(136, 23);
         this.btnEncodeLossless.TabIndex = 8;
         this.btnEncodeLossless.Text = "Encode &Lossless";
         this.btnEncodeLossless.Click += new System.EventHandler(this.btnEncodeLossless_Click);
         // 
         // lblError
         // 
         this.lblError.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.lblError.ForeColor = System.Drawing.Color.IndianRed;
         this.lblError.Location = new System.Drawing.Point(16, 392);
         this.lblError.Name = "lblError";
         this.lblError.Size = new System.Drawing.Size(816, 64);
         this.lblError.TabIndex = 10;
         // 
         // lblStatus
         // 
         this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.lblStatus.ForeColor = System.Drawing.SystemColors.ActiveCaption;
         this.lblStatus.Location = new System.Drawing.Point(16, 80);
         this.lblStatus.Name = "lblStatus";
         this.lblStatus.Size = new System.Drawing.Size(616, 80);
         this.lblStatus.TabIndex = 11;
         this.lblStatus.Text = "Status";
         // 
         // btnAbort
         // 
         this.btnAbort.Location = new System.Drawing.Point(880, 96);
         this.btnAbort.Name = "btnAbort";
         this.btnAbort.Size = new System.Drawing.Size(75, 23);
         this.btnAbort.TabIndex = 12;
         this.btnAbort.Text = "&Abort";
         this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
         // 
         // dataGridView1
         // 
         this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
         this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4});
         this.dataGridView1.Location = new System.Drawing.Point(12, 163);
         this.dataGridView1.Name = "dataGridView1";
         this.dataGridView1.Size = new System.Drawing.Size(960, 293);
         this.dataGridView1.TabIndex = 13;
         this.dataGridView1.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.CellBeginEdit);
         this.dataGridView1.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.CellEnter);
         // 
         // Column1
         // 
         this.Column1.HeaderText = "FileName";
         this.Column1.Name = "Column1";
         this.Column1.Width = 300;
         // 
         // Column2
         // 
         this.Column2.HeaderText = "Message Title";
         this.Column2.Name = "Column2";
         this.Column2.Width = 150;
         // 
         // Column3
         // 
         this.Column3.HeaderText = "Description";
         this.Column3.Name = "Column3";
         this.Column3.Width = 300;
         // 
         // Column4
         // 
         this.Column4.HeaderText = "Author";
         this.Column4.Name = "Column4";
         // 
         // MultipleFileFrm
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(984, 470);
         this.Controls.Add(this.dataGridView1);
         this.Controls.Add(this.btnAbort);
         this.Controls.Add(this.lblStatus);
         this.Controls.Add(this.lblError);
         this.Controls.Add(this.btnEncodeLossless);
         this.Controls.Add(this.btnEncodeAll);
         this.Controls.Add(this.tbOutputDirectory);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.Name = "MultipleFileFrm";
         this.Text = "MultipleFileFrm";
         ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }
		#endregion

      private void btnOK_Click(object sender, System.EventArgs e)
      {
         this.Close();
      
      }

      private void btnCancel_Click(object sender, System.EventArgs e)
      {
         this.Close();
      }

      private void btnAbort_Click(object sender, System.EventArgs e)
      {
         m_AbortRequested = true;
      }


      private void btnEncodeAll_Click(object sender, System.EventArgs e)
      {
         m_EncodingSourceInfo = ReadGridInfo ();
         this.m_CurrentIndex  = 0;
         m_CurrentProfile = EncodingProfileEnum.LOW;
         m_StartingProfile = m_CurrentProfile;
         this.Cursor =  Cursors.AppStarting;
         EncodeFile( m_CurrentProfile , m_EncodingSourceInfo[0]);
      }

      private void btnEncodeLossless_Click(object sender, System.EventArgs e)
      {
         m_EncodingSourceInfo = ReadGridInfo ();
         this.m_CurrentIndex  = 0;
         m_CurrentProfile = EncodingProfileEnum.LOSSLESS;
         m_StartingProfile = m_CurrentProfile;
         this.Cursor =  Cursors.AppStarting;
         EncodeFile( m_CurrentProfile , m_EncodingSourceInfo[0]);
      }

      private void CellEnter(object sender, DataGridViewCellEventArgs e)
      {
         if (e.ColumnIndex == 0)
         {
            EncoderInfo Info = new EncoderInfo();
            GetFileName(ref Info);
            dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Info.FileNameAndPath;
         }
      }


      private void CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
      {
         
         if (e.ColumnIndex == 0)
         {
            EncoderInfo Info = new EncoderInfo();
            GetFileName(ref Info);
            dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Info.FileNameAndPath;
         }
      }


      private void GetFileName( ref EncoderInfo Info )
      {
         OpenFileDialog FileDialog1 = new OpenFileDialog();
 
         FileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"  ;
         FileDialog1.FilterIndex = 2 ;
         FileDialog1.RestoreDirectory = true ;
 
         if(FileDialog1.ShowDialog() == DialogResult.OK)
         {
            Info.FileNameAndPath     = FileDialog1.FileName;
            Info.FilePath            = Path.GetDirectoryName(Info.FileNameAndPath) + @"\";
            Info.FileName            = Path.GetFileName(Info.FileNameAndPath);
            Info.FileNameNoExtension = Path.GetFileNameWithoutExtension(Info.FileNameAndPath);
         }
         else
         {
            Info.FileNameAndPath = "";
         }
      }
      private EncoderInfo [] ReadGridInfo ()
      {
         Int32 RowIndex = 0;
         string FileName = null;
         ArrayList List = new ArrayList();
         do
         {
            FileName = (string)dataGridView1.Rows[RowIndex].Cells[0].Value;
            if ( !(FileName == "" || FileName == null))
            {
               EncoderInfo Info = new EncoderInfo();
               Info.FileNameAndPath = FileName;
               Info.FilePath            = Path.GetDirectoryName(Info.FileNameAndPath) + @"\";
               Info.FileName            = Path.GetFileName(Info.FileNameAndPath);
               Info.FileNameNoExtension = Path.GetFileNameWithoutExtension(Info.FileNameAndPath);
               Info.MessageTitle       = (string)dataGridView1.Rows[RowIndex].Cells[1].Value;
               Info.Description = (string)dataGridView1.Rows[RowIndex].Cells[2].Value;
               Info.Author = (string)dataGridView1.Rows[RowIndex].Cells[3].Value;
               List.Add ( Info );
               RowIndex++;
            }

         } while ( !(FileName == "" || FileName == null));
         EncoderInfo [] FileInfo = (EncoderInfo[] )List.ToArray( typeof ( EncoderInfo));
         return FileInfo;
      }
      #region OnStateChange
      public void OnStateChange( WMENC_ENCODER_STATE enumState )
      {
         switch( enumState )
         {
            case WMENC_ENCODER_STATE.WMENC_ENCODER_RUNNING:
               // TODO: Handle running state.
               break;

            case WMENC_ENCODER_STATE.WMENC_ENCODER_STOPPED:
               // TODO: Handle stopped state.
               EncodingCompleted();
               break;

            case WMENC_ENCODER_STATE.WMENC_ENCODER_STARTING:
               // TODO: Handle starting state.
               break;

            case WMENC_ENCODER_STATE.WMENC_ENCODER_PAUSING:
               // TODO: Handle pausing state.
               break;

            case WMENC_ENCODER_STATE.WMENC_ENCODER_STOPPING:
               // TODO: Handle stopping state.
               break;

            case WMENC_ENCODER_STATE.WMENC_ENCODER_PAUSED:
               // TODO: Handle paused state.
               break;

            case WMENC_ENCODER_STATE.WMENC_ENCODER_END_PREPROCESS:
               // TODO: Handle end preprocess state.
               break;
         }
      }
      #endregion

      #region EncodeFile
      private void EncodeFile ( EncodingProfileEnum Profile , EncoderInfo Info)
      {
         try 
         {
            m_CurrentInfo = Info;
            DateTime Now = DateTime.Now;
            lblStatus.Text = " Starting to encode " + m_CurrentProfile.ToString() +
               " Starting time = " + Now.ToLongTimeString();
            m_EncodingDone = false;
            // Create a WMEncoder object.
            m_Encoder = new WMEncoder();
            m_Encoder.OnStateChange += new _IWMEncoderEvents_OnStateChangeEventHandler(
               OnStateChange );

            // Retrieve the source group collection.
            IWMEncSourceGroupCollection SrcGrpColl = m_Encoder.SourceGroupCollection;

            // Add a source group to the collection.
            IWMEncSourceGroup SrcGrp  = SrcGrpColl.Add("SG_1");

            // Add a video and audio source to the source group.
            IWMEncSource SrcAud = SrcGrp.AddSource(WMENC_SOURCE_TYPE.WMENC_AUDIO);
            SrcAud.SetInput(Info.FileNameAndPath, "", "");
            // Specify a file object in which to save encoded content.
            SetOutputFileName ( Info.FileNameNoExtension);
            SelectProfile ( SrcGrp );

            // Fill in the description object members.
            IWMEncDisplayInfo Descr = m_Encoder.DisplayInfo;
            Descr.Author      = Info.Author;
            Descr.Copyright   = "Valley Bible Church @2005";
            Descr.Description = Info.Description;
            Descr.Rating      = "All Audiences";
            Descr.Title       = Info.MessageTitle;

            // Add an attribute to the collection.
            IWMEncAttributes Attr = m_Encoder.Attributes;
            Attr.Add ("URL", "IP address");

            // Start the encoding process.
            // Wait until the encoding process stops before exiting the application.
            m_Encoder.PrepareToEncode(true);
            m_Encoder.Start();
         } 
         catch (Exception e) 
         {  
            lblError.Text = e.ToString();
            Debug.WriteLine ( e.ToString());
            m_CurrentInfo = null;
         }
      }

      #endregion

      #region SetOutputFileName 
      private void SetOutputFileName ( string BaseFileName )
      {
         IWMEncFile File = m_Encoder.File;
         string FilePath = m_CurrentInfo.FilePath;
         switch ( m_CurrentProfile )
         {
            case EncodingProfileEnum.LOW:
               File.LocalFileName = FilePath + BaseFileName + ".wmv";
               break;
            case EncodingProfileEnum.HIGH:
               File.LocalFileName = FilePath + BaseFileName + "_high.wmv";
               break;
            case EncodingProfileEnum.LOSSLESS:
               File.LocalFileName = FilePath + BaseFileName + "_ll.wmv";
               break;
         }
      }
      #endregion
      #region SelectProfile IWMEncSourceGroup SrcGrp 

      private bool SelectProfile ( IWMEncSourceGroup SrcGrp )
      {
         string ProfileName = null;
         switch ( m_CurrentProfile )
         {
            case EncodingProfileEnum.LOW:
               ProfileName = VbcLowProfile;
               break;
            case EncodingProfileEnum.HIGH:
               ProfileName = VbcHighProfile;
               break;
            case EncodingProfileEnum.LOSSLESS:
               ProfileName = VbcLosslessProfile;
               break;
         }
         IWMEncProfileCollection ProColl = m_Encoder.ProfileCollection;
         IWMEncProfile Pro;
         for (int i = 0; i < ProColl.Count; i++)
         {
            Pro = ProColl.Item(i);
            if (Pro.Name == ProfileName)
            {
               SrcGrp.set_Profile(Pro);
               return true;
            }
         }
         return false;
      }
      #endregion


      private void EncodingCompleted()
      {
         m_EncodingDone = true;
         Debug.WriteLine ( "Completed pass " + m_CurrentProfile.ToString() + 
            " WMENC_ENCODER_STATE.WMENC_ENCODER_STOPPED");
         lblStatus.Text = " Finished encoding " + m_CurrentProfile.ToString();
         m_Encoder.OnStateChange -= new _IWMEncoderEvents_OnStateChangeEventHandler(
            OnStateChange );
         m_Encoder = null;
         if ( m_AbortRequested == true )
         {
            this.Cursor = Cursors.Arrow;
            m_CurrentProfile = m_StartingProfile;
            m_CurrentIndex = 0;
            m_AbortRequested = false;
            return;
         }

         switch ( m_CurrentProfile )
         {
            case EncodingProfileEnum.LOW:
               m_CurrentProfile = EncodingProfileEnum.HIGH;
               EncodeFile( m_CurrentProfile , this.m_CurrentInfo);
               break;
            case EncodingProfileEnum.HIGH:
               m_CurrentProfile = EncodingProfileEnum.LOSSLESS;
               EncodeFile( m_CurrentProfile , this.m_CurrentInfo);
               break;
            case EncodingProfileEnum.LOSSLESS:
               m_CurrentIndex++;
               if ( m_CurrentIndex >= m_EncodingSourceInfo.Length )
               {
                  this.Cursor = Cursors.Arrow;
               }
               else
               {
                  m_CurrentProfile = m_StartingProfile;
                  EncodeFile( m_CurrentProfile , m_EncodingSourceInfo[m_CurrentIndex]);
               }
               break;
         }

      }




	}
}
