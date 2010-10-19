using System;
namespace AutoTest.Messages
{
	[Serializable]
	public class RunAction
	{
		private InformationType _type;
        private string _project;
        private string _assembly;
        private TimeSpan _timeSpent;
        private bool _succeeded;

        public InformationType Type { get { return _type; } }
        public string Project { get { return _project; } }
        public string Assembly { get { return _assembly; } }
        public TimeSpan TimeSpent { get { return _timeSpent; } }
        public bool Succeeded { get { return _succeeded; } }

        public RunAction(InformationType type, string project, TimeSpan timeSpent, bool succeeded)
        {
            setProperties(type, project, "", timeSpent, succeeded);
        }

        public RunAction(InformationType type, string project, string assembly, TimeSpan timeSpent, bool succeeded)
        {
            setProperties(type, project, assembly, timeSpent, succeeded);
        }

        private void setProperties(InformationType type, string project, string assembly, TimeSpan timeSpent, bool succeeded)
        {
            _type = type;
            _project = project;
            _assembly = assembly;
            _timeSpent = timeSpent;
            _succeeded = succeeded;
        }
	}
}

