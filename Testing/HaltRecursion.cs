﻿using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;

namespace Testing
{
	public class HaltRecursion : Service
	{
		public override string Description {
			get {
				return "This service may not be crossed twice.";
			}
		}

		Service Continue;

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "continue") {
				this.Continue = e.NewValue;
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			if (Closest<RecursionMarkerInteraction>.From (parameters).Placer == this) {
				throw new Exception ("Recursion halted");
			} else {
				return this.Continue.TryProcess (new RecursionMarkerInteraction (this));
			}
		}
	}
}
