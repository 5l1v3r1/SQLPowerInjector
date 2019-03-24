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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using mshtml;
using SQLPowerInjector.DatabaseString;

namespace SQLPowerInjector
{
	public class Injector : System.Windows.Forms.Form
	{
		#region All variables
		#region Non system variables
		private Thread[] _createdThreads;
		private int _numberThreadsFinished = 0;
		private ArrayList _arrStatus;
		private ArrayList[] _arrInputModifiable;
		private ArrayList[] _arrInputNotModifiable;
		private bool _hasFinishedLoad = false;
		private string _loadedSetCookie;
		private string _loadedCookie;
		private string _oldTooltipValue = "";
		private string[] _wordByThread;
		private string _charSet = "";
		private string _initialUri = "";
		private byte _threadsStop = (byte)enumThreadsState.Started;
		DateTime _begTime = DateTime.Now;
		DateTime _endTime;
		TimeSpan _span = new TimeSpan(0);
		#endregion
		#region Form variables
		private System.Windows.Forms.Label lblCurrent;
		private System.Windows.Forms.TextBox txtWord;
		private System.Windows.Forms.Label lblLength;
		private System.Windows.Forms.Label lblWord;
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.GroupBox grbParameters;
		private System.Windows.Forms.TextBox txtLength;
		private System.Windows.Forms.TextBox txtPositiveAnswer;
		private System.Windows.Forms.Label lblPositiveAnswer;
		private System.Windows.Forms.GroupBox grbResults;
		private System.Windows.Forms.Button butClearResults;
		private System.Windows.Forms.Button butPause;
		private System.Windows.Forms.GroupBox grbStatus;
		private MyDataGrid dtgCurrentState;
		private System.Windows.Forms.RadioButton radCount;
		private System.Windows.Forms.RadioButton radLength;
		private System.Windows.Forms.GroupBox grbType;
		private System.Windows.Forms.GroupBox grbMethod;
		private System.Windows.Forms.RadioButton radGet;
		private System.Windows.Forms.RadioButton radPost;
		private System.Windows.Forms.TextBox txtURL;
		private System.Windows.Forms.Label lblURL;
		private System.Windows.Forms.Button butLoadPage;
		private AxSHDocVw.AxWebBrowser axwbHtmlResult;
		private System.Windows.Forms.Label lblHTMLMsgLength;
		private System.Windows.Forms.NumericUpDown nudHtmlMsgLength;
		private SQLPowerInjector.MyDataGrid dtgLoadedInput;
		private System.Windows.Forms.Button butStartStop;
		private System.Windows.Forms.GroupBox grbTechnique;
		private System.Windows.Forms.RadioButton radBlind;
		private System.Windows.Forms.RadioButton radNormal;
		private System.Windows.Forms.Label lblSubmitURL;
		private System.Windows.Forms.CheckBox ckbDelay;
		private System.Windows.Forms.Label lblDelaySeconds;
		private System.Windows.Forms.NumericUpDown nudDelaySeconds;
		private System.Windows.Forms.GroupBox grbDBType;
		private System.Windows.Forms.TextBox txtCurrentChar;
		private System.Windows.Forms.Label lblCurrentChar;
		private System.Windows.Forms.Label lblStringParameters;
		private System.Windows.Forms.Label lblStartingCount;
		private System.Windows.Forms.NumericUpDown nudStartingCount;
		private System.Windows.Forms.CheckBox chkDistinct;
		private System.Windows.Forms.NumericUpDown nudStartingLength;
		private System.Windows.Forms.Label lblStartingLength;
		private System.Windows.Forms.TabControl tabHTML;
		private System.Windows.Forms.TabPage tabpgHTML;
		private System.Windows.Forms.TabPage tabpgSource;
		private System.Windows.Forms.RichTextBox rtxtViewSource;
		private System.Windows.Forms.TextBox txtSQLErrorString;
		private System.Windows.Forms.Label lblSQLErrorString;
		private System.Windows.Forms.CheckBox chkTrapErrorString;
		private System.Windows.Forms.NumericUpDown nudNbrThread;
		private System.Windows.Forms.Label lblNbrThread;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem mnFile;
		private System.Windows.Forms.MenuItem mnQuestion;
		private System.Windows.Forms.MenuItem mnAbout;
		private System.Windows.Forms.Label lblTimeTaken;
		private System.Windows.Forms.TextBox txtTimeTaken;
		private System.Windows.Forms.MenuItem mnExit;
		private System.Windows.Forms.CheckBox chkInsertEmptyComments;
		private System.Windows.Forms.RadioButton radWord;
		private System.Windows.Forms.MenuItem mnInsert;
		private System.Windows.Forms.MenuItem mnInsertCookie;
		private System.Windows.Forms.ComboBox cmbDBType;
		private System.Windows.Forms.MenuItem mnUseCookie;
		private System.Windows.Forms.MenuItem mnUse;
		private System.Windows.Forms.MenuItem mnLine1;
		private System.Windows.Forms.MenuItem mnTutorial;
		private System.Windows.Forms.MenuItem mnSite;
		private AxSHDocVw.AxWebBrowser axwbHtmlInitializor;
		private System.Windows.Forms.ComboBox cmbSubmitURL;
		private DataGridTableStyle tableStyle;
		private System.Windows.Forms.MenuItem mnSaveSession;
		private System.Windows.Forms.MenuItem mnLoadSession;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.SaveFileDialog sfdSaveSession;
		private System.Windows.Forms.OpenFileDialog ofdLoadSession;
		private System.Windows.Forms.MenuItem mnCharacterSetAutoDetect;
		private System.Windows.Forms.MenuItem mnCheckForUpdates;
		private System.Windows.Forms.ProgressBar pbProcessing;
		private System.Windows.Forms.Label lblProcessing;
		#endregion
		#endregion

		#region Constants
		private const byte FIRST_DATAGRID_INDEX_ID = 0;
		private const byte CHECKED_COLUMN_INPUT = 0;
		private const int XML_ERROR_FORMAT = -2146233079;
		#endregion
		#region Enums
		private enum enumTypeCommand 
		{
			Count = 1,
			Length,
			Word,
			Normal
		}
		
		private enum enumThreadsState
		{
			Stoped = 1,
			Started,
			Paused
		}
		#endregion

		#region Constructor
		public Injector()
		{
			InitializeComponent();
		}
		#endregion

		#region Main
		[STAThread]
		static void Main() 
		{
			Application.Run(new Injector());
		}
		#endregion

		private delegate void myDelegate(string aString1, string aString2);
		private delegate void myThreadDelegate();

		private void SQLInjection()
		{
			try
			{
				int stringLength = 0;

				if(radBlind.Checked)
				{
					if(radWord.Checked)
					{
						stringLength = GetStringLength();
						txtLength.Text = Convert.ToString(stringLength);
						txtWord.Text = txtWord.Text.PadLeft(stringLength, '?');
		
						if(_threadsStop != (byte)enumThreadsState.Stoped)
						{
							if(stringLength > 0)
							{
								if(stringLength < nudNbrThread.Value)
									nudNbrThread.Value = stringLength;

								LaunchGetWordThread();
							}
							else
							{
								butPause.Enabled = false;
								nudNbrThread.Enabled = true;
								butStartStop.Text = "&Start";
								this.Cursor = Cursors.Default;
							}
						}
						else
						{
							butPause.Enabled = false;
							nudNbrThread.Enabled = true;
							butStartStop.Text = "&Start";
							this.Cursor = Cursors.Default;
						}
					}
					else if(radCount.Checked)
					{
						stringLength = GetStringCount();
						txtLength.Text = Convert.ToString(stringLength);
						butPause.Enabled = false;
						nudNbrThread.Enabled = true;
						butStartStop.Text = "&Start";
						this.Cursor = Cursors.Default;
					}
					else
					{
						stringLength = GetStringLength();
						txtLength.Text = Convert.ToString(stringLength);
						txtWord.Text = txtWord.Text.PadLeft(stringLength, '?');
						butPause.Enabled = false;
						nudNbrThread.Enabled = true;
						butStartStop.Text = "&Start";
						this.Cursor = Cursors.Default;
					}
				}
				else
				{
					GetSingleResult();
					butPause.Enabled = false;
					nudNbrThread.Enabled = true;
					butStartStop.Text = "&Start";
					this.Cursor = Cursors.Default;
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				butStartStop.Enabled = true;
			}
		}

		private int GetStringLength()
		{
			bool isFound = false;
			bool SQLTrue = false;
			bool isConnectionDead = false;

			int upperNbr = Convert.ToInt32(nudStartingLength.Value);
			int currentNbr = upperNbr;
			int lowerNbr = 0;
			int stringLength = 0;

			string requestData = "";
			string msgReturn = "";
			string previousMsgReturn = "";
			string sqlMsg = "";

			try
			{
				while(isFound != true && currentNbr != 0 && _threadsStop != (byte)enumThreadsState.Stoped)
				{
					while(_threadsStop == (byte)enumThreadsState.Paused)
						Application.DoEvents();

					// In case that the stop has been pressed right after the pause
					if(_threadsStop == (byte)enumThreadsState.Stoped)
						break;

					if((upperNbr - currentNbr) == 1)
					{
						for(int j=currentNbr-1; j<upperNbr && _threadsStop != (byte)enumThreadsState.Stoped; j++)
						{
							while(_threadsStop == (byte)enumThreadsState.Paused)
								Application.DoEvents();

							// In case that the stop has been pressed right after the pause
							if(_threadsStop == (byte)enumThreadsState.Stoped)
								break;

							txtCurrentChar.Text = Convert.ToString(j);

							requestData = GetPostData(enumTypeCommand.Length, "=", j, 0);
							isFound = RequestURL(requestData, txtPositiveAnswer.Text, out msgReturn, out sqlMsg);
							
							if(msgReturn.ToUpper().StartsWith("CONNECTION ERROR - "))
							{
								isConnectionDead = true;
								break;
							}

							if(isFound) 
							{
								stringLength = j;
								break;
							}						
						}
					}
					else
					{
						txtCurrentChar.Text = Convert.ToString(currentNbr);
						previousMsgReturn = msgReturn;

						requestData = GetPostData(enumTypeCommand.Length, ">", currentNbr, 0);
						SQLTrue = RequestURL(requestData, txtPositiveAnswer.Text, out msgReturn, out sqlMsg);
						
						if(msgReturn.ToUpper().StartsWith("CONNECTION ERROR - "))
							isConnectionDead = true;
					}

					while(_threadsStop == (byte)enumThreadsState.Paused)
						Application.DoEvents();

					// In case that the stop has been pressed right after the pause or if the connection is dead
					if(_threadsStop == (byte)enumThreadsState.Stoped || isConnectionDead == true)
						break;

					Invoke(new myDelegate(FillDataGrid), new Object[] {msgReturn, sqlMsg});
					FillHtmlContent(sqlMsg);

					// Check if the message is the same than the previous one, if so it means there is something
					// wrong so stop it
					if(previousMsgReturn == msgReturn || (previousMsgReturn == "" && msgReturn == ""))
					{
						MessageBox.Show("There is a problem with your injection!\n\nHere are the possible reasons of the problem:\n\n" +
							"   1. Length number lower than the real value. Try to raise the length value to a greater number\n" +
							"   2. Positive string answer doesn't find a match. Make sure your positive string exists.\n" +
							"       To do so you need to try it first with the option \"Normal\", not the \"Blind\"\n" +
							"   3. There is no row checked with the three parts injection. Make sure there is one checked\n" +
							"        before starting the blind injection\n" +
							"   4. The DBMS selected is not the right one. Try another and click start again\n" +
							"   5. The SQL injection string might contain some mistakes in it. Try to make it works with\n" +
							"        the option \"Normal\" first" + (cmbDBType.SelectedIndex==1 ? 
							"\n   6. You might have forgotten to add the FROM DUAL part in the ending string portion" : ""),
							"Error of injection with Length or Word option", MessageBoxButtons.OK, MessageBoxIcon.Error);
						butPause.Enabled = false;
						nudNbrThread.Enabled = true;
						butStartStop.Text = "&Start";
						this.Cursor = Cursors.Default;
						break;
					}

					if(SQLTrue)
					{
						upperNbr = currentNbr;
						currentNbr = ((currentNbr - lowerNbr) / 2) + lowerNbr;
					}
					else if(!isFound)
					{
						lowerNbr = currentNbr;
						currentNbr = ((upperNbr - currentNbr) / 2) + currentNbr;
					}
				}

				return stringLength;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name + 
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);

				return stringLength;
			}
		}

		private int GetStringCount()
		{
			bool isFound = false;
			bool SQLTrue = false;
			bool isConnectionDead = false;

			int upperNbr = Convert.ToInt32(nudStartingCount.Value);
			int currentNbr = upperNbr;
			int lowerNbr = 0;
			int stringLength = 0;

			string requestData = "";
			string msgReturn = "";
			string previousMsgReturn = "";
			string sqlMsg = "";

			try
			{
				while(isFound != true && currentNbr != 0 && _threadsStop != (byte)enumThreadsState.Stoped)
				{
					while(_threadsStop == (byte)enumThreadsState.Paused)
						Application.DoEvents();

					// In case that the stop has been pressed right after the pause
					if(_threadsStop == (byte)enumThreadsState.Stoped)
						break;

					if((upperNbr - currentNbr) == 1)
					{
						for(int j=currentNbr-1; j<upperNbr && _threadsStop != (byte)enumThreadsState.Stoped; j++)
						{
							while(_threadsStop == (byte)enumThreadsState.Paused)
								Application.DoEvents();

							// In case that the stop has been pressed right after the pause
							if(_threadsStop == (byte)enumThreadsState.Stoped)
								break;

							txtCurrentChar.Text = Convert.ToString(j);

							requestData = GetPostData(enumTypeCommand.Count, "=", j, 0);
							isFound = RequestURL(requestData, txtPositiveAnswer.Text, out msgReturn, out sqlMsg);
							
							if(msgReturn.ToUpper().StartsWith("CONNECTION ERROR - "))
							{
								isConnectionDead = true;
								break;
							}
							if(isFound) 
							{
								stringLength = j;
								break;
							}						
						}
					}
					else
					{
						txtCurrentChar.Text = Convert.ToString(currentNbr);
						previousMsgReturn = msgReturn;

						requestData = GetPostData(enumTypeCommand.Count, ">", currentNbr, 0);
						SQLTrue = RequestURL(requestData, txtPositiveAnswer.Text, out msgReturn, out sqlMsg);
						
						if(msgReturn.ToUpper().StartsWith("CONNECTION ERROR - "))
							isConnectionDead = true;
					}

					while(_threadsStop == (byte)enumThreadsState.Paused)
						Application.DoEvents();

					// In case that the stop has been pressed right after the pause or if the connection is dead
					if(_threadsStop == (byte)enumThreadsState.Stoped || isConnectionDead)
						break;

					Invoke(new myDelegate(FillDataGrid), new Object[] {msgReturn, sqlMsg});
					FillHtmlContent(sqlMsg);

					// Check if the message is the same than the previous one, if so it means there is something
					// wrong so stop it
					if(previousMsgReturn == msgReturn || (previousMsgReturn == "" && msgReturn == ""))
					{
						MessageBox.Show("There is a problem with your injection!\n\nHere are the possible reasons of the problem:\n\n" +
							"   1. Count number lower than the real value. Try to raise the count value to a greater number\n" +
							"   2. Positive string answer doesn't find a match. Make sure your positive string exists.\n" +
							"       To do so you need to try it first with the option \"Normal\", not the \"Blind\"\n" +
							"   3. There is no row checked with the three parts injection. Make sure there is one checked\n" +
							"       before starting the blind injection\n" +
							"   4. The DBMS selected is not the right one. Try another and click start again\n" +
							"   5. The SQL injection string might contain some mistakes in it. Try to make it works with\n" +
							"        the option \"Normal\" first" + (cmbDBType.SelectedIndex==1 ? 
							"\n   6. You might have forgotten to add the FROM DUAL part in the ending string portion" : ""),
							"Error of injection with Count option", MessageBoxButtons.OK, MessageBoxIcon.Error);
						butPause.Enabled = false;
						nudNbrThread.Enabled = true;
						butStartStop.Text = "&Start";
						this.Cursor = Cursors.Default;
						break;
					}

					if(SQLTrue)
					{
						upperNbr = currentNbr;
						currentNbr = ((currentNbr - lowerNbr) / 2) + lowerNbr;
					}
					else if(!isFound)
					{
						lowerNbr = currentNbr;
						currentNbr = ((upperNbr - currentNbr) / 2) + currentNbr;
					}
				}

				return stringLength;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);

				return stringLength;
			}
		}

