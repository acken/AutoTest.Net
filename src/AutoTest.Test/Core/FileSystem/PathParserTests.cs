using System;
using NUnit.Framework;
using AutoTest.Core.FileSystem;
namespace AutoTest.Test.Core.FileSystem
{
	[TestFixture]
	public class PathParserTests
	{
		[Test]
		public void When_no_relative_path_it_should_return_passed_path()
		{
			Assert.That(new PathParser(".ignorefile").ToAbsolute("/some/reference/path"), Is.EqualTo("/some/reference/path/.ignorefile"));
		}
		
		[Test]
		public void When_relative_path_it_should_return_path_relative_to_passed_path()
		{
			Assert.That(new PathParser("../.ignorefile").ToAbsolute("/some/reference/path"), Is.EqualTo("/some/reference/.ignorefile"));
		}
	}
}

