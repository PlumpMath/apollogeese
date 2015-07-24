using System;
using System.IO;
using BorrehSoft.Utensils;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;

namespace TestSuite
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			SettingsComposer composer = new SettingsComposer ();

			TestAgainst (RunTestcase2 (composer), "Testcase2.conf");
			TestAgainst (RunTestcase1(composer), "TestCase1.conf");
		}

		static string RunTestcase2 (SettingsComposer composer)
		{
			Settings data = new Settings ();
		
			data ["taart"] = 2;

			return composer.Serialize (data);
		}

		static string RunTestcase1 (SettingsComposer composer)
		{
			Settings data = new Settings ();

			IEnumerable<string> plugins = new string[] { "/usr/lib/BasicHttpServer.dll", "/usr/lib/OutputComposing.dll" };
			data ["plugins"] = plugins;
				Settings instances = new Settings ();
					Settings firstInstance = new Settings ();
					firstInstance ["type"] = "HttpService";
					Settings modConf = new Settings ();
						IEnumerable<string> prefixes = new string[] { "http://localhost:8080/" };
						modConf ["prefixes"] = prefixes;
					firstInstance ["modconf"] = modConf;
					Settings underlyingInstance = new Settings ();
						underlyingInstance ["type"] = "Template";
						Settings templateConf = new Settings ();
							templateConf ["templatefile"] = "/var/www/index.html";
						underlyingInstance ["modconf"] = templateConf;
					firstInstance ["http_branch"] = underlyingInstance;
				instances ["firstinstance"] = firstInstance;
			data ["instances"] = instances;

			return composer.Serialize (data);
		}

		public static void TestAgainst(string data, string verifile) {			
			Console.WriteLine (verifile);

			using (StreamReader reader = new StreamReader(File.OpenRead(verifile))) {
				string verificationData = reader.ReadToEnd ();

				if (data == verificationData) {
					Console.WriteLine ("All is good");
				} else {
					Console.WriteLine ("Burp");
				}
			}
		}
	}
}
