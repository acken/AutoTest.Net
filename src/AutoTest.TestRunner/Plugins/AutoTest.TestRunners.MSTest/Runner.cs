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
using AutoTest.TestRunners.Shared.AssemblyAnalysis;

namespace AutoTest.TestRunners.MSTest
{
    public class Runner : IAutoTestNetTestRunner
    {
        public string Identifier { get { return "MSTest"; } }

        public void SetLogger(ILogger logger)
        {
        }

        public bool IsTest(string assembly, string member)
        {
            var locator = new SimpleTypeLocator(assembly, member);
            var method = locator.Locate();
            if (method.Category != TypeCategory.Method)
                return false;
            return method.Attributes.Contains("Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod");
        }

        public bool ContainsTestsFor(string assembly, string member)
        {
            var locator = new SimpleTypeLocator(assembly, member);
            var cls = locator.Locate();
            if (cls.Category != TypeCategory.Class)
                return false;
            return cls.Attributes.Contains("Microsoft.VisualStudio.TestTools.UnitTesting.TestClass");
        }

        public bool ContainsTestsFor(string assembly)
        {
            var parser = new AssemblyReader();
            return parser.GetReferences(assembly).Contains("Microsoft.VisualStudio.QualityTools.UnitTestFramework");
        }

        public bool Handles(string identifier)
        {
            return identifier.ToLower().Equals(identifier.ToLower());
        }

        public IEnumerable<TestResult> Run(RunSettings settings)
        {
            //var cmd = new Executor();
            //var result = cmd.Execute();
            //if (result == false)
            //    throw new Exception("failed");

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
            return new TestResult[] {};
        }
    }
}
