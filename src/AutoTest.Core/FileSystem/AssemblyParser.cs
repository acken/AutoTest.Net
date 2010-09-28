using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
namespace AutoTest.Core.FileSystem
{
	public class AssemblyParser : IResolveAssemblyReferences, IRetrieveAssemblyIdentifiers
	{
		#region IResolveAssemblyReferences implementation
		public string[] GetReferences(string assembly)
		{
			var a = System.Reflection.Assembly.LoadFrom(assembly);
			var references = a.GetReferencedAssemblies();
			var names = new List<string>();
			foreach (var reference in references)
				names.Add(reference.Name);
			return names.ToArray();
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

