﻿using System;
using System.Text;
using BorrehSoft.ApolloGeese.CoreTypes;

namespace Validating
{
	public class Matches : Equals
	{
		protected override Service Compare (object testPattern, object testSubject)
		{
			if (System.Text.RegularExpressions.Regex.Match (testSubject, testPattern)) {
				return Successful;
			} else {
				return Failure;
			}
		}
	}
}

