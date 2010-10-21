using System;
using System.IO;
namespace AutoTest.Messages
{
	public class StackLineMessage : IStackLine
	{
		private string _method;
		private string _file;
		private int _lineNumber;
		
		#region IStackLine implementation
		public string Method {
			get {
				return _method;
			}
		}

		public string File {
			get {
				return _file;
			}
		}

		public int LineNumber {
			get {
				return _lineNumber;
			}
		}
		#endregion

		public StackLineMessage(string method, string file, int lineNumber)
		{
			_method = method;
			_file = file;
			_lineNumber = lineNumber;
		}
	}
}

