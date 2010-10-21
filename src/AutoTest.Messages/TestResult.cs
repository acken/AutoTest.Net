using System;
using System.IO;
using System.Collections.Generic;
namespace AutoTest.Messages
{
	public class TestResult : ICustomBinarySerializable
	{
		private TestRunStatus _status;
        private string _name;
        private string _message = "";
        private IStackLine[] _stackTrace;
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
            return new TestResult(TestRunStatus.Failed, message);
        }

        public TestResult(TestRunStatus status, string name)
        {
            _name = name;
            _status = status;
        }

        public TestResult(TestRunStatus status, string name, string message)
        {
            _status = status;
            _name = name;
            _message = message;
        }

        public TestResult(TestRunStatus status, string name, string message, IStackLine[] stackTrace)
        {
            _status = status;
            _name = name;
            _message = message;
            _stackTrace = stackTrace;
        }

        static TestResult()
        {
            _passResult = new TestResult(TestRunStatus.Passed, string.Empty);
        }

        public TestRunStatus Status
        {
            get { return _status; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Message
        {
            get { return _message; } set { _message = value; }
        }

        public IStackLine[] StackTrace
        {
            get { return _stackTrace; } set { _stackTrace = value; }
        }

        public override bool Equals(object obj)
        {
            var other = (TestResult) obj;
            return GetHashCode().Equals(other.GetHashCode());
        }

        public override int GetHashCode()
        {
            return string.Format("{0}|{1}|{2}|{3}", _status, _name, _message, _stackTrace).GetHashCode();
        }

		#region ICustomBinarySerializable implementation
		public void WriteDataTo(BinaryWriter writer)
		{
			writer.Write((int) _status);
			writer.Write((string) _name);
			writer.Write((string) _message);
			writer.Write((int) _stackTrace.Length);
			foreach (var line in _stackTrace)
			{
				writer.Write((string) line.Method);
				writer.Write((string) line.File);
				writer.Write((int) line.LineNumber);
			}
		}

		public void SetDataFrom(BinaryReader reader)
		{
			var stackTrace = new List<IStackLine>();
			_status = (TestRunStatus) reader.ReadInt32();
			_name = reader.ReadString();
			_message = reader.ReadString();
			var count = reader.ReadInt32();
			for (int i = 0; i < count; i++)
			{
				var method = reader.ReadString();
				var file = reader.ReadString();
				var lineNumber = reader.ReadInt32();
				var line = new StackLineMessage(method, file, lineNumber);
				stackTrace.Add(line);
			}
			_stackTrace = stackTrace.ToArray();
		}
		#endregion
}
}

