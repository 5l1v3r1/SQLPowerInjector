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
using System.Drawing;
using System.Reflection;

namespace SQLPowerInjector
{
	/// <summary>
	/// Summary description for MyDataGrid.
	/// </summary>
	public class MyDataGrid : DataGrid
	{
		private int hitRow;
		private ToolTip toolTip1; 

		public MyDataGrid() 
		{
			hitRow = -1; 
 
			this.toolTip1 = new ToolTip(); 
			this.toolTip1.InitialDelay = 500;
 		}

		public int HitRow
		{
			get { return hitRow; }
			set { hitRow = value; }
		}

		public ToolTip DTGToolTip
		{
			get { return toolTip1; }
		}

		public void ScrollToRow(int theRow)
		{
			try
			{
				// Expose the protected GridVScrolled method allowing you
				// to programmatically scroll the grid to a particular row.
				if (DataSource != null)
					this.GridVScrolled(this, new ScrollEventArgs(ScrollEventType.LargeIncrement, theRow));
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\nMethod name: " + MethodInfo.GetCurrentMethod().Name +
					"\nSource: " + ex.Source +
					"\n\nPlease send me an email with this message at bugs@sqlpowerinjector.com", "Error message", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
