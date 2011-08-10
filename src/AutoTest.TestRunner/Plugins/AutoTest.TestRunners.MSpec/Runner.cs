using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Logging;
using AutoTest.TestRunners.Shared.Communication;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;
using AutoTest.TestRunners.Shared.Results;
using Machine.Specifications.Runner.Impl;
using System.Reflection;
using System.IO;

namespace AutoTest.TestRunners.MSpec
{
    public class Runner : IAutoTestNetTestRunner
    {
        private ILogger _logger;
        private Func<string, IReflectionProvider> _reflectionProviderFactory = (assembly) => { return Reflect.On(assembly); };
        private ITestFeedbackProvider _feedback;
        private List<TestResult> _results;

        public string Identifier { get { return "MSpec"; } }

        public void SetLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void SetReflectionProvider(Func<string, IReflectionProvider> reflectionProviderFactory)
        {
            _reflectionProviderFactory = reflectionProviderFactory;
        }

        public void SetLiveFeedbackChannel(ITestFeedbackProvider channel)
        {
            _feedback = channel;
        }

        public bool IsTest(string assembly, string member)
        {
            using (var locator = _reflectionProviderFactory(assembly))
            {
                var fixture = locator.LocateClass(member);
                if (fixture == null)
                    return false;
                return !fixture.IsAbstract &&
                    fixture.Fields.Count(x =>
                    x.FieldType == "Machine.Specifications.Establish" ||
                    x.FieldType == "Machine.Specifications.It" ||
                    x.FieldType == "Machine.Specifications.Because") > 0;
            }
        }

        public bool ContainsTestsFor(string assembly, string member)
        {
            return IsTest(assembly, member);
        }

        public bool ContainsTestsFor(string assembly)
        {
            using (var parser = _reflectionProviderFactory(assembly))
            {
                return parser.GetReferences().Contains("Machine.Specifications");
            }
        }

        public bool Handles(string identifier)
        {
            return identifier.ToLower().Equals("mspec");
        }

        public IEnumerable<TestResult> Run(RunSettings settings)
        {
            _results = new List<TestResult>();
            var listener = new TestListener(_feedback, settings.Assembly.Assembly);
            var assembly = getAssembly(settings.Assembly.Assembly);
            var runner = new AppDomainRunner(listener, Machine.Specifications.Runner.RunOptions.Default);
            runTests(settings, assembly, runner);
            _results.AddRange(listener.Results);
            return _results;
        }

        private void runTests(RunSettings settings, Assembly assembly, AppDomainRunner runner)
        {
            if (runAllTests(settings))
            {
                runner.RunAssembly(assembly);
                return;
            }
            foreach (var member in settings.Assembly.Tests)
                runner.RunMember(assembly, assembly.GetType(member));
            foreach (var member in settings.Assembly.Members)
                runner.RunMember(assembly, assembly.GetType(member));
            foreach (var ns in settings.Assembly.Namespaces)
                runner.RunNamespace(assembly, ns);
        }

        private bool runAllTests(RunSettings settings)
        {
            return
                settings.Assembly.Tests.Count() == 0 &&
                settings.Assembly.Members.Count() == 0 &&
                settings.Assembly.Namespaces.Count() == 0;
        }

        private Assembly getAssembly(string assembly)
        {
            try
            {
                return Assembly.Load(getAssemblySignature(assembly));
            }
            catch (FileLoadException ex)
            {
                _results.Add(new TestResult(Identifier, assembly, "", 0, "Error while preparing runner", TestState.Panic, getMessage(ex) + Environment.NewLine + ex.FileName + Environment.NewLine + ex.FusionLog));
                _results[_results.Count - 1].AddStackLines(getStackLines(ex));
                return null;
            }
            catch (FileNotFoundException ex)
            {
                _results.Add(new TestResult(Identifier, assembly, "", 0, "Error while preparing runner", TestState.Panic, getMessage(ex) + Environment.NewLine + ex.FileName + Environment.NewLine + ex.FusionLog));
                _results[_results.Count - 1].AddStackLines(getStackLines(ex));
                return null;
            }
            catch (BadImageFormatException ex)
            {
                _results.Add(new TestResult(Identifier, assembly, "", 0, "Error while preparing runner", TestState.Panic, getMessage(ex) + Environment.NewLine + ex.FileName + Environment.NewLine + ex.FusionLog));
                _results[_results.Count - 1].AddStackLines(getStackLines(ex));
                return null;
            }
            catch (Exception ex)
            {
                _results.Add(new TestResult(Identifier, assembly, "", 0, "Error while preparing runner", TestState.Panic, getMessage(ex)));
                _results[_results.Count - 1].AddStackLines(getStackLines(ex));
                return null;
            }
        }

        private string getAssemblySignature(string assembly)
        {
            using (var provider = _reflectionProviderFactory(assembly))
            {
                return provider.GetName();
            }
        }

        private string getMessage(Exception ex)
        {
            var message = ex.Message;
            if (ex.InnerException != null)
                message += Environment.NewLine + getMessage(ex.InnerException);
            return message;
        }

        private StackLine[] getStackLines(Exception ex)
        {
            if (ex == null)
                return new StackLine[] { };
            var stackLines = new List<StackLine>();
            if (ex.InnerException != null)
                stackLines.AddRange(getStackLines(ex.InnerException));
            stackLines.AddRange(ex.StackTrace.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Select(x => new StackLine(x)));
            return stackLines.ToArray();
        }
    }
}
