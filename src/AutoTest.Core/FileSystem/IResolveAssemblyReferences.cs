using System;
namespace AutoTest.Core.FileSystem
{
	public interface IResolveAssemblyReferences
	{
		string[] GetReferences(string assembly);
	}
}

