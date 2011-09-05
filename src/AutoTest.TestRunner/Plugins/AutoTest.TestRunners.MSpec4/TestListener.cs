using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications.Runner;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.Communication;

namespace AutoTest.TestRunners.MSpec4
{
    class TestListener : ISpecificationRunListener
    {
        private string _assembly;
        private ITestFeedbackProvider _feedback;
        private List<TestResult> _results = new List<TestResult>();
        private DateTime _start = DateTime.MinValue;

        public TestResult[] Results { get { return _results.ToArray(); } }

        public TestListener(ITestFeedbackProvider feedback, string assembly)
        {
            _feedback = feedback;
            _assembly = assembly;
        }

        public void OnAssemblyEnd(AssemblyInfo assembly)
        {
        }

        public void OnAssemblyStart(AssemblyInfo assembly)
        {
        }

        public void OnContextEnd(ContextInfo context)
        {
        }

        public void OnContextStart(ContextInfo context)
        {
        }

        public void OnFatalError(Machine.Specifications.ExceptionResult exception)
        {
        }

        public void OnRunEnd()
        {
        }

        public void OnRunStart()
        {
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Machine.Specifications.Result result)
        {
            var test = new TestResult(
                    "MSpec",
                    _assembly,
                    specification.ContainingType,
                    DateTime.Now.Subtract(_start).TotalMilliseconds,
                    specification.ContainingType,
                    specification.ContainingType + "." + specification.FieldName,
                    getState(result.Status),
                    getMessage(result.Exception));
            test.AddStackLines(getStackLines(result.Exception));
            _results.Add(test);
            if (_feedback != null)
                _feedback.TestFinished(test);
        }

        private StackLine[] getStackLines(Machine.Specifications.ExceptionResult exceptionResult)
        {
            if (exceptionResult == null)
                return new StackLine[] { };
            return exceptionResult.StackTrace
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => new StackLine(x)).ToArray();
        }

        private string getMessage(Machine.Specifications.ExceptionResult exceptionResult)
        {
            if (exceptionResult == null)
                return "";
            return exceptionResult.Message;
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
            _start = DateTime.Now;
        }

        private TestState getState(Machine.Specifications.Status status)
        {
            if (status == Machine.Specifications.Status.Failing)
                return TestState.Failed;
            else if (status == Machine.Specifications.Status.Ignored)
                return TestState.Ignored;
            else if (status == Machine.Specifications.Status.NotImplemented)
                return TestState.Ignored;
            else if (status == Machine.Specifications.Status.Passing)
                return TestState.Passed;
            return TestState.Panic;
        }
    }
}
