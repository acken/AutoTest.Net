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
using AutoTest.TestRunners.Shared;

namespace AutoTest.TestRunners.NUnit
{
    class NUnitRunner
    {
        public static readonly int OK = 0;
		public static readonly int INVALID_ARG = -1;
		public static readonly int FILE_NOT_FOUND = -2;
		public static readonly int FIXTURE_NOT_FOUND = -3;
		public static readonly int UNEXPECTED_ERROR = -100;

        public void Initialize()
        {
            // Create SettingsService early so we know the trace level right at the start
            SettingsService settingsService = new SettingsService();
            //InternalTraceLevel level = (InternalTraceLevel)settingsService.GetSetting("Options.InternalTraceLevel", InternalTraceLevel.Default);

            //InternalTrace.Initialize("nunit-console_%p.log", level);

            // Add Standard Services to ServiceManager
            ServiceManager.Services.AddService(settingsService);
            ServiceManager.Services.AddService(new DomainManager());
            //ServiceManager.Services.AddService( new RecentFilesService() );
            ServiceManager.Services.AddService(new ProjectService());
            //ServiceManager.Services.AddService( new TestLoader() );
            ServiceManager.Services.AddService(new AddinRegistry());
            ServiceManager.Services.AddService(new AddinManager());
            ServiceManager.Services.AddService(new TestAgency());

            // Initialize Services
            ServiceManager.Services.InitializeServices();
        }

		public IEnumerable<AutoTest.Runners.Shared.TestResult> Execute(Options options)
		{
            TestPackage package = MakeTestPackage(options);

            Console.WriteLine("ProcessModel: {0}    DomainUsage: {1}", 
                package.Settings.Contains("ProcessModel")
                    ? package.Settings["ProcessModel"]
                    : "Default", 
                package.Settings.Contains("DomainUsage")
                    ? package.Settings["DomainUsage"]
                    : "Default");

            Console.WriteLine("Execution Runtime: {0}", 
                package.Settings.Contains("RuntimeFramework")
                    ? package.Settings["RuntimeFramework"]
                    : "Default");
            
            using (TestRunner testRunner = new DefaultTestRunnerFactory().MakeTestRunner(package))
            {
                return runTests(options, package, testRunner);
            }
		}

