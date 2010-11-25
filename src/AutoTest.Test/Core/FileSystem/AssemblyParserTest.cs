using System;
using NUnit.Framework;
using AutoTest.Core.FileSystem;
namespace AutoTest.Test
{
	[TestFixture]
	public class AssemblyParserTest
	{
		[Test]
		public void Should_retrieve_references()
		{
			var parser = new AssemblyParser();
			var references = parser.GetReferences("AutoTest.Test.dll");
			references.Length.ShouldEqual(9);
		}
	}
}

