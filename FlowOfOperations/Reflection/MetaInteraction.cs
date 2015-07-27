using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections;
using System.Collections.Generic;
using System.Web;
using System.Text;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	class MetaInteraction : QuickInteraction
	{
		public MetaInteraction (IInteraction parentParameters, 
			Service origin, string branchName, Service target) : base(
				parentParameters)
		{
			this["origin"] = origin.ModelID;
			this["branchname"] = branchName;
			this["target"] = target.ModelID;
		}
	}

}
