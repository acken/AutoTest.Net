using System;
namespace AutoTest.Messages
{
	public enum InformationType
    {
        Build,
        TestRun
    }

	[Serializable]
    public class RunInformationMessage : IMessage
    {
        public InformationType Type { get; private set; }
        public string Project { get; private set; }
        public string Assembly { get; private set; }
        public Type Runner { get; private set; }

        public RunInformationMessage(InformationType type, string project, string assembly, Type runner)
        {
            Type = type;
            Project = project;
            Assembly = assembly;
            Runner = runner;
        }
    }
}

