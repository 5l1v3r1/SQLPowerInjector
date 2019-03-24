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
using System.Collections;

namespace SQLPowerInjector
{
	/// <summary>
	/// Summary description for HTMLForm.
	/// </summary>
	[Serializable]
	public class HTMLForm
	{
		#region Members
		#region Private
		private string _submitUri;
		private ArrayList _htmlInputInjectArray;
		private ArrayList _htmlInputNotInjectArray;
		#endregion
		#endregion

		#region Constructor
		public HTMLForm() 
		{
			_submitUri = null;
			_htmlInputInjectArray = new ArrayList();
			_htmlInputNotInjectArray = new ArrayList();
		}
		#endregion

		#region Public Attributes
		[XmlAttributeAttribute(DataType="anyURI")]
		public string SubmitUri
		{
			get { return _submitUri; }
			set { _submitUri = value; }
		}

		[XmlArray ("ArrayInputInjects"), XmlArrayItem("ArrayHtmlInputInject", typeof(CHtmlInputInject))]
		public ArrayList HtmlInputInjectArray
		{
			get { return _htmlInputInjectArray; }
			set { _htmlInputInjectArray = value; }
		}

		[XmlArray ("ArrayInputNotInjects"), XmlArrayItem("ArrayHtmlInputNotInject", typeof(CHtmlInputNotInject))]
		public ArrayList HtmlInputNotInjectArray
		{
			get { return _htmlInputNotInjectArray; }
			set { _htmlInputNotInjectArray = value; }
		}
		#endregion
	}
}
