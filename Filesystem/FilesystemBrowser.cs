using System;
using System.IO;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.Http;
using System.Web;

namespace BorrehSoft.ApolloGeese.Extensions.Filesystem
{
	/// <summary>
	/// Branches for each file and directory in the root path + a certain folder.
	/// </summary>
	public class FilesystemBrowser : Service
	{
		bool useHttp;

		string rootFilesystem;
		Service 
			dirNotFound = Stub,
			directoryItem = Stub,
			fileItem = Stub;

		public override string Description {
			get {
				return this.rootFilesystem;
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "notfound") dirNotFound = e.NewValue;
			if (e.Name == "diritem") directoryItem = e.NewValue;
			if (e.Name == "fileitem") fileItem = e.NewValue;
		}

		protected override void Initialize (Settings modSettings)
		{
			rootFilesystem = modSettings.GetString("rootpath", ".");
			useHttp = modSettings.GetBool ("usehttp", true);
		}

		protected override bool Process (IInteraction parameters)
		{
			bool success = true;
			IInteraction uncastParameters;
			IHttpInteraction httpParameters = null;
			string requestedPath = rootFilesystem;
			string coreUrl = "";

			if (parameters.TryGetClosest (typeof(IHttpInteraction), out uncastParameters) && useHttp) {
				httpParameters = (IHttpInteraction)uncastParameters;

				string[] urlArray = httpParameters.URL.ToArray ();

				coreUrl = string.Join ("/", urlArray);

				string decodedPathFromURL = HttpUtility.UrlDecode (Path.Combine (urlArray));

				requestedPath = Path.Combine (requestedPath, decodedPathFromURL);
			}

			DirectoryInfo requestedInfo = new DirectoryInfo (requestedPath);

			if (requestedInfo.Exists) {
				FilesystemItemInteraction itemInteraction = new FilesystemItemInteraction(parameters, rootFilesystem, coreUrl);

				foreach(DirectoryInfo info in requestedInfo.GetDirectories())
				{				
					itemInteraction.Assume(info);
					success &= directoryItem.TryProcess(itemInteraction);
				}

				foreach(FileInfo info in requestedInfo.GetFiles())
				{
					itemInteraction.Assume(info);
					success &= fileItem.TryProcess(itemInteraction);
				}
			} else {
				httpParameters.SetStatuscode (404);
				success &= dirNotFound.TryProcess(httpParameters);
			}

			return success;
		}
	}
}

