using System;
using System.IO;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.Duckling.Http;
using BorrehSoft.ApolloGeese.Duckling.Http.Headers;
using System.Web;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Filesystem
{
	public class Fileserver : Service
	{
		public Fileserver ()
		{
		}

		Settings mimeTypes;
		Service notFoundBranch, badRequestBranch;
		bool optionalMimetypes;
		string rootPath;

		public override string Description {
			get {
				return this.rootPath;
			}
		}

		protected override void Initialize (Settings modSettings)
		{
			mimeTypes = new Settings ();

			if (modSettings.Has ("allowedmimetypes")) {
				mimeTypes = modSettings["allowedmimetypes"] as Settings ?? new Settings();
			} else {
				foreach (string key in modSettings.Dictionary.Keys) 
					if (key.StartsWith ("dot_"))				
						mimeTypes [key.Substring (4)] = modSettings.GetString (key, "");
			}

			rootPath = modSettings.GetString("rootpath", ".");
			optionalMimetypes = modSettings.GetBool("optionalmimetypes", false);

			Branches["notfound"] = Stub;
			Branches["badrequest"] = Stub;
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "notfound") notFoundBranch = e.NewValue;
			if (e.Name == "badrequest") badRequestBranch  = e.NewValue;
		}

		protected override bool Process (IInteraction uncastParameters)
		{
			IHttpInteraction parameters;
			string trimmedrootpath, trimmedurl, finalpath, extension, mimeType = "application/octet-stream";

			parameters = (IHttpInteraction)uncastParameters.GetClosest (typeof(IHttpInteraction));
			trimmedrootpath = rootPath.TrimEnd ('/');
			trimmedurl = HttpUtility.UrlDecode(parameters.URL.ReadToEnd ().TrimStart ('/'));
			finalpath = string.Format ("{0}/{1}", trimmedrootpath, trimmedurl);

			FileInfo sourcefile = new FileInfo (finalpath);

			extension = sourcefile.Extension.TrimStart ('.').ToLower();

			if (mimeTypes.TryGetString(extension, out mimeType) || optionalMimetypes) {
				if (sourcefile.Exists) {
					FileStream sourceStream = new FileStream (finalpath, FileMode.Open, FileAccess.Read);

					parameters.ResponseHeaders.ContentType = new MimeType(mimeType);
					parameters.ResponseHeaders.ContentLength = sourceStream.Length;
					
					Secretary.Report (5, 
					                  "Fileserve:", sourcefile.Name, 
					                  "Size:", sourcefile.Length.ToString(), 
					                  "MIME:", mimeType);

					if (parameters.HasWriter())
					{
						throw new Exception ("can't serve files to outgoing stream that has a writer");
					}

					sourceStream.CopyTo (parameters.OutgoingBody, 4096);										
				} else {
					parameters.StatusCode = 404;
					notFoundBranch.TryProcess(uncastParameters);
				}
			} else {
				parameters.StatusCode = 410;
				badRequestBranch.TryProcess(uncastParameters);
			}

			return true;
		}
	}
}

