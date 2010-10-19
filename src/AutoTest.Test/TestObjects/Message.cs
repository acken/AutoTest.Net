using AutoTest.Core.Messaging;
using AutoTest.Messages;

namespace AutoTest.Test.TestObjects
{
    internal class Message : IMessage
    {
        public Message(string body)
        {
            Body = body;
        }

        public string Body { get; private set; }
    }
}