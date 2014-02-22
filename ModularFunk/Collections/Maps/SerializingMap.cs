using System;
using System.Text;
using System.Collections.Generic;

namespace BorrehSoft.Utensils.Collections.Maps
{
	/// <summary>
	/// A regular map with added functionality of parsing and composing.
	/// </summary>
	public class SerializingMap<T> : Map<T>
	{
		public delegate T Parser(string data);

		/// <summary>
		/// Adds elements from string stream.
		/// </summary>
		/// <param name="data">Data to parse</param>
		/// <param name="parser">String parser for element value.</param>
		/// <param name="assigner">Value-assigning character.</param>
		/// <param name="concatenator">Concatenator of assignments.</param>
		public void AddFromString(string source, Parser parser, char assigner, char concatenator)
		{
			StringBuilder buffer = new StringBuilder ();

			string identifier = "";

			int inByte;
			string id, data;

			foreach(char c in source)
			{
				if (c == concatenator)
				{
					this [identifier] = parser (buffer.ToString ());
					buffer.Clear ();
				}
				else if (c == assigner)
				{
					identifier = buffer.ToString ();
					buffer.Clear();
				}
				else {
					buffer.Append(c);
				}
			}

			if (buffer.Length > 0) {
				this [identifier] = parser (buffer.ToString ());
				buffer.Clear ();
			}
		}

		/// <summary>
		/// Writes the pairs using the supplied write method.
		/// </summary>
		/// <param name="writeMethod">Write method.</param>
		/// <param name="connector">Connector.</param>
		/// <param name="seperator">Seperator.</param>
		public void WritePairsTo(Action<string> writeMethod, char connector, char seperator)
		{
			foreach (KeyValuePair<string, T> kvp in BackEnd) {
				writeMethod (kvp.Key);
				writeMethod (connector.ToString());
				writeMethod (kvp.Value.ToString());
				writeMethod (seperator.ToString());
			}
		}

		/// <summary>
		/// Writes the pairs to the supplied stringbuilder using a format.
		/// </summary>
		/// <param name="builder">Builder.</param>
		/// <param name="format">Format.</param>
		public void WritePairsTo(StringBuilder builder, string format)
		{
			foreach (KeyValuePair<string, T> kvp in BackEnd) 
				builder.AppendFormat (format, kvp.Key, kvp.Value.ToString ());
		}
	}
}
