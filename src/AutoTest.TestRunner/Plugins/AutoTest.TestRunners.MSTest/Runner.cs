using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.Options;
using System.Reflection;
using System.IO;
using AutoTest.TestRunners.Shared.Logging;

namespace AutoTest.TestRunners.MSTest
{
    public class Runner : IAutoTestNetTestRunner
    {
        public void SetLogger(ILogger logger)
        {
        }

        public bool Handles(string identifier)
        {
            return identifier == "MSTest";
        }

        public IEnumerable<TestResult> Run(RunnerOptions options)
        {
            var cmd = new Executor();
            var result = cmd.Execute();
            if (result == false)
                throw new Exception("failed");

            //string executableDir = @"C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE\PrivateAssemblies\Microsoft.VisualStudio.QualityTools.CommandLine.dll";
            //string executableDir = @"C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.QualityTools.CommandLine\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.QualityTools.CommandLine.dll";
            //var asm = Assembly.LoadFrom(executableDir);
            //var asm = Assembly.Load("Microsoft.VisualStudio.QualityTools.CommandLine, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            //var asm = Assembly.Load("Microsoft.VisualStudio.QualityTools.CommandLine.dll");
            //throw new Exception("meh");
            //var cmdLine = asm.GetType("Microsoft.VisualStudio.TestTools.CommandLine.Executor");
            //var mstest = Activator.CreateInstance(cmdLine);

            

            // Register commands with the executor to set command-line arguments.
            //Type commandFactoryType =
            //    commandLineAssembly.GetType("Microsoft.VisualStudio.TestTools.CommandLine.CommandFactory");
            //CreateAndAddCommand(executor, commandFactoryType, "/nologo", null);
            //CreateAndAddCommand(executor, commandFactoryType, "/noisolation", null);
            //CreateAndAddCommand(executor, commandFactoryType, "/testmetadata", testMetadataPath);
            //CreateAndAddCommand(executor, commandFactoryType, "/resultsfile", testResultsPath);
            //CreateAndAddCommand(executor, commandFactoryType, "/runconfig", runConfigPath);
            //CreateAndAddCommand(executor, commandFactoryType, "/searchpathroot", searchPathRoot);
            //CreateAndAddCommand(executor, commandFactoryType, "/testlist", SelectedTestListName);

            //var mstest = Activator.CreateInstance(executableDir, "Microsoft.VisualStudio.TestTools.CommandLine.Executor");
            return null;
        }
    }
}
