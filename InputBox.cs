//*********************************************************************
//																	 //
//	SQL Power Injector 1.1.1 Copyright (c) 2006 Francois Larouche	 //
//																	 //
//  Author	: francois.larouche@sqlpowerinjector.com				 //
//	Web Site: www.sqlpowerinjector.com								 //
//																	 //
//*******************************************************************//
using System;
using System.Windows.Forms;

namespace SQLPowerInjector
{
	/// <summary>
	/// Summary description for InputBox.
	/// </summary>
	internal class InputBox : Form
	{
		#region All variables
		#region Non system variables
		private static InputBox _curBox;
		private static string _inputText = "";
		private static string _title;
		private static string _question;
		private static int _labelHeight;
		private static int _textboxTop;
		private static int _butOkTop;
		private static int _butCancelTop;
		private static int _inputBoxHeight;

		public static string Title
		{
			get { return _title; }
			set { _title = value; }
		}
	
		internal static string Question
		{
			get { return _question; }
			set { _question = value; }
		}

		public static int LabelHeight
		{
			get { return _labelHeight; }
			set { _labelHeight = value; }
		}

		public static int TextboxTop
		{
			get { return _textboxTop; }
		}

		public static int ButOkTop
		{
			get { return _butOkTop; }
		}

		public static int ButCancelTop
		{
			get { return _butCancelTop; }
		}

		public static int InputBoxHeight
		{
			get { return _inputBoxHeight; }
		}
		#endregion
		#region Form variables
		private TextBox txtInput;
		private System.Windows.Forms.Label lblQuestion;
		private System.Windows.Forms.Button butCancel;
		private System.Windows.Forms.Button butOK;
		private System.ComponentModel.Container components = null;
		#endregion
		#endregion

		#region Constructor
		private InputBox()
		{
			InitializeComponent();
			_title = (_title != "" && _title != null) ? _title : "InputBox";
			_question = (_question != "" && _question != null) ? _question : "Please insert your input";
			_labelHeight = (_labelHeight >= this.lblQuestion.Height) ? _labelHeight : this.lblQuestion.Height;
			_textboxTop = txtInput.Location.Y;
			_butOkTop = butOK.Location.Y;
			_butCancelTop = butCancel.Location.Y;
			_inputBoxHeight = this.Height;
		}

		private InputBox(string boxTitle, string boxQuestion)
		{
			InitializeComponent();
			_title = boxTitle;
			_question = boxQuestion;
		}

		// Automatically will add the difference between the default labelheight to the other parameters
		private InputBox(string boxTitle, string boxQuestion, int labelHeight)
		{
			int difLabelHeight = 0;

			InitializeComponent();
			_title = boxTitle;
			_question = boxQuestion;
			_labelHeight = labelHeight;

			// If it's a negative number we make it positive
			labelHeight = (labelHeight < 0) ? 0 : labelHeight;
			difLabelHeight = labelHeight - lblQuestion.Height;

			difLabelHeight = (difLabelHeight < 0) ? 0 : difLabelHeight;
			_textboxTop = txtInput.Location.Y + difLabelHeight;
			_butOkTop = butOK.Location.Y + difLabelHeight;
			_butCancelTop = butCancel.Location.Y + difLabelHeight;
			_inputBoxHeight = this.Height + Convert.ToInt32(difLabelHeight * 1.5);
		}
//		Not implemented yet...
//		private InputBox(string boxTitle, string boxQuestion, int labelHeight, int textboxTop, int butOkTop, int butCancelTop, int inputBoxHeight)
//		{
//			InitializeComponent();
//			_title = boxTitle;
//			_question = boxQuestion;
//			_labelHeight = (lblQuestion.Height - labelHeight);
//			_textboxTop = textboxTop;
//			_butOkTop = butOkTop;
//			_butCancelTop = butCancelTop;
//			_inputBoxHeight = inputBoxHeight;
//		}

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

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(InputBox));
			this.txtInput = new System.Windows.Forms.TextBox();
			this.lblQuestion = new System.Windows.Forms.Label();
			this.butCancel = new System.Windows.Forms.Button();
			this.butOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtInput
			// 
			this.txtInput.Location = new System.Drawing.Point(8, 64);
			this.txtInput.Name = "txtInput";
			this.txtInput.Size = new System.Drawing.Size(456, 20);
			this.txtInput.TabIndex = 0;
			this.txtInput.Text = "";
			// 
			// lblQuestion
			// 
			this.lblQuestion.BackColor = System.Drawing.Color.Transparent;
			this.lblQuestion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblQuestion.ForeColor = System.Drawing.Color.White;
			this.lblQuestion.Location = new System.Drawing.Point(16, 8);
			this.lblQuestion.Name = "lblQuestion";
			this.lblQuestion.Size = new System.Drawing.Size(448, 48);
			this.lblQuestion.TabIndex = 19;
			// 
			// butCancel
			// 
			this.butCancel.BackColor = System.Drawing.Color.LightSlateGray;
			this.butCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.butCancel.ForeColor = System.Drawing.Color.White;
			this.butCancel.Location = new System.Drawing.Point(240, 96);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(128, 24);
			this.butCancel.TabIndex = 23;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.BackColor = System.Drawing.Color.LightSlateGray;
			this.butOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.butOK.ForeColor = System.Drawing.Color.White;
			this.butOK.Location = new System.Drawing.Point(104, 96);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(128, 24);
			this.butOK.TabIndex = 22;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// InputBox
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.RoyalBlue;
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = new System.Drawing.Size(482, 128);
			this.ControlBox = false;
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.lblQuestion);
			this.Controls.Add(this.txtInput);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "InputBox";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "InputBox";
			this.ResumeLayout(false);

		}
		#endregion

		// If someone prefers to use the default values or has previously changed  
		// some value directly with the parameters
		public static string ShowInputBox()
		{
			_curBox = new InputBox();
			CreateInputBox();
			_curBox.ShowDialog();

			return _inputText;
		}   
 
		public static string ShowInputBox(string boxTitle, string boxQuestion)
		{
			_curBox = new InputBox(boxTitle, boxQuestion);
			CreateInputBox();
			_curBox.ShowDialog();

			return _inputText;
		}

		public static string ShowInputBox(string boxTitle, string boxQuestion, int labelHeight)
		{
			_curBox = new InputBox(boxTitle, boxQuestion, labelHeight);
			CreateInputBox();
			_curBox.ShowDialog();

			return _inputText;
		}

		private static void CreateInputBox()
		{
			_curBox.Text = _title;
			_curBox.lblQuestion.Text = _question;
			_curBox.lblQuestion.Height = _labelHeight;
			_curBox.txtInput.Location = new System.Drawing.Point(_curBox.txtInput.Location.X, _textboxTop);
			_curBox.butOK.Location = new System.Drawing.Point(_curBox.butOK.Location.X, _butOkTop);
			_curBox.butCancel.Location = new System.Drawing.Point(_curBox.butCancel.Location.X, _butCancelTop);
			_curBox.Height = _inputBoxHeight;

		}

		private void butOK_Click(object sender, System.EventArgs e)
		{
			_inputText = this.txtInput.Text;
			this.Close();
		}

		private void butCancel_Click(object sender, System.EventArgs e)
		{
			_inputText = "!!@@!!@@!!@@!!";
			this.Close();
		}
	}
}
