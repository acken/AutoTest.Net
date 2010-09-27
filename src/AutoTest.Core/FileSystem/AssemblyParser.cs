using System;
using System.Collections.Generic;
namespace AutoTest.Core.FileSystem
{
	public class AssemblyParser : IResolveAssemblyReferences
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
	}
}

