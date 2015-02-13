using System;
using System.Collections.Generic;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.ApolloGeese.Http;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Extensions.Auth
{
	class SessionException : Exception
	{
		public SessionException (string message) : base(message)
		{

		}
	}
}
