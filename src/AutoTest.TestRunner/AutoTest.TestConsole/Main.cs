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
			var runner = new TestRunProcess();
			var options = 
				new RunOptions()
					.AddTestRun(
						new RunnerOptions("NUnit")
							.AddAssembly(
								new AssemblyOptions(assembly)
							)
					);
			var client = runner.Prepare(options);
			client.Load(assembly, "NUnit");
			var result = client.RunTests(
				assembly,
				"NUnit",
				(name) => Console.Write("\tRunning " + name),
				(test) => Console.WriteLine(" => " + 
											test.State.ToString() +
											" (" +
											test.DurationInMilliseconds.ToString() + ")"));
			client.Exit();
		}
	}
}
