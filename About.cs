//*********************************************************************
//																	 //
//	SQL Power Injector 1.1.1 Copyright (c) 2006 Francois Larouche	 //
//																	 //
//  Author	: francois.larouche@sqlpowerinjector.com				 //
//	Web Site: www.sqlpowerinjector.com								 //
//																	 //
//*******************************************************************//
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SQLPowerInjector
{
	/// <summary>
	/// Summary description for About.
	/// </summary>
	public class About : Form
	{
		private System.Windows.Forms.Button butExitAbout;
		private System.Windows.Forms.Label lblVersion;
		private System.Windows.Forms.PictureBox pbLogo;
		private System.Windows.Forms.RichTextBox rtbLicense;
		private System.Windows.Forms.Label lblCopyright;
		private System.Windows.Forms.Label lblContact;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region Constructor
		public About()
		{
			InitializeComponent();
		}
		#endregion

		#region Windows Form Designer generated code
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(About));
			this.butExitAbout = new System.Windows.Forms.Button();
			this.lblVersion = new System.Windows.Forms.Label();
			this.pbLogo = new System.Windows.Forms.PictureBox();
			this.rtbLicense = new System.Windows.Forms.RichTextBox();
			this.lblCopyright = new System.Windows.Forms.Label();
			this.lblContact = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butExitAbout
			// 
			this.butExitAbout.BackColor = System.Drawing.Color.LightSlateGray;
			this.butExitAbout.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butExitAbout.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.butExitAbout.ForeColor = System.Drawing.Color.White;
			this.butExitAbout.Location = new System.Drawing.Point(312, 344);
			this.butExitAbout.Name = "butExitAbout";
			this.butExitAbout.Size = new System.Drawing.Size(128, 24);
			this.butExitAbout.TabIndex = 20;
			this.butExitAbout.Text = "OK";
			this.butExitAbout.Click += new System.EventHandler(this.butExitAbout_Click);
			// 
			// lblVersion
			// 
			this.lblVersion.BackColor = System.Drawing.Color.Transparent;
			this.lblVersion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.lblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblVersion.ForeColor = System.Drawing.Color.White;
			this.lblVersion.Location = new System.Drawing.Point(152, 8);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(264, 23);
			this.lblVersion.TabIndex = 24;
			this.lblVersion.Text = "SQL Power Injector ver 1.1.1";
			// 
			// pbLogo
			// 
			this.pbLogo.BackColor = System.Drawing.Color.Transparent;
			this.pbLogo.Image = ((System.Drawing.Image)(resources.GetObject("pbLogo.Image")));
			this.pbLogo.Location = new System.Drawing.Point(8, 8);
			this.pbLogo.Name = "pbLogo";
			this.pbLogo.Size = new System.Drawing.Size(134, 80);
			this.pbLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pbLogo.TabIndex = 25;
			this.pbLogo.TabStop = false;
			// 
			// rtbLicense
			// 
			this.rtbLicense.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.rtbLicense.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.rtbLicense.Location = new System.Drawing.Point(8, 104);
			this.rtbLicense.Name = "rtbLicense";
			this.rtbLicense.ReadOnly = true;
			this.rtbLicense.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.rtbLicense.Size = new System.Drawing.Size(432, 208);
			this.rtbLicense.TabIndex = 26;
			this.rtbLicense.Text = @"DISCLAIMER You should only use this software to test the security of your own web applications or those you are authorized to do so. I, Francois Larouche, will take no responsibility for any problems or unfortunate consequences brought about by the use of SQL Power Injector.

This program is free software; you can redistribute it and/or modify it under the terms of the Clarified Artistic License as published in the Free Software Foundation. See the Clarified Artistic License for more details.

THIS PACKAGE IS PROVIDED ""AS IS"" AND WITHOUT ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, WITHOUT LIMITATION, THE IMPLIED WARRANTIES OF MERCHANTIBILITY AND FITNESS FOR A PARTICULAR PURPOSE.";
			// 
			// lblCopyright
			// 
			this.lblCopyright.BackColor = System.Drawing.Color.Transparent;
			this.lblCopyright.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.lblCopyright.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblCopyright.ForeColor = System.Drawing.Color.White;
			this.lblCopyright.Location = new System.Drawing.Point(152, 32);
			this.lblCopyright.Name = "lblCopyright";
			this.lblCopyright.Size = new System.Drawing.Size(280, 23);
			this.lblCopyright.TabIndex = 27;
			this.lblCopyright.Text = "Copyright (c) 2006 Francois Larouche";
			// 
			// lblContact
			// 
			this.lblContact.BackColor = System.Drawing.Color.Transparent;
			this.lblContact.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.lblContact.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblContact.ForeColor = System.Drawing.Color.White;
			this.lblContact.Location = new System.Drawing.Point(152, 56);
			this.lblContact.Name = "lblContact";
			this.lblContact.Size = new System.Drawing.Size(296, 40);
			this.lblContact.TabIndex = 29;
			this.lblContact.Text = "Contact: francois.larouche@sqlpowerinjector.com Website: www.sqlpowerinjector.com" +
				"";
			// 
			// About
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.RoyalBlue;
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = new System.Drawing.Size(446, 380);
			this.ControlBox = false;
			this.Controls.Add(this.lblContact);
			this.Controls.Add(this.lblCopyright);
			this.Controls.Add(this.rtbLicense);
			this.Controls.Add(this.pbLogo);
			this.Controls.Add(this.lblVersion);
			this.Controls.Add(this.butExitAbout);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "About";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "About";
			this.TopMost = true;
			this.ResumeLayout(false);

		}
		#endregion
		#endregion

		private void butExitAbout_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}
	}
}
