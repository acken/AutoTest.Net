using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AutoTest.Core.DebugLog;
namespace AutoTest.Core.FileSystem
{
	public class AssemblyParser : IResolveAssemblyReferences, IRetrieveAssemblyIdentifiers
	{
		#region IResolveAssemblyReferences implementation
		public string[] GetReferences(string assembly)
		{
			try
			{
				var a = System.Reflection.Assembly.LoadFrom(assembly);
				var references = a.GetReferencedAssemblies();
				var names = new List<string>();
				foreach (var reference in references)
					names.Add(reference.Name);
				return names.ToArray();
			}
			catch
			{
				Debug.WriteMessage(string.Format("Could not load assemblies for {0}", assembly));
			}
			return new string[] { };
		}
		#endregion
		
		public int GetAssemblyIdentifier(string assembly)
		{
			var fileInfo = new FileInfo(assembly);
			var builder = new StringBuilder();
			builder.Append(fileInfo.Length.ToString());
			builder.Append("|");
			foreach (var reference in GetReferences(assembly))
				builder.Append(reference);
			return builder.ToString().GetHashCode();
		}
	}
}

