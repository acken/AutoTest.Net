using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.Messaging;
using System.IO;

namespace AutoTest.WinForms
{
    class SummaryBuilder
    {
        private RunReport _report;

        public SummaryBuilder(RunReport report)
        {
            _report = report;
        }

        public string Build()
        {
            var total = new TimeSpan();
            var builder = new StringBuilder();
            foreach (var action in _report.RunActions)
                total = addLinePrAction(action, total, builder);
            addTotalLine(total, builder);
            return builder.ToString();
        }

        private TimeSpan addLinePrAction(RunAction action, TimeSpan total, StringBuilder builder)
        {
            var state = action.Succeeded ? "succeeded" : "failed";
            var project = Path.GetFileName(action.Project);
            var timeSpent = action.TimeSpent;
            return addLineByType(action, total, timeSpent, builder, state, project);
        }

        private TimeSpan addLineByType(RunAction action, TimeSpan total, TimeSpan timeSpent, StringBuilder builder, string state, string project)
        {
            switch (action.Type)
            {
                case InformationType.Build:
                    total = addBuildLine(total, timeSpent, builder, state, project);
                    break;
                case InformationType.TestRun:
                    total = addTestLine(action, total, timeSpent, builder, state, project);
                    break;
            }
            return total;
        }

        private TimeSpan addBuildLine(TimeSpan total, TimeSpan timeSpent, StringBuilder builder, string state, string project)
        {
            total = total.Add(timeSpent);
            builder.AppendLine(string.Format("{1} build {0} ({2},{3} sec)",
                                             state,
                                             project,
                                             timeSpent.Seconds.ToString(),
                                             timeSpent.Milliseconds.ToString()));
            return total;
        }

        private TimeSpan addTestLine(RunAction action, TimeSpan total, TimeSpan timeSpent, StringBuilder builder, string state, string project)
        {
            total = total.Add(timeSpent);
            var assembly = Path.GetFileName(action.Assembly);
            builder.AppendLine(string.Format("Test run for assembly {1} ({2}) {0} ({3},{4} sec)",
                                             state,
                                             assembly,
                                             project,
                                             timeSpent.Seconds.ToString(),
                                             timeSpent.Milliseconds.ToString()));
            return total;
        }

        private void addTotalLine(TimeSpan total, StringBuilder builder)
        {
            builder.AppendLine(string.Format("Finished {0} steps in {1}:{2}:{3}",
                                             _report.RunActions.Length,
                                             getPaddedString(total.Hours),
                                             getPaddedString(total.Minutes),
                                             getPaddedString(total.Seconds)));
        }

        private string getPaddedString(int number)
        {
            return number.ToString().PadLeft(2, '0');
        }
    }
}
