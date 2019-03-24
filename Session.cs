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
using System.IO;
using System.Collections;

namespace SQLPowerInjector
{
	/// <summary>
	/// Summary description for Session.
	/// </summary>
	[XmlRootAttribute("SQL_Power_Injector_Session", Namespace="", IsNullable=false)]
	public class Session
	{
		#region Members
		#region Private
		private string _sessionName;
		private string _fileName;
		private DateTime _creationDate;
		private bool _useCookie;
		private bool _autoDetectEncoding;
		private string _loadedUri;
		private int _currentSubmitSelectedIndex;
		private Database_Type _databaseType;
		private Method_Injection _method;
		private Technique_Injection _technique;
		private bool _replaceSpace;
		private bool _waitforDelay;
		private byte _delaySecond;
		private Type_Injection _typeInjection;
		private bool _distinct;
		private string _positiveAnswer;
		private uint _startingLength;
		private uint _startingCount;
		private byte _numberThreads;
		private ArrayList _HtmlForms;
		private ushort _htmlMessageLength;
		private string _sqlPositiveInjectionResult;
		private bool _trapErrorString;
		#endregion
		#region Constants
		const byte NUMBER_MAX_DELAYSECONDS = 255;
		const uint STARTING_LENGTH_MAX = 10000000;
		const uint STARTING_COUNT_MAX = 10000000;
		const byte NUMBER_THREADS_MAX = 8;
		const ushort HTML_MESSAGE_LENGTH_MAX = 10000;
		#endregion
		#endregion

		#region Public Enums
		public enum Database_Type
		{
			SQL_Server  = 0,
			Oracle		= 1,
			MySQL		= 2,
			Sybase		= 3
		}

		public enum Method_Injection
		{
			POST	= 1,
			GET		= 2
		}

		public enum Technique_Injection
		{
			Normal	= 1,
			Blind	= 2
		}

		public enum Type_Injection
		{
			Word	= 1,
			Length	= 2,
			Count	= 3
		}
		#endregion

		#region Constructor
		public Session() 
		{
			_sessionName = "";
			_fileName = "";
			_creationDate = DateTime.Now;
			_useCookie = false;
			_autoDetectEncoding = true;
			_loadedUri = "";
			_currentSubmitSelectedIndex = 0;
			_databaseType = Database_Type.SQL_Server;
			_method = Method_Injection.POST;
			_technique = Technique_Injection.Normal;
			_replaceSpace = false;
			_waitforDelay = false;
			_delaySecond = 0;
			_typeInjection = Type_Injection.Word;
			_distinct = false;
			_positiveAnswer = "";
			_startingLength = 100;
			_startingCount = 10;
			_numberThreads = 1;
			_HtmlForms = new ArrayList();
			_htmlMessageLength = 255;
			_sqlPositiveInjectionResult = "";
			_trapErrorString = false;
		}
		#endregion

		#region Public Attributes
		[XmlAttributeAttribute(DataType="Name")]
		public string SessionName
		{
			get { return _sessionName; }
			set { _sessionName = value; }
		}

		[XmlIgnoreAttribute()]
		public string FileName
		{
			get { return _fileName; }
			set { _fileName = value; }
		}

		[XmlAttributeAttribute(DataType="date")]
		public DateTime CreationDate
		{
			get { return _creationDate; }
			set { _creationDate = value; }
		}

		public bool UseCookie
		{
			get { return _useCookie; }
			set { _useCookie = value; }
		}

		public bool AutoDetectEncoding
		{
			get { return _autoDetectEncoding; }
			set { _autoDetectEncoding = value; }
		}

		public string LoadedUri
		{
			get { return _loadedUri; }
			set { _loadedUri = value; }
		}

		public int CurrentSubmitSelectedIndex 
		{
			get { return _currentSubmitSelectedIndex; }
			set { _currentSubmitSelectedIndex = value; }
		}

		[XmlElementAttribute("DatabaseType", typeof(Database_Type))]
		public Database_Type DatabaseType
		{
			get { return _databaseType; }
			set { _databaseType = value; }
		}

		public Method_Injection Method
		{
			get { return _method; }
			set	{ _method = value; }
		}

