using System;
using BorrehSoft.ApolloGeese.Duckling.HTML;

namespace BorrehSoft.ApolloGeese.Duckling
{
	public class TaggedBodyEntity : HtmlEntity
	{
		public override string Body {
			get; set;
		}

		public override void WriteUsingCallback (FormattedWriter WriteMethod)
		{

		}
	}
}

