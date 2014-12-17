using System;
using BorrehSoft.ApolloGeese.Duckling;
using System.IO;

namespace Designer
{
	public class SiblingInteraction : IOutgoingBodiedInteraction, QuickInteraction
	{
		StreamWriter outgoingBody;
		MemoryStream body;

		public SiblingInteraction (IInteraction parent) : base(parent)
		{
			body = new MemoryStream();
			outgoingBody = new StreamWriter(body);
		}

		public string GetFinished()
		{
			body.Position = 0;
			StreamReader reader = new StreamReader(body);
			string list = reader.ReadToEnd();
			reader.Close();
		}

		StreamWriter OutgoingBody { get { return outgoingBody; } }
	}
}
