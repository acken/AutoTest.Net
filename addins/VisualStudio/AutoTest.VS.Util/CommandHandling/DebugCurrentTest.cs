using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE80;
using EnvDTE;
using AutoTest.VS.Util;
using AutoTest.Messages;
using AutoTest.VS.Util.Debugger;
using AutoTest.VS.Util.Builds;

namespace AutoTest.VS.Util.CommandHandling
{
    public class DebugCurrentTest : ICommandHandler
    {
        private readonly string _commandName;
        private readonly Func<bool> _isEnabled;
        private readonly Func<bool> _manualBuild;
        private readonly Func<string, string> _getAssemblyFromProject;
        private readonly Action<CacheTestMessage> _debug;
        private readonly DTE2 _applicationObject;
        private readonly IVSBuildRunner _buildRunner;

        public DebugCurrentTest(string commandName, Func<bool> isEnabled, Func<bool> manualBuild, Func<string, string> getAssemblyFromProject, Action<CacheTestMessage> debug, DTE2 applicationObject, IVSBuildRunner buildRunner)
        {
            _commandName = commandName;
            _isEnabled = isEnabled;
            _manualBuild = manualBuild;
            _getAssemblyFromProject = getAssemblyFromProject;
            _debug = debug;
            _applicationObject = applicationObject;
            _buildRunner = buildRunner;
        }

        public void Exec(vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            var pos = new OnDemandRunFromCursorPosition(_applicationObject);
            var types = pos.FromCurrentPosition();
            if (types.Tests.Count() == 1 || types.Members.Count() == 1)
            {
                var assembly = _getAssemblyFromProject(_applicationObject.ActiveDocument.ProjectItem.ContainingProject.FullName);
                if (assembly == null)
                    return;
                var test = types.Tests.Count() > 0 ? types.Tests.ElementAt(0) : types.Members.ElementAt(0);

                if (!_manualBuild() || _buildRunner.Build())
                {
                    var debugger = new DebugHandler(_applicationObject);
                    var testMessage = new CacheTestMessage(assembly, new Messages.TestResult(Messages.TestRunner.Any, Messages.TestRunStatus.Passed, test));
                    _debug(testMessage);
                }
            }
        }

        public void QueryStatus(vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
        {
            StatusOption = _isEnabled()
                               ? vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled
                               : vsCommandStatus.vsCommandStatusSupported;
        }

        public string Name
        {
            get { return _commandName; }
        }
    }
}
