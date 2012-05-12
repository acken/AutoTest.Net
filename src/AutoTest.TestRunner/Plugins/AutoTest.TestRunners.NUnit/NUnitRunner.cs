using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Core;
using NUnit.Util;
using NUnit.Core.Filters;
using AutoTest.TestRunners.Shared.Errors;
using AutoTest.TestRunners.Shared.Communication;
using AutoTest.TestRunners.Shared.Options;

namespace AutoTest.TestRunners.NUnit
{
    class NUnitRunner : IDisposable
    {
        private readonly ITestFeedbackProvider _channel;
		private TestRunner _testRunner;

        public NUnitRunner(ITestFeedbackProvider channel)
        {
            _channel = channel;
        }

        public void Initialize(string assembly)
        {
            var settingsService = new SettingsService();
            ServiceManager.Services.AddService(settingsService);
            ServiceManager.Services.AddService(new DomainManager());
            ServiceManager.Services.AddService(new ProjectService());
            ServiceManager.Services.AddService(new AddinRegistry());
            ServiceManager.Services.AddService(new AddinManager());
            ServiceManager.Services.AddService(new TestAgency());
            
            ServiceManager.Services.InitializeServices();
			
			var package = createPackage(assembly);
            _testRunner = new DefaultTestRunnerFactory()
				.MakeTestRunner(package);

			_testRunner.Load(package);
            if (_testRunner.Test == null) {
                _testRunner.Unload();
                _channel.TestFinished(ErrorHandler.GetError("NUnit", "Unable to locate fixture"));
				return;
            }
        }

		public void Execute(Options options)
		{
            runTests(options);
		}

		public void Dispose()
		{
			_testRunner.Dispose();
		}

        private void runTests(Options options)
        {   
            var testFilter = getTestFilter(options);
            string savedDirectory = Environment.CurrentDirectory;
            run(testFilter, savedDirectory);
        }

        private void run(TestFilter testFilter, string savedDirectory)
        {
            TestResult result = null;
            try
            {
				_channel.RunStarted();
				var harvester = new TestHarvester(_channel);
                result = _testRunner.Run(harvester, testFilter);
				_channel.RunFinished(harvester.Results.Count());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Environment.CurrentDirectory = savedDirectory;
            }
        }

        private TestFilter getTestFilter(Options options)
        {
            var testFilter = TestFilter.Empty;
            if (!string.IsNullOrEmpty(options.Tests))
                testFilter = new SimpleNameFilter(options.Tests);

            if (!string.IsNullOrEmpty(options.Categories))
            {
                TestFilter excludeFilter = new NotFilter(new CategoryExpression(options.Categories).Filter);
                if (testFilter.IsEmpty)
                    testFilter = excludeFilter;
                else if (testFilter is AndFilter)
                    ((AndFilter)testFilter).Add(excludeFilter);
                else
                    testFilter = new AndFilter(testFilter, excludeFilter);
            }

            var notFilter = testFilter as NotFilter;
            if (notFilter != null)
                notFilter.TopLevel = true;
            return testFilter;
        }
        
        private TestPackage createPackage(string assembly)
        {
            const ProcessModel processModel = ProcessModel.Default;
            
            var package = new TestPackage(assembly);
            var domainUsage = DomainUsage.Single;
			package.TestName = null;
            
            package.Settings["ProcessModel"] = processModel;
            package.Settings["DomainUsage"] = domainUsage;
            //if (framework != null)
            //package.Settings["RuntimeFramework"] = Environment.Version.ToString();
            
            //TODO GFY THIS IS ALWAYS FALSE
            if (domainUsage == DomainUsage.None)
                CoreExtensions.Host.AddinRegistry = Services.AddinRegistry;

            package.Settings["ShadowCopyFiles"] = false;
			package.Settings["UseThreadedRunner"] = false;
            package.Settings["DefaultTimeout"] = 0;

            return package;
		}
    }
}
