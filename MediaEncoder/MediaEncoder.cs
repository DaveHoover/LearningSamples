using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using WMEncoderLib;
using System.Diagnostics;
//using Microsoft.Office.Interop.Publisher;
//using Microsoft.Office.Interop.Word;

namespace MediaEncoder
{

   /// <summary>
   /// Summary description for Form1.
   /// </summary>
   public class MediaEncoder : System.Windows.Forms.Form
   {
      private System.Windows.Forms.MainMenu mainMenu1;
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.Container components = null;
      private System.Windows.Forms.Button btnStartEncode;
      private System.Windows.Forms.Button btnProfileEditor;

      private bool                        m_EncodingDone;
      private EncoderInfo                 m_Info               = new EncoderInfo();
      private EncodingProfileEnum         m_Profile;
      private WMEncoder                   m_Encoder;
      private string                      VbcLowProfile        = "VBC_LowAudioQuality";
      private string                      VbcHighProfile       = "VBC_HighAudioQuality";
      private string                      VbcLosslessProfile   = "VBC_Lossless";
      private System.Windows.Forms.Label lblStatus;
      private System.Windows.Forms.Label lblError;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.TextBox tbMessageTitle;
      private System.Windows.Forms.TextBox tbDescription;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.TextBox tbFileNameAndPath;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.TextBox tbAuthor;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.MenuItem menuItem1;
      private System.Windows.Forms.MenuItem mnuFileExit;
      private System.Windows.Forms.MenuItem menuItem3;
      private System.Windows.Forms.MenuItem mnuHelpAbout;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.HelpProvider helpProvider1;
      private System.Windows.Forms.MenuItem menuItem2;
      private System.Windows.Forms.MenuItem mnuPublisherCreatePsOutput;
      private System.Windows.Forms.MenuItem menuItem4;
      private System.Windows.Forms.MenuItem mnuAmiProConvert;
      private System.Windows.Forms.Button btnEncodeLossless;
      private System.Windows.Forms.Button btnMultipleFiles;
      private System.Windows.Forms.CheckBox cbMultiple;
      