        private static IEnumerable<AutoTest.Runners.Shared.TestResult> runTests(Options options, TestPackage package, TestRunner testRunner)
        {
            testRunner.Load(package);

            if (testRunner.Test == null)
            {
                testRunner.Unload();
                //Console.Error.WriteLine("Unable to locate fixture {0}", options.fixture);
                return new AutoTest.Runners.Shared.TestResult[] { new AutoTest.Runners.Shared.TestResult(options.Assemblies, "", "", Runners.Shared.TestState.Panic, "Unable to locate fixture") };
            }

            EventCollector collector = new EventCollector(options, null, null);

            TestFilter testFilter = TestFilter.Empty;
            if (options.Tests != null && options.Tests != string.Empty)
            {
                Console.WriteLine("Selected test(s): " + options.Tests);
                testFilter = new SimpleNameFilter(options.Tests);
            }

            if (options.Categories != null && options.Categories != string.Empty)
            {
                Console.WriteLine("Excluded categories: " + options.Categories);
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

            TestResult result = null;
            string savedDirectory = Environment.CurrentDirectory;
            TextWriter savedOut = Console.Out;
            TextWriter savedError = Console.Error;

            try
            {
                result = testRunner.Run(collector, testFilter);
            }
            finally
            {
                Environment.CurrentDirectory = savedDirectory;
                Console.SetOut(savedOut);
                Console.SetError(savedError);
            }

            Console.WriteLine();

            int returnCode = UNEXPECTED_ERROR;

            if (result != null)
            {
                string xmlOutput = CreateXmlOutput(result);
                ResultSummarizer summary = new ResultSummarizer(result);
                returnCode = summary.ErrorsAndFailures;
                return collector.Results;
            }

            if (collector.HasExceptions)
            {
                return new AutoTest.Runners.Shared.TestResult[] { new AutoTest.Runners.Shared.TestResult(options.Assemblies, "", "", Runners.Shared.TestState.Panic, collector.WriteExceptions()) };
            }
            return collector.Results;
        }

		#region Helper Methods
        // TODO: See if this can be unified with the Gui's MakeTestPackage
        private static TestPackage MakeTestPackage(Options options)
        {
			TestPackage package;
			DomainUsage domainUsage = DomainUsage.Default;
            ProcessModel processModel = ProcessModel.Default;
            RuntimeFramework framework = null;

            //string[] parameters = new string[options.ParameterCount];
            //for (int i = 0; i < options.ParameterCount; i++)
            //    parameters[i] = Path.GetFullPath((string)options.Parameters[i]);

            //if (options.IsTestProject)
            //{
                //NUnitProject project = 
                //    Services.ProjectService.LoadProject(options.Assemblies);

                ////string configName = options.config;
                ////if (configName != null)
                ////    project.SetActiveConfig(configName);

                //package = project.ActiveConfig.MakeTestPackage();
                //processModel = project.ProcessModel;
                //domainUsage = project.DomainUsage;
                //framework = project.ActiveConfig.RuntimeFramework;
            //}
            //else if (parameters.Length == 1)
            //{
                package = new TestPackage(options.Assemblies);
                domainUsage = DomainUsage.Single;
            //}
            //else
            //{
            //    // TODO: Figure out a better way to handle "anonymous" packages
            //    package = new TestPackage(null, parameters);
            //    package.AutoBinPath = true;
            //    domainUsage = DomainUsage.Multiple;
            //}

            //if (options.process != ProcessModel.Default)
            //    processModel = options.process;

            //if (options.domain != DomainUsage.Default)
            //    domainUsage = options.domain;

            if (options.Framework != null)
                framework = RuntimeFramework.Parse(options.Framework);

			package.TestName = null;
            
            package.Settings["ProcessModel"] = processModel;
            package.Settings["DomainUsage"] = domainUsage;
            if (framework != null)
                package.Settings["RuntimeFramework"] = framework;

            
            if (domainUsage == DomainUsage.None)
            {
                // Make sure that addins are available
                CoreExtensions.Host.AddinRegistry = Services.AddinRegistry;
            }

            package.Settings["ShadowCopyFiles"] = false;
			package.Settings["UseThreadedRunner"] = false;
            package.Settings["DefaultTimeout"] = 0;

            return package;
		}

		private static string CreateXmlOutput( TestResult result )
		{
			StringBuilder builder = new StringBuilder();
			new XmlResultWriter(new StringWriter( builder )).SaveTestResult(result);

			return builder.ToString();
		}

		private static void WriteSummaryReport( ResultSummarizer summary )
		{
            Console.WriteLine(
                "Tests run: {0}, Errors: {1}, Failures: {2}, Inconclusive: {3}, Time: {4} seconds",
                summary.TestsRun, summary.Errors, summary.Failures, summary.Inconclusive, summary.Time);
            Console.WriteLine(
                "  Not run: {0}, Invalid: {1}, Ignored: {2}, Skipped: {3}",
                summary.TestsNotRun, summary.NotRunnable, summary.Ignored, summary.Skipped);
            Console.WriteLine();
        }

        private void WriteErrorsAndFailuresReport(TestResult result)
        {
            reportIndex = 0;
            Console.WriteLine("Errors and Failures:");
            WriteErrorsAndFailures(result);
            Console.WriteLine();
        }

        private void WriteErrorsAndFailures(TestResult result)
        {
            if (result.Executed)
            {
                if (result.HasResults)
                {
                    if ( (result.IsFailure || result.IsError) && result.FailureSite == FailureSite.SetUp)
                        WriteSingleResult(result);

                    foreach (TestResult childResult in result.Results)
                        WriteErrorsAndFailures(childResult);
                }
                else if (result.IsFailure || result.IsError)
                {
                    WriteSingleResult(result);
                }
            }
        }

        private void WriteNotRunReport(TestResult result)
        {
	        reportIndex = 0;
            Console.WriteLine("Tests Not Run:");
	        WriteNotRunResults(result);
            Console.WriteLine();
        }

	    private int reportIndex = 0;
        private void WriteNotRunResults(TestResult result)
        {
            if (result.HasResults)
                foreach (TestResult childResult in result.Results)
                    WriteNotRunResults(childResult);
            else if (!result.Executed)
                WriteSingleResult( result );
        }

        private void WriteSingleResult( TestResult result )
        {
            string status = result.IsFailure || result.IsError
                ? string.Format("{0} {1}", result.FailureSite, result.ResultState)
                : result.ResultState.ToString();

            Console.WriteLine("{0}) {1} : {2}", ++reportIndex, status, result.FullName);

            if ( result.Message != null && result.Message != string.Empty )
                 Console.WriteLine("   {0}", result.Message);

            if (result.StackTrace != null && result.StackTrace != string.Empty)
                Console.WriteLine( result.IsFailure
                    ? StackTraceFilter.Filter(result.StackTrace)
                    : result.StackTrace + Environment.NewLine );
        }
	    #endregion
    }

