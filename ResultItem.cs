//*********************************************************************
//																	 //
//	SQL Power Injector 1.1.1 Copyright (c) 2006 Francois Larouche	 //
//																	 //
//  Author	: francois.larouche@sqlpowerinjector.com				 //
//	Web Site: www.sqlpowerinjector.com								 //
//																	 //
//*******************************************************************//
using System;
using System.Xml.Serialization;

namespace SQLPowerInjector
{
	/// <summary>
	/// Summary description for ResultItem.
	/// </summary>
	internal class CResultItem
	{
		private string  _status;
		private string  _SQLError;

		public CResultItem()
		{
			_status  = "";
			_SQLError = "";
		}

		public CResultItem(string st, string SQLErr)
		{
			_status  = st;
			_SQLError = SQLErr;
		}

		public string ItemStatus
		{
			get { return _status; }
			set { _status = value; }
		}
	
		public string ItemSQLError
		{
			get { return _SQLError; }
			set { _SQLError = value; }
		}
	}

	[Serializable]
	public class CHtmlInputInject
	{
		private string _name;
		private string _startStr;
		private string _varyingStr;
		private string _endStr;
		private bool _isSelected;

		public CHtmlInputInject()
		{
			_name  = "";
			_startStr = "";
			_varyingStr = "";
			_endStr = "";
			_isSelected = false;
		}

		public CHtmlInputInject(string name, string startingString, string varyingString, string endingString, bool isSelected)
		{
			_name = name;
			_startStr = startingString;
			_varyingStr = varyingString;
			_endStr = endingString;
			_isSelected = isSelected;
		}

		[XmlAttribute]
		public string ItemName
		{
			get { return _name; }
			set { _name = value; }
		}
	
		[XmlAttribute]
		public string ItemStartingString
		{
			get { return _startStr; }
			set { _startStr = value; }
		}

		[XmlAttribute]
		public string ItemVaryingString
		{
			get { return _varyingStr; }
			set { _varyingStr = value; }
		}

		[XmlAttribute]
		public string ItemEndingString
		{
			get { return _endStr; }
			set { _endStr = value; }
		}
		
		[XmlAttribute]
		public bool ItemIsSelected
		{
			get { return _isSelected; }
			set { _isSelected = value; }
		}
	}

	[Serializable]
	public class CHtmlInputNotInject
	{
		private string _name;
		private string _inputValue;

		public CHtmlInputNotInject()
		{
			_name  = "";
			_inputValue = "";
		}

		public CHtmlInputNotInject(string name, string inputValue)
		{
			_name  = name;
			_inputValue = inputValue;
		}

		[XmlAttribute]
		public string ItemName
		{
			get { return _name; }
			set { _name = value; }
		}
	
		[XmlAttribute]
		public string ItemInputValue
		{
			get { return _inputValue; }
			set { _inputValue = value; }
		}
	}
}
