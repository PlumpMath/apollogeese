using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.ApolloGeese.Loader;
using BorrehSoft.Utensils.Collections;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.Module
{
	/// <summary>
	/// Executes into branch from another file.
	/// </summary>
	public class Module : Service
	{
		public override string Description {
			get {
				if (branchName == null)
					return string.Format("{0}:directed", file);
				else 
					return string.Format("{0}:{1}", file, branchName);
			}
		}

		string file, branchName = null;

		protected override void Initialize (Settings modSettings)
		{
			if (modSettings.Has ("default")) {
				string[] pathAndBranch = modSettings.GetString ("default").Split ('@');

				file = pathAndBranch [0];

				if (pathAndBranch.Length > 1) {
					branchName = pathAndBranch [1]; 
				}
			} else {
				file = (string)modSettings ["file"];
				if (modSettings.Has("branch"))
					branchName = (string)modSettings.Get ("branch");
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{

		}

		/// <summary>
		/// Gets the branches within the module file 
		/// </summary>
		/// <value>The module branches.</value>
		public Map<Service> ModuleBranches { 
			get {
				return InstanceLoader.GetInstances (file); 
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			Service referredService;
			JumpInteraction jumpInteraction;

			string pickedBranchName;

			if (branchName == null)
				pickedBranchName = ((DirectedInteraction)parameters).BranchName;
			else
				pickedBranchName = branchName;

			referredService = ModuleBranches [pickedBranchName];
			jumpInteraction = new JumpInteraction (parameters, Branches, GetSettings ());

			return referredService.TryProcess (jumpInteraction);
		}
	}
}

