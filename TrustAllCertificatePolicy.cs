//*********************************************************************
//																	 //
//	SQL Power Injector 1.1.1 Copyright (c) 2006 Francois Larouche	 //
//																	 //
//  Author	: francois.larouche@sqlpowerinjector.com				 //
//	Web Site: www.sqlpowerinjector.com								 //
//																	 //
//*******************************************************************//
using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace SQLPowerInjector
{
	/// <summary>
	/// Summary description for TrustAllCertificatePolicy.
	/// </summary>
	public class TrustAllCertificatePolicy : System.Net.ICertificatePolicy
	{
		public TrustAllCertificatePolicy() {}

		public bool CheckValidationResult(ServicePoint sp, X509Certificate cert, WebRequest req, int problem)
		{
			return true;
		}
	}
}
