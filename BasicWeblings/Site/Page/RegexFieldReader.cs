using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System.Text.RegularExpressions;
using System.Globalization;
using BorrehSoft.Utensils.Parsing;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Page
{
	/// <summary>
	/// Regex field reader. This is exactly what you think it is.
	/// A dirty hack.
	/// </summary>
	public class RegexFieldReader : Service
	{
		private Regex matcher;
		private Service successful;

		public override string Description {
			get {
				return string.Join(", ", matcher.GetGroupNames());
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "successful")
				successful = e.NewValue;
		}

		protected override void Initialize (Settings modSettings)
		{
			matcher = new Regex(modSettings["regex"] as String);
		}

		protected override bool Process (IInteraction parameters)
		{
			IIncomingBodiedInteraction incoming = (IIncomingBodiedInteraction)parameters;
			QuickInteraction parsed = new QuickInteraction (incoming);
			Match results = matcher.Match (incoming.IncomingBody.ReadToEnd ());

			foreach (string groupName in matcher.GetGroupNames()) {
				parsed [groupName] = Parser.GetBestPossible(results.Groups [groupName].Value);
			}

			return successful.TryProcess(parsed);
		}
	}
}
