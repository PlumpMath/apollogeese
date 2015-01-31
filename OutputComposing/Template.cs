using System;
using System.Text.RegularExpressions;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using System.Net;
using System.IO;
using BorrehSoft.Utensils.Collections;
using Stringtionary = System.Collections.Generic.Dictionary<string, string>;
using System.Text;
using BorrehSoft.ApolloGeese.Duckling.Http;
using BorrehSoft.ApolloGeese.Duckling.Http.Headers;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Page
{
	/// <summary>
	/// Simple template service which fills 
	/// </summary>
	public class Template : Service
	{
		private Settings settings;
		/// <summary>
		/// Regex Matches that may be replaced
		/// </summary>
		private MatchCollection replaceables;
		/// <summary>
		/// Variables this template supports
		/// </summary>
		private StringList templateVariables = new StringList();

		private string templateFile, rawTemplate, chunkPattern, title;
		private string unsuppliedFormat, inacquirableFormat;

		public bool WillCheckForTemplateUpdates { get; private set; }

		public bool ChecksContextFirst { get; private set; }

		public DateTime LastTemplateUpdate { get; private set; }

		/// <summary>
		/// Returns the title with this service.
		/// </summary>
		/// <value>The title of this page</value>
		public override string Description {
			get { return (title ?? "").Length == 0 ? templateFile ?? "[FILE NOT SET]" : title; }
		}

		protected override void Initialize (Settings modSettings)
		{
			title = modSettings.GetString ("title", "untitled");
			chunkPattern = modSettings.GetString ("chunkpattern", @"\{% ([a-z|\_]+) %\}");
			if (modSettings.Has ("templatefile"))
				templateFile = (string)modSettings ["templatefile"];
			else
				throw new Exception ("templatefile mandatory");

			WillCheckForTemplateUpdates = modSettings.GetBool("checkfortemplateupdates", true);
			ChecksContextFirst = modSettings.GetBool ("contextfirst", false);
			inacquirableFormat = modSettings.GetString("forwardfail", "");
			unsuppliedFormat = modSettings.GetString("backwardfail",  "");
			settings = modSettings;

			if (WillCheckForTemplateUpdates)
				CheckForTemplateUpdates();
			else
				LoadTemplateAndRegisterReplacableSegments();

			if (ChecksContextFirst) 
				WriteElement = WriteContextFirst;
			else
				WriteElement = WriteBranchFirst;
		}

		/// <summary>
		/// Checks for template updates.
		/// </summary>
		private void CheckForTemplateUpdates ()
		{
			FileInfo info = new FileInfo(templateFile);

			if ((LastTemplateUpdate == null) || 
			    (LastTemplateUpdate != info.LastWriteTime)) {

				LastTemplateUpdate = info.LastWriteTime;
				LoadTemplateAndRegisterReplacableSegments ();

				Secretary.Report(5, "Template file was updated: ", templateFile);
			} 
		}

		/// <summary>
		/// Loads the template and register replacable segments.
		/// </summary>
		private void LoadTemplateAndRegisterReplacableSegments ()
		{
			if (!File.Exists (templateFile))
				File.Create (templateFile).Close();

			rawTemplate = File.ReadAllText (templateFile);

			replaceables = templateVariables.AddUniqueRegexMatches (rawTemplate, chunkPattern);

			foreach(string templateVariable in templateVariables)
				if (!Branches.Has(templateVariable))
					Branches[templateVariable] = Stub;
		}

		delegate void WriteElementDelegate(StreamWriter outputWriter, IInteraction source, string groupName);

		WriteElementDelegate WriteElement;

		void WriteBranchFirst (StreamWriter outputWriter, IInteraction source, string groupName)
		{			
			Service branch = Branches[groupName];

			if ((branch ?? Stub) == Stub) {
				object chunk;
				if (source.TryGetFallback(groupName, out chunk))
				{
					outputWriter.Write(chunk.ToString());
				}
				else 
				{
					outputWriter.Write(string.Format(unsuppliedFormat, groupName));
				}
			} else if (!branch.TryProcess(source)) {
				outputWriter.Write(string.Format(inacquirableFormat, groupName));
			}		
		}

		void WriteContextFirst (StreamWriter outputWriter, IInteraction source, string groupName)
		{			
			object chunk;
			if (source.TryGetFallback (groupName, out chunk)) {
				outputWriter.Write (chunk.ToString ());
			} else {
				Service branch = Branches[groupName];

				if ((branch ?? Stub) == Stub) {
					outputWriter.Write(string.Format(unsuppliedFormat, groupName));
				} else if (!branch.TryProcess(source)) {
					outputWriter.Write(string.Format(inacquirableFormat, groupName));
				}	
			}				
		}

		protected override bool Process (IInteraction source)
		{
			IOutgoingBodiedInteraction target;
			MimeType type;
			int cursor = 0;
			string groupName;

			target = (IOutgoingBodiedInteraction)source.GetClosest (typeof(IOutgoingBodiedInteraction));


			if (target is IHttpInteraction) {
				type = MimeType.Text.Html;
				type.Encoding = Encoding.UTF8;
				((IHttpInteraction)target).ResponseHeaders.ContentType = type;
			}

			if (WillCheckForTemplateUpdates) CheckForTemplateUpdates();

			StreamWriter outputWriter = target.GetOutgoingBodyWriter ();

			try	{
				foreach (Match replaceable in replaceables) {
					outputWriter.Write(rawTemplate.Substring (cursor, replaceable.Index - cursor));

					groupName = replaceable.Groups[1].Value;

					WriteElement(outputWriter, source, groupName);							

					cursor = replaceable.Index + replaceable.Length;
				}

				// In the very likely event the cursor is not at the end of the document,
				// the last bit of the document needs to be written to the body as well.
				if (cursor < rawTemplate.Length)
					outputWriter.Write(rawTemplate.Substring(cursor));

				outputWriter.Flush();
			}
			catch (Exception ex) {
				// Forward the exception but append a cursor position and a message
				// so the user won't be entirely in the dark.
				throw new Exception (
					string.Format (
					"Replacing template variables failed at cursor position {0}, {2}:\n{1}",
					cursor.ToString (), 
					rawTemplate.Substring (cursor, 40), 
					ex.Message), ex);
			}


			return true;
		}
	}
}
