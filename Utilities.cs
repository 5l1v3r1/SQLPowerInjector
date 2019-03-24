//*********************************************************************
//																	 //
//	SQL Power Injector 1.1.1 Copyright (c) 2006 Francois Larouche    //
//																	 //
//  Author	: francois.larouche@sqlpowerinjector.com				 //
//	Web Site: www.sqlpowerinjector.com								 //
//																	 //
//*******************************************************************//
using System;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Text;
using System.Drawing;
using System.Xml;
using System.Xml.XPath;

namespace SQLPowerInjector
{
	/// <summary>
	/// Summary description for Utilities.
	/// </summary>
	public class Utilities
	{
		public Utilities() {}
		
		public const int ENCODE_ERROR_CODE = -111111;

		public static string EncodeURL(string stringToEncode)
		{
			if(stringToEncode != null)
			{
				stringToEncode = stringToEncode.Replace("%", "%25");
				stringToEncode = stringToEncode.Replace("#", "%23");
				stringToEncode = stringToEncode.Replace("+", "%2B");
				stringToEncode = stringToEncode.Replace(" ", "+");
				stringToEncode = stringToEncode.Replace("'", "%27");
				stringToEncode = stringToEncode.Replace("=", "%3D");
				stringToEncode = stringToEncode.Replace("(", "%28");
				stringToEncode = stringToEncode.Replace(")", "%29");
			}

			return stringToEncode;
		}

		public static string Char(int asciiCode)
		{
			Byte[] charBytes = {Convert.ToByte(asciiCode)};    
			return System.Text.Encoding.ASCII.GetString(charBytes);
		}

		public static string GetStreamHTMLData(Stream currentStream, string charSet, bool supportSeek)
		{
			StringBuilder sb = new StringBuilder();
			string tempString = null;
			int count = 0;
						
			// used on each read operation
			byte[] buf = new byte[8192];
			
			if(supportSeek) // Restart it at the beginning
				currentStream.Position = 0;

			try
			{
				do
				{
					// fill the buffer with data
					count = currentStream.Read(buf, 0, buf.Length);

					// make sure we read some data
					if (count != 0)
					{
						if(charSet != null)
						{
							if(charSet.Trim() != "")
							{
								try
								{
									tempString = Encoding.GetEncoding(charSet).GetString(buf, 0, count);
								}
								catch(ArgumentException ex)
								{
									throw new ArgumentException(Convert.ToString(ENCODE_ERROR_CODE), ex);
								}
							}
							else
								tempString = Encoding.ASCII.GetString(buf, 0, count);
						}
						else // translate from bytes to ASCII text
							tempString = Encoding.ASCII.GetString(buf, 0, count);

						// continue building the string
						sb.Append(tempString);
					}
				}
				while (count > 0); // any more data to read?

				// Clean up
				if(currentStream != null)
					currentStream.Close();

				return sb.ToString();
			}
			catch(Exception ex1)
			{
				throw(ex1);
			}
		}

		public static void CopyStream(Stream sourceStream, Stream destinationStream)
		{
			byte[] buf = new byte[4096];
			
			try
			{
				while (true)
				{
					int bytesRead = sourceStream.Read(buf, 0, buf.Length);

					// Read returns 0 or -1 when it reaches end of stream.
					if (bytesRead <= 0)
						break;
					else
						destinationStream.Write(buf, 0, bytesRead);
				}
			}
			catch(Exception ex)
			{
				throw(ex);
			}
		}

		public static string GetNewGETUrl(string newQuery, string currentUri)
		{
			Uri queryUri = new Uri(currentUri);

			return queryUri.GetLeftPart(UriPartial.Path) + "?" + newQuery;	
		}

		public static bool IsNumeric(string valueToCheck)
		{
			try
			{
				Int32.Parse(valueToCheck);
			}
			catch 
			{
				return false;
			}
			return true;
		}

		public static string GetCurrentVersion()
		{
			XPathDocument myXPathDocument = null;
			XPathNavigator myXPathNavigator = null;
			XPathNodeIterator myXPathNodeIterator = null;
			string uriVersion = "http://www.sqlpowerinjector.com/Common/CurrentSQLPinjVersionInfo.xml";
			string currentVersionNumber = Application.ProductVersion;

			try
			{
				myXPathDocument = new XPathDocument(uriVersion);
				myXPathNavigator = myXPathDocument.CreateNavigator();

				// Get the current version
				myXPathNodeIterator = myXPathNavigator.Select("SQL_Power_Injector_Version_Info/CurrentVersionNumber");

				while (myXPathNodeIterator.MoveNext())
				{
					currentVersionNumber = myXPathNodeIterator.Current.Value;
					break;
				}

				return currentVersionNumber;
			}
			catch(Exception)
			{
				return currentVersionNumber;
			}
		}

		public static bool IsCurrentVersionNewer(string currentVersionNumber, string productVersion)
		{
			bool isCurVerNewer = false;
			string[] arrCurVerNumber = currentVersionNumber.Split('.');
			string[] arrProductVersion = productVersion.Split('.');

			if(arrCurVerNumber.Length >= 3 && arrProductVersion.Length >= 3)
			{
				// Verify first if all the parts are numeric, if not there is a error and we go out
				for(int i=0;i<arrCurVerNumber.Length;i++)
				{
					if(!IsNumeric(arrCurVerNumber[i]))
						return false;
				}

				if(Convert.ToInt32(arrCurVerNumber[0]) > Convert.ToInt32(arrProductVersion[0]))
					isCurVerNewer = true;
				else if(Convert.ToInt32(arrCurVerNumber[1]) > Convert.ToInt32(arrProductVersion[1]))
					isCurVerNewer = true;
				else if(Convert.ToInt32(arrCurVerNumber[2]) > Convert.ToInt32(arrProductVersion[2]))
					isCurVerNewer = true;
			}

			return isCurVerNewer;
		}
	}
}
