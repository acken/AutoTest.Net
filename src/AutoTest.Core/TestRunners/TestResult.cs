namespace AutoTest.Core.TestRunners
{
    public class TestResult
    {
        private readonly TestStatus _status;
        private readonly string _message;
        private static readonly TestResult _passResult;

        /// <summary>
        /// Factory method to return passed message
        /// </summary>
        /// <returns>Passed test result with no message</returns>
        public static TestResult Pass()
        {
            return _passResult;
        }

        /// <summary>
        /// Factory method to return failed result
        /// </summary>
        /// <param name="message">The failure message</param>
        /// <returns>A failed result</returns>
        public static TestResult Fail(string message)
        {
            return new TestResult(TestStatus.Failed, message);
        }

        public TestResult(TestStatus status, string message)
        {
            _status = status;
            _message = message;
        }

        static TestResult()
        {
            _passResult = new TestResult(TestStatus.Passed, string.Empty);
        }

        public TestStatus Status
        {
            get { return _status; }
        }

        public string Message
        {
            get { return _message; }
        }
    }
}