using System.Collections.Generic;
namespace AutoTest.Core.BuildRunners
{
    public class BuildRunResults
    {
        private List<BuildMessage> _errors = new List<BuildMessage>();
        private List<BuildMessage> _warnings = new List<BuildMessage>();

        public int ErrorCount { get { return _errors.Count; } }
        public int WarningCount { get { return _warnings.Count; } }
        public BuildMessage[] Errors { get { return _errors.ToArray(); } }
        public BuildMessage[] Warnings { get { return _warnings.ToArray(); } }

        public void AddError(BuildMessage error)
        {
            _errors.Add(error);
        }

        public void AddWarning(BuildMessage warning)
        {
            _warnings.Add(warning);
        }
    }
}