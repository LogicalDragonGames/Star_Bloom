using UnityEngine;
using System.Collections;
using System.Xml;			// TODO: Lightweight solution
using System.IO;			// 			There are a few problems with this class (it's a quick and dirty solution)
							//			The biggest problem is that System.Xml adds a LOT of bloat
							//			I'm also not sure how this will behave on non-windows systems

namespace Zephyr
{
	public class XMLContext
	{
	}

	public class XMLReader : XMLContext
	{
		protected System.Xml.XmlReader m_XmlReader = null;

		public XMLReader( string file )
		{
			XmlReaderSettings rs = new XmlReaderSettings();

			m_XmlReader = System.Xml.XmlReader.Create( file, rs );
		}

		~XMLReader()
		{
			Close();
		}

		public void Close()
		{
			if( m_XmlReader != null )
				m_XmlReader.Close();

			m_XmlReader = null;
		}

		public XMLReader( Stream stream )
		{
			m_XmlReader = System.Xml.XmlReader.Create( stream );
		}

		public bool ReadToDescendant( string name )
		{
			return m_XmlReader.ReadToDescendant( name );
		}
		
		public bool ReadToFollowing( string name )
		{
			return m_XmlReader.ReadToFollowing( name );
		}
		
		public bool ReadToNextSibling( string name )
		{
			return m_XmlReader.ReadToNextSibling( name );
		}

		public bool ReadNext()
		{
			return m_XmlReader.Read();
		}

		public void SkipChildren()
		{
			m_XmlReader.Skip();
		}

		// Gets the current node's name
		public string GetName()
		{
			return m_XmlReader.Name;
		}
		
		public string GetAttribute( string name )
		{
			return m_XmlReader.GetAttribute( name );
		}
		
		public string GetText()
		{
			return m_XmlReader.ReadElementContentAsString();
		}
		
		public int GetInt()
		{
			return m_XmlReader.ReadElementContentAsInt();
		}
		
		public float GetFloat()
		{
			return m_XmlReader.ReadElementContentAsFloat();
		}
		
		public bool GetBool()
		{
			return m_XmlReader.ReadElementContentAsBoolean();
		}
		
		public bool IsStartElement()
		{
			return m_XmlReader.IsStartElement();
		}
	}

	public class XMLWriter : XMLContext
	{
		protected System.Xml.XmlWriter m_XmlWriter = null;

		public XMLWriter( string file )
		{
			XmlWriterSettings ws = new XmlWriterSettings();
			ws.Indent = true;
			
			m_XmlWriter = System.Xml.XmlWriter.Create( file, ws );
		}

		public XMLWriter( Stream stream )
		{
			XmlWriterSettings ws = new XmlWriterSettings();
			ws.Indent = true;

			m_XmlWriter = System.Xml.XmlWriter.Create( stream, ws );
		}

		~XMLWriter()
		{
			Close();
		}
		
		public void Close()
		{
			if( m_XmlWriter != null )
			{
				m_XmlWriter.Flush();
				m_XmlWriter.Close();
			}

			m_XmlWriter = null;
		}

		public void StartDocument()
		{
			m_XmlWriter.WriteStartDocument();
		}

		public void EndDocument()
		{
			m_XmlWriter.WriteEndDocument();
		}

		public void WriteElement( string name )
		{
			m_XmlWriter.WriteStartElement( name );
		}

		public void EndElement()
		{
			m_XmlWriter.WriteEndElement();
		}

		public void EndElementFull()
		{
			m_XmlWriter.WriteFullEndElement();
		}

		public void WriteText( string value )
		{
			m_XmlWriter.WriteString( value );
		}

		public void WriteAttribute( string attribute, string value )
		{
			m_XmlWriter.WriteAttributeString( attribute, value );
		}
	}

	public class XML
	{
		public enum ParseType
		{
			Read,
			Write
		}

		public bool IsFileOpen { get { return m_Stream != null; } }

		protected string m_Path = "";
		public XMLReader Reader = null;
		public XMLWriter Writer = null;

		protected FileStream m_Stream = null;

		public XML( string path = "", ParseType type = ParseType.Read )
		{
			// Create a new (or truncated) file for write ops
			// (Attempt to) open a file for read ops
			if( type == ParseType.Read )
				Reader = new XMLReader( path );
			else
				Writer = new XMLWriter( path );
		}

		~XML()
		{
			Close();
		}

		public void Close()
		{
			if( null != m_Stream )
			{
				m_Stream.Flush();
				m_Stream.Close();
				m_Stream.Dispose();
			}

			if( null != Reader )
				Reader.Close();

			if( null != Writer )
				Writer.Close();

			Reader = null;
			Writer = null;
			m_Stream = null;
		}
	}
}