      public MediaEncoder()
      {
         InitializeComponent();
         m_Info.Author        = "Paul Winslow";
         m_Info.MessageTitle  = "Message Title";
         m_Info.Description   = "Message Description";
         tbMessageTitle.Text  = m_Info.MessageTitle;
         tbDescription.Text   = m_Info.Description;
         tbAuthor.Text        = m_Info.Author;
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
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MediaEncoder));
         this.mainMenu1 = new System.Windows.Forms.MainMenu();
         this.menuItem1 = new System.Windows.Forms.MenuItem();
         this.mnuFileExit = new System.Windows.Forms.MenuItem();
         this.menuItem2 = new System.Windows.Forms.MenuItem();
         this.mnuPublisherCreatePsOutput = new System.Windows.Forms.MenuItem();
         this.menuItem3 = new System.Windows.Forms.MenuItem();
         this.mnuHelpAbout = new System.Windows.Forms.MenuItem();
         this.menuItem4 = new System.Windows.Forms.MenuItem();
         this.mnuAmiProConvert = new System.Windows.Forms.MenuItem();
         this.btnStartEncode = new System.Windows.Forms.Button();
         this.btnProfileEditor = new System.Windows.Forms.Button();
         this.lblStatus = new System.Windows.Forms.Label();
         this.lblError = new System.Windows.Forms.Label();
         this.label1 = new System.Windows.Forms.Label();
         this.tbMessageTitle = new System.Windows.Forms.TextBox();
         this.tbDescription = new System.Windows.Forms.TextBox();
         this.label2 = new System.Windows.Forms.Label();
         this.tbFileNameAndPath = new System.Windows.Forms.TextBox();
         this.label3 = new System.Windows.Forms.Label();
         this.tbAuthor = new System.Windows.Forms.TextBox();
         this.label4 = new System.Windows.Forms.Label();
         this.label5 = new System.Windows.Forms.Label();
         this.helpProvider1 = new System.Windows.Forms.HelpProvider();
         this.btnEncodeLossless = new System.Windows.Forms.Button();
         this.btnMultipleFiles = new System.Windows.Forms.Button();
         this.cbMultiple = new System.Windows.Forms.CheckBox();
         this.SuspendLayout();
         // 
         // mainMenu1
         // 
         this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                  this.menuItem1,
                                                                                  this.menuItem2,
                                                                                  this.menuItem3,
                                                                                  this.menuItem4});
         // 
         // menuItem1
         // 
         this.menuItem1.Index = 0;
         this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                  this.mnuFileExit});
         this.menuItem1.Text = "&File";
         // 
         // mnuFileExit
         // 
         this.mnuFileExit.Index = 0;
         this.mnuFileExit.Shortcut = System.Windows.Forms.Shortcut.F12;
         this.mnuFileExit.Text = "E&xit";
         this.mnuFileExit.Click += new System.EventHandler(this.mnuFileExit_Click);
         // 
         // menuItem2
         // 
         this.menuItem2.Index = 1;
         this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                  this.mnuPublisherCreatePsOutput});
         this.menuItem2.Text = "&Publisher";
         // 
         // mnuPublisherCreatePsOutput
         // 
         this.mnuPublisherCreatePsOutput.Index = 0;
         this.mnuPublisherCreatePsOutput.Text = "&Create PS PrinterOutput";
         this.mnuPublisherCreatePsOutput.Click += new System.EventHandler(this.mnuPublisherCreatePsOutput_Click);
         // 
         // menuItem3
         // 
         this.menuItem3.Index = 2;
         this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                  this.mnuHelpAbout});
         this.menuItem3.Text = "&Help";
         // 
         // mnuHelpAbout
         // 
         this.mnuHelpAbout.Index = 0;
         this.mnuHelpAbout.Text = "&About";
         this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
         // 
         // menuItem4
         // 
         this.menuItem4.Index = 3;
         this.menuItem4.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                  this.mnuAmiProConvert});
         this.menuItem4.Text = "&AmiPro";
         // 
         // mnuAmiProConvert
         // 
         this.mnuAmiProConvert.Index = 0;
         this.mnuAmiProConvert.Text = "&Convert";
         this.mnuAmiProConvert.Click += new System.EventHandler(this.mnuAmiProConvert_Click);
         // 
         // btnStartEncode
         // 
         this.btnStartEncode.Location = new System.Drawing.Point(8, 224);
         this.btnStartEncode.Name = "btnStartEncode";
         this.btnStartEncode.Size = new System.Drawing.Size(104, 23);
         this.btnStartEncode.TabIndex = 10;
         this.btnStartEncode.Text = "&Encode All";
         this.btnStartEncode.Click += new System.EventHandler(this.btnStartEncode_Click);
         // 
         // btnProfileEditor
         // 
         this.btnProfileEditor.Location = new System.Drawing.Point(8, 288);
         this.btnProfileEditor.Name = "btnProfileEditor";
         this.btnProfileEditor.Size = new System.Drawing.Size(104, 23);
         this.btnProfileEditor.TabIndex = 11;
         this.btnProfileEditor.Text = "P&rofileEditor";
         this.btnProfileEditor.Click += new System.EventHandler(this.btnProfileEditor_Click);
         // 
         // lblStatus
         // 
         this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
         this.lblStatus.ForeColor = System.Drawing.SystemColors.ActiveCaption;
         this.lblStatus.Location = new System.Drawing.Point(136, 264);
         this.lblStatus.Name = "lblStatus";
         this.lblStatus.Size = new System.Drawing.Size(456, 136);
         this.lblStatus.TabIndex = 0;
         this.lblStatus.Text = "Status";
         // 
         // lblError
         // 
         this.lblError.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
         this.lblError.ForeColor = System.Drawing.Color.IndianRed;
         this.lblError.Location = new System.Drawing.Point(88, 16);
         this.lblError.Name = "lblError";
         this.lblError.Size = new System.Drawing.Size(472, 80);
         this.lblError.TabIndex = 1;
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(24, 112);
         this.label1.Name = "label1";
         this.label1.TabIndex = 2;
         this.label1.Text = "&Message Title";
         // 
         // tbMessageTitle
         // 
         this.helpProvider1.SetHelpKeyword(this.tbMessageTitle, "Title Information");
         this.helpProvider1.SetHelpString(this.tbMessageTitle, "Title Information");
         this.tbMessageTitle.Location = new System.Drawing.Point(144, 112);
         this.tbMessageTitle.Name = "tbMessageTitle";
         this.helpProvider1.SetShowHelp(this.tbMessageTitle, true);
         this.tbMessageTitle.Size = new System.Drawing.Size(416, 20);
         this.tbMessageTitle.TabIndex = 3;
         this.tbMessageTitle.Text = "textBox1";
         // 
         // tbDescription
         // 
         this.tbDescription.Location = new System.Drawing.Point(144, 136);
         this.tbDescription.Name = "tbDescription";
         this.tbDescription.Size = new System.Drawing.Size(416, 20);
         this.tbDescription.TabIndex = 5;
         this.tbDescription.Text = "textBox2";
         // 
         // label2
         // 
         this.label2.Location = new System.Drawing.Point(24, 136);
         this.label2.Name = "label2";
         this.label2.TabIndex = 4;
         this.label2.Text = "&Description";
         // 
         // tbFileNameAndPath
         // 
         this.tbFileNameAndPath.Location = new System.Drawing.Point(144, 184);
         this.tbFileNameAndPath.Name = "tbFileNameAndPath";
         this.tbFileNameAndPath.Size = new System.Drawing.Size(416, 20);
         this.tbFileNameAndPath.TabIndex = 9;
         this.tbFileNameAndPath.Text = "textBox3";
         this.tbFileNameAndPath.Enter += new System.EventHandler(this.OnFocusEnterForFileName);
         // 
         // label3
         // 
         this.label3.Location = new System.Drawing.Point(24, 184);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(104, 23);
         this.label3.TabIndex = 8;
         this.label3.Text = "File &Name and Path";
         // 
         // tbAuthor
         // 
         this.tbAuthor.Location = new System.Drawing.Point(144, 160);
         this.tbAuthor.Name = "tbAuthor";
         this.tbAuthor.Size = new System.Drawing.Size(416, 20);
         this.tbAuthor.TabIndex = 7;
         this.tbAuthor.Text = "textBox3";
         // 
         // label4
         // 
         this.label4.Location = new System.Drawing.Point(24, 160);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(104, 23);
         this.label4.TabIndex = 6;
         this.label4.Text = "&Author";
         // 
         // label5
         // 
         this.label5.Location = new System.Drawing.Point(136, 224);
         this.label5.Name = "label5";
         this.label5.Size = new System.Drawing.Size(224, 23);
         this.label5.TabIndex = 12;
         this.label5.Text = "Encoding Status/Progress";
         // 
         // helpProvider1
         // 
         this.helpProvider1.HelpNamespace = "C:\\PROGRAMS\\NETLearn\\MediaEncoder\\Help\\MediaEncoderHelp.chm";
         // 
         // btnEncodeLossless
         // 
         this.btnEncodeLossless.Location = new System.Drawing.Point(8, 256);
         this.btnEncodeLossless.Name = "btnEncodeLossless";
         this.btnEncodeLossless.Size = new System.Drawing.Size(104, 23);
         this.btnEncodeLossless.TabIndex = 13;
         this.btnEncodeLossless.Text = "Encode Lossless";
         this.btnEncodeLossless.Click += new System.EventHandler(this.btnEncodeLossless_Click);
         // 
         // btnMultipleFiles
         // 
         this.btnMultipleFiles.Location = new System.Drawing.Point(8, 320);
         this.btnMultipleFiles.Name = "btnMultipleFiles";
         this.btnMultipleFiles.Size = new System.Drawing.Size(104, 23);
         this.btnMultipleFiles.TabIndex = 14;
         this.btnMultipleFiles.Text = "&Multiple Files";
         this.btnMultipleFiles.Click += new System.EventHandler(this.btnMultipleFiles_Click);
         // 
         // cbMultiple
         // 
         this.cbMultiple.Location = new System.Drawing.Point(8, 352);
         this.cbMultiple.Name = "cbMultiple";
         this.cbMultiple.TabIndex = 15;
         this.cbMultiple.Text = "&Use Multiple";
         // 
         // MediaEncoder
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(616, 430);
         this.Controls.Add(this.cbMultiple);
         this.Controls.Add(this.btnMultipleFiles);
         this.Controls.Add(this.btnEncodeLossless);
         this.Controls.Add(this.label5);
         this.Controls.Add(this.tbAuthor);
         this.Controls.Add(this.tbFileNameAndPath);
         this.Controls.Add(this.tbDescription);
         this.Controls.Add(this.tbMessageTitle);
         this.Controls.Add(this.label4);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.lblError);
         this.Controls.Add(this.lblStatus);
         this.Controls.Add(this.btnProfileEditor);
         this.Controls.Add(this.btnStartEncode);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Menu = this.mainMenu1;
         this.Name = "MediaEncoder";
         this.Text = "MediaEncoder";
         this.ResumeLayout(false);

      }
      #endregion

      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main() 
      {
         System.Windows.Forms.Application.Run(new MediaEncoder());
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

      private void EncodeFile ( EncodingProfileEnum Profile)
      {
         try 
         {
            DateTime Now = DateTime.Now;
            lblStatus.Text = " Starting to encode " + m_Profile.ToString() +
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
            SrcAud.SetInput(m_Info.FileNameAndPath, "", "");
            // Specify a file object in which to save encoded content.
            SetOutputFileName ( m_Info.FileNameNoExtension);
            SelectProfile ( SrcGrp );

            // Fill in the description object members.
            IWMEncDisplayInfo Descr = m_Encoder.DisplayInfo;
            Descr.Author      = tbAuthor.Text;
            Descr.Copyright   = "Valley Bible Church @2005";
            Descr.Description = tbDescription.Text;
            Descr.Rating      = "All Audiences";
            Descr.Title       = tbMessageTitle.Text;

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
         }
      }

      #endregion

      #region SetOutputFileName 
      private void SetOutputFileName ( string BaseFileName )
      {
         IWMEncFile File = m_Encoder.File;
         string FilePath = m_Info.FilePath;
         switch ( m_Profile )
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
         switch ( m_Profile )
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
         Debug.WriteLine ( "Completed pass " + m_Profile.ToString() + 
            " WMENC_ENCODER_STATE.WMENC_ENCODER_STOPPED");
         lblStatus.Text = " Finished encoding " + m_Profile.ToString();
         m_Encoder.OnStateChange -= new _IWMEncoderEvents_OnStateChangeEventHandler(
            OnStateChange );
         m_Encoder = null;

         switch ( m_Profile )
         {
            case EncodingProfileEnum.LOW:
               m_Profile = EncodingProfileEnum.HIGH;
               EncodeFile( m_Profile );
               break;
            case EncodingProfileEnum.HIGH:
               m_Profile = EncodingProfileEnum.LOSSLESS;
               EncodeFile( m_Profile );
               break;
            case EncodingProfileEnum.LOSSLESS:
               break;
         }

      }


      #region DisplayProfileManager
      private void DisplayProfileManager ()
      {
         // Create a WMEncProfileManager object.
         WMEncProfileManager ProfileMgr = new  WMEncProfileManager();

         // Display the Profile Manager and list only profiles that supportaudio and video.
         ProfileMgr.WMEncProfileList ( WMENC_MEDIA_FILTER.WMENC_FILTER_A, 0 );
         // Display the Profile Editor.
         ProfileMgr.WMEncProfileEdit ("", WMENC_MEDIA_FILTER.WMENC_FILTER_A, 0 );
         // To edit an existing custom profile, specify its name.
         ProfileMgr.WMEncProfileEdit ("MyProfileName", WMENC_MEDIA_FILTER.WMENC_FILTER_A, 0 );
         // Display the details of a specific profile.
         MessageBox.Show (ProfileMgr.GetDetailsString("Windows Media Video 8 for Local Area Network (384 Kbps)", 0));
         // Display the path of the directory in which custom profiles are stored.
         MessageBox.Show (ProfileMgr.ProfileDirectory);

      }
      #endregion

      #region CreateEncoderAppWithUI

      private void CreateEncoderAppWithUI ()
      {
         WMEncoderApp EncoderApp = new WMEncoderApp();
         IWMEncoder Encoder = EncoderApp.Encoder;

         // Display the predefined Encoder UI.
         EncoderApp.Visible = true;
      }
      #endregion

      private void btnStartEncode_Click(object sender, System.EventArgs e)
      {
         m_Profile = EncodingProfileEnum.LOW;
         EncodeFile( m_Profile );
      }

      private void btnEncodeLossless_Click(object sender, System.EventArgs e)
      {
         m_Profile = EncodingProfileEnum.LOSSLESS;
         EncodeFile( m_Profile );
      }


      private void btnProfileEditor_Click(object sender, System.EventArgs e)
      {
         DisplayProfileManager();
      }

      private void OnFocusEnterForFileName(object sender, System.EventArgs e)
      {
         OpenFileDialog FileDialog1 = new OpenFileDialog();
 
         FileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"  ;
         FileDialog1.FilterIndex = 2 ;
         FileDialog1.RestoreDirectory = true ;
 
         if(FileDialog1.ShowDialog() == DialogResult.OK)
         {
            m_Info.FileNameAndPath     = FileDialog1.FileName;
            m_Info.FilePath            = Path.GetDirectoryName(m_Info.FileNameAndPath) + @"\";
            m_Info.FileName            = Path.GetFileName(m_Info.FileNameAndPath);
            m_Info.FileNameNoExtension = Path.GetFileNameWithoutExtension(m_Info.FileNameAndPath);
            tbFileNameAndPath.Text = FileDialog1.FileName;


         }

      
      }

      private void mnuFileExit_Click(object sender, System.EventArgs e)
      {
         this.Close();
      }

      private void mnuHelpAbout_Click(object sender, System.EventArgs e)
      {
         AboutForm Frm = new AboutForm();
         Frm.ShowDialog();
      }

      private void mnuPublisherCreatePsOutput_Click(object sender, System.EventArgs e)
      {
         OpenFileDialog FD = new OpenFileDialog();
 
         FD.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"  ;
         FD.Multiselect = true;
         FD.FilterIndex = 2 ;
         FD.RestoreDirectory = true ;
         if(FD.ShowDialog() == DialogResult.OK)
         {
            string [] Names = FD.FileNames;
            foreach ( string Name in Names )
            {
               Debug.WriteLine ( "File Name = " + Name );
            }
            PrintPostScript ( Names );
         }


      }
      private void PrintPostScript ( string [] FileNames )
      {
         //Microsoft.Office.Interop.Publisher.ApplicationClass Pub = null;
         //Microsoft.Office.Interop.Publisher.Document Doc = null;
         //Microsoft.Office.Interop.Publisher.PageSetup PageSet = null;
         //foreach ( string FileName in FileNames )
         //{
         //   try 
         //   {
         //      Pub = new Microsoft.Office.Interop.Publisher.ApplicationClass();
         //      Doc =  Pub.Open ( FileName , false , true , PbSaveOptions.pbDoNotSaveChanges );
         //      PageSet = Doc.PageSetup;
         //      Int32 NumPages = Doc.Pages.Count;
         //      string CommercialColorPrinter = "Generic Color PS for Commercial Printing";
         //      Doc.ActivePrinter = CommercialColorPrinter;
         //      string FilePath      = Path.GetDirectoryName(FileName) + @"\";
         //      string BaseFileName  = Path.GetFileNameWithoutExtension(FileName);
         //      string OutFileName = FilePath + BaseFileName + ".ps";
         //      Doc.PrintOut ( 1 , NumPages ,OutFileName , 1 , true );
         //   }
         //   catch (Exception e )
         //   {
         //      Debug.WriteLine ( e.ToString() );
         
         //   }
         //   finally 
         //   {
         //      if ( Doc != null )
         //      {
         //         Doc.Close();            
         //      }
         //   }
         //}

      }

      private void mnuAmiProConvert_Click(object sender, System.EventArgs e)
      {
         OpenFileDialog FD = new OpenFileDialog();
 
         FD.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"  ;
         FD.Multiselect = true;
         FD.FilterIndex = 2 ;
         FD.RestoreDirectory = true ;
         if(FD.ShowDialog() == DialogResult.OK)
         {
            string [] Names = FD.FileNames;
            foreach ( string Name in Names )
            {
               Debug.WriteLine ( "File Name = " + Name );
            }
            PrintPostScript ( Names );
         }
      
      }

      private void ConvertAmiProDocs ( string [] FileNames )
      {
         //Microsoft.Office.Interop.Word.ApplicationClass  Word = null;
         //Microsoft.Office.Interop.Word.Document          Doc = null;

         
         //foreach ( string FileName in FileNames )
         //{
         //   try 
         //   {
         //      Word = new Microsoft.Office.Interop.Word.ApplicationClass();
         //      //Doc =  Word.Documents.Open ( ref FileName , ref ;
         //      string FilePath      = Path.GetDirectoryName(FileName) + @"\";
         //      string BaseFileName  = Path.GetFileNameWithoutExtension(FileName);
         //      string OutFileName = FilePath + BaseFileName + ".ps";
         //   }
         //   catch (Exception e )
         //   {
         //      Debug.WriteLine ( e.ToString() );
         
         //   }
         //   finally 
         //   {
         //      if ( Doc != null )
         //      {
         //         Doc.Save();            
         //      }
         //   }
         //}
      }

      private void btnMultipleFiles_Click(object sender, System.EventArgs e)
      {

         MultipleFileFrm Frm = new MultipleFileFrm();
         if (Frm.ShowDialog() == DialogResult.OK)
         {
            this.cbMultiple.Checked = true;
         }
         else
         {
            this.cbMultiple.Checked = false;
         }
      }
   }

   public enum EncodingProfileEnum
   {
      LOW   ,
      HIGH  ,
      LOSSLESS
   }
   public class EncoderInfo
   {
      public string Author;
      public string MessageTitle;
      public string Description;
      public string FileNameAndPath;
      public string FilePath;
      public string FileName;
      public string FileNameNoExtension;
   }

}
