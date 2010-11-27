using System;
using NUnit.Framework;
using System.Net.Sockets;
using System.IO;
using AutoTest.Messages.Serializers;
using AutoTest.Messages;
using AutoTest.Core.Messaging;
namespace AutoTest.Test
{
	[TestFixture]
	public class SerializerTests
	{
		private CustomBinaryFormatter _formatter;
		
		[SetUp]
		public void SetUp()
		{
			_formatter = new CustomBinaryFormatter();
		}
		
		[Test]
		public void Should_serialize_build_run_message()
		{
			var results = new BuildRunResults("Project");
			results.AddError(new BuildMessage() { File = "file", LineNumber = 15, LinePosition = 20, ErrorMessage = "Error message" });
			results.AddWarning(new BuildMessage() { File = "file2", LineNumber = 35, LinePosition = 40, ErrorMessage = "Error message2" });
			results.SetTimeSpent(new TimeSpan(23567));
			var message = new BuildRunMessage(results);
			var output = serializeDeserialize<BuildRunMessage>(message);
			output.Results.Project.ShouldEqual("Project");
			output.Results.TimeSpent.ShouldEqual(new TimeSpan(23567));
			output.Results.ErrorCount.ShouldEqual(1);
			output.Results.Errors[0].File.ShouldEqual("file");
			output.Results.Errors[0].LineNumber.ShouldEqual(15);
			output.Results.Errors[0].LinePosition.ShouldEqual(20);
			output.Results.Errors[0].ErrorMessage.ShouldEqual("Error message");
			output.Results.WarningCount.ShouldEqual(1);
			output.Results.Warnings[0].File.ShouldEqual("file2");
			output.Results.Warnings[0].LineNumber.ShouldEqual(35);
			output.Results.Warnings[0].LinePosition.ShouldEqual(40);
			output.Results.Warnings[0].ErrorMessage.ShouldEqual("Error message2");
		}
		
		[Test]
		public void Should_serialize_error_message()
		{
			var message = new ErrorMessage("erro message");
			var output = serializeDeserialize<ErrorMessage>(message);
			output.Error.ShouldEqual("erro message");
		}
		
		[Test]
		public void Should_serialize_information_message()
		{
			var message = new InformationMessage("information message");
			var output = serializeDeserialize<InformationMessage>(message);
			output.Message.ShouldEqual("information message");
		}
		
		[Test]
		public void Should_serialize_run_finished_message()
		{
			var runreport = new RunReport();
			runreport.AddBuild("project 1", new TimeSpan(23), true);
			runreport.AddBuild("project 2", new TimeSpan(12), false);
			runreport.AddTestRun("project 2", "assembly", new TimeSpan(52), 12, 1, 2);
			var message = new RunFinishedMessage(runreport);
			var output = serializeDeserialize<RunFinishedMessage>(message);
			output.Report.NumberOfBuildsSucceeded.ShouldEqual(1);
			output.Report.NumberOfBuildsFailed.ShouldEqual(1);
			output.Report.RunActions[0].Project.ShouldEqual("project 1");
			output.Report.RunActions[0].Type.ShouldEqual(InformationType.Build);
			output.Report.RunActions[0].Succeeded.ShouldEqual(true);
			output.Report.RunActions[0].TimeSpent.ShouldEqual(new TimeSpan(23));
			output.Report.RunActions[1].Project.ShouldEqual("project 2");
			output.Report.RunActions[1].Type.ShouldEqual(InformationType.Build);
			output.Report.RunActions[1].Succeeded.ShouldEqual(false);
			output.Report.RunActions[1].TimeSpent.ShouldEqual(new TimeSpan(12));
			
			output.Report.NumberOfTestsPassed.ShouldEqual(12);
			output.Report.NumberOfTestsFailed.ShouldEqual(2);
			output.Report.NumberOfTestsIgnored.ShouldEqual(1);
			output.Report.RunActions[2].Project.ShouldEqual("project 2");
			output.Report.RunActions[2].Assembly.ShouldEqual("assembly");
			output.Report.RunActions[2].Type.ShouldEqual(InformationType.TestRun);
			output.Report.RunActions[2].Succeeded.ShouldEqual(false);
			output.Report.RunActions[2].TimeSpent.ShouldEqual(new TimeSpan(52));
		}
		
		[Test]
		public void Should_serialize_run_information_message()
		{
			var message = new RunInformationMessage(InformationType.TestRun, "project 1", "assembly", typeof(RunFinishedMessage));
			var output = serializeDeserialize<RunInformationMessage>(message);
			output.Project.ShouldEqual("project 1");
			output.Assembly.ShouldEqual("assembly");
			output.Type.ShouldEqual(InformationType.TestRun);
			output.Runner.ShouldEqual(typeof(RunFinishedMessage));
		}
		
		[Test]
		public void Should_serialize_run_started_message()
		{
			var files = new ChangedFile[] { new ChangedFile(System.Reflection.Assembly.GetExecutingAssembly().FullName) };
			var message = new RunStartedMessage(files);
			var output = serializeDeserialize<RunStartedMessage>(message);
			output.Files.Length.ShouldEqual(1);
			output.Files[0].Name.ShouldEqual(files[0].Name);
			output.Files[0].FullName.ShouldEqual(files[0].FullName);
			output.Files[0].Extension.ShouldEqual(files[0].Extension);
		}
		
		[Test]
		public void Should_serialize_warning_message()
		{
			var message = new WarningMessage("warning");
			var output = serializeDeserialize<WarningMessage>(message);
			output.Warning.ShouldEqual("warning");
		}
		
		[Test]
		public void Should_serialize_test_run_message()
		{
			var testResults = new TestResult[] { new TestResult(TestRunStatus.Passed, "Test name", "message", new IStackLine[] { new StackLineMessage("method name", "file", 13) }) };
			var results = new TestRunResults("project 1", "assembly", false, testResults);
			results.SetTimeSpent(new TimeSpan(12345));
			var message = new TestRunMessage(results);
			var output = serializeDeserialize<TestRunMessage>(message);
			output.Results.Project.ShouldEqual("project 1");
			output.Results.Assembly.ShouldEqual("assembly");
            output.Results.IsPartialTestRun.ShouldBeFalse();
			output.Results.TimeSpent.ShouldEqual(new TimeSpan(12345));
			output.Results.All.Length.ShouldEqual(1);
			output.Results.All[0].Status.ShouldEqual(TestRunStatus.Passed);
			output.Results.All[0].Name.ShouldEqual("Test name");
			output.Results.All[0].Message.ShouldEqual("message");
			output.Results.All[0].StackTrace[0].Method.ShouldEqual("method name");
			output.Results.All[0].StackTrace[0].File.ShouldEqual("file");
			output.Results.All[0].StackTrace[0].LineNumber.ShouldEqual(13);
		}
		
		[Test]
		public void Should_serialize_file_change_message()
		{
			var file = new ChangedFile(System.Reflection.Assembly.GetExecutingAssembly().FullName);
			var message = new FileChangeMessage();
			message.AddFile(file);
			var output = serializeDeserialize<FileChangeMessage>(message);
			output.Files.Length.ShouldEqual(1);
			output.Files[0].Name.ShouldEqual(file.Name);
			output.Files[0].FullName.ShouldEqual(file.FullName);
			output.Files[0].Extension.ShouldEqual(file.Extension);
		}
		
		private T serializeDeserialize<T>(T message)
		{
			using (var memStream = new MemoryStream())
			{
				_formatter.Serialize(memStream, message);
				memStream.Seek(0, SeekOrigin.Begin);
				return (T) _formatter.Deserialize(memStream);
			}
		}
	}
}

