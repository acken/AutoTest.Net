using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Logging;
using AutoTest.TestRunners.Shared.Options;
using System.Reflection;
using Gallio.Common.Reflection;
using Gallio.Model;
using Gallio.Runtime;
using System.IO;
using Gallio.Model.Tree;
using Gallio.Model.Messages;
using Gallio.Common.Messaging;
using Gallio.Model.Isolation;
using Gallio.Model.Messages.Execution;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Model.Schema;
using Gallio.Model.Messages.Exploration;
using AutoTest.TestRunners.Shared.Communication;

namespace AutoTest.TestRunners.MbUnit
{
    public class Runner : IAutoTestNetTestRunner
    {
        private static Gallio.Runtime.RuntimeSetup _setup = null;
        private static Gallio.Runtime.Logging.ILogger _logger = null;
        private static ITestDriver _testDriver;
        private ILogger _internalLogger;
        private ITestFeedbackProvider _channel = null;

        public Runner()
        {
            if (_setup != null)
                return;
            // Create a runtime setup.
            // There are a few things you can tweak here if you need.
            _setup = new Gallio.Runtime.RuntimeSetup();
            _setup.RuntimePath = @"C:\Users\ack\bin\GallioBundle-3.2.750.0\bin"; // @"C:\Users\ack\bin\Gallio2"; //Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); //@"C:\Users\ack\bin\GallioBundle-3.2.750.0\bin";

            // Create a logger.
            // You can use the NullLogger but you will probably want a wrapper around your own ILogger thingy.
            _logger = Gallio.Runtime.Logging.NullLogger.Instance;

            // Initialize the runtime.
            // You only do this once.
            RuntimeBootstrap.Initialize(_setup, _logger);

            // Create a test framework selector.
            // This is used by Gallio to filter the set of frameworks it will support.
            // You can set a predicate Filter here.  I've hardcoded MbUnit here but you could leave the filter out and set it to null.
            // The fallback mode tells Gallio what to do if it does not recognize the test framework associated with the test assembly.
            // Strict means don't do anything.  You might want to use Default or Approximate.  See docs.
            // You can also set options, probably don't care.
            var testFrameworkSelector = new TestFrameworkSelector()
            {
                Filter = testFrameworkHandle => testFrameworkHandle.Id == "MbUnit.TestFramework",
                FallbackMode = TestFrameworkFallbackMode.Strict
            };

            // Now we need to get a suitably configured ITestDriver...
            var testFrameworkManager = RuntimeAccessor.ServiceLocator.Resolve<ITestFrameworkManager>();
            _testDriver = testFrameworkManager.GetTestDriver(testFrameworkSelector, _logger);
        }

        public string Identifier
        {
            get { throw new NotImplementedException(); }
        }

        public void SetLogger(ILogger logger)
        {
            _internalLogger = logger;
        }

        public void SetLiveFeedbackChannel(ITestFeedbackProvider channel)
        {
            _channel = channel;
        }

        public bool IsTest(string assembly, string member)
        {
            MemberInfo mem = getTestFromName(assembly, member);
            if (mem == null)
                return false;
            IMemberInfo memberInfo = Gallio.Common.Reflection.Reflector.Wrap(mem);
            IList<TestPart> testParts = _testDriver.GetTestParts(Reflector.NativeReflectionPolicy, memberInfo);
            foreach (TestPart testPart in testParts)
            {
                if (testPart.IsTest)
                    return true;
            }
            return false;
        }

        public bool ContainsTestsFor(string assembly, string member)
        {
            return getTests(assembly).Exists(x => x.Equals(member));
        }

        public bool ContainsTestsFor(string assembly)
        {
            return getTests(assembly).Count > 0;
        }

        public bool Handles(string identifier)
        {
            return identifier.ToLower().Equals("mbunit");
        }

