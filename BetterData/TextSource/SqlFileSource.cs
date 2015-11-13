using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using System.Data;
using BorrehSoft.Utensils.Collections.Settings;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.IO;
using BorrehSoft.Utensils.IO;

namespace BetterData
{
	class SqlFileSource : SqlSource
	{
		public override string ToString ()
		{
			return string.Format ("[SqlFileSource: {0}]", BackEnd.FilePath);
		}

		public override bool IsOutdated {
			get {
				return this.BackEnd.IsOutdated;
			}
		}

		protected FileSource BackEnd {
			get;
			set;
		}

		public SqlFileSource (string filePath)
		{
			this.BackEnd = new FileSource (filePath);
		}

		public override string GetText() {
			return this.BackEnd.GetText ();
		}
	}
}

