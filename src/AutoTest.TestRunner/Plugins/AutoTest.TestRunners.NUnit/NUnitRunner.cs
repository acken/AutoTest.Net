using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Core;
using NUnit.Util;
using NUnit.Core.Filters;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using AutoTest.TestRunners.Shared.Errors;
using AutoTest.TestRunners.Shared.Communication;

namespace AutoTest.TestRunners.NUnit
{
    class NUnitRunner
    {
        private ITestFeedbackProvider _channel;

        public NUnitRunner(ITestFeedbackProvider channel)
        {
            _channel = channel;
        }

        public void Initialize()
        {
            SettingsService settingsService = new SettingsService();
            ServiceManager.Services.AddService(settingsService);
            ServiceManager.Services.AddService(new DomainManager());
            ServiceManager.Services.AddService(new ProjectService());
            ServiceManager.Services.AddService(new AddinRegistry());
            ServiceManager.Services.AddService(new AddinManager());
            ServiceManager.Services.AddService(new TestAgency());
            
            ServiceManager.Services.InitializeServices();
        }

		public IEnumerable<AutoTest.TestRunners.Shared.Results.TestResult> Execute(Options options)
		{
            TestPackage package = createPackage(options);
            using (TestRunner testRunner = new DefaultTestRunnerFactory().MakeTestRunner(package))
            {
                return runTests(options, package, testRunner);
            }
		}

        private IEnumerable<AutoTest.TestRunners.Shared.Results.TestResult> runTests(Options options, TestPackage package, TestRunner testRunner)
        {
            testRunner.Load(package);
            if (testRunner.Test == null)
            {
                testRunner.Unload();
                return new AutoTest.TestRunners.Shared.Results.TestResult[] { ErrorHandler.GetError("NUnit", "Unable to locate fixture") };
            }

            var harvester = new TestHarvester(_channel);
            var testFilter = getTestFilter(options);
            string savedDirectory = Environment.CurrentDirectory;
            var result = run(testRunner, harvester, testFilter, savedDirectory);

            if (result != null)
                return harvester.Results;
            return harvester.Results;
        }

        private TestResult run(TestRunner testRunner, TestHarvester harvester, TestFilter testFilter, string savedDirectory)
        {
            TestResult result = null;
            try
            {
                result = testRunner.Run(harvester, testFilter);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Environment.CurrentDirectory = savedDirectory;
            }
            return result;
        }

        private TestFilter getTestFilter(Options options)
        {
            TestFilter testFilter = TestFilter.Empty;
            if (options.Tests != null && options.Tests != string.Empty)
                testFilter = new SimpleNameFilter(options.Tests);

            if (options.Categories != null && options.Categories != string.Empty)
            {
                TestFilter excludeFilter = new NotFilter(new CategoryExpression(options.Categories).Filter);
                if (testFilter.IsEmpty)
                    testFilter = excludeFilter;
                else if (testFilter is AndFilter)
                    ((AndFilter)testFilter).Add(excludeFilter);
                else
                    testFilter = new AndFilter(testFilter, excludeFilter);
            }

            if (testFilter is NotFilter)
                ((NotFilter)testFilter).TopLevel = true;
            return testFilter;
        }
        
        private TestPackage createPackage(Options options)
        {
			TestPackage package;
			DomainUsage domainUsage = DomainUsage.Default;
            ProcessModel processModel = ProcessModel.Default;
            
            package = new TestPackage(options.Assembly);
            domainUsage = DomainUsage.Single;
			package.TestName = null;
            
            package.Settings["ProcessModel"] = processModel;
            package.Settings["DomainUsage"] = domainUsage;
            //if (framework != null)
            //package.Settings["RuntimeFramework"] = Environment.Version.ToString();
            
            if (domainUsage == DomainUsage.None)
                CoreExtensions.Host.AddinRegistry = Services.AddinRegistry;

            package.Settings["ShadowCopyFiles"] = false;
			package.Settings["UseThreadedRunner"] = false;
            package.Settings["DefaultTimeout"] = 0;

            return package;
		}
    }
}
