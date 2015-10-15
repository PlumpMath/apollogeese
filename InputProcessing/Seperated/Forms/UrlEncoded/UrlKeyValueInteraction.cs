using System;
using BorrehSoft.ApolloGeese.CoreTypes;

using System.IO;
using BorrehSoft.Utensils.Parsing;
using System.Text;
using BorrehSoft.Utensils.Collections;

namespace InputProcessing
{
	class UrlKeyValueInteraction : SimpleInteraction, IIncomingKeyValueInteraction
	{
		ReluctantTextReader dataReader;

		Map<Service> ActionMap {
			get;
			set;
		}

		string currentName;

		public UrlKeyValueInteraction (IInteraction parent, TextReader dataReader) : base(parent)
		{
			this.dataReader = new ReluctantTextReader (dataReader);
		}

		public bool HasReader() {
			return true;
		}

		public TextReader GetIncomingBodyReader() {
			return this.dataReader;
		}

		public bool ReadName() {
			this.dataReader.StopCharacter = '=';

			if (-1 < this.dataReader.Peek ()) {
				this.currentName = this.dataReader.ReadToEnd ();

				this.dataReader.StopCharacter = '&';

				return true;
			} else {
				return false;
			}
		}

		public object ReadValue() {
			return this.dataReader.ReadToEnd();
		}

		public string GetName() {
			return this.currentName;
		}

		public void SetCurrentValue(object value) {
			this [GetName ()] = value;
		}

		public void SetCurrentAction(Service action) {
			this.ActionMap [GetName ()] = action;
		}

		public Service GetAction(string name) {
			return this.ActionMap[name];
		}
	}
}