        public IEnumerable<AutoTest.TestRunners.Shared.Results.TestResult> Run(RunSettings settings)
        {
            var tests = settings.Assembly.Tests.ToList();
            var members = settings.Assembly.Members.ToList();
            var namespaces = settings.Assembly.Namespaces.ToList();

            var runAll = namespaces.Count == 0 && members.Count == 0 && tests.Count == 0;
            var steps = new List<TestStepData>();
            var testResults = new List<AutoTest.TestRunners.Shared.Results.TestResult>();

             // Get a test isolation context.  Here we want to run tests in the same AppDomain.
            var testIsolationProvider = (ITestIsolationProvider) RuntimeAccessor.ServiceLocator.ResolveByComponentId("Gallio.LocalTestIsolationProvider");
            var testIsolationOptions = new TestIsolationOptions();
            ITestIsolationContext testIsolationContext = testIsolationProvider.CreateContext(testIsolationOptions, _logger);

            // Create a test package.
            // You can set a whole bunch of options here.
            var testPackage = new TestPackage();
            testPackage.AddFile(new FileInfo(settings.Assembly.Assembly));
            testPackage.TestFrameworkFallbackMode = TestFrameworkFallbackMode.Strict;

            // Create some test exploration options.  Nothing interesting for you here, probably.
            var testExplorationOptions = new TestExplorationOptions();
            
            // Provide a message sink of your own.
            // Here we are using a message consumer to make it easier but you can implement IMessageSink yourself.
            // While the tests are running, your message sink will receive messages about the tests.
            // Exploration messages: http://www.gallio.org/api/html/N_Gallio_Model_Messages_Exploration.htm
            // Execution messages: http://www.gallio.org/api/html/N_Gallio_Model_Messages_Execution.htm
            var messageSink = new MessageConsumer()
                .Handle<TestStepStartedMessage>((message) =>
                    {
                        steps.Add(message.Step);
                        if (runAll)
                            return;
                        
                        var test = message.Step;
                        if (!test.IsTestCase)
                            return;
                        if (test.ParentId == null)
                            return;
                        if (test.CodeReference.NamespaceName == null)
                            return;

                        var ns = test.CodeReference.NamespaceName;
                        var fixture = string.Format("{0}.{1}", test.CodeReference.NamespaceName, steps.First(x => x.Id.Equals(test.ParentId)).Name);
                        var testName = string.Format("{0}.{1}", fixture, test.Name);
                        if (namespaces.Exists(x => ns.StartsWith(x)) || members.Exists(x => x.Equals(fixture)) || tests.Exists(x => x.Equals(testName)))
                            return;
                        message.Step.IsTestCase = false;
                    })
                .Handle<TestStepFinishedMessage>(message =>
                {
                    var test = steps.FirstOrDefault(x => x.Id.Equals(message.StepId) && x.IsTestCase);
                    if (test == null)
                        return;
                    var fixture = string.Format("{0}.{1}", test.CodeReference.NamespaceName, steps.First(x => x.Id.Equals(test.ParentId)).Name);
                    var result = new AutoTest.TestRunners.Shared.Results.TestResult(
                        "MbUnit",
                        settings.Assembly.Assembly,
                        fixture,
                        message.Result.Duration.TotalMilliseconds,
                        string.Format("{0}.{1}", fixture, test.Name),
                        convertState(message.Result.Outcome.Status),
                        message.Result.Outcome.DisplayName);
                    testResults.Add(result);
                    _channel.TestFinished(result);
                });

            // Provide a progress monitor.
            var logProgressMonitorProvider = new LogProgressMonitorProvider(_logger);

            //return Run(testListener, assemblyPath, new AndFilter<ITestDescriptor>(new Filter<ITestDescriptor>[]
            //{ 
            //    new NamespaceFilter<ITestDescriptor>(new EqualityFilter<string>(@namespace))
            //}), facadeOptions);

            // Run the tests.
            logProgressMonitorProvider.Run((progressMonitor) => {
                _testDriver.Run(testIsolationContext, testPackage, testExplorationOptions, new TestExecutionOptions(), messageSink, progressMonitor);
            });

            return testResults;
        }

        private Shared.Results.TestState convertState(TestStatus testStatus)
        {
            switch (testStatus)
            {
                case TestStatus.Passed:
                    return Shared.Results.TestState.Passed;
                case TestStatus.Inconclusive:
                case TestStatus.Skipped:
                    return Shared.Results.TestState.Ignored;
                case TestStatus.Failed:
                    return Shared.Results.TestState.Failed;
            }
            return Shared.Results.TestState.Panic;
        }

        private List<Test> getTests(string assembly)
        {
            var testIsolationProvider = (ITestIsolationProvider) RuntimeAccessor.ServiceLocator.ResolveByComponentId("Gallio.LocalTestIsolationProvider");
            var testIsolationOptions = new TestIsolationOptions();
            ITestIsolationContext testIsolationContext = testIsolationProvider.CreateContext(testIsolationOptions, _logger);

            // Create a test package.
            // You can set a whole bunch of options here.
            var testPackage = new TestPackage();
            testPackage.AddFile(new FileInfo(assembly));
            testPackage.TestFrameworkFallbackMode = TestFrameworkFallbackMode.Strict;

            var testExplorationOptions = new TestExplorationOptions();

            // This query you can answer by exploring tests and looking at their metadata for "TestsOn".
            // You can explore tests using the Explore method of TestDriver. It sends a bunch of
            // messages to an IMessageSink just like Run.  Just scan these messages for tests that
            // match the pattern you like.

            // Alternately, you can build a TestModel once up front and traverse the resulting test tree
            // to find what you need.  The Explore method of testDriver is just like Run, except that
            // it does not run the tests.
            // TestModelSerializer is a little helper we can use to build up a TestModel from test messages on the fly.
            // The TestModel is just a tree of test metadata.  Pretty straightforward.
            TestModel testModel = new TestModel();
            IMessageSink messageSink = TestModelSerializer.CreateMessageSinkToPopulateTestModel(testModel);

            var logProgressMonitorProvider = new LogProgressMonitorProvider(_logger);

            logProgressMonitorProvider.Run((progressMonitor) => {
                _testDriver.Explore(testIsolationContext, testPackage, testExplorationOptions, messageSink, progressMonitor);
            });

            return testModel.AllTests.Where(x => x.IsTestCase).ToList();
        }

        private MemberInfo getTestFromName(string assembly, string member)
        {
            var members = new List<MemberInfo>();
            Assembly.LoadFrom(assembly).GetModules().ToList()
                .ForEach(x => x.GetTypes().ToList()
                    .ForEach(t => t.GetMembers().Where(tm => string.Format("{0}.{1}", t.FullName, tm.Name).Equals(member)).ToList()
                        .ForEach(m => members.Add(m))));
            return members.FirstOrDefault();
        }
    }
}
