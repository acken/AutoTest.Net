using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AutoTest.Core.BuildRunners
{
    class MSBuildOutputParser
    {
        private BuildRunResults _results;
        private string[] _lines;
		private string _line;
		private string _currentProjectPath = null;

        public MSBuildOutputParser(BuildRunResults results, string[] lines)
        {
            _results = results;
            _lines = lines;
        }

        public void Parse()
        {
            foreach (var line in _lines) {
				_line = line;
				if (!detectAndValidateBuildTarget())
					break;
				if (_currentProjectPath == null)
				    continue;
				parseLine();
			}
        }
		
		private bool detectAndValidateBuildTarget()
		{
			if (!_line.EndsWith(" (default targets) ->"))
				return true;
			
			if (failedBuildAlreadyDetected())
				return false;
			
		    _currentProjectPath = Path.GetDirectoryName(_line.Substring(0, _line.Length - " (default targets) ->".Length));
			return true;
		}
		
		private bool failedBuildAlreadyDetected()
		{
			return _results.ErrorCount > 0;
		}
		
		private void parseLine()
		{
			if (_line.Contains(": error"))
            {
                BuildMessage message = parseMessage(": error");
                if (!_results.Errors.Contains(message))
                    _results.AddError(message);
            }
            else if (_line.Contains(": warning"))
            {
                BuildMessage message = parseMessage(": warning");
                if (!_results.Warnings.Contains(message))
                    _results.AddWarning(message);
            }
		}

        private BuildMessage parseMessage(string type)
        {
            var message = new BuildMessage();
            try
            {
                message.File = Path.Combine(_currentProjectPath, _line.Substring(0, _line.IndexOf('(')).Trim());
                var position = _line.Substring(_line.IndexOf('(') + 1, _line.IndexOf(')') - (_line.IndexOf('(') + 1)).Split(',');
                message.LineNumber = int.Parse(position[0]);
                message.LinePosition = int.Parse(position[1]);
                message.ErrorMessage = _line.Substring(_line.IndexOf(type) + type.Length,
                                                       _line.Length - (_line.IndexOf(type) + type.Length)).Trim();
            }
            catch (Exception)
            {
                message.File = "Unknown";
                message.LineNumber = 0;
                message.LinePosition = 0;
                message.ErrorMessage = _line;
            }
            return message;
        }
    }
}
