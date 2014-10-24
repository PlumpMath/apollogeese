using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.Extensions.BasicWeblings.Server
{
	public class UdpAdvertiser : Service
	{
		public override string Description {
			get {
				return "Advertise string over UDP by listening to broadcast";
			}
		}

		UdpClient udpClient; IPEndPoint anyIP;
		string requestString, responseString;
		byte[] responseBytes;
		int advertisePort;

		protected override void Initialize (Settings modSettings)
		{
			requestString = modSettings.GetString("request", "tost");
			responseString = modSettings.GetString("response", "test");
			advertisePort = int.Parse(modSettings.GetString("port", "15325"));

			responseBytes = Encoding.ASCII.GetBytes(responseString);

			anyIP = new IPEndPoint(IPAddress.Any, advertisePort);
			udpClient = new UdpClient(anyIP);

			BeginAdvertising();
		}

		private void BeginAdvertising()
		{	
			udpClient.BeginReceive(AdvertiseRoutine, null);
		}

		private void AdvertiseRoutine (IAsyncResult result)
		{
			byte[] receivedBytes = udpClient.EndReceive (result, ref anyIP);
			string receivedString = Encoding.ASCII.GetString (receivedBytes);

			Secretary.Report("received", receivedString, "from", anyIP.ToString());

			if (receivedString == requestString) {
				udpClient.Send(responseBytes, responseBytes.Length, anyIP);
			}

			BeginAdvertising();
		}

		protected override void HandleBranchChanged (object sender)
		{

		}

		protected override bool Process (IInteraction parameters)
		{

		}
	
	}
}
