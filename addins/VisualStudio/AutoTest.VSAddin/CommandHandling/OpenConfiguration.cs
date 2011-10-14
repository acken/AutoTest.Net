using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.VS.Util.CommandHandling;
using EnvDTE80;
using EnvDTE;
using System.Diagnostics;
using System.IO;
using AutoTest.Messages.FileStorage;

namespace AutoTest.VSAddin.CommandHandling
{
    class OpenConfiguration : ICommandHandler
    {
        private readonly string _commandName;
        private readonly Func<string> _getPath;
        private readonly Func<string, string> _translate;
        private readonly string _configurationDirectory;

        public OpenConfiguration(string commandName, Func<string> getPath, Func<string, string> translate)
        {
            _commandName = commandName;
            _getPath = getPath;
            _translate = translate;
        }

        public void Exec(vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            var file = _translate(Path.Combine(_getPath(), "AutoTest.config"));
            if (!File.Exists(file))
                File.WriteAllText(file, "");
            var process = new System.Diagnostics.Process();
            process.StartInfo = new System.Diagnostics.ProcessStartInfo(file, "");
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            process.Start();
            Handled = true;
        }

        public void QueryStatus(vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
        {
            StatusOption = Directory.Exists(_getPath()) ?
                vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled :
                vsCommandStatus.vsCommandStatusSupported;
        }

        public string Name
        {
            get { return _commandName; }
        }
    }
}