		private void LaunchGetWordThread()
		{
			int numberThreadMax = Convert.ToInt32(nudNbrThread.Value);

			_createdThreads = new Thread[numberThreadMax];
			_wordByThread = new String[Convert.ToInt32(txtLength.Text)];
			pbProcessing.Maximum = Convert.ToInt32(txtLength.Text);

			try
			{
				for(int curThread=0; curThread<numberThreadMax; curThread++)
				{
					switch(curThread)
					{
						case 0: _createdThreads[curThread] = new Thread(new ThreadStart(GetWordWithOneThread));
							break;

						case 1: _createdThreads[curThread] = new Thread(new ThreadStart(GetWordWithTwoThread));
							break;

						case 2:	_createdThreads[curThread] = new Thread(new ThreadStart(GetWordWithThreeThread));
							break;

						case 3: _createdThreads[curThread] = new Thread(new ThreadStart(GetWordWithFourThread));
							break;

						case 4:	_createdThreads[curThread] = new Thread(new ThreadStart(GetWordWithFiveThread));
							break;

						case 5:	_createdThreads[curThread] = new Thread(new ThreadStart(GetWordWithSixThread));
							break;

						case 6:	_createdThreads[curThread] = new Thread(new ThreadStart(GetWordWithSevenThread));
							break;

						case 7:	_createdThreads[curThread] = new Thread(new ThreadStart(GetWordWithEightThread));
							break;
					}

					_createdThreads[curThread].IsBackground = true;
					_createdThreads[curThread].Start();
					Debug.Write(Convert.ToString(curThread) + ":" + Convert.ToString(_createdThreads[curThread].IsBackground) + "\n");
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void GetWordWithOneThread()
		{
			try
			{
				GenerateWordIndexes(1);
				Invoke(new myThreadDelegate(CheckEndingThreadsState), null);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void GetWordWithTwoThread()
		{
			try
			{
				GenerateWordIndexes(2);
				Invoke(new myThreadDelegate(CheckEndingThreadsState), null);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void GetWordWithThreeThread()
		{
			try
			{
				GenerateWordIndexes(3);
				Invoke(new myThreadDelegate(CheckEndingThreadsState), null);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void GetWordWithFourThread()
		{
			try
			{
				GenerateWordIndexes(4);
				Invoke(new myThreadDelegate(CheckEndingThreadsState), null);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void GetWordWithFiveThread()
		{
			try
			{
				GenerateWordIndexes(5);
				Invoke(new myThreadDelegate(CheckEndingThreadsState), null);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void GetWordWithSixThread()
		{
			try
			{
				GenerateWordIndexes(6);
				Invoke(new myThreadDelegate(CheckEndingThreadsState), null);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void GetWordWithSevenThread()
		{
			try
			{
				GenerateWordIndexes(7);
				Invoke(new myThreadDelegate(CheckEndingThreadsState), null);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void GetWordWithEightThread()
		{
			try
			{
				GenerateWordIndexes(8);
				Invoke(new myThreadDelegate(CheckEndingThreadsState), null);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void GenerateWordIndexes(int threadNumber)
		{
			int wordLenght = Convert.ToInt32(txtLength.Text);
			int totalNbrThreads = Convert.ToInt32(nudNbrThread.Value);
			double sizeToGet = 1.0;
			int startingIndex = 0;
			int endingIndex = 0;
			
			try
			{
				sizeToGet = Convert.ToDouble(wordLenght) / Convert.ToDouble(totalNbrThreads);
				sizeToGet = Math.Round(sizeToGet);
				sizeToGet = sizeToGet < 1 ? 1 : sizeToGet;
				startingIndex = (Convert.ToInt32(sizeToGet) * (threadNumber - 1)) + 1;

				if(totalNbrThreads == threadNumber)
					endingIndex = wordLenght;
				else if(totalNbrThreads >= threadNumber + 1)
					endingIndex = startingIndex + Convert.ToInt32(sizeToGet) - 1;

				if(startingIndex > wordLenght)
					startingIndex = wordLenght;

				if(endingIndex > wordLenght)
					endingIndex = wordLenght;

				GetWord(startingIndex, endingIndex, Convert.ToString(threadNumber));
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void GetWord(int startingIndexString, int endingIndexString, string currentThread)
		{
			_begTime = DateTime.Now;

			bool isFound = false;
			bool SQLTrue = false;

			int lengthString = endingIndexString;
			int upperNbr = 255;
			int currentNbr = upperNbr;
			int lowerNbr = 0;

			string requestData = "";
			string msgReturn = "";
			string sqlMsg = "";

			try
			{
				for(int i=startingIndexString;i<=lengthString && _threadsStop != (byte)enumThreadsState.Stoped;i++)
				{
					while(_threadsStop == (byte)enumThreadsState.Paused)
						Application.DoEvents();

					isFound = false;
					SQLTrue = false;
					currentNbr = 255;
					upperNbr = 255;
					lowerNbr = 0;

					while(isFound != true && currentNbr != 0 && _threadsStop != (byte)enumThreadsState.Stoped)
					{
						while(_threadsStop == (byte)enumThreadsState.Paused)
							Application.DoEvents();

						// In case that the stop has been pressed right after the pause
						if(_threadsStop == (byte)enumThreadsState.Stoped)
							break;

						if((upperNbr - currentNbr) == 1)
						{
							for(int j=currentNbr-1; j<upperNbr; j++)
							{
								txtCurrentChar.Text = Utilities.Char(j);
								
								requestData = GetPostData(enumTypeCommand.Word, "=", j, i);
								isFound = RequestURL(requestData, txtPositiveAnswer.Text, out msgReturn, out sqlMsg);

								if(isFound)
								{
									_wordByThread[i-1] = Utilities.Char(j);
									txtWord.Text = txtWord.Text.Insert(i, Utilities.Char(j));
									txtWord.Text = txtWord.Text.Remove(i-1, 1);
									if(pbProcessing.Value < pbProcessing.Maximum)
										pbProcessing.Value += 1;
									lblProcessing.Text = "Processing... " + Convert.ToString((pbProcessing.Value * 100)/ pbProcessing.Maximum) + "%";
									break;
								}						
							}
						}
						else
						{
							txtCurrentChar.Text = Utilities.Char(currentNbr);

							requestData = GetPostData(enumTypeCommand.Word, ">", currentNbr, i);
							SQLTrue = RequestURL(requestData, txtPositiveAnswer.Text, out msgReturn, out sqlMsg);
						}
						
						while(_threadsStop == (byte)enumThreadsState.Paused)
							Application.DoEvents();

						// In case that the stop has been pressed right after the pause
						if(_threadsStop == (byte)enumThreadsState.Stoped)
							break;

						msgReturn = "Thread " + currentThread + " : " + msgReturn;

						Invoke(new myDelegate(FillDataGrid), new Object[] {msgReturn, sqlMsg});
						
						if(sqlMsg.Trim() != "")
							FillHtmlContent(sqlMsg);

						if(SQLTrue)
						{
							upperNbr = currentNbr;
							currentNbr = ((currentNbr - lowerNbr) / 2) + lowerNbr;
						}
						else if(!isFound)
						{
							lowerNbr = currentNbr;
							currentNbr = ((upperNbr - currentNbr) / 2) + currentNbr;
						}
					}
				}
			}
			catch(ArgumentOutOfRangeException)
			{
				// Do nothing... It might happen if it's too fast, it's ok no real impact whatsoever
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void GetSingleResult()
		{
			bool isFound = false;

			string postData = "";
			string msgReturn = "";
			string sqlMsg = "";

			try
			{
				postData = GetPostData(enumTypeCommand.Normal, "", 0, 0);

				isFound = RequestURL(postData, txtPositiveAnswer.Text, out msgReturn, out sqlMsg);

				Invoke(new myDelegate(FillDataGrid), new Object[] {msgReturn, sqlMsg});

				if(sqlMsg.Trim() != "")
					FillHtmlContent(sqlMsg);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private bool RequestURL(string dataToSend, string positiveString, out string currentMsg, out string sqlMessage)
		{
			HttpWebRequest Request = null;
			HttpWebResponse Response = null;
			Stream responseStream = null;
			Stream requestStream = null;
			SubmitURLParam smtUrlParam = (SubmitURLParam)cmbSubmitURL.Items[cmbSubmitURL.SelectedIndex];
			string responseString = "";
			string strCookieInfo = "";
			string urlToRequest = smtUrlParam.SubmitUri;
			bool resultRequest = false;
			bool hasRedirection = false;
			int intCurrentState = 0;
			int posSQLString = 0;
			int posEndSQLString = 0;

			DateTime begTime = DateTime.Now;
			DateTime endTime;
			TimeSpan span = new TimeSpan(0);

			currentMsg = "";
			sqlMessage = "";

			try
			{
				// Remove the [GET] or [POST] prefix
				if(urlToRequest.StartsWith("[POST]"))
					urlToRequest = urlToRequest.Substring("[POST] ".Length);
				else if(urlToRequest.StartsWith("[GET]"))
					urlToRequest = urlToRequest.Substring("[GET] ".Length);

				if(urlToRequest != "")
				{
					if(radGet.Checked)
						urlToRequest = Utilities.GetNewGETUrl(dataToSend, urlToRequest);

					Request = (System.Net.HttpWebRequest)HttpWebRequest.Create(urlToRequest);
					Request.Method = radGet.Checked ? "GET" : "POST";
					Request.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
					Request.Headers.Add("Accept-Language", "en-us");
					// Pretend we are Internet Explorer 6, really widely spread...
					Request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322)";
					Request.Timeout = (int) new TimeSpan(0,0,120).TotalMilliseconds;
					Request.AllowAutoRedirect = false;
					Request.KeepAlive = false;

					if(mnUseCookie.Checked)
					{
						Request.CookieContainer = new CookieContainer();

						if(_loadedSetCookie != null)
							Request.CookieContainer.SetCookies(Request.RequestUri, _loadedSetCookie);

						if(_loadedCookie != null)
							Request.CookieContainer.SetCookies(Request.RequestUri, _loadedCookie);
					}

					if(radPost.Checked)
					{
						byte[] bytes;

						if(mnCharacterSetAutoDetect.Checked && _charSet != "")
							bytes = Encoding.GetEncoding(_charSet).GetBytes(dataToSend);
						else
							bytes = Encoding.ASCII.GetBytes(dataToSend);

						Request.ContentType = "application/x-www-form-urlencoded";
						Request.ContentLength = dataToSend.Length;
						Request.ServicePoint.Expect100Continue = false;

						try
						{
							requestStream = Request.GetRequestStream();
							requestStream.Write(bytes, 0, bytes.Length);
							requestStream.Close();
						}
						catch(WebException ex2)
						{
							// First we need to see if the connection to internet is still valid
							if(ex2.Status == WebExceptionStatus.ConnectFailure || ex2.Status == WebExceptionStatus.ProxyNameResolutionFailure || ex2.Status == WebExceptionStatus.NameResolutionFailure)
							{
								MessageBox.Show("The Internet connection or the requested\n" +
									"POST page is no more working\n\n" +
									"Verify your connection or/and web page with a normal browser\n\n" +
									"HINT: It might be the proxy that has been changed",
									"Connection failure", MessageBoxButtons.OK, MessageBoxIcon.Warning);

								intCurrentState = 777;
								currentMsg = "Connection error - " + dataToSend;
								return false;
							}
						}
					}

					try
					{
						begTime = DateTime.Now;

						Response = (HttpWebResponse)Request.GetResponse();

						endTime = DateTime.Now;
						span = endTime.Subtract(begTime);

						strCookieInfo = Response.Headers["Set-Cookie"];

						// verify if there is a redirection (302) with found option
						hasRedirection = CheckIfRedirection(Response, true, out responseString);

						// Make sure that the cookie is not null for further tests
						strCookieInfo = strCookieInfo == null ? "~~||@||~~" : strCookieInfo;
						responseStream = Response.GetResponseStream();
					}
					catch(WebException ex1)
					{
						// First we need to see if the connection to internet is still valid
						if(ex1.Status == WebExceptionStatus.ConnectFailure || ex1.Status == WebExceptionStatus.ProxyNameResolutionFailure || ex1.Status == WebExceptionStatus.NameResolutionFailure)
						{
							MessageBox.Show("The Internet connection or the requested\n" +
								"GET page is no more working\n\n" +
								"Verify your connection or/and web page with a normal browser\n\n" +
								"HINT: It might be the proxy that has been changed",
								"Connection failure", MessageBoxButtons.OK, MessageBoxIcon.Warning);

							intCurrentState = 777;
							currentMsg = "Connection error - " + dataToSend;
							return false;
						}

						// We don't want to stop after, we continue to read content,
						// might be interesting...
						responseStream = ex1.Response.GetResponseStream();

						if(ex1.Message.IndexOf("404") != -1)
						{
							intCurrentState = 404;
							currentMsg = "Error 404 - " + dataToSend;
							resultRequest = false;
						}
						else if(ex1.Message.IndexOf("403") != -1)
						{
							intCurrentState = 403;
							currentMsg = "Error 403 - " + dataToSend;
							resultRequest = false;
						}
						else if(ex1.Message.IndexOf("500") != -1)
						{
							intCurrentState = 500;
							currentMsg = "Error 500 - " + dataToSend;
							resultRequest = false;
						}
						else
						{
							currentMsg = ex1.Message + " - " + dataToSend;
							resultRequest = false;
						}
					}

					if(!hasRedirection)
					{
						if(mnCharacterSetAutoDetect.Checked && _charSet != "")
							responseString = Utilities.GetStreamHTMLData(responseStream, _charSet, false);
						else
							responseString = Utilities.GetStreamHTMLData(responseStream, null, false);
					}

					// First initialize the positive answer in case they are empty, since it will always end up
					// true because they are empty strings in any page we must initialize them with almost impossible string
					// occurance in a page
					positiveString = positiveString.Trim() == "" ? "~~|@@|~~" : positiveString.Trim();

					if(intCurrentState != 404 && intCurrentState != 403)
					{
						// Check first if there is no delay, if so act normally
						if(!ckbDelay.Checked)
						{
							// Need to clean up this code to make it more generic with a xml file and
							// separate the normal and blind mode
							if(responseString.ToUpper().IndexOf(positiveString.ToUpper()) != -1 || strCookieInfo.ToUpper().IndexOf(positiveString.ToUpper()) != -1)
							{
								currentMsg = "Is true - " + dataToSend;
								resultRequest = true;
							}
							else if(responseString.ToUpper().IndexOf("HTTP 404") != -1)
							{
								currentMsg = "Error 404 - " + dataToSend;
								resultRequest = false;
							}
							else if(responseString.ToUpper().IndexOf(txtSQLErrorString.Text.ToUpper()) != -1 && txtSQLErrorString.Text.Trim() != "" && radNormal.Checked)
							{
								posSQLString = responseString.ToUpper().IndexOf(txtSQLErrorString.Text.ToUpper());
								if((posSQLString + nudHtmlMsgLength.Value) > responseString.Length)
									posEndSQLString = responseString.Length - posSQLString;
								else
									posEndSQLString = Convert.ToInt32(nudHtmlMsgLength.Value);
								sqlMessage= responseString.Substring(posSQLString, posEndSQLString);
							}
							else if(chkTrapErrorString.Checked && radNormal.Checked)
							{
								posSQLString = responseString.ToUpper().IndexOf("ERR");
								if(posSQLString != -1)
								{
									if((posSQLString + nudHtmlMsgLength.Value) > responseString.Length)
										posEndSQLString = responseString.Length - posSQLString;
									else
										posEndSQLString = Convert.ToInt32(nudHtmlMsgLength.Value);
									sqlMessage= responseString.Substring(posSQLString, posEndSQLString);
								}
							}
							else if(responseString.ToUpper().IndexOf("MICROSOFT OLE") != -1  && radNormal.Checked)
							{
								posSQLString = responseString.ToUpper().IndexOf("MICROSOFT OLE");
								if((posSQLString + nudHtmlMsgLength.Value) > responseString.Length)
									posEndSQLString = responseString.Length - posSQLString;
								else
									posEndSQLString = Convert.ToInt32(nudHtmlMsgLength.Value);
								sqlMessage= responseString.Substring(posSQLString, posEndSQLString);
							}
							else if(responseString.ToUpper().IndexOf("MICROSOFT ODBC") != -1  && radNormal.Checked)
							{
								posSQLString = responseString.ToUpper().IndexOf("MICROSOFT ODBC");
								if((posSQLString + nudHtmlMsgLength.Value) > responseString.Length)
									posEndSQLString = responseString.Length - posSQLString;
								else
									posEndSQLString = Convert.ToInt32(nudHtmlMsgLength.Value);

								sqlMessage= responseString.Substring(posSQLString, posEndSQLString);
							}
							else if(responseString.ToUpper().IndexOf("ENCOUNTERED SQLEXCEPTION") != -1  && radNormal.Checked)
							{
								posSQLString = responseString.ToUpper().IndexOf("ENCOUNTERED SQLEXCEPTION");
								if((posSQLString + nudHtmlMsgLength.Value) > responseString.Length)
									posEndSQLString = responseString.Length - posSQLString;
								else
									posEndSQLString = Convert.ToInt32(nudHtmlMsgLength.Value);
								sqlMessage= responseString.Substring(posSQLString, posEndSQLString);
							}
							else
							{
								currentMsg = "Is false - " + dataToSend;
								resultRequest = false;
							}
						}
						else
						{
							if(span.Seconds >= nudDelaySeconds.Value-1) //(span.Seconds - nudDelaySeconds.Value >= -1 && span.Seconds - nudDelaySeconds.Value < 3)
							{
								currentMsg = "Is true - " + dataToSend;
								resultRequest = true;
							}
							else
							{
								currentMsg = "Is not found - " + dataToSend;
								resultRequest = false;
							}
						}
					}
					else if(intCurrentState == 403)
					{
						currentMsg = "Error 403 - " + dataToSend;
						resultRequest = false;
					}
				}
				else
				{
					if(radGet.Checked)
						MessageBox.Show("There is no GET URL in the URL textbox!\n\n" +
							"You might have deleted the URL by accident", "No URL provided",
							MessageBoxButtons.OK, MessageBoxIcon.Warning);
					else
						MessageBox.Show("There is no URL in the submited URL textbox!\n\n" +
							"You might have deleted the URL by accident", "No URL provided",
							MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}

				if(radNormal.Checked)
					FillHtmlContent(responseString);

				return resultRequest;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString() + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				currentMsg = "Weird error - " + dataToSend;
				return false;
			}
			finally
			{
				if(Response != null) Response.Close();
				if(Request != null)	Request.Abort();
			}
		}

		private void FillDataGrid(string status, string sqlError)
		{
			try
			{
				CResultItem item = new CResultItem(status, sqlError);

				_arrStatus.Add(item);

				dtgCurrentState.DataSource = null;
				dtgCurrentState.DataSource = _arrStatus;

				//Point to the last added item
				dtgCurrentState.ScrollToRow(_arrStatus.Count);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void FillInputDataGrid(int curDatagridId, string name, string startStr, string varStr, string endStr, bool isSelected)
		{
			try
			{
				CHtmlInputInject item = new CHtmlInputInject(name, startStr.Substring(0,(startStr.Length > 50000 ? 50000 : startStr.Length)), varStr, endStr, isSelected);

				if(_arrInputModifiable[curDatagridId] == null)
					_arrInputModifiable[curDatagridId] = new ArrayList();

				_arrInputModifiable[curDatagridId].Add(item);

				dtgLoadedInput.DataSource = null;
				dtgLoadedInput.DataSource = _arrInputModifiable[curDatagridId];
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void FillInputNotModifiableDataGrid(int curDatagridId, string name, string inputValue)
		{
			try
			{
				CHtmlInputNotInject item = new CHtmlInputNotInject(name, inputValue);

				if(_arrInputNotModifiable[curDatagridId] == null)
					_arrInputNotModifiable[curDatagridId] = new ArrayList();

				_arrInputNotModifiable[curDatagridId].Add(item);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void FillHtmlContent(string htmlContentMsg)
		{
			ChangeHtmlContent(htmlContentMsg);
		}

		private bool LoadInitialURLInfo(out string responseString, Uri requestURI)
		{
			HttpWebRequest Request;
			HttpWebResponse Response = null;
			WebProxy webProxy = null;
			Stream responseStream = null;
			MemoryStream memOfResponseStream = null;
			MemoryStream copyOfMemoryResponse = null;
			IHTMLDocument3 loadedHtmlDoc3 = null;
			string responseCharSet = "";
			string dummyValue = "";
			bool resultGet = false;
			bool hasBeenRedirected = false;
			responseString = "";
			requestURI = null;

			try
			{
				// Create the HttpWebRequest object.
				if(txtURL.Text.Trim() != "")
				{
					ClearLoadedInfo();

					if(!txtURL.Text.ToUpper().StartsWith("HTTP://") && !txtURL.Text.ToUpper().StartsWith("HTTPS://"))
					{
						MessageBox.Show("You forgot to specify the protocol to use!\n\n" +
							"Add in the URL prefix either http:// or https://", "No protocol provided",
							MessageBoxButtons.OK, MessageBoxIcon.Warning);

						return false;
					}

					Request = (System.Net.HttpWebRequest)HttpWebRequest.Create(txtURL.Text);
					Request.Accept = "*/*";
					Request.Headers.Add("Accept-Language", "en-us");
					// Pretend we are Internet Explorer 6, really widely spread...
					Request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322)";
					Request.Method = "GET";

					if(mnUseCookie.Checked)
					{
						Request.CookieContainer = new CookieContainer();

						try
						{
							if(_loadedSetCookie != null)
								Request.CookieContainer.SetCookies(Request.RequestUri, _loadedSetCookie);
						}
						catch(Exception ex)
						{
							if(ex.Message.ToUpper().IndexOf("AN ERROR HAS OCCURRED WHEN PARSING COOKIE HEADER FOR URI") > 0)
								throw(ex);
						}
					}

					Request.AllowAutoRedirect = false;

					webProxy = WebProxy.GetDefaultProxy();
					webProxy.Credentials = CredentialCache.DefaultCredentials;
					Request.Proxy = webProxy;

					Request.Timeout = (int) new TimeSpan(0,0,60).TotalMilliseconds;
					
					// Accept all certificates, to go thru SSL
					ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();

					try
					{
						Response = (HttpWebResponse)Request.GetResponse();
						responseCharSet = Response.CharacterSet;
						_loadedSetCookie = Response.Headers["Set-Cookie"];
						_loadedCookie = Response.Headers["Cookie"];
						responseStream = Response.GetResponseStream();

						// There is always the possibility of redirection, let's check first
						if(CheckIfRedirection(Response, false, out dummyValue))
						{
							hasBeenRedirected = true;
							resultGet = false;
						}
						else
							resultGet = true;
					}
					catch(WebException ex1)
					{
						resultGet = false;

						// Check first if it's not a problem of resolving the URL
						if(ex1.Status == WebExceptionStatus.NameResolutionFailure)
						{
							MessageBox.Show("The page requested does not exist or could not be resolved or/and\n" +
								"the Internet connection is no more working\n\n" +
								"Please first check if the connection is valid and if so the validy of you URL\n\n" +
								"HINT: Verify if http:// is not written twice in the URL or the proxy has not changed",
								"Page doesn't exist or could not be resolved or connection problems", MessageBoxButtons.OK, MessageBoxIcon.Warning);

							return resultGet;
						}
						else if(ex1.Status == WebExceptionStatus.ConnectFailure || ex1.Status == WebExceptionStatus.ProxyNameResolutionFailure)
						{
							MessageBox.Show("The connection to internet is no more working\n\n" +
								"Verify your connection with a normal browser\n\n" +
								"HINT: It might be the proxy that has been changed",
								"Connection failure", MessageBoxButtons.OK, MessageBoxIcon.Warning);

							return resultGet;
						}
						else
						{	
							// We are just interested in the returning error message
							if(ex1.Response != null)
								responseStream = ex1.Response.GetResponseStream();	
							else
							{
								responseStream = null;
								responseString = "<html><body>" + ex1.Message + "</body></html>";
								
								return resultGet;
							}
						}
					}
					finally
					{
						requestURI = Request.RequestUri;
					}

					if(responseStream != null)
					{
						memOfResponseStream = new MemoryStream();
						copyOfMemoryResponse = new MemoryStream();
						Utilities.CopyStream(responseStream, memOfResponseStream);
						memOfResponseStream.Position = 0;
						Utilities.CopyStream(memOfResponseStream, copyOfMemoryResponse);
						responseString = Utilities.GetStreamHTMLData(memOfResponseStream, null, true);
					}
					if(responseString.Trim() == "")
						responseString = "<html><body></body></html>";

					// First we check if it has been redirected
					if(!hasBeenRedirected)
					{
						if(CheckIfPageOk(responseString, requestURI, out loadedHtmlDoc3))
						{
							// Check for charset and meta content-type charset here, it has  
							// a minor impact of performance, that why we offer the option
							if(mnCharacterSetAutoDetect.Checked)
								responseString = TransformToRightCharacterSet(responseString, copyOfMemoryResponse, responseCharSet, ref loadedHtmlDoc3, out _charSet);

							if(radPost.Checked)
								FillPostDataArray(loadedHtmlDoc3, requestURI);
							else
								FillGetDataArray(responseString, requestURI);
						}
						else
						{
							mnSaveSession.Enabled = false;
							butStartStop.Enabled = false;
						}
						
						resultGet = true;
					}
				}
				else
				{
					this.Cursor = Cursors.Default;
					butStartStop.Enabled = false;
					nudNbrThread.Enabled = true;
					butStartStop.Text = "&Start";
					butPause.Enabled = false;
				}

				return resultGet;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);

				return resultGet;
			}
			finally
			{
				if(responseStream != null) responseStream.Close();
				if(memOfResponseStream != null) memOfResponseStream.Close();
				if(copyOfMemoryResponse != null) copyOfMemoryResponse.Close();
			}
		}

		private string GetPostData(enumTypeCommand typeCmd, string compStr, int curNumber, int curChar)
		{
			string tmpPostData = "";
			int lengthString = 4000;
			int submitURLIndex = 0;
			CHtmlInputInject curRow1;
			CHtmlInputNotInject curRow2;

			try
			{

				submitURLIndex = cmbSubmitURL.SelectedIndex;

				IEnumerator myEnum1 = _arrInputModifiable[submitURLIndex].GetEnumerator();
				IEnumerator myEnum2 = null;

				if(_arrInputNotModifiable != null)
					if(_arrInputNotModifiable[submitURLIndex] != null)
						myEnum2 = _arrInputNotModifiable[submitURLIndex].GetEnumerator();

				while(myEnum1.MoveNext())
				{
					curRow1 = (CHtmlInputInject)myEnum1.Current;

					if(radBlind.Checked && curRow1.ItemIsSelected)
					{
						DatabaseStringBaseType dbStringType = null;

						switch(cmbDBType.SelectedIndex)
						{
							case (int)DatabaseStringBaseType.enumDatabaseType.SqlServer:
								dbStringType = new DatabaseString.DatabaseStringSQLServerType();
								break;

							case (int)DatabaseStringBaseType.enumDatabaseType.Oracle:
								dbStringType = new DatabaseString.DatabaseStringOracleType();
								break;

							case (int)DatabaseStringBaseType.enumDatabaseType.MySql:
								dbStringType = new DatabaseString.DatabaseStringMySQLType();
								break;

							case (int)DatabaseStringBaseType.enumDatabaseType.Sybase:
								dbStringType = new DatabaseString.DatabaseStringSybaseType();
								break;
						}
						
						if(!curRow1.ItemStartingString.EndsWith(" "))
							curRow1.ItemStartingString += " ";

						if(typeCmd == enumTypeCommand.Length)
							tmpPostData += Utilities.EncodeURL(curRow1.ItemName) + "=" + dbStringType.GetLengthPostDataString(curRow1.ItemStartingString, curRow1.ItemVaryingString, curRow1.ItemEndingString, Convert.ToString(curNumber), compStr, chkDistinct.Checked) + Utilities.EncodeURL("&");
						else if(typeCmd == enumTypeCommand.Count)
							tmpPostData += Utilities.EncodeURL(curRow1.ItemName) + "=" + dbStringType.GetCountPostDataString(curRow1.ItemStartingString, curRow1.ItemVaryingString, curRow1.ItemEndingString, Convert.ToString(curNumber), compStr) + Utilities.EncodeURL("&");
						else if(typeCmd == enumTypeCommand.Word)
						{
							if(txtLength.Text != "")
								lengthString = Convert.ToInt32(txtLength.Text) > 4000 ? 4000 : Convert.ToInt32(txtLength.Text);

							tmpPostData += Utilities.EncodeURL(curRow1.ItemName) + "=" + dbStringType.GetWordPostDataString(curRow1.ItemStartingString, curRow1.ItemVaryingString, curRow1.ItemEndingString, Convert.ToString(curNumber), Convert.ToString(curChar), Convert.ToString(lengthString), compStr, chkDistinct.Checked) + Utilities.EncodeURL("&");
						}
						else
							tmpPostData += Utilities.EncodeURL(curRow1.ItemName) + "=" + Utilities.EncodeURL(curRow1.ItemStartingString) + "&";
					}
					else
						tmpPostData += Utilities.EncodeURL(curRow1.ItemName) + "=" + Utilities.EncodeURL(curRow1.ItemStartingString) + "&";
				}

				if(myEnum2 != null)
				{
					while(myEnum2.MoveNext())
					{
						curRow2 = (CHtmlInputNotInject)myEnum2.Current;

						tmpPostData += Utilities.EncodeURL(curRow2.ItemName) + "=" + Utilities.EncodeURL(curRow2.ItemInputValue) + "&";
					}
				}
				if(tmpPostData.EndsWith("&"))
					tmpPostData = tmpPostData.Remove(tmpPostData.Length - 1, 1);

				if(chkInsertEmptyComments.Checked)
					tmpPostData = tmpPostData.Replace("+", "/*xxx*/");

				return tmpPostData;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);

				return tmpPostData;
			}
		}

		private bool CheckIfUniqueSelection()
		{
			bool isAlreadyChecked = true;
			int submitURLIndex = 0;
			CHtmlInputInject curRow;

			try
			{
				submitURLIndex = cmbSubmitURL.SelectedIndex;

				IEnumerator enuCurrentItem = _arrInputModifiable[submitURLIndex].GetEnumerator();

				while(enuCurrentItem.MoveNext())
				{
					curRow = (CHtmlInputInject)enuCurrentItem.Current;

					if(curRow.ItemIsSelected)
					{
						isAlreadyChecked = false;
						break;
					}
				}

				return isAlreadyChecked;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);

				return false;
			}
		}

		private void FillPostDataArray(IHTMLDocument3 loadedHtmlDoc3, Uri curURI)
		{
			IHTMLElementCollection colForm = null;
			IHTMLElementCollection colIFRAME = null;
			IHTMLElementCollection colIFrameForm = null;
			IHTMLDocument2 curHtmlDocument2 = null;
			HTMLDocument curDoc = new HTMLDocument();
			int curDatagridId = 0;
			int numberOfForms = 0;
			
			try
			{
				dtgLoadedInput.DataSource = null;

				colForm = loadedHtmlDoc3.getElementsByTagName("Form");
				colIFRAME = loadedHtmlDoc3.getElementsByTagName("IFRAME");

				// We first need to get the number of Forms before we start...
				foreach(HTMLIFrameClass curFrame in colIFRAME)
				{
					((IHTMLDocument2)curDoc).write(curFrame.innerHTML);
					curHtmlDocument2 = (IHTMLDocument2)curDoc;
							
					colIFrameForm = curHtmlDocument2.forms;
					curHtmlDocument2.close();
					curDoc.close();

					numberOfForms += colIFrameForm.length;
				}

				cmbSubmitURL.Items.Clear();

				_arrInputModifiable = new ArrayList[colForm.length + numberOfForms];
				_arrInputNotModifiable = new ArrayList[colForm.length + numberOfForms];

				foreach(IHTMLFormElement curForm in colForm)
					FillLoadedPostDataArray(curForm, curURI, ref curDatagridId);

				foreach(HTMLIFrameClass curFrame in colIFRAME)
				{
					((IHTMLDocument2)curDoc).write(curFrame.innerHTML);
					curHtmlDocument2 = (IHTMLDocument2)curDoc;
							
					colIFrameForm = curHtmlDocument2.forms;
					curHtmlDocument2.close();
					curDoc.close();

					foreach(IHTMLFormElement curForm in colIFrameForm)
						FillLoadedPostDataArray(curForm, curURI, ref curDatagridId);
				}

				// Force the selection to the first form
				if(cmbSubmitURL.Items.Count > 0)
					cmbSubmitURL.SelectedIndex = 0;

				butStartStop.Enabled = true;
				mnSaveSession.Enabled = true;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void FillLoadedPostDataArray(IHTMLFormElement curForm, Uri curURI, ref int curDatagridId)
		{
			string tmpValue = "";
			string strCurUri = "";
			Uri myNewUri = null;

			try
			{
				if(curForm.action != null)
				{
					myNewUri = new Uri(curURI, curForm.action);
					strCurUri = myNewUri.ToString();
				}
				else
					strCurUri = curURI.ToString();

				if(curForm.method.ToUpper() == "GET")
					cmbSubmitURL.Items.Add(new SubmitURLParam("[GET] " + strCurUri, Brushes.Beige, SubmitURLParam.Method.GET));
				else
					cmbSubmitURL.Items.Add(new SubmitURLParam("[POST] " + strCurUri, Brushes.LightBlue, SubmitURLParam.Method.POST));

				IHTMLElementCollection inputs = (IHTMLElementCollection)curForm.tags("input");
						
				if(inputs != null)
				{
					Hashtable radios = new Hashtable();
					Hashtable checkboxes = new Hashtable();

					foreach (IHTMLInputElement curInput in inputs)
					{
						tmpValue = "";
						if(curInput.value != null) tmpValue = curInput.value;
					
						if(curInput.type == "hidden" || curInput.type == "text" || curInput.type == "password")
							FillInputDataGrid(curDatagridId, curInput.name, tmpValue, "", "", false);
						else if(curInput.type == "radio")
						{
							// We first build the collection of radio buttons, will fill the value of the first
							// radio button found and change it if it is checked by default
							if(!radios.Contains(curInput.name))
								radios.Add(curInput.name, curInput.value);
							else if(curInput.@checked)
								radios[curInput.name] = curInput.value;
						}
						else if(curInput.type == "checkbox")
						{
							// We first build the collection of radio buttons, will fill the value of the first
							// radio button found and change it if it is checked by default
							if(!checkboxes.Contains(curInput.name))
								checkboxes.Add(curInput.name, curInput.value);
							else if(curInput.@checked)
								checkboxes[curInput.name] = curInput.value;
						}
						else
							FillInputNotModifiableDataGrid(curDatagridId, curInput.name, tmpValue);
					}
					// We fill the input datagrid with the collection of radio button if one found
					if(radios.Count > 0)
					{
						foreach (string name in radios.Keys) 
						{
							tmpValue = "";
							if(radios[name] != null) tmpValue = radios[name].ToString();
								
							FillInputDataGrid(curDatagridId, name, tmpValue, "", "", false);
						}
					}
					// We fill the input datagrid with the collection of checkbox if one found
					if(checkboxes.Count > 0)
					{
						foreach (string name in checkboxes.Keys) 
						{
							tmpValue = "";
							if(checkboxes[name] != null) tmpValue = checkboxes[name].ToString();
								
							FillInputDataGrid(curDatagridId, name, tmpValue, "", "", false);
						}
					}
				}
				// Now let's get all the select object
				IHTMLElementCollection selects = (IHTMLElementCollection)curForm.tags("select");

				if(selects != null)
				{
					foreach (IHTMLSelectElement curSelect in selects)
					{
						tmpValue = "";
						if(curSelect.value != null) tmpValue = curSelect.value;
					
						FillInputDataGrid(curDatagridId, curSelect.name, tmpValue, "", "", false);
					}
				}

				// Finally let's get all the Textarea object
				IHTMLElementCollection textareas = (IHTMLElementCollection)curForm.tags("textarea");

				if(textareas != null)
				{
					foreach (IHTMLTextAreaElement curTextarea in textareas)
					{
						tmpValue = "";
						if(curTextarea.value != null) tmpValue = curTextarea.value;
					
						FillInputDataGrid(curDatagridId, curTextarea.name, tmpValue, "", "", false);
					}
				}

				curDatagridId++;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void CheckEndingThreadsState()
		{
			string finalWord = "";

			try
			{
				_numberThreadsFinished+=1;

				if(_numberThreadsFinished == Convert.ToInt32(nudNbrThread.Value))
				{
					for(int i=0; i<=_wordByThread.GetUpperBound(0); i++)
						finalWord += _wordByThread[i];

					// We want to keep the ? if it has been stoped before it has been finished
					if(finalWord.Length == Convert.ToInt32(txtLength.Text))
						txtWord.Text = finalWord;

					butPause.Enabled = false;
					nudNbrThread.Enabled = true;
					butStartStop.Text = "&Start";
					butStartStop.Enabled = true;
					this.Cursor = Cursors.Default;

					_endTime = DateTime.Now;
					_span = _endTime.Subtract(_begTime);
				
					txtTimeTaken.Text = (_span.Hours > 0 ? _span.Hours + " h " : "") + (_span.Minutes > 0 ? _span.Minutes + " m " : "") + _span.Seconds + " s " + _span.Milliseconds + " ms";

					if(radBlind.Checked)
					{
						lblProcessing.Visible = false;
						pbProcessing.Visible = false;
						pbProcessing.Value = 0;
						lblProcessing.Text = "Processing... 0%";
					}
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void FillGetDataArray(string responseString, Uri curURI)
		{
			bool isValid = false;
			string tmpValue = "";
			string[] allQueryPairValue;
			string[] queryPairValue;

			try
			{
				IHTMLDocument2 htmlDocument = (IHTMLDocument2)axwbHtmlResult.Document;

				// Make sure the object is ready before to fill in
				while(htmlDocument.readyState != "complete")
					Application.DoEvents();

				htmlDocument.clear();
				htmlDocument.write(new object[] {responseString});
				axwbHtmlResult.CtlRefresh();

				_arrInputModifiable = new ArrayList[FIRST_DATAGRID_INDEX_ID+1];
				dtgLoadedInput.DataSource = null;

				if(curURI.Query != "")
				{
					cmbSubmitURL.Items.Clear();
					cmbSubmitURL.Items.Add(new SubmitURLParam("[GET] " + curURI.GetLeftPart(UriPartial.Path), Brushes.Beige, SubmitURLParam.Method.GET));
					cmbSubmitURL.SelectedIndex = 0;

					tmpValue = curURI.Query.Substring(1, curURI.Query.Length - 1);

					allQueryPairValue = tmpValue.Split('&');

					foreach(string el in allQueryPairValue)
					{
						queryPairValue = el.Split('=');
						
						if(queryPairValue.Length == 2)
						{
							isValid = true;
							FillInputDataGrid(FIRST_DATAGRID_INDEX_ID, queryPairValue[0], queryPairValue[1], "", "", false);
						}
					}
				}
				else
				{
					if(_arrInputModifiable[FIRST_DATAGRID_INDEX_ID] != null)
						_arrInputModifiable[FIRST_DATAGRID_INDEX_ID].Clear();
					dtgLoadedInput.DataSource = null;
					dtgLoadedInput.Refresh();
					FillHtmlContent(htmlDocument.body.outerHTML);
					butStartStop.Enabled = false;
					butPause.Enabled = false;
				}

				if(isValid)
				{
					butStartStop.Enabled = true;
					mnSaveSession.Enabled = true;
				}
				else
				{
					// There is no GET values in the URL warn the user
					MessageBox.Show("The page requested with the option GET checked contains no Query string(s)\n\n" +
						"Make sure to check the POST option if your intention was to use that option.\n" +
						"If not please verify your URL", 
						"No Query string(s) with the GET option checked", MessageBoxButtons.OK, MessageBoxIcon.Warning);

					mnSaveSession.Enabled = false;
					butStartStop.Enabled = false;
				}

				if(htmlDocument != null)
					htmlDocument.close();
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private bool CheckIfPageOk(string responseString, Uri requestURI, out IHTMLDocument3 myHtmlDoc3)
		{
			IHTMLDocument2 curHtmlDocument2 = null;
			bool isOk = true;
			object o = null;
			Uri myNewUri = null;
			myHtmlDoc3 = null;

			try
			{	
				curHtmlDocument2 = (IHTMLDocument2)axwbHtmlInitializor.Document;

				// Make sure the object is ready before to fill in
				while(curHtmlDocument2.readyState != "complete" && curHtmlDocument2.readyState != "interactive")
					Application.DoEvents();

				// Reinitialize the document
				curHtmlDocument2.close();
				curHtmlDocument2.open("about:blank", o, o, o);
				curHtmlDocument2.clear();
				axwbHtmlInitializor.Stop();

				try // To treat redirect in the jscript
				{
					curHtmlDocument2.write(responseString);
					_initialUri = "";
				}
				catch(Exception ex)
				{
					isOk = false;
					if(_initialUri.Trim() != "")
					{	// Error caused by the redirect in a jscript
						myNewUri  = new Uri(requestURI, _initialUri);

						CreateResponseMessageBox("The requested page has a redirection due probably by a javascript code\nto the following URL:\n\n", "", "Redirection with javascript", myNewUri);

						return isOk;
					}
					else
					{	// Real error
						throw ex;
					}
				}

				myHtmlDoc3 = (IHTMLDocument3)curHtmlDocument2;

				//  Check if it has been moved
				if(CheckIfRefresh(myHtmlDoc3, requestURI))
					isOk = false;
				else if(CheckIfMoved(myHtmlDoc3, requestURI))
					isOk = false;
				else if(CheckIfFrameset(myHtmlDoc3, requestURI))
					isOk = false;
				else if(radPost.Checked)
				{
					if(!CheckIfThereIsFormWithPOST(myHtmlDoc3))
						isOk = false;
				}

				return isOk;
			}
			catch(Exception ex1)
			{
				MessageBox.Show(ex1.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex1.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			finally
			{
				if(curHtmlDocument2 != null) curHtmlDocument2.close();
			}
		}
		/// <summary>
		/// CheckIfRedirection
		/// </summary>
		/// <param name="response"></param>
		/// <param name="silentMode">Put to true if we don't want to have a messagebox displayed</param>
		/// <param name="msgRedirection"></param>
		/// <returns>bool: true is there is direction, false there is none</returns>
		private bool CheckIfRedirection(HttpWebResponse response, bool silentMode, out string msgRedirection)
		{
			bool hasRedirection = false;
			string location = "";
			string title = "";
			msgRedirection = "";
			Uri myNewUri;

			try
			{
				if(response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.Found
					|| response.StatusCode == HttpStatusCode.Moved || response.StatusCode == HttpStatusCode.MovedPermanently)
				{
					location = response.Headers.GetValues("Location")[0];

					myNewUri  = new Uri(response.ResponseUri, location);

					if(response.ResponseUri.ToString() != myNewUri.ToString())
					{
						if(response.StatusCode == HttpStatusCode.Moved || response.StatusCode == HttpStatusCode.MovedPermanently)
						{
							msgRedirection = "The requested page has been moved\nto the following URL:\n\n" + (silentMode ? " " + myNewUri.ToString() : "");
							title = "Page moved";
						}
						else
						{
							msgRedirection = "The requested page has a redirection\nto the following URL:\n\n" + (silentMode ? " " + myNewUri.ToString() : "");
							title = "Redirection";
						}

						if(!silentMode) CreateResponseMessageBox(msgRedirection, "", title, myNewUri);

						hasRedirection = true;
					}
				}
				return hasRedirection;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);

				return hasRedirection;
			}
		}

		private bool CheckIfRefresh(IHTMLDocument3 curHmtlDoc3, Uri curURI)
		{
			bool isRefresh = false;
			string[] arrUrl;
			string httpPart = "";
			string uriPart = "";
			string tmpUriPart = "";
			int posEquals = 0;
			Uri myNewUri = null;
			IHTMLElementCollection colMeta = curHmtlDoc3.getElementsByTagName("meta");
			
			try
			{
				foreach (IHTMLMetaElement curMeta in colMeta)
				{
					if(curMeta.httpEquiv != null)
						if(curMeta.httpEquiv.ToUpper() == "REFRESH")
						{
							// Now we check if the refresh is on itself or not, if it is, 
							// we don't raise an error
							if(curMeta.url != null)
							{
								if(curMeta.url != curURI.AbsoluteUri)
								{
									isRefresh = true;
									myNewUri  = new Uri(curURI, curMeta.url);
								}
								break;
							}
							else
							{
								arrUrl = curMeta.content.Split(';');
								httpPart = arrUrl[arrUrl.Length - 1];

								posEquals = httpPart.IndexOf("=", 0);

								// Might have one = but it could be in the querystrings, we don't know yet
								if(posEquals != -1)
								{
									tmpUriPart = httpPart.Substring(0, posEquals + 1);
									
									// Remove all the space, we never know if someone put ' url =' instead of 'url='
									tmpUriPart = tmpUriPart.Replace(" ", "");

									if(tmpUriPart.ToUpper() == "URL=")
										uriPart = httpPart.Substring(posEquals + 1);
								}
								else
									uriPart = httpPart;

								if(uriPart != curURI.AbsoluteUri && !Utilities.IsNumeric(httpPart))
								{
									isRefresh = true;
									myNewUri  = new Uri(curURI, uriPart.Trim());
								}
								break;
							}
						}
				}

				if(isRefresh)
					CreateResponseMessageBox("The requested page has a redirection due to a refresh META command\nto the following URL:\n\n", "", "Redirection with Refresh", myNewUri);

				return isRefresh;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);

				return isRefresh;
			}
		}

		private bool CheckIfMoved(IHTMLDocument3 curHmtlDoc3, Uri curURI)
		{
			bool hasMoved = false;
			bool hasBeenFound = false;
			string strCurDoc = "";
			string location = "";
			string[] stringToCompare = {"OBJECT MOVED", "HAS MOVED", "301 MOVED PERMANENTLY"};
			Uri myNewUri = null;
			IHTMLElementCollection colAnchor = null;

			try
			{
				strCurDoc = curHmtlDoc3.documentElement.innerHTML;

				for(int curString=0; curString<stringToCompare.Length; curString++)
				{
					if(strCurDoc.ToUpper().IndexOf(stringToCompare[curString], 0) != -1)
					{
						hasBeenFound = true;
						break;
					}
				}

				if(hasBeenFound)
				{
					colAnchor = curHmtlDoc3.getElementsByTagName("A");

					// If no anchor has been found it might means that it was a false positive, however
					// the occurence to find one even though it's a false positive is high since a normal
					// page will be likely to have at least one
					if(colAnchor.length > 0)
					{
						hasMoved = true;

						foreach (IHTMLAnchorElement curA in colAnchor)
						{
							// We want the first occurence
							location = curA.href;
							break;
						}
						
						myNewUri  = new Uri(curURI, location);

						CreateResponseMessageBox("The requested page 'might' have been moved to the following URL:\n\n", 
							"\n\nNote: In this sole case, you might get that message even though there is no redirection, if " +
							"it happens just answer no and ignore it. The reason is that the application searches for an occurrence" +
							" for some words and this occurrence could be found in any pages related to that topic, such as forums", 
							"Page moved", myNewUri);
					}
				}
				return hasMoved;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);

				return false;
			}
		}

		private bool CheckIfFrameset(IHTMLDocument3 curHmtlDoc3, Uri curURI)
		{
			bool hasFrameset = false;
			string listOfUrls = "";
			string chosenFrame = "0";
			string[] locations;
			Uri[] myNewUris = null;
			IHTMLElementCollection colFrame = null;
			int curUrlNbr = 0;
			int lblHeight = 0;
			int intChosenFrame = 0;
			
			try
			{
				colFrame = curHmtlDoc3.getElementsByTagName("Frame");

				// Check if there is at least one frameset, if so let's get each frame
				if(colFrame.length > 0)
				{
					hasFrameset = true;
					locations = new String[colFrame.length];
					myNewUris = new Uri[colFrame.length];
					
					listOfUrls = "0 - I don't want to automatically copy this any of these addresses in the URL\n";

					foreach (IHTMLFrameBase curFrame in colFrame)
					{
						locations[curUrlNbr] = curFrame.src;
						myNewUris[curUrlNbr] = new Uri(curURI, locations[curUrlNbr]);
						listOfUrls += Convert.ToString(curUrlNbr+1) + " - " + myNewUris[curUrlNbr].AbsoluteUri + "\n";
						curUrlNbr += 1;
					}

					lblHeight = 45 + (curUrlNbr * 25);
					chosenFrame = InputBox.ShowInputBox("Frames found in the web page", "Insert the number you want to automatically copy the address in the URL:\n\n" + listOfUrls, lblHeight);

					if(Utilities.IsNumeric(chosenFrame))
					{
						intChosenFrame = Convert.ToInt32(chosenFrame);

						if(curUrlNbr < intChosenFrame)
							intChosenFrame = 0;
					}
					else
						intChosenFrame = 0;

					if(intChosenFrame > 0)
						txtURL.Text = myNewUris[intChosenFrame-1].AbsoluteUri;
				}
				return hasFrameset;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);

				return false;
			}
		}

		private bool CheckIfThereIsFormWithPOST(IHTMLDocument3 hmtlDoc3)
		{
			IHTMLElementCollection colForm = null;
			IHTMLElementCollection colIFRAME = null;
			IHTMLElementCollection colIFrameForm = null;
			IHTMLDocument2 curHtmlDocument2 = null;
			HTMLDocument curDoc = new HTMLDocument();
			bool isFormExistsWithPOST = false;
			int numberOfForms = 0;

			try
			{
				colForm = hmtlDoc3.getElementsByTagName("Form");
				colIFRAME = hmtlDoc3.getElementsByTagName("IFRAME");

				foreach(HTMLIFrameClass curFrame in colIFRAME)
				{
					((IHTMLDocument2)curDoc).write(curFrame.innerHTML);
					curHtmlDocument2 = (IHTMLDocument2)curDoc;
							
					colIFrameForm = curHtmlDocument2.forms;
					curHtmlDocument2.close();
					curDoc.close();

					numberOfForms += colIFrameForm.length;
				}

				numberOfForms += colForm.length;

				// Check if there is at least one form, if so let's get each frame
				if(numberOfForms > 0)
					isFormExistsWithPOST = true;
				else
				{
					MessageBox.Show("The page requested with the option POST checked contains no FORM tag\n\n" +
						"Make sure to check the GET option if your intention was to use that option.\n" +
						"If not please verify your URL", 
						"No FORM tag with the POST option checked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}

				return isFormExistsWithPOST;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);

				return false;
			}
		}

		private string TransformToRightCharacterSet(string responseString, Stream responseStream, string responseHeaderCharSet, ref IHTMLDocument3 curHtmlDoc3, out string charSet)
		{
			object o = null;
			IHTMLDocument2 curHtmlDocument2 = null;
			string newResponseString = responseString;
			charSet = "";			

			try
			{
				charSet = GetCharSet(responseHeaderCharSet, curHtmlDoc3);
				
				if(charSet != "")
				{
					newResponseString = Utilities.GetStreamHTMLData(responseStream, charSet, true);
					curHtmlDocument2 = (IHTMLDocument2)axwbHtmlInitializor.Document;

					// Make sure the object is ready before to fill in
					while(curHtmlDocument2.readyState != "complete" && curHtmlDocument2.readyState != "interactive")
						Application.DoEvents();

					// Reinitialize the document
					curHtmlDocument2.close();
					curHtmlDocument2.open("about:blank", o, o, o);

					curHtmlDocument2.clear();
					curHtmlDocument2.write(newResponseString);
				
					curHtmlDoc3 = (IHTMLDocument3)curHtmlDocument2;
				}
			}
			catch(Exception ex)
			{
				if(ex.Message != Convert.ToString(Utilities.ENCODE_ERROR_CODE))
					MessageBox.Show(ex.ToString() + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
						"\nSource: " + ex.Source +
						"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
						MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
				{
					MessageBox.Show("The detected page encoding string does not exist or is not\n supported by your browser\n\n" +
						"Please uncheck the option 'Character Set Auto Detection' in the Use menu",
						"Encoding string detection error",
						MessageBoxButtons.OK, MessageBoxIcon.Warning);
					_charSet = "";
				}
			}
			finally
			{
				if(curHtmlDocument2 != null)
					curHtmlDocument2.close();
			}

			return newResponseString;
		}

		private string GetCharSet(string responseHeaderCharSet, IHTMLDocument3 curHtmlDoc3)
		{
			IHTMLElementCollection colMeta = null;
			string charSet = "";
			string charSetStringToBreak = "";
			string[] arrCharSetParts;
			int posCharSet = -1;

			colMeta = curHtmlDoc3.getElementsByTagName("meta");

			try
			{
				// First we check if the character set is not already in the header
				if(responseHeaderCharSet.Trim() != "")
					charSet = responseHeaderCharSet;
				else // We have to search it by ourself
				{
					foreach (IHTMLMetaElement curMeta in colMeta)
					{
						if(curMeta.httpEquiv != null)
						{
							if(curMeta.httpEquiv.ToUpper() == "CONTENT-TYPE")
							{
								if(curMeta.charset != null)
								{
									charSet = curMeta.charset;
									break;
								}
								else if(curMeta.content.ToUpper().IndexOf("CHARSET") != -1) // If there is charset settings
								{
									posCharSet = curMeta.content.ToUpper().IndexOf("CHARSET");
									charSetStringToBreak = curMeta.content.Substring(posCharSet);
									arrCharSetParts = charSetStringToBreak.Split('=');
									charSet = arrCharSetParts[arrCharSetParts.Length - 1].Trim();
									break;
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			return charSet;
		}

		private void CreateResponseMessageBox(string questionToAsk, string suffixComment, string title, Uri curURI)
		{
			string strNewURL = "";

			try
			{
				strNewURL = curURI.ToString().Replace("&amp;", "&");

				if(MessageBox.Show(questionToAsk + strNewURL + 
					"\n\nWould you like to automatically copy this new address in the URL?" +
					"\n\nIf you do so, you will need to reload the page to see the new result" + 
					suffixComment, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					txtURL.Text = strNewURL;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void ChangeHtmlContent(string htmlContent)
		{
			try
			{
				IHTMLDocument2 htmlDocument = (IHTMLDocument2)axwbHtmlResult.Document;

				htmlDocument.clear();
				htmlDocument.write(htmlContent);
				axwbHtmlResult.CtlRefresh();
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private Session BuildSessionFromForm(string sessionName, string fileName)
		{
			Session session = new Session();

			try
			{
				session.CreationDate = DateTime.Now;
				if(cmbDBType.SelectedIndex == 0)
					session.DatabaseType = Session.Database_Type.SQL_Server;
				else if(cmbDBType.SelectedIndex == 1)
					session.DatabaseType = Session.Database_Type.Oracle;
				else if(cmbDBType.SelectedIndex == 2)
					session.DatabaseType = Session.Database_Type.MySQL;
				else if(cmbDBType.SelectedIndex == 3)
					session.DatabaseType = Session.Database_Type.Sybase;
				else
					session.DatabaseType = Session.Database_Type.SQL_Server; // By default we save with SQL Server
				session.DelaySecond = Convert.ToByte(nudDelaySeconds.Value);
				session.Distinct = chkDistinct.Checked;
				session.FileName = fileName;

				session.HtmlForms = BuildHtmlForm();

				session.HtmlMessageLength = Convert.ToUInt16(nudHtmlMsgLength.Value);
				session.LoadedUri = txtURL.Text;
				session.CurrentSubmitSelectedIndex = cmbSubmitURL.SelectedIndex;
				session.Method = radPost.Checked ? Session.Method_Injection.POST : Session.Method_Injection.GET;
				session.NumberThreads = Convert.ToByte(nudNbrThread.Value);
				session.PositiveAnswer = txtPositiveAnswer.Text;
				session.ReplaceSpace = chkInsertEmptyComments.Checked;
				session.SessionName = sessionName;
				session.SqlPositiveInjectionResult = txtSQLErrorString.Text;
				session.StartingCount = Convert.ToUInt32(nudStartingCount.Value);
				session.StartingLength = Convert.ToUInt32(nudStartingLength.Value);
				session.Technique = radNormal.Checked ? Session.Technique_Injection.Normal : Session.Technique_Injection.Blind;
				session.TrapErrorString = chkTrapErrorString.Checked;
				if(radWord.Checked)
					session.TypeInjection = Session.Type_Injection.Word;
				else if(radLength.Checked)
					session.TypeInjection = Session.Type_Injection.Length;
				else
					session.TypeInjection = Session.Type_Injection.Count;
				session.UseCookie = mnUseCookie.Checked;
				session.AutoDetectEncoding = mnCharacterSetAutoDetect.Checked;
				session.WaitforDelay = ckbDelay.Checked;

				return session;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);

				return null;
			}
		}

		private ArrayList BuildHtmlForm()
		{
			ArrayList htmlForms = new ArrayList();
			IEnumerator htmlInputInjectEnum = _arrInputModifiable.GetEnumerator();
			HTMLForm curHtmlForm = null;
			int curFormNumber = 0;
			ArrayList curHtmlInputInject = new ArrayList();
			ArrayList curHtmlInputNotInject = new ArrayList();

			try
			{
				// Make sure it is at the first position
				htmlInputInjectEnum.Reset();

				// We can use either htmlInputInjectEnum or _arrInputNotModifiable since they both show
				// the number of array detected in the web page
				while(htmlInputInjectEnum.MoveNext())
				{
					curHtmlForm = new HTMLForm();

					curHtmlForm.SubmitUri = ((SubmitURLParam)cmbSubmitURL.Items[curFormNumber]).SubmitUri;

					if(htmlInputInjectEnum.Current != null)
					{	
						curHtmlInputInject = (ArrayList)htmlInputInjectEnum.Current;
												
						for(int curInputNumber=0;curInputNumber<curHtmlInputInject.Count;curInputNumber++)
							curHtmlForm.HtmlInputInjectArray.Add((CHtmlInputInject)curHtmlInputInject[curInputNumber]);
					}

					if(_arrInputNotModifiable[curFormNumber] != null)
					{	
						curHtmlInputNotInject = (ArrayList)_arrInputNotModifiable[curFormNumber];
												
						for(int curInputNumber=0;curInputNumber<curHtmlInputNotInject.Count;curInputNumber++)
							curHtmlForm.HtmlInputNotInjectArray.Add((CHtmlInputNotInject)curHtmlInputNotInject[curInputNumber]);
					}

					htmlForms.Add(curHtmlForm);

					curFormNumber++;
				}
				return htmlForms;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);

				return null;
			}
		}

		private bool LoadSessionInForm(Session session)
		{
			bool hasLoadedSession = false;

			try
			{
				ClearResults();

				if(LoadHtmlForms(session.HtmlForms, session.CurrentSubmitSelectedIndex))
				{
					if(session.DatabaseType == Session.Database_Type.SQL_Server)
						cmbDBType.SelectedIndex = 0;
					else if(session.DatabaseType == Session.Database_Type.Oracle)
						cmbDBType.SelectedIndex = 1;
					else if(session.DatabaseType == Session.Database_Type.MySQL)
						cmbDBType.SelectedIndex = 2;
					else if(session.DatabaseType == Session.Database_Type.Sybase)
						cmbDBType.SelectedIndex = 3;
					else
						cmbDBType.SelectedIndex = 0; // By default we save with SQL Server

					nudDelaySeconds.Value = session.DelaySecond;
					chkDistinct.Checked = session.Distinct;

					nudHtmlMsgLength.Value = session.HtmlMessageLength;
					txtURL.Text = session.LoadedUri;

					if(session.Method == Session.Method_Injection.POST)
					{
						radPost.Checked = true;
						radGet.Checked = false;
					}
					else
					{
						radPost.Checked = false;
						radGet.Checked = true;
					}
					nudNbrThread.Value = session.NumberThreads;
					txtPositiveAnswer.Text = session.PositiveAnswer;
					chkInsertEmptyComments.Checked = session.ReplaceSpace;
					txtSQLErrorString.Text = session.SqlPositiveInjectionResult;
					nudStartingCount.Value = session.StartingCount;
					nudStartingLength.Value = session.StartingLength;

					if(session.Technique == Session.Technique_Injection.Normal)
					{
						radNormal.Checked = true;
						radBlind.Checked = false;
					}
					else
					{
						radNormal.Checked = false;
						radBlind.Checked = true;
					}

					chkTrapErrorString.Checked = session.TrapErrorString;

					if(session.TypeInjection == Session.Type_Injection.Word)
					{
						radWord.Checked = true;
						radLength.Checked = false;
						radCount.Checked = false;
					}
					else if(session.TypeInjection == Session.Type_Injection.Length)
					{
						radWord.Checked = false;
						radLength.Checked = true;
						radCount.Checked = false;
					}
					else
					{
						radWord.Checked = false;
						radLength.Checked = false;
						radCount.Checked = true;
					}
					mnUseCookie.Checked = session.UseCookie;
					mnCharacterSetAutoDetect.Checked = session.AutoDetectEncoding;
					ckbDelay.Checked = session.WaitforDelay;

					hasLoadedSession = true;
				}

				return hasLoadedSession;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);

				return hasLoadedSession;
			}
		}

		private bool LoadHtmlForms(ArrayList forms, int submitUrlSelectedIndex)
		{
			IEnumerator enumForm = forms.GetEnumerator();
			IEnumerator enumHtmlInput = null;
			IEnumerator enumHtmlInputNotInject = null;
			HTMLForm curForm = null;
			CHtmlInputInject curHtmlInput = null;
			CHtmlInputNotInject curHtmlInputNotInject = null;
			string curUri = "";
			Brush curBrush = null;
			SubmitURLParam.Method curMethod = SubmitURLParam.Method.GET;
			int curFormIndex = 0;
			bool hasLoadedHtmlForms = false;

			try
			{
				// Reinitialize the loaded info 
				ClearLoadedInfo();

				if(forms.Count > 0)
				{
					_arrInputModifiable = new ArrayList[forms.Count];
					_arrInputNotModifiable = new ArrayList[forms.Count];

					enumForm.Reset();

					while(enumForm.MoveNext())
					{
						curForm = (HTMLForm)enumForm.Current;

						// Parse the submit uri to transform it in the object SubmitURLParam
						if(curForm.SubmitUri.StartsWith("[POST]"))
						{
							curBrush = Brushes.LightBlue;
							curMethod = SubmitURLParam.Method.POST;
						}
						else if(curForm.SubmitUri.StartsWith("[GET]"))
						{
							curBrush = Brushes.Beige;
							curMethod = SubmitURLParam.Method.GET;
						}
						else
						{
							curBrush = Brushes.LightBlue;
							curMethod = SubmitURLParam.Method.POST;
						}
						curUri = curForm.SubmitUri;
						cmbSubmitURL.Items.Add(new SubmitURLParam(curUri, curBrush, curMethod));

						enumHtmlInput = curForm.HtmlInputInjectArray.GetEnumerator();
						enumHtmlInput.Reset();

						while(enumHtmlInput.MoveNext())	
						{
							curHtmlInput = (CHtmlInputInject)enumHtmlInput.Current;
							if(_arrInputModifiable[curFormIndex] == null)
								_arrInputModifiable[curFormIndex] = new ArrayList();

							_arrInputModifiable[curFormIndex].Add(curHtmlInput);
						}

						enumHtmlInputNotInject = curForm.HtmlInputNotInjectArray.GetEnumerator();
						enumHtmlInputNotInject.Reset();

						while(enumHtmlInputNotInject.MoveNext())	
						{
							curHtmlInputNotInject = (CHtmlInputNotInject)enumHtmlInputNotInject.Current;
							if(_arrInputNotModifiable[curFormIndex] == null)
								_arrInputNotModifiable[curFormIndex] = new ArrayList();

							_arrInputNotModifiable[curFormIndex].Add(curHtmlInputNotInject);
						}

						curFormIndex++;
					}

					cmbSubmitURL.SelectedIndex = submitUrlSelectedIndex;
				
					dtgLoadedInput.DataSource = null;
					if(_arrInputModifiable.Length > 0)
						dtgLoadedInput.DataSource = _arrInputModifiable[submitUrlSelectedIndex];
				}

				hasLoadedHtmlForms = true;

				return hasLoadedHtmlForms;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);

				return hasLoadedHtmlForms;
			}
		}

		private void ClearResults()
		{
			try
			{
				if(_arrStatus != null) _arrStatus.Clear();

				tabHTML.SelectedIndex = 0;

				ChangeHtmlContent("");
				dtgCurrentState.DataSource = null;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void ClearLoadedInfo()
		{
			try
			{
				ClearResults();

				cmbSubmitURL.Items.Clear();

				_arrInputModifiable = null;
				_arrInputNotModifiable = null;
				
				dtgLoadedInput.DataSource = null;
				dtgLoadedInput.DTGToolTip.SetToolTip(dtgLoadedInput, "");
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#region Windows Form Designer generated code
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


		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Injector));
			this.grbParameters = new System.Windows.Forms.GroupBox();
			this.cmbSubmitURL = new System.Windows.Forms.ComboBox();
			this.chkInsertEmptyComments = new System.Windows.Forms.CheckBox();
			this.nudNbrThread = new System.Windows.Forms.NumericUpDown();
			this.lblNbrThread = new System.Windows.Forms.Label();
			this.nudStartingLength = new System.Windows.Forms.NumericUpDown();
			this.lblStartingLength = new System.Windows.Forms.Label();
			this.chkDistinct = new System.Windows.Forms.CheckBox();
			this.nudStartingCount = new System.Windows.Forms.NumericUpDown();
			this.lblStartingCount = new System.Windows.Forms.Label();
			this.lblStringParameters = new System.Windows.Forms.Label();
			this.grbDBType = new System.Windows.Forms.GroupBox();
			this.cmbDBType = new System.Windows.Forms.ComboBox();
			this.nudDelaySeconds = new System.Windows.Forms.NumericUpDown();
			this.lblDelaySeconds = new System.Windows.Forms.Label();
			this.ckbDelay = new System.Windows.Forms.CheckBox();
			this.lblSubmitURL = new System.Windows.Forms.Label();
			this.grbTechnique = new System.Windows.Forms.GroupBox();
			this.radBlind = new System.Windows.Forms.RadioButton();
			this.radNormal = new System.Windows.Forms.RadioButton();
			this.butStartStop = new System.Windows.Forms.Button();
			this.butLoadPage = new System.Windows.Forms.Button();
			this.txtURL = new System.Windows.Forms.TextBox();
			this.lblURL = new System.Windows.Forms.Label();
			this.grbMethod = new System.Windows.Forms.GroupBox();
			this.radGet = new System.Windows.Forms.RadioButton();
			this.radPost = new System.Windows.Forms.RadioButton();
			this.grbType = new System.Windows.Forms.GroupBox();
			this.radLength = new System.Windows.Forms.RadioButton();
			this.radCount = new System.Windows.Forms.RadioButton();
			this.radWord = new System.Windows.Forms.RadioButton();
			this.butPause = new System.Windows.Forms.Button();
			this.butClearResults = new System.Windows.Forms.Button();
			this.dtgLoadedInput = new SQLPowerInjector.MyDataGrid();
			this.txtPositiveAnswer = new System.Windows.Forms.TextBox();
			this.lblPositiveAnswer = new System.Windows.Forms.Label();
			this.txtWord = new System.Windows.Forms.TextBox();
			this.lblCurrent = new System.Windows.Forms.Label();
			this.lblLength = new System.Windows.Forms.Label();
			this.lblWord = new System.Windows.Forms.Label();
			this.txtLength = new System.Windows.Forms.TextBox();
			this.grbResults = new System.Windows.Forms.GroupBox();
			this.lblTimeTaken = new System.Windows.Forms.Label();
			this.txtTimeTaken = new System.Windows.Forms.TextBox();
			this.txtCurrentChar = new System.Windows.Forms.TextBox();
			this.lblCurrentChar = new System.Windows.Forms.Label();
			this.grbStatus = new System.Windows.Forms.GroupBox();
			this.lblProcessing = new System.Windows.Forms.Label();
			this.pbProcessing = new System.Windows.Forms.ProgressBar();
			this.axwbHtmlInitializor = new AxSHDocVw.AxWebBrowser();
			this.chkTrapErrorString = new System.Windows.Forms.CheckBox();
			this.tabHTML = new System.Windows.Forms.TabControl();
			this.tabpgHTML = new System.Windows.Forms.TabPage();
			this.axwbHtmlResult = new AxSHDocVw.AxWebBrowser();
			this.tabpgSource = new System.Windows.Forms.TabPage();
			this.rtxtViewSource = new System.Windows.Forms.RichTextBox();
			this.txtSQLErrorString = new System.Windows.Forms.TextBox();
			this.lblSQLErrorString = new System.Windows.Forms.Label();
			this.nudHtmlMsgLength = new System.Windows.Forms.NumericUpDown();
			this.lblHTMLMsgLength = new System.Windows.Forms.Label();
			this.dtgCurrentState = new SQLPowerInjector.MyDataGrid();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.mnFile = new System.Windows.Forms.MenuItem();
			this.mnSaveSession = new System.Windows.Forms.MenuItem();
			this.mnLoadSession = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.mnExit = new System.Windows.Forms.MenuItem();
			this.mnInsert = new System.Windows.Forms.MenuItem();
			this.mnInsertCookie = new System.Windows.Forms.MenuItem();
			this.mnUse = new System.Windows.Forms.MenuItem();
			this.mnUseCookie = new System.Windows.Forms.MenuItem();
			this.mnCharacterSetAutoDetect = new System.Windows.Forms.MenuItem();
			this.mnQuestion = new System.Windows.Forms.MenuItem();
			this.mnTutorial = new System.Windows.Forms.MenuItem();
			this.mnCheckForUpdates = new System.Windows.Forms.MenuItem();
			this.mnSite = new System.Windows.Forms.MenuItem();
			this.mnLine1 = new System.Windows.Forms.MenuItem();
			this.mnAbout = new System.Windows.Forms.MenuItem();
			this.sfdSaveSession = new System.Windows.Forms.SaveFileDialog();
			this.ofdLoadSession = new System.Windows.Forms.OpenFileDialog();
			this.grbParameters.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudNbrThread)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudStartingLength)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudStartingCount)).BeginInit();
			this.grbDBType.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudDelaySeconds)).BeginInit();
			this.grbTechnique.SuspendLayout();
			this.grbMethod.SuspendLayout();
			this.grbType.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dtgLoadedInput)).BeginInit();
			this.grbResults.SuspendLayout();
			this.grbStatus.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.axwbHtmlInitializor)).BeginInit();
			this.tabHTML.SuspendLayout();
			this.tabpgHTML.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.axwbHtmlResult)).BeginInit();
			this.tabpgSource.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudHtmlMsgLength)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dtgCurrentState)).BeginInit();
			this.SuspendLayout();
			// 
			// grbParameters
			// 
			this.grbParameters.BackColor = System.Drawing.Color.Transparent;
			this.grbParameters.Controls.Add(this.cmbSubmitURL);
			this.grbParameters.Controls.Add(this.chkInsertEmptyComments);
			this.grbParameters.Controls.Add(this.nudNbrThread);
			this.grbParameters.Controls.Add(this.lblNbrThread);
			this.grbParameters.Controls.Add(this.nudStartingLength);
			this.grbParameters.Controls.Add(this.lblStartingLength);
			this.grbParameters.Controls.Add(this.chkDistinct);
			this.grbParameters.Controls.Add(this.nudStartingCount);
			this.grbParameters.Controls.Add(this.lblStartingCount);
			this.grbParameters.Controls.Add(this.lblStringParameters);
			this.grbParameters.Controls.Add(this.grbDBType);
			this.grbParameters.Controls.Add(this.nudDelaySeconds);
			this.grbParameters.Controls.Add(this.lblDelaySeconds);
			this.grbParameters.Controls.Add(this.ckbDelay);
			this.grbParameters.Controls.Add(this.lblSubmitURL);
			this.grbParameters.Controls.Add(this.grbTechnique);
			this.grbParameters.Controls.Add(this.butStartStop);
			this.grbParameters.Controls.Add(this.butLoadPage);
			this.grbParameters.Controls.Add(this.txtURL);
			this.grbParameters.Controls.Add(this.lblURL);
			this.grbParameters.Controls.Add(this.grbMethod);
			this.grbParameters.Controls.Add(this.grbType);
			this.grbParameters.Controls.Add(this.butPause);
			this.grbParameters.Controls.Add(this.butClearResults);
			this.grbParameters.Controls.Add(this.dtgLoadedInput);
			this.grbParameters.Controls.Add(this.txtPositiveAnswer);
			this.grbParameters.Controls.Add(this.lblPositiveAnswer);
			this.grbParameters.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.grbParameters.ForeColor = System.Drawing.Color.White;
			this.grbParameters.Location = new System.Drawing.Point(8, 8);
			this.grbParameters.Name = "grbParameters";
			this.grbParameters.Size = new System.Drawing.Size(992, 240);
			this.grbParameters.TabIndex = 6;
			this.grbParameters.TabStop = false;
			this.grbParameters.Text = "Parameters";
			// 
			// cmbSubmitURL
			// 
			this.cmbSubmitURL.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.cmbSubmitURL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbSubmitURL.Location = new System.Drawing.Point(96, 40);
			this.cmbSubmitURL.Name = "cmbSubmitURL";
			this.cmbSubmitURL.Size = new System.Drawing.Size(608, 21);
			this.cmbSubmitURL.TabIndex = 41;
			this.cmbSubmitURL.SelectedIndexChanged += new System.EventHandler(this.cmbSubmitURL_SelectedIndexChanged);
			this.cmbSubmitURL.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.DrawItemHandler);
			// 
			// chkInsertEmptyComments
			// 
			this.chkInsertEmptyComments.Location = new System.Drawing.Point(808, 72);
			this.chkInsertEmptyComments.Name = "chkInsertEmptyComments";
			this.chkInsertEmptyComments.Size = new System.Drawing.Size(168, 16);
			this.chkInsertEmptyComments.TabIndex = 40;
			this.chkInsertEmptyComments.Text = "Replace Space by /* */";
			// 
			// nudNbrThread
			// 
			this.nudNbrThread.Location = new System.Drawing.Point(928, 112);
			this.nudNbrThread.Maximum = new System.Decimal(new int[] {
																		 8,
																		 0,
																		 0,
																		 0});
			this.nudNbrThread.Minimum = new System.Decimal(new int[] {
																		 1,
																		 0,
																		 0,
																		 0});
			this.nudNbrThread.Name = "nudNbrThread";
			this.nudNbrThread.Size = new System.Drawing.Size(40, 20);
			this.nudNbrThread.TabIndex = 38;
			this.nudNbrThread.Value = new System.Decimal(new int[] {
																	   1,
																	   0,
																	   0,
																	   0});
			this.nudNbrThread.Visible = false;
			// 
			// lblNbrThread
			// 
			this.lblNbrThread.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblNbrThread.ForeColor = System.Drawing.Color.White;
			this.lblNbrThread.Location = new System.Drawing.Point(808, 112);
			this.lblNbrThread.Name = "lblNbrThread";
			this.lblNbrThread.Size = new System.Drawing.Size(112, 16);
			this.lblNbrThread.TabIndex = 37;
			this.lblNbrThread.Text = "Number of Threads";
			this.lblNbrThread.Visible = false;
			// 
			// nudStartingLength
			// 
			this.nudStartingLength.Location = new System.Drawing.Point(520, 112);
			this.nudStartingLength.Maximum = new System.Decimal(new int[] {
																			  10000000,
																			  0,
																			  0,
																			  0});
			this.nudStartingLength.Name = "nudStartingLength";
			this.nudStartingLength.Size = new System.Drawing.Size(88, 20);
			this.nudStartingLength.TabIndex = 36;
			this.nudStartingLength.Value = new System.Decimal(new int[] {
																			100,
																			0,
																			0,
																			0});
			this.nudStartingLength.Visible = false;
			// 
			// lblStartingLength
			// 
			this.lblStartingLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblStartingLength.ForeColor = System.Drawing.Color.White;
			this.lblStartingLength.Location = new System.Drawing.Point(424, 112);
			this.lblStartingLength.Name = "lblStartingLength";
			this.lblStartingLength.Size = new System.Drawing.Size(96, 16);
			this.lblStartingLength.TabIndex = 35;
			this.lblStartingLength.Text = "Starting Length";
			this.lblStartingLength.Visible = false;
			// 
			// chkDistinct
			// 
			this.chkDistinct.Location = new System.Drawing.Point(704, 72);
			this.chkDistinct.Name = "chkDistinct";
			this.chkDistinct.Size = new System.Drawing.Size(88, 16);
			this.chkDistinct.TabIndex = 34;
			this.chkDistinct.Text = "Distinct";
			this.chkDistinct.Visible = false;
			// 
			// nudStartingCount
			// 
			this.nudStartingCount.Location = new System.Drawing.Point(704, 112);
			this.nudStartingCount.Maximum = new System.Decimal(new int[] {
																			 10000000,
																			 0,
																			 0,
																			 0});
			this.nudStartingCount.Name = "nudStartingCount";
			this.nudStartingCount.Size = new System.Drawing.Size(88, 20);
			this.nudStartingCount.TabIndex = 33;
			this.nudStartingCount.Value = new System.Decimal(new int[] {
																		   10,
																		   0,
																		   0,
																		   0});
			this.nudStartingCount.Visible = false;
			// 
			// lblStartingCount
			// 
			this.lblStartingCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblStartingCount.ForeColor = System.Drawing.Color.White;
			this.lblStartingCount.Location = new System.Drawing.Point(616, 112);
			this.lblStartingCount.Name = "lblStartingCount";
			this.lblStartingCount.Size = new System.Drawing.Size(80, 23);
			this.lblStartingCount.TabIndex = 32;
			this.lblStartingCount.Text = "Starting Count";
			this.lblStartingCount.Visible = false;
			// 
			// lblStringParameters
			// 
			this.lblStringParameters.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblStringParameters.ForeColor = System.Drawing.Color.White;
			this.lblStringParameters.Location = new System.Drawing.Point(8, 136);
			this.lblStringParameters.Name = "lblStringParameters";
			this.lblStringParameters.Size = new System.Drawing.Size(104, 16);
			this.lblStringParameters.TabIndex = 30;
			this.lblStringParameters.Text = "String Parameters";
			// 
			// grbDBType
			// 
			this.grbDBType.Controls.Add(this.cmbDBType);
			this.grbDBType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.grbDBType.ForeColor = System.Drawing.Color.White;
			this.grbDBType.Location = new System.Drawing.Point(8, 64);
			this.grbDBType.Name = "grbDBType";
			this.grbDBType.Size = new System.Drawing.Size(168, 40);
			this.grbDBType.TabIndex = 29;
			this.grbDBType.TabStop = false;
			this.grbDBType.Text = "Database Type";
			// 
			// cmbDBType
			// 
			this.cmbDBType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbDBType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.cmbDBType.ItemHeight = 13;
			this.cmbDBType.Items.AddRange(new object[] {
														   "SQL Server",
														   "Oracle",
														   "MySQL",
														   "Sybase/Adaptive Server Enterprise"});
			this.cmbDBType.Location = new System.Drawing.Point(8, 16);
			this.cmbDBType.Name = "cmbDBType";
			this.cmbDBType.Size = new System.Drawing.Size(144, 21);
			this.cmbDBType.TabIndex = 2;
			// 
			// nudDelaySeconds
			// 
			this.nudDelaySeconds.Location = new System.Drawing.Point(928, 88);
			this.nudDelaySeconds.Maximum = new System.Decimal(new int[] {
																			255,
																			0,
																			0,
																			0});
			this.nudDelaySeconds.Name = "nudDelaySeconds";
			this.nudDelaySeconds.Size = new System.Drawing.Size(40, 20);
			this.nudDelaySeconds.TabIndex = 28;
			this.nudDelaySeconds.Visible = false;
			// 
			// lblDelaySeconds
			// 
			this.lblDelaySeconds.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblDelaySeconds.ForeColor = System.Drawing.Color.White;
			this.lblDelaySeconds.Location = new System.Drawing.Point(808, 90);
			this.lblDelaySeconds.Name = "lblDelaySeconds";
			this.lblDelaySeconds.Size = new System.Drawing.Size(104, 16);
			this.lblDelaySeconds.TabIndex = 27;
			this.lblDelaySeconds.Text = "Delay in seconds";
			this.lblDelaySeconds.Visible = false;
			// 
			// ckbDelay
			// 
			this.ckbDelay.Location = new System.Drawing.Point(704, 88);
			this.ckbDelay.Name = "ckbDelay";
			this.ckbDelay.Size = new System.Drawing.Size(96, 16);
			this.ckbDelay.TabIndex = 25;
			this.ckbDelay.Text = "Waitfor delay";
			this.ckbDelay.CheckedChanged += new System.EventHandler(this.ckbDelay_CheckedChanged);
			// 
			// lblSubmitURL
			// 
			this.lblSubmitURL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblSubmitURL.ForeColor = System.Drawing.Color.White;
			this.lblSubmitURL.Location = new System.Drawing.Point(8, 40);
			this.lblSubmitURL.Name = "lblSubmitURL";
			this.lblSubmitURL.Size = new System.Drawing.Size(80, 23);
			this.lblSubmitURL.TabIndex = 23;
			this.lblSubmitURL.Text = "Submit URL";
			// 
			// grbTechnique
			// 
			this.grbTechnique.Controls.Add(this.radBlind);
			this.grbTechnique.Controls.Add(this.radNormal);
			this.grbTechnique.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.grbTechnique.ForeColor = System.Drawing.Color.White;
			this.grbTechnique.Location = new System.Drawing.Point(320, 64);
			this.grbTechnique.Name = "grbTechnique";
			this.grbTechnique.Size = new System.Drawing.Size(144, 40);
			this.grbTechnique.TabIndex = 22;
			this.grbTechnique.TabStop = false;
			this.grbTechnique.Text = "Technique";
			// 
			// radBlind
			// 
			this.radBlind.Location = new System.Drawing.Point(80, 16);
			this.radBlind.Name = "radBlind";
			this.radBlind.Size = new System.Drawing.Size(56, 16);
			this.radBlind.TabIndex = 1;
			this.radBlind.Text = "Blind";
			this.radBlind.CheckedChanged += new System.EventHandler(this.radBlind_CheckedChanged);
			// 
			// radNormal
			// 
			this.radNormal.Checked = true;
			this.radNormal.Location = new System.Drawing.Point(8, 16);
			this.radNormal.Name = "radNormal";
			this.radNormal.Size = new System.Drawing.Size(64, 16);
			this.radNormal.TabIndex = 0;
			this.radNormal.TabStop = true;
			this.radNormal.Text = "Normal";
			this.radNormal.CheckedChanged += new System.EventHandler(this.radNormal_CheckedChanged);
			// 
			// butStartStop
			// 
			this.butStartStop.BackColor = System.Drawing.Color.LightSlateGray;
			this.butStartStop.Enabled = false;
			this.butStartStop.Location = new System.Drawing.Point(856, 16);
			this.butStartStop.Name = "butStartStop";
			this.butStartStop.Size = new System.Drawing.Size(128, 24);
			this.butStartStop.TabIndex = 21;
			this.butStartStop.Text = "&Start";
			this.butStartStop.Click += new System.EventHandler(this.butStartStop_Click);
			// 
			// butLoadPage
			// 
			this.butLoadPage.BackColor = System.Drawing.Color.LightSlateGray;
			this.butLoadPage.Location = new System.Drawing.Point(720, 16);
			this.butLoadPage.Name = "butLoadPage";
			this.butLoadPage.Size = new System.Drawing.Size(128, 24);
			this.butLoadPage.TabIndex = 19;
			this.butLoadPage.Text = "&Load Page";
			this.butLoadPage.Click += new System.EventHandler(this.butLoadPage_Click);
			// 
			// txtURL
			// 
			this.txtURL.Location = new System.Drawing.Point(96, 16);
			this.txtURL.Name = "txtURL";
			this.txtURL.Size = new System.Drawing.Size(608, 20);
			this.txtURL.TabIndex = 17;
			this.txtURL.Text = "";
			// 
			// lblURL
			// 
			this.lblURL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblURL.ForeColor = System.Drawing.Color.White;
			this.lblURL.Location = new System.Drawing.Point(8, 16);
			this.lblURL.Name = "lblURL";
			this.lblURL.Size = new System.Drawing.Size(80, 16);
			this.lblURL.TabIndex = 18;
			this.lblURL.Text = "URL";
			// 
			// grbMethod
			// 
			this.grbMethod.Controls.Add(this.radGet);
			this.grbMethod.Controls.Add(this.radPost);
			this.grbMethod.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.grbMethod.ForeColor = System.Drawing.Color.White;
			this.grbMethod.Location = new System.Drawing.Point(184, 64);
			this.grbMethod.Name = "grbMethod";
			this.grbMethod.Size = new System.Drawing.Size(128, 40);
			this.grbMethod.TabIndex = 16;
			this.grbMethod.TabStop = false;
			this.grbMethod.Text = "Method";
			// 
			// radGet
			// 
			this.radGet.Location = new System.Drawing.Point(72, 16);
			this.radGet.Name = "radGet";
			this.radGet.Size = new System.Drawing.Size(48, 16);
			this.radGet.TabIndex = 1;
			this.radGet.Text = "GET";
			// 
			// radPost
			// 
			this.radPost.Checked = true;
			this.radPost.Location = new System.Drawing.Point(8, 16);
			this.radPost.Name = "radPost";
			this.radPost.Size = new System.Drawing.Size(56, 16);
			this.radPost.TabIndex = 0;
			this.radPost.TabStop = true;
			this.radPost.Text = "POST";
			// 
			// grbType
			// 
			this.grbType.Controls.Add(this.radLength);
			this.grbType.Controls.Add(this.radCount);
			this.grbType.Controls.Add(this.radWord);
			this.grbType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.grbType.ForeColor = System.Drawing.Color.White;
			this.grbType.Location = new System.Drawing.Point(472, 64);
			this.grbType.Name = "grbType";
			this.grbType.Size = new System.Drawing.Size(208, 40);
			this.grbType.TabIndex = 15;
			this.grbType.TabStop = false;
			this.grbType.Text = "Type";
			this.grbType.Visible = false;
			// 
			// radLength
			// 
			this.radLength.Location = new System.Drawing.Point(72, 16);
			this.radLength.Name = "radLength";
			this.radLength.Size = new System.Drawing.Size(64, 16);
			this.radLength.TabIndex = 2;
			this.radLength.Text = "Length";
			this.radLength.CheckedChanged += new System.EventHandler(this.radLength_CheckedChanged);
			// 
			// radCount
			// 
			this.radCount.Location = new System.Drawing.Point(144, 16);
			this.radCount.Name = "radCount";
			this.radCount.Size = new System.Drawing.Size(56, 16);
			this.radCount.TabIndex = 1;
			this.radCount.Text = "Count";
			this.radCount.CheckedChanged += new System.EventHandler(this.radCount_CheckedChanged);
			// 
			// radWord
			// 
			this.radWord.Checked = true;
			this.radWord.Location = new System.Drawing.Point(8, 16);
			this.radWord.Name = "radWord";
			this.radWord.Size = new System.Drawing.Size(64, 16);
			this.radWord.TabIndex = 0;
			this.radWord.TabStop = true;
			this.radWord.Text = "Word";
			this.radWord.CheckedChanged += new System.EventHandler(this.radWord_CheckedChanged);
			// 
			// butPause
			// 
			this.butPause.BackColor = System.Drawing.Color.LightSlateGray;
			this.butPause.Enabled = false;
			this.butPause.Location = new System.Drawing.Point(856, 40);
			this.butPause.Name = "butPause";
			this.butPause.Size = new System.Drawing.Size(128, 24);
			this.butPause.TabIndex = 13;
			this.butPause.Text = "&Pause";
			this.butPause.Click += new System.EventHandler(this.butPause_Click);
			// 
			// butClearResults
			// 
			this.butClearResults.BackColor = System.Drawing.Color.LightSlateGray;
			this.butClearResults.Location = new System.Drawing.Point(720, 40);
			this.butClearResults.Name = "butClearResults";
			this.butClearResults.Size = new System.Drawing.Size(128, 24);
			this.butClearResults.TabIndex = 12;
			this.butClearResults.Text = "&Clear Results";
			this.butClearResults.Click += new System.EventHandler(this.butClearResults_Click);
			// 
			// dtgLoadedInput
			// 
			this.dtgLoadedInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.dtgLoadedInput.CaptionVisible = false;
			this.dtgLoadedInput.DataMember = "";
			this.dtgLoadedInput.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dtgLoadedInput.HitRow = -1;
			this.dtgLoadedInput.Location = new System.Drawing.Point(8, 152);
			this.dtgLoadedInput.Name = "dtgLoadedInput";
			this.dtgLoadedInput.PreferredColumnWidth = 300;
			this.dtgLoadedInput.RowHeadersVisible = false;
			this.dtgLoadedInput.Size = new System.Drawing.Size(976, 80);
			this.dtgLoadedInput.TabIndex = 14;
			this.dtgLoadedInput.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dtgLoadedInput_MouseUp);
			this.dtgLoadedInput.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dtgLoadedInput_HandleMouseMove);
			this.dtgLoadedInput.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dtgLoadedInput_KeyUp);
			// 
			// txtPositiveAnswer
			// 
			this.txtPositiveAnswer.Location = new System.Drawing.Point(104, 112);
			this.txtPositiveAnswer.Name = "txtPositiveAnswer";
			this.txtPositiveAnswer.Size = new System.Drawing.Size(312, 20);
			this.txtPositiveAnswer.TabIndex = 7;
			this.txtPositiveAnswer.Text = "";
			// 
			// lblPositiveAnswer
			// 
			this.lblPositiveAnswer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblPositiveAnswer.ForeColor = System.Drawing.Color.White;
			this.lblPositiveAnswer.Location = new System.Drawing.Point(8, 112);
			this.lblPositiveAnswer.Name = "lblPositiveAnswer";
			this.lblPositiveAnswer.Size = new System.Drawing.Size(88, 23);
			this.lblPositiveAnswer.TabIndex = 8;
			this.lblPositiveAnswer.Text = "Positive Answer";
			// 
			// txtWord
			// 
			this.txtWord.Location = new System.Drawing.Point(272, 24);
			this.txtWord.Name = "txtWord";
			this.txtWord.ReadOnly = true;
			this.txtWord.Size = new System.Drawing.Size(520, 20);
			this.txtWord.TabIndex = 8;
			this.txtWord.Text = "";
			// 
			// lblCurrent
			// 
			this.lblCurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblCurrent.ForeColor = System.Drawing.Color.White;
			this.lblCurrent.Location = new System.Drawing.Point(8, 16);
			this.lblCurrent.Name = "lblCurrent";
			this.lblCurrent.Size = new System.Drawing.Size(200, 23);
			this.lblCurrent.TabIndex = 10;
			this.lblCurrent.Text = "Current String";
			// 
			// lblLength
			// 
			this.lblLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblLength.ForeColor = System.Drawing.Color.White;
			this.lblLength.Location = new System.Drawing.Point(128, 24);
			this.lblLength.Name = "lblLength";
			this.lblLength.Size = new System.Drawing.Size(40, 23);
			this.lblLength.TabIndex = 11;
			this.lblLength.Text = "Length";
			// 
			// lblWord
			// 
			this.lblWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblWord.ForeColor = System.Drawing.Color.White;
			this.lblWord.Location = new System.Drawing.Point(232, 24);
			this.lblWord.Name = "lblWord";
			this.lblWord.Size = new System.Drawing.Size(32, 23);
			this.lblWord.TabIndex = 12;
			this.lblWord.Text = "Word";
			// 
			// txtLength
			// 
			this.txtLength.Location = new System.Drawing.Point(176, 24);
			this.txtLength.Name = "txtLength";
			this.txtLength.ReadOnly = true;
			this.txtLength.Size = new System.Drawing.Size(48, 20);
			this.txtLength.TabIndex = 13;
			this.txtLength.Text = "";
			this.txtLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// grbResults
			// 
			this.grbResults.BackColor = System.Drawing.Color.Transparent;
			this.grbResults.Controls.Add(this.lblTimeTaken);
			this.grbResults.Controls.Add(this.txtTimeTaken);
			this.grbResults.Controls.Add(this.txtCurrentChar);
			this.grbResults.Controls.Add(this.lblCurrentChar);
			this.grbResults.Controls.Add(this.lblLength);
			this.grbResults.Controls.Add(this.txtLength);
			this.grbResults.Controls.Add(this.lblWord);
			this.grbResults.Controls.Add(this.txtWord);
			this.grbResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.grbResults.ForeColor = System.Drawing.Color.White;
			this.grbResults.Location = new System.Drawing.Point(8, 256);
			this.grbResults.Name = "grbResults";
			this.grbResults.Size = new System.Drawing.Size(992, 56);
			this.grbResults.TabIndex = 14;
			this.grbResults.TabStop = false;
			this.grbResults.Text = "Results";
			this.grbResults.Visible = false;
			// 
			// lblTimeTaken
			// 
			this.lblTimeTaken.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblTimeTaken.ForeColor = System.Drawing.Color.White;
			this.lblTimeTaken.Location = new System.Drawing.Point(800, 24);
			this.lblTimeTaken.Name = "lblTimeTaken";
			this.lblTimeTaken.Size = new System.Drawing.Size(64, 23);
			this.lblTimeTaken.TabIndex = 24;
			this.lblTimeTaken.Text = "Time taken";
			// 
			// txtTimeTaken
			// 
			this.txtTimeTaken.Location = new System.Drawing.Point(872, 24);
			this.txtTimeTaken.Name = "txtTimeTaken";
			this.txtTimeTaken.ReadOnly = true;
			this.txtTimeTaken.Size = new System.Drawing.Size(112, 20);
			this.txtTimeTaken.TabIndex = 25;
			this.txtTimeTaken.Text = "";
			this.txtTimeTaken.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtCurrentChar
			// 
			this.txtCurrentChar.Location = new System.Drawing.Point(88, 24);
			this.txtCurrentChar.Name = "txtCurrentChar";
			this.txtCurrentChar.ReadOnly = true;
			this.txtCurrentChar.Size = new System.Drawing.Size(32, 20);
			this.txtCurrentChar.TabIndex = 23;
			this.txtCurrentChar.Text = "";
			this.txtCurrentChar.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// lblCurrentChar
			// 
			this.lblCurrentChar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblCurrentChar.ForeColor = System.Drawing.Color.White;
			this.lblCurrentChar.Location = new System.Drawing.Point(8, 24);
			this.lblCurrentChar.Name = "lblCurrentChar";
			this.lblCurrentChar.Size = new System.Drawing.Size(80, 23);
			this.lblCurrentChar.TabIndex = 22;
			this.lblCurrentChar.Text = "Current Char";
			// 
			// grbStatus
			// 
			this.grbStatus.BackColor = System.Drawing.Color.Transparent;
			this.grbStatus.Controls.Add(this.lblProcessing);
			this.grbStatus.Controls.Add(this.pbProcessing);
			this.grbStatus.Controls.Add(this.axwbHtmlInitializor);
			this.grbStatus.Controls.Add(this.chkTrapErrorString);
			this.grbStatus.Controls.Add(this.tabHTML);
			this.grbStatus.Controls.Add(this.txtSQLErrorString);
			this.grbStatus.Controls.Add(this.lblSQLErrorString);
			this.grbStatus.Controls.Add(this.nudHtmlMsgLength);
			this.grbStatus.Controls.Add(this.lblHTMLMsgLength);
			this.grbStatus.Controls.Add(this.dtgCurrentState);
			this.grbStatus.Controls.Add(this.lblCurrent);
			this.grbStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.grbStatus.ForeColor = System.Drawing.Color.White;
			this.grbStatus.Location = new System.Drawing.Point(8, 320);
			this.grbStatus.Name = "grbStatus";
			this.grbStatus.Size = new System.Drawing.Size(992, 360);
			this.grbStatus.TabIndex = 15;
			this.grbStatus.TabStop = false;
			this.grbStatus.Text = "Status";
			// 
			// lblProcessing
			// 
			this.lblProcessing.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblProcessing.ForeColor = System.Drawing.Color.White;
			this.lblProcessing.Location = new System.Drawing.Point(376, 16);
			this.lblProcessing.Name = "lblProcessing";
			this.lblProcessing.Size = new System.Drawing.Size(112, 16);
			this.lblProcessing.TabIndex = 29;
			this.lblProcessing.Text = "Processing... 0%";
			this.lblProcessing.Visible = false;
			// 
			// pbProcessing
			// 
			this.pbProcessing.Location = new System.Drawing.Point(488, 16);
			this.pbProcessing.Name = "pbProcessing";
			this.pbProcessing.Size = new System.Drawing.Size(496, 16);
			this.pbProcessing.TabIndex = 28;
			this.pbProcessing.Visible = false;
			// 
			// axwbHtmlInitializor
			// 
			this.axwbHtmlInitializor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.axwbHtmlInitializor.ContainingControl = this;
			this.axwbHtmlInitializor.Enabled = true;
			this.axwbHtmlInitializor.Location = new System.Drawing.Point(784, 320);
			this.axwbHtmlInitializor.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axwbHtmlInitializor.OcxState")));
			this.axwbHtmlInitializor.Size = new System.Drawing.Size(300, 150);
			this.axwbHtmlInitializor.TabIndex = 24;
			this.axwbHtmlInitializor.BeforeNavigate2 += new AxSHDocVw.DWebBrowserEvents2_BeforeNavigate2EventHandler(this.axwbHtmlInitializor_beforeNavigate2);
			// 
			// chkTrapErrorString
			// 
			this.chkTrapErrorString.Location = new System.Drawing.Point(768, 176);
			this.chkTrapErrorString.Name = "chkTrapErrorString";
			this.chkTrapErrorString.Size = new System.Drawing.Size(200, 24);
			this.chkTrapErrorString.TabIndex = 23;
			this.chkTrapErrorString.Text = "Trap Error String?";
			// 
			// tabHTML
			// 
			this.tabHTML.Controls.Add(this.tabpgHTML);
			this.tabHTML.Controls.Add(this.tabpgSource);
			this.tabHTML.Location = new System.Drawing.Point(8, 72);
			this.tabHTML.Name = "tabHTML";
			this.tabHTML.SelectedIndex = 0;
			this.tabHTML.Size = new System.Drawing.Size(752, 280);
			this.tabHTML.TabIndex = 20;
			this.tabHTML.SelectedIndexChanged += new System.EventHandler(this.tabHTML_SelectedIndexChanged);
			// 
			// tabpgHTML
			// 
			this.tabpgHTML.Controls.Add(this.axwbHtmlResult);
			this.tabpgHTML.Location = new System.Drawing.Point(4, 22);
			this.tabpgHTML.Name = "tabpgHTML";
			this.tabpgHTML.Size = new System.Drawing.Size(744, 254);
			this.tabpgHTML.TabIndex = 0;
			this.tabpgHTML.Text = "HTML Result or Error";
			// 
			// axwbHtmlResult
			// 
			this.axwbHtmlResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.axwbHtmlResult.ContainingControl = this;
			this.axwbHtmlResult.Enabled = true;
			this.axwbHtmlResult.Location = new System.Drawing.Point(0, 0);
			this.axwbHtmlResult.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axwbHtmlResult.OcxState")));
			this.axwbHtmlResult.Size = new System.Drawing.Size(744, 240);
			this.axwbHtmlResult.TabIndex = 14;
			this.axwbHtmlResult.NavigateComplete2 += new AxSHDocVw.DWebBrowserEvents2_NavigateComplete2EventHandler(this.axwbHtmlResult_navigateComplete2);
			// 
			// tabpgSource
			// 
			this.tabpgSource.Controls.Add(this.rtxtViewSource);
			this.tabpgSource.Location = new System.Drawing.Point(4, 22);
			this.tabpgSource.Name = "tabpgSource";
			this.tabpgSource.Size = new System.Drawing.Size(744, 254);
			this.tabpgSource.TabIndex = 1;
			this.tabpgSource.Text = "View Source";
			// 
			// rtxtViewSource
			// 
			this.rtxtViewSource.Location = new System.Drawing.Point(0, 8);
			this.rtxtViewSource.Name = "rtxtViewSource";
			this.rtxtViewSource.Size = new System.Drawing.Size(736, 264);
			this.rtxtViewSource.TabIndex = 0;
			this.rtxtViewSource.Text = "";
			// 
			// txtSQLErrorString
			// 
			this.txtSQLErrorString.Location = new System.Drawing.Point(768, 152);
			this.txtSQLErrorString.Name = "txtSQLErrorString";
			this.txtSQLErrorString.Size = new System.Drawing.Size(216, 20);
			this.txtSQLErrorString.TabIndex = 19;
			this.txtSQLErrorString.Text = "";
			// 
			// lblSQLErrorString
			// 
			this.lblSQLErrorString.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblSQLErrorString.ForeColor = System.Drawing.Color.White;
			this.lblSQLErrorString.Location = new System.Drawing.Point(768, 128);
			this.lblSQLErrorString.Name = "lblSQLErrorString";
			this.lblSQLErrorString.Size = new System.Drawing.Size(168, 23);
			this.lblSQLErrorString.TabIndex = 18;
			this.lblSQLErrorString.Text = "SQL positive injection result";
			// 
			// nudHtmlMsgLength
			// 
			this.nudHtmlMsgLength.Location = new System.Drawing.Point(768, 96);
			this.nudHtmlMsgLength.Maximum = new System.Decimal(new int[] {
																			 10000,
																			 0,
																			 0,
																			 0});
			this.nudHtmlMsgLength.Name = "nudHtmlMsgLength";
			this.nudHtmlMsgLength.Size = new System.Drawing.Size(128, 20);
			this.nudHtmlMsgLength.TabIndex = 16;
			this.nudHtmlMsgLength.Value = new System.Decimal(new int[] {
																		   256,
																		   0,
																		   0,
																		   0});
			// 
			// lblHTMLMsgLength
			// 
			this.lblHTMLMsgLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblHTMLMsgLength.ForeColor = System.Drawing.Color.White;
			this.lblHTMLMsgLength.Location = new System.Drawing.Point(768, 72);
			this.lblHTMLMsgLength.Name = "lblHTMLMsgLength";
			this.lblHTMLMsgLength.Size = new System.Drawing.Size(136, 23);
			this.lblHTMLMsgLength.TabIndex = 15;
			this.lblHTMLMsgLength.Text = "HTML Message Length";
			// 
			// dtgCurrentState
			// 
			this.dtgCurrentState.CaptionVisible = false;
			this.dtgCurrentState.ColumnHeadersVisible = false;
			this.dtgCurrentState.DataMember = "";
			this.dtgCurrentState.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dtgCurrentState.HitRow = -1;
			this.dtgCurrentState.Location = new System.Drawing.Point(8, 40);
			this.dtgCurrentState.Name = "dtgCurrentState";
			this.dtgCurrentState.PreferredColumnWidth = 300;
			this.dtgCurrentState.ReadOnly = true;
			this.dtgCurrentState.RowHeadersVisible = false;
			this.dtgCurrentState.Size = new System.Drawing.Size(976, 24);
			this.dtgCurrentState.TabIndex = 13;
			this.dtgCurrentState.CurrentCellChanged += new System.EventHandler(this.dtgCurrentState_CurCellChange);
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnFile,
																					  this.mnInsert,
																					  this.mnUse,
																					  this.mnQuestion});
			// 
			// mnFile
			// 
			this.mnFile.Index = 0;
			this.mnFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				   this.mnSaveSession,
																				   this.mnLoadSession,
																				   this.menuItem3,
																				   this.mnExit});
			this.mnFile.Text = "&File";
			// 
			// mnSaveSession
			// 
			this.mnSaveSession.Enabled = false;
			this.mnSaveSession.Index = 0;
			this.mnSaveSession.Text = "&Save Session";
			this.mnSaveSession.Click += new System.EventHandler(this.mnSaveSession_Click);
			// 
			// mnLoadSession
			// 
			this.mnLoadSession.Index = 1;
			this.mnLoadSession.Text = "&Load Session";
			this.mnLoadSession.Click += new System.EventHandler(this.mnLoadSession_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 2;
			this.menuItem3.Text = "-";
			// 
			// mnExit
			// 
			this.mnExit.Index = 3;
			this.mnExit.Text = "&Exit";
			this.mnExit.Click += new System.EventHandler(this.mnExit_Click);
			// 
			// mnInsert
			// 
			this.mnInsert.Index = 1;
			this.mnInsert.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.mnInsertCookie});
			this.mnInsert.Text = "&Insert";
			// 
			// mnInsertCookie
			// 
			this.mnInsertCookie.Index = 0;
			this.mnInsertCookie.Text = "&Cookie";
			this.mnInsertCookie.Click += new System.EventHandler(this.mnCookie_Click);
			// 
			// mnUse
			// 
			this.mnUse.Index = 2;
			this.mnUse.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				  this.mnUseCookie,
																				  this.mnCharacterSetAutoDetect});
			this.mnUse.Text = "&Use";
			// 
			// mnUseCookie
			// 
			this.mnUseCookie.Checked = true;
			this.mnUseCookie.Index = 0;
			this.mnUseCookie.Text = "&Cookie";
			this.mnUseCookie.Click += new System.EventHandler(this.mnUseCookie_Click);
			// 
			// mnCharacterSetAutoDetect
			// 
			this.mnCharacterSetAutoDetect.Checked = true;
			this.mnCharacterSetAutoDetect.Index = 1;
			this.mnCharacterSetAutoDetect.Text = "Character Set Auto Detection";
			this.mnCharacterSetAutoDetect.Click += new System.EventHandler(this.mnCharacterSetAutoDetect_Click);
			// 
			// mnQuestion
			// 
			this.mnQuestion.Index = 3;
			this.mnQuestion.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.mnTutorial,
																					   this.mnCheckForUpdates,
																					   this.mnSite,
																					   this.mnLine1,
																					   this.mnAbout});
			this.mnQuestion.Text = "?";
			// 
			// mnTutorial
			// 
			this.mnTutorial.Index = 0;
			this.mnTutorial.Text = "&Tutorial";
			this.mnTutorial.Click += new System.EventHandler(this.mnTutorial_Click);
			// 
			// mnCheckForUpdates
			// 
			this.mnCheckForUpdates.Index = 1;
			this.mnCheckForUpdates.Text = "&Check for Update";
			this.mnCheckForUpdates.Click += new System.EventHandler(this.mnCheckForUpdates_Click);
			// 
			// mnSite
			// 
			this.mnSite.Index = 2;
			this.mnSite.Text = "www.sqlpowerinjector.com";
			this.mnSite.Click += new System.EventHandler(this.mnSite_Click);
			// 
			// mnLine1
			// 
			this.mnLine1.Index = 3;
			this.mnLine1.Text = "-";
			// 
			// mnAbout
			// 
			this.mnAbout.Index = 4;
			this.mnAbout.Text = "&About";
			this.mnAbout.Click += new System.EventHandler(this.mnAbout_Click);
			// 
			// sfdSaveSession
			// 
			this.sfdSaveSession.DefaultExt = "xml";
			this.sfdSaveSession.Filter = "xml files |*.xml";
			this.sfdSaveSession.Title = "Session Save";
			// 
			// ofdLoadSession
			// 
			this.ofdLoadSession.DefaultExt = "xml";
			this.ofdLoadSession.Filter = "xml files |*.xml";
			this.ofdLoadSession.Title = "Session Load";
			// 
			// Injector
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(74)), ((System.Byte)(114)), ((System.Byte)(147)));
			this.ClientSize = new System.Drawing.Size(1006, 687);
			this.Controls.Add(this.grbStatus);
			this.Controls.Add(this.grbResults);
			this.Controls.Add(this.grbParameters);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Menu = this.mainMenu1;
			this.Name = "Injector";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "SQL Power Injector 1.1.1";
			this.Load += new System.EventHandler(this.Injector_Load);
			this.grbParameters.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudNbrThread)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudStartingLength)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudStartingCount)).EndInit();
			this.grbDBType.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudDelaySeconds)).EndInit();
			this.grbTechnique.ResumeLayout(false);
			this.grbMethod.ResumeLayout(false);
			this.grbType.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dtgLoadedInput)).EndInit();
			this.grbResults.ResumeLayout(false);
			this.grbStatus.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.axwbHtmlInitializor)).EndInit();
			this.tabHTML.ResumeLayout(false);
			this.tabpgHTML.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.axwbHtmlResult)).EndInit();
			this.tabpgSource.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudHtmlMsgLength)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dtgCurrentState)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void Injector_Load(object sender, System.EventArgs e)
		{
			try
			{
				grbParameters.Height = 275;
				grbStatus.Location = new System.Drawing.Point(8, 290);
				grbStatus.Height = 390;
				dtgCurrentState.Height = 24;
				tabHTML.Height = 310;
				rtxtViewSource.Height = 294;

				lblNbrThread.Location = new System.Drawing.Point(616, 112);
				nudNbrThread.Location = new System.Drawing.Point(736, 112);

				tableStyle = new DataGridTableStyle();
				DataGridTextBoxColumn column = new DataGridTextBoxColumn();
				DataGridBoolColumn colBool = new DataGridBoolColumn();
				tableStyle.MappingName = "ArrayList";

				colBool = new DataGridBoolColumn();
				colBool.MappingName = "ItemIsSelected";
				colBool.HeaderText = "";
				colBool.Width = 17;
				colBool.AllowNull = false;
				tableStyle.GridColumnStyles.Add(colBool);

				column = new DataGridTextBoxColumn();
				column.MappingName = "ItemName";
				column.HeaderText = "Name";
				column.ReadOnly = true;
				column.Width = 100;
				tableStyle.GridColumnStyles.Add(column);

				column = new DataGridTextBoxColumn();
				column.MappingName = "ItemStartingString";
				column.HeaderText = "Starting string";
				column.Width = 150;
				tableStyle.GridColumnStyles.Add(column);

				column = new DataGridTextBoxColumn();
				column.MappingName = "ItemVaryingString";
				column.HeaderText = "Varying string";
				column.Width = 100;
				tableStyle.GridColumnStyles.Add(column);

				column = new DataGridTextBoxColumn();
				column.MappingName = "ItemEndingString";
				column.HeaderText = "Ending string";
				column.Width = 553;
				tableStyle.GridColumnStyles.Add(column);

				dtgLoadedInput.TableStyles.Add(tableStyle);

				// Datagrid result
				tableStyle = new DataGridTableStyle();
				tableStyle.MappingName = "ArrayList";

				column = new DataGridTextBoxColumn();
				column.MappingName = "ItemStatus";
				column.HeaderText = "Status";
				column.Width = 921;
				tableStyle.GridColumnStyles.Add(column);

				column = new DataGridTextBoxColumn();
				column.MappingName = "ItemSQLError";
				column.HeaderText = "SQL Error";
				column.Width = 0;
				tableStyle.GridColumnStyles.Add(column);

				dtgCurrentState.TableStyles.Add(tableStyle);

				dtgCurrentState.ReadOnly = true;

				object o = null;
				axwbHtmlResult.Silent = true;
				axwbHtmlResult.Navigate("about:blank", ref o, ref o, ref o, ref o);
				axwbHtmlResult.Width = 752;
				axwbHtmlResult.Height = 290;

				object ob = null;
				axwbHtmlInitializor.Silent = true;
				axwbHtmlInitializor.Navigate("about:blank", ref ob, ref ob, ref ob, ref ob);
				axwbHtmlInitializor.Width = 0;
				axwbHtmlInitializor.Height = 0;

				txtURL.Text = "";

				ckbDelay.Location = new System.Drawing.Point(480, 88);

				lblDelaySeconds.Location = new System.Drawing.Point(584, 90);
				nudDelaySeconds.Location = new System.Drawing.Point(704, 88);

				chkInsertEmptyComments.Location = new System.Drawing.Point(480, 72);

				cmbDBType.SelectedIndex = 0;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void butClearResults_Click(object sender, System.EventArgs e)
		{
			ClearResults();
		}

		private void butPause_Click(object sender, System.EventArgs e)
		{
			try
			{
				if(butPause.Text == "&Pause" || butPause.Text == "Pause")
				{
					_threadsStop = (byte)enumThreadsState.Paused;
					butPause.Text = "&Resume";
				}
				else
				{
					_threadsStop = (byte)enumThreadsState.Started;
					butPause.Text = "&Pause";
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void axwbHtmlResult_navigateComplete2(object sender, AxSHDocVw.DWebBrowserEvents2_NavigateComplete2Event e)
		{
			// We want to empty the browser only the first time that the application started, if not
			// there will have a error generated if someone click on the View source since the htmldocument
			// has nothing in it yet
			if(!_hasFinishedLoad)
			{
				ChangeHtmlContent("<body></body>");
				_hasFinishedLoad = true;
			}
		}

		private void axwbHtmlInitializor_beforeNavigate2(object sender, AxSHDocVw.DWebBrowserEvents2_BeforeNavigate2Event e)
		{
			string curUri = (string)e.uRL;
			_initialUri = "";

			if(curUri.StartsWith("about:blank"))
			{
				_initialUri = curUri.Replace("about:blank", "");
			}
			else
				_initialUri = (string)e.uRL;
		}

		protected void dtgCurrentState_CurCellChange(object sender, EventArgs e)
		{
			try
			{
				ChangeHtmlContent(Convert.ToString(dtgCurrentState[dtgCurrentState.CurrentCell.RowNumber, 1]));
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void radCount_CheckedChanged(object sender, System.EventArgs e)
		{
			lblLength.Text = "Count";
			txtWord.Visible = false;
			lblWord.Visible = false;
			lblStartingLength.Visible = false;
			nudStartingLength.Visible = false;
			lblStartingCount.Visible = true;
			nudStartingCount.Visible = true;

			lblStartingCount.Location = new System.Drawing.Point(424, 112);
			nudStartingCount.Location = new System.Drawing.Point(520, 112);
			lblNbrThread.Location = new System.Drawing.Point(616, 112);
			nudNbrThread.Location = new System.Drawing.Point(736, 112);

			lblTimeTaken.Location = new System.Drawing.Point(232, 24);
			txtTimeTaken.Location = new System.Drawing.Point(304, 24);
		}

		private void radWord_CheckedChanged(object sender, System.EventArgs e)
		{
			lblLength.Text = "Length";
			txtWord.Visible = true;
			lblWord.Visible = true;
			lblStartingLength.Visible = true;
			nudStartingLength.Visible = true;
			lblStartingCount.Visible = false;
			nudStartingCount.Visible = false;
            
			lblNbrThread.Location = new System.Drawing.Point(616, 112);
			nudNbrThread.Location = new System.Drawing.Point(736, 112);

			lblTimeTaken.Location = new System.Drawing.Point(800, 24);
			txtTimeTaken.Location = new System.Drawing.Point(872, 24);
		}

		private void radLength_CheckedChanged(object sender, System.EventArgs e)
		{
			lblLength.Text = "Length";
			txtWord.Visible = false;
			lblWord.Visible = false;
			lblStartingLength.Visible = true;
			nudStartingLength.Visible = true;
			lblStartingCount.Visible = false;
			nudStartingCount.Visible = false;

			lblNbrThread.Location = new System.Drawing.Point(616, 112);
			nudNbrThread.Location = new System.Drawing.Point(736, 112);

			lblTimeTaken.Location = new System.Drawing.Point(232, 24);
			txtTimeTaken.Location = new System.Drawing.Point(304, 24);
		}

		private void butLoadPage_Click(object sender, System.EventArgs e)
		{
			try
			{
				string responseStr = "";
				Uri uri = null;
				this.Cursor = Cursors.WaitCursor;

				tabHTML.SelectedIndex = 0;
				cmbSubmitURL.Items.Clear();
				FillHtmlContent("");

				LoadInitialURLInfo(out responseStr, uri);

				if(_initialUri.Trim() == "")
					FillHtmlContent(responseStr);
				else if(_initialUri.ToUpper().IndexOf(@"HTTP://ABOUT:BLANK/") <= -1 &&  _initialUri.ToUpper().IndexOf("SHDOCLC.DLL") <= -1)
				{
					_initialUri = "";
					FillHtmlContent("<html><body>The page has been redirected by jscript</body></html>");
				}

				this.Cursor = Cursors.Default;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void butStartStop_Click(object sender, System.EventArgs e)
		{
			try
			{
				if(butStartStop.Text == "&Start" || butStartStop.Text == "Start")
				{
					if(dtgLoadedInput.CurrentRowIndex != -1)
					{
						if(!(mnUseCookie.Checked && radBlind.Checked && ckbDelay.Checked && nudNbrThread.Value > 1))
						{
							_threadsStop = (byte)enumThreadsState.Started;
							butStartStop.Text = "&Stop";
							txtLength.Text = "";
							txtWord.Text = "";
							txtTimeTaken.Text = "";
							ChangeHtmlContent("");

							butPause.Enabled = true;
							nudNbrThread.Enabled = false;
							tabHTML.SelectedIndex = 0;

							_arrStatus = new ArrayList();

							_numberThreadsFinished = 0;

							this.Cursor = Cursors.WaitCursor;
							if(radBlind.Checked)
							{
								lblProcessing.Visible = true;
								pbProcessing.Visible = true;
								pbProcessing.Value = 0;
							}
							_createdThreads = new Thread[1];
							_createdThreads[0] = new Thread(new ThreadStart(SQLInjection));
							_createdThreads[0].IsBackground = true;
							_createdThreads[0].Start();
						}
						else
							MessageBox.Show("You cannot use more than one thread at the time with the time delay when you use the cookie!" +
								"\n\nEither uncheck the option use cookie in the menu or use one thread",
								"Error with multithreading time delay using cookie", 
								MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
					else
						MessageBox.Show("You must have at least one value in the datagrid before to Start the injection!" +
							"\n\nPlease reload the page to fetch the values or change the mode option to POST or GET",
							"Starting injection with no value to send", 
							MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
				else
				{
					// To make sure someone is not clicking on Start when it's cleaning up (hanging web calls)
					butStartStop.Enabled = false;
					butPause.Enabled = false;
					nudNbrThread.Enabled = true;
					butStartStop.Text = "&Start";
					_threadsStop = (byte)enumThreadsState.Stoped;

					if(butPause.Text == "&Resume" || butPause.Text == "Resume")
						butPause.Text = "&Pause";
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void radBlind_CheckedChanged(object sender, System.EventArgs e)
		{
			if(radBlind.Checked)
			{
				grbResults.Visible = true;
				grbParameters.Height = 240;
				grbStatus.Height = 360;
				grbStatus.Location = new System.Drawing.Point(8, 320);

				dtgCurrentState.Height = 312;
				tabHTML.Visible = false;

				chkDistinct.Location = new System.Drawing.Point(704, 72);
				ckbDelay.Location = new System.Drawing.Point(704, 88);
				lblDelaySeconds.Location = new System.Drawing.Point(808, 90);
				nudDelaySeconds.Location = new System.Drawing.Point(928, 88);
				chkInsertEmptyComments.Location = new System.Drawing.Point(808, 72);

				lblHTMLMsgLength.Visible = false;
				nudHtmlMsgLength.Visible = false;
				lblSQLErrorString.Visible = false;
				txtSQLErrorString.Visible = false;
				chkTrapErrorString.Visible = false;

				chkDistinct.Visible = true;
				radWord.Checked = true;
				lblStartingLength.Visible = true;
				nudStartingLength.Visible = true;
				lblNbrThread.Visible = true;
				nudNbrThread.Visible = true;

				grbType.Visible = true;
			}
		}

		private void radNormal_CheckedChanged(object sender, System.EventArgs e)
		{
			if(radNormal.Checked)
			{
				grbResults.Visible = false;
				dtgCurrentState.Height = 24;
				grbParameters.Height = 275;
				tabHTML.Location = new System.Drawing.Point(8, 72);
				tabHTML.Visible = true;

				grbStatus.Location = new System.Drawing.Point(8, 290);
				grbStatus.Height = 390;

				ckbDelay.Location = new System.Drawing.Point(480, 88);
				lblDelaySeconds.Location = new System.Drawing.Point(584, 88);
				nudDelaySeconds.Location = new System.Drawing.Point(704, 88);
				chkInsertEmptyComments.Location = new System.Drawing.Point(480, 72);

				lblHTMLMsgLength.Visible = true;
				nudHtmlMsgLength.Visible = true;
				lblSQLErrorString.Visible = true;
				txtSQLErrorString.Visible = true;
				chkTrapErrorString.Visible = true;

				chkDistinct.Visible = false;
				lblStartingCount.Visible = false;
				nudStartingCount.Visible = false;
				lblStartingLength.Visible = false;
				nudStartingLength.Visible = false;
				lblNbrThread.Visible = false;
				nudNbrThread.Visible = false;

				grbType.Visible = false;
			}
		}

		private void dtgLoadedInput_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			try
			{
				DataGrid.HitTestInfo hti =	dtgLoadedInput.HitTest(e.X, e.Y);

				if(hti.Type == DataGrid.HitTestType.Cell && hti.Column == CHECKED_COLUMN_INPUT)
				{
					if(!CheckIfUniqueSelection() && !(bool)dtgLoadedInput[hti.Row, hti.Column])
						dtgLoadedInput[hti.Row, hti.Column] = false;
					else
						dtgLoadedInput[hti.Row, hti.Column] = !(bool)dtgLoadedInput[hti.Row, hti.Column];
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void dtgLoadedInput_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			try
			{
				DataGridCell curCell = ((DataGrid)sender).CurrentCell;

				if(e.KeyCode == System.Windows.Forms.Keys.Space && curCell.ColumnNumber == CHECKED_COLUMN_INPUT)
					if(!(bool)dtgLoadedInput[curCell.RowNumber, curCell.ColumnNumber] && !CheckIfUniqueSelection())
						dtgLoadedInput[curCell.RowNumber, curCell.ColumnNumber] = false;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void dtgLoadedInput_HandleMouseMove(object sender, MouseEventArgs e)
		{
			string tmpPostData = "";
			int nbrInputs = 0;
			int curIndex = 0;

			try
			{	
				if(_arrInputModifiable != null)
				{
					if(butStartStop.Enabled && _arrInputModifiable[cmbSubmitURL.SelectedIndex] != null)
					{
						curIndex = cmbSubmitURL.SelectedIndex;
						nbrInputs = _arrInputModifiable[cmbSubmitURL.SelectedIndex].Count;

						DataGrid.HitTestInfo hti = ((MyDataGrid)sender).HitTest(new Point(e.X, e.Y)); 
						DataGridCell curCell = ((MyDataGrid)sender).CurrentCell;

						CHtmlInputInject curRow1;
				
						// Make it changes the tooltip only if we change the row or if there is only one row, if not, we
						// we will never update the tooltip with the new value since it normally changes it when the user
						// change row. To keep it simple I'll use the VisibleRowCount method, since  we are sure that if there
						// is only one row it will be the only one visible at any time AND that the _arrInputModifiable has come values inside
						if(((hti.Type == DataGrid.HitTestType.Cell && hti.Row != dtgLoadedInput.HitRow) || (dtgLoadedInput.VisibleRowCount == 1 && hti.Row == 0)) && nbrInputs != 0)
						{
							dtgLoadedInput.HitRow = hti.Row;

							curRow1 = (CHtmlInputInject)_arrInputModifiable[curIndex][dtgLoadedInput.HitRow];

							if(radBlind.Checked && curRow1.ItemIsSelected)
							{
								DatabaseStringBaseType dbStringType = null;

								switch(cmbDBType.SelectedIndex)
								{
									case (int)DatabaseStringBaseType.enumDatabaseType.SqlServer:
										dbStringType = new DatabaseString.DatabaseStringSQLServerType();
										break;

									case (int)DatabaseStringBaseType.enumDatabaseType.Oracle:
										dbStringType = new DatabaseString.DatabaseStringOracleType();
										break;

									case (int)DatabaseStringBaseType.enumDatabaseType.MySql:
										dbStringType = new DatabaseString.DatabaseStringMySQLType();
										break;

									case (int)DatabaseStringBaseType.enumDatabaseType.Sybase:
										dbStringType = new DatabaseString.DatabaseStringSybaseType();
										break;
								}
						
								if(!curRow1.ItemStartingString.EndsWith(" "))
									curRow1.ItemStartingString += " ";

								if(radLength.Checked)
									tmpPostData += curRow1.ItemName + "=" + dbStringType.GetToolTipLengthPostDataString(curRow1.ItemStartingString, curRow1.ItemVaryingString, curRow1.ItemEndingString, Convert.ToString(nudStartingLength.Value), chkDistinct.Checked);
								else if(radCount.Checked)
									tmpPostData += curRow1.ItemName + "=" + dbStringType.GetToolTipCountPostDataString(curRow1.ItemStartingString, curRow1.ItemVaryingString, curRow1.ItemEndingString, Convert.ToString(nudStartingCount.Value));
								else if(radWord.Checked)
									tmpPostData += curRow1.ItemName + "=" + dbStringType.GetToolTipWordPostDataString(curRow1.ItemStartingString, curRow1.ItemVaryingString, curRow1.ItemEndingString, Convert.ToString(nudStartingLength.Value), chkDistinct.Checked);
								else
									tmpPostData += curRow1.ItemName + "=" + curRow1.ItemStartingString;
							}
							else
								tmpPostData += curRow1.ItemName + "=" + curRow1.ItemStartingString;
			
							if(chkInsertEmptyComments.Checked)
								tmpPostData = tmpPostData.Replace(" ", "/**/");

							// Must compare the old value with the old one, if not, when it's only one row
							// it will be constantly called and the tooltip will never show up
							if(tmpPostData != _oldTooltipValue)
							{
								if(dtgLoadedInput.DTGToolTip != null && dtgLoadedInput.DTGToolTip.Active) 
									dtgLoadedInput.DTGToolTip.Active = false;

								dtgLoadedInput.DTGToolTip.SetToolTip(dtgLoadedInput, tmpPostData);
								dtgLoadedInput.DTGToolTip.Active = true;

								_oldTooltipValue = tmpPostData;
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void ckbDelay_CheckedChanged(object sender, System.EventArgs e)
		{
			if(!ckbDelay.Checked)
			{
				lblDelaySeconds.Visible = false;
				nudDelaySeconds.Visible = false;
			}
			else
			{
				lblDelaySeconds.Visible = true;
				nudDelaySeconds.Visible = true;
			}
		}

		protected void tabHTML_SelectedIndexChanged(object sender, EventArgs e) 
		{
			try
			{
				IHTMLDocument3 htmlDocument = (IHTMLDocument3)axwbHtmlResult.Document;
				string allHTML = "";
	
				if(htmlDocument != null)
					if(htmlDocument.documentElement != null)
						allHTML = htmlDocument.documentElement.outerHTML;

				if (tabHTML.SelectedIndex == 1) 
					rtxtViewSource.Text = allHTML;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void mnExit_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void mnAbout_Click(object sender, System.EventArgs e)
		{
			Form aboutForm = new About();
			aboutForm.ShowDialog();
		}

		private void mnCookie_Click(object sender, System.EventArgs e)
		{
			string newCookieValue = "";

			InputBox.Title = "Cookie insertion";
			InputBox.Question = "Please insert the value of your cookie";
			InputBox.LabelHeight = 48;
			newCookieValue = InputBox.ShowInputBox();

			_loadedSetCookie = newCookieValue == "!!@@!!@@!!@@!!" ? _loadedSetCookie : newCookieValue;
		}

		private void mnUseCookie_Click(object sender, System.EventArgs e)
		{
			if(mnUseCookie.Checked)
				mnUseCookie.Checked = false;
			else
				mnUseCookie.Checked = true;
		}

		private void mnSite_Click(object sender, System.EventArgs e)
		{
			ProcessStartInfo sInfo = new ProcessStartInfo("http://www.sqlpowerinjector.com/");
			Process.Start(sInfo);
		}

		private void mnTutorial_Click(object sender, System.EventArgs e)
		{
			string pathPDF = "\\TutorialSPInj.pdf";

			try
			{
				#if DEBUG
					pathPDF = "\\..\\..\\" + pathPDF;
				#endif

				ProcessStartInfo sInfo = new ProcessStartInfo(Application.StartupPath + pathPDF);
				Process.Start(sInfo);
			}
			catch(Win32Exception ex)
			{
				if(ex.Message.ToUpper() == "THE SYSTEM CANNOT FIND THE FILE SPECIFIED")
					MessageBox.Show("The application cannot find the tutorial.\n\nHere are the possible reasons of the problem:\n\n" +
						"   1. You might have renamed the tutorial file, it should be TutorialSPInj.pdf\n" +
						"   2. You might have moved the file, it should be in the same folder than the exe\n" +
						"   3. You might have deleted the file, go back on the web site and get it in the Tutorial section\n",
						"Error while loading the file", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void cmbSubmitURL_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ComboBox curSubmitURL = (ComboBox)sender;
			
			dtgLoadedInput.DataSource = null;
			dtgLoadedInput.DataSource = _arrInputModifiable[curSubmitURL.SelectedIndex];
		}

		private void mnSaveSession_Click(object sender, System.EventArgs e)
		{
			string fileName = "";
			string sessionName = "";
			Uri pathFileName = null;
			string initialPath = "\\Saved Session";

			#if DEBUG
				initialPath = "\\..\\.." + initialPath;
			#endif

			try
			{
				sfdSaveSession.InitialDirectory = Application.StartupPath + initialPath;

				if(sfdSaveSession.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					fileName = sfdSaveSession.FileName;
					pathFileName = new Uri(fileName);
					sessionName = pathFileName.Segments[pathFileName.Segments.GetUpperBound(0)];
					
					// Remove the extension .xml to get only the name
					if(sessionName.Trim() != "" && sessionName.Length > 4)
						sessionName = sessionName.Substring(0, sessionName.Length - 4);

					Session saveSession = BuildSessionFromForm(sessionName, fileName);

					Session.SaveSession(saveSession, fileName);
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void mnLoadSession_Click(object sender, System.EventArgs e)
		{
			string fileName = "";
			string initialPath = "\\Saved Session";

			#if DEBUG
				initialPath = "\\..\\.." + initialPath;
			#endif

			try
			{
				ofdLoadSession.InitialDirectory = Application.StartupPath + initialPath;

				if(ofdLoadSession.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					fileName = ofdLoadSession.FileName;
					Session loadedSession = Session.LoadSession(fileName);

					if(LoadSessionInForm(loadedSession))
					{
						mnSaveSession.Enabled = true;
						butStartStop.Enabled = true;
					}
				}
			}
			catch(MyException ex)
			{
				if(ex.MyHref == XML_ERROR_FORMAT)
					MessageBox.Show("The xml file you have loaded doesn't have the right formatting!\n\n" +
						"It could be because it has been modified and it is no more valid\n" +
						"or it not a xml from SQL Power Injector", "Bad XML file formatting",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
					MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
						"\nSource: " + ex.Source +
						"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
						MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void mnCharacterSetAutoDetect_Click(object sender, System.EventArgs e)
		{
			if(mnCharacterSetAutoDetect.Checked)
				mnCharacterSetAutoDetect.Checked = false;
			else
				mnCharacterSetAutoDetect.Checked = true;
		}

		private void DrawItemHandler(object sender,  DrawItemEventArgs e)
		{
			if(e.Index == -1)
				return;

			SubmitURLParam surlParam = (SubmitURLParam)((ComboBox)sender).Items[e.Index];

			// Highlight the item with the mouse on
			e.Graphics.FillRectangle(Brushes.Orange, e.Bounds); 
			e.Graphics.DrawString(surlParam.SubmitUri, cmbSubmitURL.Font, Brushes.Black, 2, e.Bounds.Y);
         
			if((e.State & DrawItemState.Focus) == 0)
			{                                                    
				e.Graphics.FillRectangle(surlParam.SubmitColor, e.Bounds); 
				e.Graphics.DrawString(surlParam.SubmitUri, cmbSubmitURL.Font, Brushes.Black, 2, e.Bounds.Y);
			}    
		}

		private void mnCheckForUpdates_Click(object sender, System.EventArgs e)
		{
			string versionNumber = Application.ProductVersion;
			string currentVersionNumber = Application.ProductVersion;
			
			currentVersionNumber = Utilities.GetCurrentVersion();

			if(Utilities.IsCurrentVersionNewer(currentVersionNumber, Application.ProductVersion))
			{
				if(DialogResult.OK == MessageBox.Show("There is a newer version available!\n\n" +
					"Click ok to go update it or cancel if you don't want to\n",
					"Newer version available",
					MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation))
				{
					ProcessStartInfo sInfo = new ProcessStartInfo("http://www.sqlpowerinjector.com/download.htm");
					Process.Start(sInfo);
				}
			}
			else
				MessageBox.Show("There is no newer version available!\n\n" +
					"Keep checking regularly",
					"No newer version available",
					MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}
}