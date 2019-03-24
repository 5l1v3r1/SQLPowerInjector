//*********************************************************************
//																	 //
//	SQL Power Injector 1.1.1 Copyright (c) 2006 Francois Larouche	 //
//																	 //
//  Author	: francois.larouche@sqlpowerinjector.com				 //
//	Web Site: www.sqlpowerinjector.com								 //
//																	 //
//*******************************************************************//
using System;

namespace SQLPowerInjector
{
	/// <summary>
	/// Summary description for MyException.
	/// </summary>
	public class MyException : InvalidOperationException
	{
		public MyException() : base() {}
		public MyException(string message) : base(message) {}
		public MyException(string message, Exception inner) : base(message, inner) {}

		public int MyHref
		{
			get { return base.HResult; }
		}
	}
}