		public Technique_Injection Technique
		{
			get { return _technique; }
			set	{ _technique = value; }
		}

		public bool ReplaceSpace
		{
			get { return _replaceSpace; }
			set { _replaceSpace = value; }
		}

		public bool WaitforDelay
		{
			get { return _waitforDelay; }
			set { _waitforDelay = value; }
		}

		public byte DelaySecond
		{
			get { return _delaySecond; }
			set
			{ 
				if(value <= NUMBER_MAX_DELAYSECONDS)
					_delaySecond = value;
				else
					_delaySecond = 0;
			}
		}

		public Type_Injection TypeInjection
		{
			get { return _typeInjection; }
			set	{ _typeInjection = value; }
		}

		public bool Distinct
		{
			get { return _distinct; }
			set { _distinct = value; }
		}

		public string PositiveAnswer
		{
			get { return _positiveAnswer; }
			set { _positiveAnswer = value; }
		}

		public uint StartingLength
		{
			get { return _startingLength; }
			set
			{ 
				if(value <= STARTING_LENGTH_MAX)
					_startingLength = value;
				else
					_startingLength = 100;
			}
		}

		public uint StartingCount
		{
			get { return _startingCount; }
			set
			{ 
				if(value <= STARTING_COUNT_MAX)
					_startingCount = value;
				else
					_startingCount = 10;
			}
		}

		public byte NumberThreads
		{
			get { return _numberThreads; }
			set
			{ 
				if(value <= NUMBER_THREADS_MAX)
					_numberThreads = value;
				else
					_numberThreads = 1;
			}
		}

		[XmlArray ("HtmlForms"), XmlArrayItem("HtmlForm", typeof(HTMLForm))]
		public ArrayList HtmlForms
		{
			get { return _HtmlForms; }
			set { _HtmlForms = value; }
		}

		public ushort HtmlMessageLength
		{
			get { return _htmlMessageLength; }
			set
			{ 
				if(value <= HTML_MESSAGE_LENGTH_MAX)
					_htmlMessageLength = value;
				else
					_htmlMessageLength = 255;
			}
		}

		public string SqlPositiveInjectionResult
		{
			get { return _sqlPositiveInjectionResult; }
			set { _sqlPositiveInjectionResult = value; }
		}

		public bool TrapErrorString
		{
			get { return _trapErrorString; }
			set { _trapErrorString = value; }
		}
		#endregion

		public static bool SaveSession(Session SessionToSave, string XMLFileName) 
		{  
			bool sessionSavedSuccessfully = false;

			sessionSavedSuccessfully = SerializeSession(SessionToSave, XMLFileName);

			return sessionSavedSuccessfully; 
		}

		public static Session LoadSession(string XMLFileName)
		{
			return LoadSerializedSession(XMLFileName);
		}

		private static bool SerializeSession(Session SessionToSave, string XMLFileName)
		{
			TextWriter textWriter = null;
			bool success = false;

			try
			{
				//Create serializer object using the type name of the Object to serialize.
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(Session));

				textWriter = new StreamWriter(XMLFileName);

				xmlSerializer.Serialize(textWriter, SessionToSave);

				success = true;
			}
			catch(Exception ex)
			{
				throw(ex);
			}
			finally
			{
				if (textWriter != null)
					textWriter.Close();								
			}

			return success;
		}

		private static Session LoadSerializedSession(string XMLFileName)
		{   	
			Session loadedSession = new Session();
			TextReader txrTextReader = null;

			try
			{
				XmlSerializer xserDocumentSerializer = new XmlSerializer(typeof(Session));

				txrTextReader = new StreamReader(XMLFileName);
				loadedSession = (Session)xserDocumentSerializer.Deserialize(txrTextReader);
			}
			catch(InvalidOperationException ex)
			{
				throw(new MyException(ex.Message, ex.InnerException));
			}
			catch(Exception ex)
			{
				throw(ex);
			}
			finally
			{
				//Make sure to close the file even if an exception is raised...
				if (txrTextReader != null)
					txrTextReader.Close();				
			}			

			return loadedSession;
		}
	}
}
