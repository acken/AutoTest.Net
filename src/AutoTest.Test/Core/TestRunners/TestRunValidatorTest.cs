using System;
using NUnit.Framework;
using AutoTest.Core.TestRunners;
using AutoTest.Core.Configuration;
using Rhino.Mocks;
namespace AutoTest.Test.Core.TestRunners
{
	[TestFixture]
	public class TestRunValidatorTest
	{
		private TestRunValidator _validator;
		private IConfiguration _configuration;
		
		[SetUp]
		public void SetUp()
		{
			_configuration = MockRepository.GenerateMock<IConfiguration>();
			_validator = new TestRunValidator(_configuration);
		}
		
		[Test]
		public void Should_not_run_tests_for_excluded_names()
		{
			_configuration.Stub(c => c.TestAssembliesToIgnore).Return(new string[] { "InvalidAssembly.dll" });
			_validator.ShouldNotTestAssembly("InvalidAssembly.dll").ShouldBeTrue();
		}
		
		[Test]
		public void Should_run_tests_for_non_excluded_names()
		{
			_configuration.Stub(c => c.TestAssembliesToIgnore).Return(new string[] {  });
			_validator.ShouldNotTestAssembly("ValidAssembly.dll").ShouldBeFalse();
		}
		
		[Test]
		public void Should_interpret_wildchars_in_front()
		{
			_configuration.Stub(c => c.TestAssembliesToIgnore).Return(new string[] { "*idAssembly.dll" });
			_validator.ShouldNotTestAssembly("InvalidAssembly.dll").ShouldBeTrue();
		}
		
		[Test]
		public void Should_interpret_trailing_wildchars()
		{
			_configuration.Stub(c => c.TestAssembliesToIgnore).Return(new string[] { "InvalidA*" });
			_validator.ShouldNotTestAssembly("InvalidAssembly.dll").ShouldBeTrue();
		}
		
		[Test]
		public void Should_interpret_wildchars_in_front_and_end()
		{
			_configuration.Stub(c => c.TestAssembliesToIgnore).Return(new string[] { "*alidA*" });
			_validator.ShouldNotTestAssembly("InvalidAssembly.dll").ShouldBeTrue();
		}
		
		[Test]
		public void Should_handle_multiple_patterns()
		{
			_configuration.Stub(c => c.TestAssembliesToIgnore).Return(new string[] { "meh", "*alidA*" });
			_validator.ShouldNotTestAssembly("InvalidAssembly.dll").ShouldBeTrue();
		}
	}
}

