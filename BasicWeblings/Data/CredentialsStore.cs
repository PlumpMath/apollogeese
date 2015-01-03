using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.Extensions.BasicWeblings
{
	/// <summary>
	/// I don't see how, yet, but this is going to be a 
	/// security risk at some point in time.
	/// </summary>
	public class CredentialsStore : Service
	{
		private static Settings credentials = null;
		public static Settings Credentials { get { return credentials; } }

		public override string Description {
			get {
				return "Credential storage, leave unattached.";
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			throw new Exception ("Credential storage isn't meant for this :(");
		}

		protected override void Initialize (Settings modSettings)
		{
			if (credentials == null)
				credentials = modSettings;
			else
				// haha this works
				credentials = new Settings (new CombinedMap<object> (credentials, modSettings));
		}

		protected override bool Process (IInteraction parameters)
		{
			throw new Exception ("Credential storage isn't meant for this either ::((");
		}
	}
}
