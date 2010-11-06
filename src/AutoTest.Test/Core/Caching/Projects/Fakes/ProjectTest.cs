using System;
using NUnit.Framework;
using AutoTest.Core.Caching.Projects;
using System.IO;
using System.Reflection;
namespace AutoTest.Test
{
	[TestFixture]
	public class ProjectTest
	{
		[Test]
		public void When_output_path_is_not_valid_return_combined_path()
		{
			var document = new ProjectDocument(ProjectType.CSharp);
			document.SetAssemblyName("mehassembly.dll");
			document.SetOutputPath(string.Format("bin{0}Debug", Path.DirectorySeparatorChar));
			var project = new Project(string.Format("C:{0}Project{0}Location{0}meh.csproj", Path.DirectorySeparatorChar), document);
			var assembly = project.GetAssembly();
			assembly.ShouldEqual(string.Format(@"C:{0}Project{0}Location{0}bin{0}Debug{0}mehassembly.dll", Path.DirectorySeparatorChar));
		}
	}
}

