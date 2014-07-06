using System;
using System.Text;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MediaEncoder
{
	/// <summary>
	/// Summary description for AboutForm.
	/// </summary>
	public class AboutForm : System.Windows.Forms.Form
	{
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Button btnClose;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.TextBox tbInsructions;
      private System.Windows.Forms.Label label3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AboutForm()
		{
			InitializeComponent();
            // Test to see if I can upload to GitHub
         StringBuilder s = new StringBuilder();
         s.Append ( "This media encoder helper application is designed to \r\n");
         s.Append ( "take an audio message and generate three encoded output \r\n");
         s.Append ( "files. The names of the output files will be the same base \r\n");
         s.Append ( "name as the input file. The low quality ( high compression) \r\n");
         s.Append ( "file will be the same as the input name with a .wmv extension \r\n");
         s.Append ( "The higher quality file will be the same as the base name \r\n");
         s.Append ( "with a _high appended. The lossless file compression will \r\n");
         s.Append ( "be the same as the base with a _ll appended. \r\n");
         s.Append ( "Three media profiles need to be in the Windows Media profile \r\n");
         s.Append ( "directory. These are \r\n");
         s.Append ( "VBC_LowAudioQuality.prx \r\n");
         s.Append ( "VBC_HighAudioQuality.prx \r\n");
         s.Append ( "VBC_Lossless.prx \r\n");
         s.Append ( "The output files will be placed in the same folder as the \r\n");
         s.Append ( "input file. Enter the information desired on the input form \r\n");
         s.Append ( "and when you tab to the file name, the file open box will appear\r\n");
         s.Append ( "When all the information is ready, click encode! When the \r\n");
         s.Append ( "Message Finshed encodeing LOSSLESS is display, you are done!\r\n");
         this.tbInsructions.Text = s.ToString();
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
         this.label1 = new System.Windows.Forms.Label();
         this.btnClose = new System.Windows.Forms.Button();
         this.label2 = new System.Windows.Forms.Label();
         this.tbInsructions = new System.Windows.Forms.TextBox();
         this.label3 = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // label1
         // 
         this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
         this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaption;
         this.label1.Location = new System.Drawing.Point(96, 64);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(264, 40);
         this.label1.TabIndex = 0;
         this.label1.Text = "Windows Media Encoder Helper Utility for Audio Messages.";
         // 
         // btnClose
         // 
         this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnClose.Location = new System.Drawing.Point(376, 72);
         this.btnClose.Name = "btnClose";
         this.btnClose.TabIndex = 1;
         this.btnClose.Text = "&Close";
         this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
         // 
         // label2
         // 
         this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
         this.label2.ForeColor = System.Drawing.SystemColors.ActiveCaption;
         this.label2.Location = new System.Drawing.Point(128, 120);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(176, 24);
         this.label2.TabIndex = 2;
         this.label2.Text = " Rev 1.2 Oct 28 2005";
         // 
         // tbInsructions
         // 
         this.tbInsructions.Location = new System.Drawing.Point(48, 192);
         this.tbInsructions.Multiline = true;
         this.tbInsructions.Name = "tbInsructions";
         this.tbInsructions.Size = new System.Drawing.Size(424, 264);
         this.tbInsructions.TabIndex = 3;
         this.tbInsructions.Text = "textBox1";
         // 
         // label3
         // 
         this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
         this.label3.Location = new System.Drawing.Point(48, 160);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(216, 23);
         this.label3.TabIndex = 4;
         this.label3.Text = "Instructions for Use";
         // 
         // AboutForm
         // 
         this.AcceptButton = this.btnClose;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnClose;
         this.ClientSize = new System.Drawing.Size(552, 582);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.tbInsructions);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.btnClose);
         this.Controls.Add(this.label1);
         this.Name = "AboutForm";
         this.Text = "Windows Media Automation About Information";
         this.ResumeLayout(false);

      }
		#endregion

      private void btnClose_Click(object sender, System.EventArgs e)
      {
         this.Close();
      }
	}
}