    class EventCollector : MarshalByRefObject, EventListener
    {
        private int testRunCount;
        private int testIgnoreCount;
        private int failureCount;
        private int level;

        private Options options;
        private TextWriter outWriter;
        private TextWriter errorWriter;

        StringCollection messages;

        private bool progress = false;
        private string currentTestName;

        private string currentAssembly = "";

        private ArrayList unhandledExceptions = new ArrayList();

        private List<AutoTest.Runners.Shared.TestResult> _results = new List<AutoTest.Runners.Shared.TestResult>();

        public IEnumerable<AutoTest.Runners.Shared.TestResult> Results { get { return _results; } }

        public EventCollector(Options options, TextWriter outWriter, TextWriter errorWriter)
        {
            level = 0;
            this.options = options;
            this.outWriter = outWriter;
            this.errorWriter = errorWriter;
            this.currentTestName = string.Empty;
            this.progress = false;

            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(OnUnhandledException);
        }

        public bool HasExceptions
        {
            get { return unhandledExceptions.Count > 0; }
        }

        public string WriteExceptions()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Unhandled exceptions:");
            int index = 1;
            foreach (string msg in unhandledExceptions)
                sb.AppendLine(string.Format("{0}) {1}", index++, msg));
            return sb.ToString();
        }

        public void RunStarted(string name, int testCount)
        {
            currentAssembly = name;
        }

        public void RunFinished(TestResult result)
        {
        }

        public void RunFinished(Exception exception)
        {
        }

        public void TestFinished(TestResult testResult)
        {
            switch (testResult.ResultState)
            {
                case ResultState.Error:
                case ResultState.Failure:
                case ResultState.Cancelled:
                    var result = new AutoTest.Runners.Shared.TestResult(currentAssembly, getFixture(testResult.Test.TestName.FullName), testResult.Test.TestName.FullName, Runners.Shared.TestState.Failed, testResult.Message);

                    testRunCount++;
                    failureCount++;

                    messages.Add(string.Format("{0}) {1} :", failureCount, testResult.Test.TestName.FullName));
                    messages.Add(testResult.Message.Trim(Environment.NewLine.ToCharArray()));
                    
                    string stackTrace = StackTraceFilter.Filter(testResult.StackTrace);
                    if (stackTrace != null && stackTrace != string.Empty)
                    {
                        string[] trace = stackTrace.Split(System.Environment.NewLine.ToCharArray());
                        foreach (string s in trace)
                        {
                            if (s != string.Empty)
                            {
                                result.AddStackLine(new Runners.Shared.StackLine(s));
                                //string link = Regex.Replace(s.Trim(), @".* in (.*):line (.*)", "$1($2)");
                                //messages.Add(string.Format("at\n{0}", link));
                            }
                        }
                    }
                    _results.Add(result);
                    break;

                case ResultState.Inconclusive:
                case ResultState.Success:
                    testRunCount++;
                    _results.Add(new AutoTest.Runners.Shared.TestResult(currentAssembly, getFixture(testResult.Test.TestName.FullName), testResult.Test.TestName.FullName, Runners.Shared.TestState.Passed, testResult.Message));
                    break;

                case ResultState.Ignored:
                case ResultState.Skipped:
                case ResultState.NotRunnable:
                    testIgnoreCount++;
                    var ignoreResult = new AutoTest.Runners.Shared.TestResult(currentAssembly, getFixture(testResult.Test.TestName.FullName), testResult.Test.TestName.FullName, Runners.Shared.TestState.Ignored, testResult.Message);

                    string ignoreStackTrace = StackTraceFilter.Filter(testResult.StackTrace);
                    if (ignoreStackTrace != null && ignoreStackTrace != string.Empty)
                    {
                        string[] ignoreTrace = ignoreStackTrace.Split(System.Environment.NewLine.ToCharArray());
                        foreach (string s in ignoreTrace)
                        {
                            if (s != string.Empty)
                            {
                                ignoreResult.AddStackLine(new Runners.Shared.StackLine(s));
                                //string link = Regex.Replace(s.Trim(), @".* in (.*):line (.*)", "$1($2)");
                                //messages.Add(string.Format("at\n{0}", link));
                            }
                        }
                    }
                    _results.Add(ignoreResult);

                    if (progress)
                        Console.Write("N");
                    break;
            }

            currentTestName = string.Empty;
        }

