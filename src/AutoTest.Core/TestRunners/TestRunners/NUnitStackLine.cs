using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.TestRunners
{
    public class NUnitStackLine : IStackLine
    {
        private string _line;
        private string _method = "";
        private string _file = "";
        private int _lineNumber = 0;
		private bool _isMonoStyleStackTrace = false;

        public string Method { get { return _method; } }
        public string File { get { return _file; } }
        public int LineNumber { get { return _lineNumber; } }

        public NUnitStackLine(string line)
        {
            _line = line;
            _method = getMethod();
            _file = getFile();
            _lineNumber = getLineNumber();
        }

        public override string ToString()
        {
            return _line;
        }

        private string getMethod()
        {
            var start = _line.IndexOf("at ");
            if (start < 0)
                return "";
            start += "at ".Length;
            var end = _line.IndexOf(")");
            if (end < 0)
                return "";
            end += 1;
            return _line.Substring(start, end - start);
        }

        private string getFile()
        {
            var start = getFileStart();
            if (start < 0)
                return "";
            var end = getFileEnd();
            if (end < 0)
                return "";
            return _line.Substring(start, end - start);
        }
		
		private int getFileStart()
		{
			var start = _line.IndexOf(") in ");
			if (start >= 0)
				return start + ") in ".Length;
			start = _line.IndexOf("] in ");
			if (start < 0)
				return start;
			_isMonoStyleStackTrace = true;
			return start + "] in ".Length;
		}
		
		private int getFileEnd()
		{
			int endStringLength;
			return getFileEnd(out endStringLength);
		}
		
		private int getFileEnd(out int endStringLength)
		{
			if (_isMonoStyleStackTrace)
				return getMonoStyleFileEnd(out endStringLength);
			else
				return getMSStyleFileEnd(out endStringLength);
		}
		
		private int getMonoStyleFileEnd(out int endStringLength)
		{
			var searchString = ":";
			endStringLength = searchString.Length;
			var fileStart = getFileStart();
			if (fileStart >= 0)
				return _line.IndexOf(searchString, fileStart);
			return -1;
		}
		
		private int getMSStyleFileEnd(out int endStringLength)
		{
			var searchString = ":line";
			endStringLength = searchString.Length;
			return _line.IndexOf(searchString);
		}

        private int getLineNumber()
        {
			int fileEndLocatorStringLength;
            var start = getFileEnd(out fileEndLocatorStringLength);
            if (start < 0)
                return 0;
            start += fileEndLocatorStringLength;
            if (start >= _line.Length)
                return 0;
            var chunk = _line.Substring(start, _line.Length - start);
            int lineNumber;
            if (int.TryParse(chunk, out lineNumber))
                return lineNumber;
            return 0;
        }
    }
}
