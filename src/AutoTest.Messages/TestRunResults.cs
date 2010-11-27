using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace AutoTest.Messages
{
	public class TestRunResults : ICustomBinarySerializable
	{
		private string _project;
        private string _assembly;
        private TimeSpan _timeSpent;
        private TestResult[] _testResults;
        private bool _isPartialTestRun;

        public string Project { get { return _project; } }
        public string Assembly { get { return _assembly; } }
        public TimeSpan TimeSpent { get { return _timeSpent; } }
        public bool IsPartialTestRun { get { return _isPartialTestRun; } }
        public TestResult[] All { get { return _testResults; } }
        public TestResult[] Passed { get { return queryByStatus(TestRunStatus.Passed); } }
        public TestResult[] Failed { get { return queryByStatus(TestRunStatus.Failed); } }
        public TestResult[] Ignored { get { return queryByStatus(TestRunStatus.Ignored); } }

        public TestRunResults(string project, string assembly, bool isPartialTestRun, TestResult[] testResults)
        {
            _project = project;
            _assembly = assembly;
            _isPartialTestRun = isPartialTestRun;
            _testResults = testResults;
        }

        public void SetTimeSpent(TimeSpan timeSpent)
        {
            _timeSpent = timeSpent;
        }

        private TestResult[] queryByStatus(TestRunStatus status)
        {
            var query = from t in _testResults
                        where t.Status.Equals(status)
                        select t;
            return query.ToArray();
        }

		#region ICustomBinarySerializable implementation
		public void WriteDataTo(BinaryWriter writer)
		{
			writer.Write((string) _project);
			writer.Write((string) _assembly);
			writer.Write((double) _timeSpent.Ticks);
            writer.Write((bool) _isPartialTestRun);
			writer.Write((int) _testResults.Length);
			foreach (var result in _testResults)
				result.WriteDataTo(writer);
		}

		public void SetDataFrom(BinaryReader reader)
		{
			var results = new List<TestResult>();
			_project = reader.ReadString();
			_assembly = reader.ReadString();
			_timeSpent = new TimeSpan((long) reader.ReadDouble());
            _isPartialTestRun = reader.ReadBoolean();
			var count = reader.ReadInt32();
			for (int i = 0; i < count; i++)
			{
				var result = new TestResult(TestRunStatus.Ignored, "");
				result.SetDataFrom(reader);
				results.Add(result);
			}
			_testResults = results.ToArray();
		}
		#endregion
	}
}