        private string getFixture(string fullname)
        {
            var end = fullname.LastIndexOf(".");
            if (end == -1)
                return "";
            return fullname.Substring(0, end);
        }

        public void TestStarted(TestName testName)
        {
            currentTestName = testName.FullName;
            //if (options.labels)
            //    outWriter.WriteLine("***** {0}", currentTestName);

            if (progress)
                Console.Write(".");
        }

        public void SuiteStarted(TestName testName)
        {
            if (level++ == 0)
            {
                messages = new StringCollection();
                testRunCount = 0;
                testIgnoreCount = 0;
                failureCount = 0;
                //Trace.WriteLine("################################ UNIT TESTS ################################");
                //Trace.WriteLine("Running tests in '" + testName.FullName + "'...");
            }
        }

        public void SuiteFinished(TestResult suiteResult)
        {
            if (--level == 0)
            {
                //Trace.WriteLine("############################################################################");

                //if (messages.Count == 0)
                //{
                //    Trace.WriteLine("##############                 S U C C E S S               #################");
                //}
                //else
                //{
                //    Trace.WriteLine("##############                F A I L U R E S              #################");

                //    foreach (string s in messages)
                //    {
                //        Trace.WriteLine(s);
                //    }
                //}

                //Trace.WriteLine("############################################################################");
                //Trace.WriteLine("Executed tests       : " + testRunCount);
                //Trace.WriteLine("Ignored tests        : " + testIgnoreCount);
                //Trace.WriteLine("Failed tests         : " + failureCount);
                //Trace.WriteLine("Unhandled exceptions : " + unhandledExceptions.Count);
                //Trace.WriteLine("Total time           : " + suiteResult.Time + " seconds");
                //Trace.WriteLine("############################################################################");
            }
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject.GetType() != typeof(System.Threading.ThreadAbortException))
            {
                this.UnhandledException((Exception)e.ExceptionObject);
            }
        }


        public void UnhandledException(Exception exception)
        {
            // If we do labels, we already have a newline
            unhandledExceptions.Add(currentTestName + " : " + exception.ToString());
            //if (!options.labels) outWriter.WriteLine();
            string msg = string.Format("##### Unhandled Exception while running {0}", currentTestName);
            //outWriter.WriteLine(msg);
            //outWriter.WriteLine(exception.ToString());

            //Trace.WriteLine(msg);
            //Trace.WriteLine(exception.ToString());
        }

        public void TestOutput(TestOutput output)
        {
            switch (output.Type)
            {
                case TestOutputType.Out:
                    outWriter.Write(output.Text);
                    break;
                case TestOutputType.Error:
                    errorWriter.Write(output.Text);
                    break;
            }
        }


        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
