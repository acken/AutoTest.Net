using System;
using System.Collections.Generic;
namespace AutoTest.Messages
{
	[Serializable]
	public class RunReport
	{
		private int _numberOfBuildsSucceeded = 0;
        private int _numberOfBuildsFailed = 0;
        private int _numberOfTestsPassed = 0;
        private int _numberOfTestsFailed = 0;
        private int _numberOfTestsIgnored = 0;
        private List<RunAction> _runActions = new List<RunAction>();

        public int NumberOfBuildsSucceeded { get { return _numberOfBuildsSucceeded; } }
        public int NumberOfBuildsFailed { get { return _numberOfBuildsFailed; } }
        public int NumberOfProjectsBuilt { get { return _numberOfBuildsSucceeded + _numberOfBuildsFailed; } }
        public int NumberOfTestsPassed { get { return _numberOfTestsPassed; } }
        public int NumberOfTestsFailed { get { return _numberOfTestsFailed; } }
        public int NumberOfTestsIgnored { get { return _numberOfTestsIgnored; } }
        public int NumberOfTestsRan { get { return _numberOfTestsPassed + _numberOfTestsFailed + _numberOfTestsIgnored; } }

        public RunAction[] RunActions { get { return _runActions.ToArray(); } }

        public void AddBuild(string project, TimeSpan timeSpent, bool succeeded)
        {
            _runActions.Add(new RunAction(InformationType.Build, project, timeSpent, succeeded));
            if (succeeded)
                _numberOfBuildsSucceeded++;
            else
                _numberOfBuildsFailed++;
        }

        public void AddTestRun(string project, string assembly, TimeSpan timeSpent, int passed, int ignored, int failed)
        {
            var succeeded = ignored == 0 && failed == 0;
            _runActions.Add(new RunAction(InformationType.TestRun, project, assembly, timeSpent, succeeded));
            _numberOfTestsPassed += passed;
            _numberOfTestsIgnored += ignored;
            _numberOfTestsFailed += failed;
        }
	}
}

