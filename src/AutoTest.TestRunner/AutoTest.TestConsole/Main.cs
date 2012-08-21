using System;
using System.IO;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Options;

namespace AutoTest.TestConsole
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var assembly = Path.GetFullPath(args[0]);
			var runner = new TestRunProcess()
                .SetInternalLoggerTo((s) => Console.WriteLine(s));
			var options = 
				new RunOptions()
					.AddTestRun(
						new RunnerOptions("NUnit")
							.AddAssembly(
								new AssemblyOptions(assembly)
							)
					);
			var process = runner.Prepare(options);
            System.Threading.Thread.Sleep(1000);
			process.CreateClient(assembly, "nunit", () => false).Load();
			//var client = process.CreateClient(assembly, "nunit", () => false);
			//client.Load();
			var result = process.CreateClient(assembly, "nunit", () => false).RunTests(
				new TestRunOptions()
					.AddTest("AutoTest.TestRunners.NUnit.Tests.RunnerTests.Should_recognize_inherited_fixture"),
				(name) => Console.Write("\tRunning " + name),
				(test) => Console.WriteLine(" => " + 
											test.State.ToString() +
											" (" +
											test.DurationInMilliseconds.ToString() + ")"));
			Console.WriteLine("Ran {0} tests", result.Count);
			process.CreateClient(assembly, "nunit", () => false).Exit();
		}
	}
}
