using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.TestRunners
{
    class Arguments
    {
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public bool StartSuspended { get; set; }
        public bool Silent { get; set; }
    }

    class ArgumentParser
    {
        private string[] _arguments;
        private Arguments _parsedArgument;

        public ArgumentParser(string[] arguments)
        {
            _arguments = arguments;
        }

        public Arguments Parse()
        {
            _parsedArgument = new Arguments();
            foreach (var argument in _arguments)
                parse(argument);
            return _parsedArgument;
        }

        private void parse(string argument)
        {
            if (iAm(argument, "--input="))
                _parsedArgument.InputFile = getValue(argument, "--input=");
            if (iAm(argument, "--output="))
                _parsedArgument.OutputFile = getValue(argument, "--output=");
            if (iAm(argument, "--startsuspended"))
                _parsedArgument.StartSuspended = true;
            if (iAm(argument, "--silent"))
                _parsedArgument.Silent = true;
        }

        private bool iAm(string argument, string parameterName)
        {
            return argument.StartsWith(parameterName);
        }

        private string getValue(string parameter, string parameterName)
        {
            if (parameterName.Length >= parameter.Length)
                return "";
            return parameter.Substring(parameterName.Length, parameter.Length - parameterName.Length).Replace("\"", "");
        }
    }
}
