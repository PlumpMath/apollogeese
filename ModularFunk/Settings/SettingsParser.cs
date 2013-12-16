using System;
using BorrehSoft.Utensils.Parsing;
using BorrehSoft.Utensils.Parsing.Parsers;
using System.Collections.Generic;

namespace BorrehSoft.Utensils.Settings
{
	/// <summary>
	/// Settings parser.
	/// </summary>
	public class SettingsParser : ConcatenationParser
	{
		public override string ToString ()
		{
			return "Settings, accolade-enclosed block with zero or more assignments";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Utensils.Settings.SettingsParser"/> class.
		/// </summary>
		public SettingsParser() : base('{', '}', ';')
		{
			ConcatenationParser listParser = new ConcatenationParser ('[', ']', '=');
			AssignmentParser assignmentParser = new AssignmentParser ();

			AnyParser valueParser = new AnyParser (
				new ValueParser<int> (int.TryParse), 
				new ValueParser<float> (float.TryParse),
				new ValueParser<bool> (bool.TryParse), 
				new IdentifierParser (),
				new StringParser (), 
				listParser, 
				this
			);			

			listParser.InnerParser = valueParser;
			assignmentParser.InnerParser = valueParser;

			this.InnerParser = assignmentParser;
		}

		/// <summary>
		/// Parsing Method for the <see cref="BorrehSoft.Utensils.Settings"/> type.
		/// </summary>
		/// <returns>
		/// Succes value; zero or higher when succesful.
		/// </returns>
		/// <param name='session'>
		/// Session in which this parsing action will be conducted.
		/// </param>
		/// <param name='result'>
		/// Result of this parsing action
		/// </param>
		internal override int ParseMethod (ParsingSession session, out object result)
		{
			object parsedAssignmentList;

			if (base.ParseMethod (session, out parsedAssignmentList) > 0) {
				List<object> assignments = (List<object>)parsedAssignmentList;

				Settings map = new Settings ();

				foreach (object assignment in assignments) {
					Tuple<string, object> t = (Tuple<string, object>)assignment;
					map [t.Key] = t.Value;
				}

				result = map;
				return 1;
			}

			result = null;
			return -1;
		}	
	}
}