using System;
using AutoTest.Messages;
namespace AutoTest.Core.Messaging.MessageConsumers
{
    public interface ILocateRemovedTests
    {
        TestRunResults SetRemovedTestsAsPassed(TestRunResults results, TestRunInfo[] infos);
    }
}
