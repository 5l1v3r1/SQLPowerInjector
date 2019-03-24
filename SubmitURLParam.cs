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
using System.Xml.Serialization;

namespace SQLPowerInjector
{
	/// <summary>
	/// Summary description for SubmitURLParam.
	/// </summary>
	[Serializable]
	public class SubmitURLParam
	{
		private string _uri;
		private Brush  _colorUri;
		private byte _method;

		public enum Method
		{
			GET = 1,
			POST
		}

		#region Constructors
		public SubmitURLParam()	
		{
			_uri = "about:blank";
			_colorUri = Brushes.Beige;
			_method = (byte)Method.GET;
		}

		public SubmitURLParam(string uri, Brush brushColor, Method method)
		{
			_uri  = uri;
			_colorUri = brushColor;
			_method = (byte)method;
		}
		#endregion

		[XmlAttribute]
		public string SubmitUri
		{
			get { return _uri; }
			set { _uri = value; }
		}
	
		[XmlAttribute]
		public Brush SubmitColor
		{
			get { return _colorUri; }
			set { _colorUri = value; }
		}

		[XmlAttribute]
		public Method SubmitMethod
		{
			get { return (Method)_method; }
			set { _method = (byte)value; }
		}
	}
}
