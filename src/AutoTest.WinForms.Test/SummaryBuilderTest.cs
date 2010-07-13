using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Messaging.MessageConsumers;
using System.IO;

namespace AutoTest.WinForms.Test
{
    [TestFixture]
    public class SummaryBuilderTest
    {
        [Test]
        public void Should_build_tooltip_string()
        {
            var report = new RunReport();
            report.AddBuild(Path.GetFullPath("Project1.csproj"), new TimeSpan(0, 0, 0, 2, 234), true);
            report.AddBuild(Path.GetFullPath("Project2.csproj"), new TimeSpan(0, 0, 0, 3, 154), false);
            report.AddTestRun(Path.GetFullPath("Project3.csproj"), Path.GetFullPath("Assembly.dll"), new TimeSpan(0, 0, 0, 6, 211), 0, 0, 0);

            var builder = new SummaryBuilder(report);
            var output = builder.Build();

            output.ToString().ShouldEqual(getResult());
        }

        private string getResult()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Project1.csproj build succeeded (2,234 sec)");
            builder.AppendLine("Project2.csproj build failed (3,154 sec)");
            builder.AppendLine("Test run for assembly Assembly.dll (Project3.csproj) succeeded (6,211 sec)");
            builder.AppendLine("Finished 3 steps in 00:00:11");
            return builder.ToString();
        }
    }
}
