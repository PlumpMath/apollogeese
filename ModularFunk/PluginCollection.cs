using System;
using System.Collections.Generic;
using System.Reflection;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.Utensils
{
	/// <summary>
	/// Plugin collection.
	/// </summary>
	public class PluginCollection<T>
	{
		class PluginNotLoadedException : Exception
		{
			public PluginNotLoadedException (string name) : base(string.Format("Couldn't find plugin named {0}", name))	{ }
		}

		Dictionary<string, Type> butt =
			new Dictionary<string, Type> ();

		public Type this[string name]
		{
			get { 
				if (butt.ContainsKey(name))
					return butt [name];
				else
					throw new PluginNotLoadedException (name);
			}
			set {
				if (butt.ContainsKey (name))
					butt.Remove (name);
				butt.Add (name, value);
			}
		}

		/// <summary>
		/// Gets the constructed type by name.
		/// </summary>
		/// <returns>The constructed type.</returns>
		/// <param name="name">Name.</param>
		public T GetConstructed(string name)
		{
			return (T)Activator.CreateInstance (typeof(T));
		}

		/// <summary>
		/// Produces a PluginCollection from one or multiple files
		/// </summary>
		/// <returns>PluginCollection</returns>
		/// <param name="files">Files.</param>
		public static PluginCollection<T> FromFiles(params string[] files)
		{
			PluginCollection<T> newcol = new PluginCollection<T> ();

			foreach(string file in files)
				AppendMatchingTypesTo (
					newcol.butt, typeof(T), 
					Assembly.LoadFrom (file));

			return newcol;
		}

		/// <summary>
		/// Appends the matching types to a dictionary.
		/// </summary>
		/// <param name="registry">Dictionary to register newly matched types in.</param>
		/// <param name="match">Type to match with.</param>
		/// <param name="assembly">Assembly to load from.</param>
		static void AppendMatchingTypesTo (Dictionary<string, Type> registry, Type match, Assembly assembly)
		{
			foreach (Type potential in assembly.GetTypes())
				if (match.IsAssignableFrom (potential))
					registry.Add (potential.Name, potential);
		}
	}
}

