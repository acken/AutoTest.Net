using AutoTest.VS.Util;
using EnvDTE;
using EnvDTE80;
using AutoTest.VS.Util.Builds;
using System;
using System.Collections.Generic;
using AutoTest.Messages;

namespace AutoTest.VS.Util.CommandHandling
{
    public class RunTestsUnderCursor : ICommandHandler
    {
        private readonly string _commandName;
        private readonly Func<bool> _isEnabled;
        private readonly Func<bool> _manualBuild;
        private readonly Action<OnDemandRun> _runTests;
        private readonly DTE2 _applicationObject;
        private readonly IVSBuildRunner _buildRunner;

        public RunTestsUnderCursor(string commandName, Func<bool> isEnabled, Func<bool> manualBuild, Action<OnDemandRun> runTests, DTE2 applicationObject, IVSBuildRunner buildRunner)
        {
            _commandName = commandName;
            _isEnabled = isEnabled;
            _manualBuild = manualBuild;
            _runTests = runTests;
            _applicationObject = applicationObject;
            _buildRunner = buildRunner;
        }

        public void Exec(vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            var pos = new OnDemandRunFromCursorPosition(_applicationObject);
            var types = pos.FromCurrentPosition();
            if (!_manualBuild() || _buildRunner.Build())
                _runTests(types);
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