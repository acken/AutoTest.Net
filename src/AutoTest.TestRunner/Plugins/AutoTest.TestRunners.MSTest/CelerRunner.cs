using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.Options;
using Mono.Cecil;
using System.Reflection;
using AutoTest.TestRunners.MSTest.Extensions;
using System.Threading;
using celer.Core.TestRunners;
using System.IO;

namespace AutoTest.TestRunners.MSTest
{
    class CelerRunner : ICelerRunner
    {
        private string Identifier = "MSTest";
        private List<TestResult> _results;

        public IEnumerable<TestResult> Run(RunSettings settings)
        {
            _results = new List<TestResult>();
            var thread = new Thread(run);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(settings);
            thread.Join();
            return _results;
        }

        private void run(object runSettings)
        {
            var settings = (RunSettings)runSettings;
            var tests = getTests(settings);
            if (tests == null)
                return;
            var fixtures = tests.GroupBy(test => test.DeclaringType);
            foreach (var fixture in fixtures)
                runTests(settings, fixture);
        }

        private void runTests(RunSettings settings, IGrouping<Type, MethodInfo> fixture)
        {
            try
            {
                using (var runner = new MSTestTestRunner(fixture.Key))
                {
                    fixture.Select(test => runner.Run(test)).ToList().ForEach(x => _results.Add(getResult(settings, fixture, x)));
                }
            }
            catch (Exception ex)
            {
                _results.Add(new TestResult(Identifier, settings.Assembly.Assembly, fixture.Key.FullName, 0, "", TestState.Failed, "There was an error in class initialize or cleanup: " + Environment.NewLine + ex.Message));
                _results[_results.Count - 1].AddStackLines(getStackLines(ex));
            }
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

        private TestResult getResult(RunSettings settings, IGrouping<Type, MethodInfo> fixture, celer.Core.RunResult x)
        {
            var result = new TestResult(Identifier, settings.Assembly.Assembly, fixture.Key.FullName, x.MillisecondsSpent, fixture.Key.FullName + "." + x.Test.Name, getState(x), getMessage(x));
            result.AddStackLines(getStackLines(x.Exception));
            return result;
        }

        private string getMessage(celer.Core.RunResult x)
        {
            if (x.Exception == null)
                return "";
            return x.Exception.Message;
        }

        private TestState getState(celer.Core.RunResult x)
        {
            if (!x.WasRun)
                return TestState.Ignored;
            if (x.Passed)
                return TestState.Passed;
            else
                return TestState.Failed;
        }

        private IEnumerable<MethodInfo> getTests(RunSettings settings)
        {
            try
            {
                var assembly = Assembly.Load(getAssemblySignature(settings.Assembly.Assembly));
                //var assembly = Assembly.LoadFrom(settings.Assembly.Assembly);
                if (settings.Assembly.Tests.Count() > 0 || settings.Assembly.Members.Count() > 0 || settings.Assembly.Namespaces.Count() > 0)
                    return getTestSelection(assembly, settings);
                else
                    return getAllTests(assembly, settings);
            }
            catch (FileLoadException ex)
            {
                _results.Add(new TestResult(Identifier, settings.Assembly.Assembly, "", 0, "Error while preparing runner", TestState.Panic, getMessage(ex) + Environment.NewLine + ex.FileName + Environment.NewLine + ex.FusionLog));
                _results[_results.Count - 1].AddStackLines(getStackLines(ex));
                return null;
            }
            catch (FileNotFoundException ex)
            {
                _results.Add(new TestResult(Identifier, settings.Assembly.Assembly, "", 0, "Error while preparing runner", TestState.Panic, getMessage(ex) + Environment.NewLine + ex.FileName + Environment.NewLine + ex.FusionLog));
                _results[_results.Count - 1].AddStackLines(getStackLines(ex));
                return null;
            }
            catch (BadImageFormatException ex)
            {
                _results.Add(new TestResult(Identifier, settings.Assembly.Assembly, "", 0, "Error while preparing runner", TestState.Panic, getMessage(ex) + Environment.NewLine + ex.FileName + Environment.NewLine + ex.FusionLog));
                _results[_results.Count - 1].AddStackLines(getStackLines(ex));
                return null;
            }
            catch (Exception ex)
            {
                _results.Add(new TestResult(Identifier, settings.Assembly.Assembly, "", 0, "Error while preparing runner", TestState.Panic, getMessage(ex)));
                _results[_results.Count - 1].AddStackLines(getStackLines(ex));
                return null;
            }
        }

        private string getMessage(Exception ex)
        {
            var message = ex.Message;
            if (ex.InnerException != null)
                message += Environment.NewLine + getMessage(ex);
            return message;
        }

        private string getAssemblySignature(string assembly)
        {
            var asm = AssemblyDefinition.ReadAssembly(assembly);
            return asm.FullName;
        }

        private IEnumerable<MethodInfo> getTestSelection(Assembly assembly, RunSettings settings)
        {
            var tests = new List<MethodInfo>();
            assembly.GetFixtures().ForEach(fixture => tests.AddRange(fixture.GetTestsMatching(settings)));
            return tests;
        }

        private IEnumerable<MethodInfo> getAllTests(Assembly assembly, RunSettings settings)
        {
            var tests = new List<MethodInfo>();
            assembly.GetFixtures().ForEach(fixture => tests.AddRange(fixture.GetTests(settings.IgnoreCategories)));
            return tests;
        }

        private string getException(Exception ex)
        {
            var error = "";
            if (ex.InnerException != null)
                error = Environment.NewLine + getException(ex.InnerException);
            return ex.ToString() + error;
        }
    }
}
