using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.ApolloGeese.Http;

namespace BorrehSoft.ApolloGeese.Extensions.InputProcessing
{
	public class QueryReader : BodyReader
	{		
		public override string AcquireData (IInteraction parameters)
		{
			IHttpInteraction request = (IHttpInteraction)parameters.GetClosest (typeof(IHttpInteraction));

			return request.GetQuery;
		}
	}
}
