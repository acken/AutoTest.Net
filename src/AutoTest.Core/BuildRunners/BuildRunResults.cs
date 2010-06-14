namespace AutoTest.Core.BuildRunners
{
    public class BuildRunResults
    {
        private readonly int _errors;
        private readonly int _warnings;
        private readonly string _buildOutput;

        public BuildRunResults(int errors, int warnings, string buildOutput)
        {
            _errors = errors;
            _warnings = warnings;
            _buildOutput = buildOutput;
        }

        public int Errors
        {
            get { return _errors; }
        }

        public int Warnings
        {
            get { return _warnings; }
        }

        public string BuildOutput { get { return _buildOutput; } }
    }
}