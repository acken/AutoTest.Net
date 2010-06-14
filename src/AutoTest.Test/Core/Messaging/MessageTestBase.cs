using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace AutoTest.Test.Core.Messaging
{
    [TestFixture]
    public abstract class MessageTestBase<T>
    {
        protected abstract T CreateMessage();

        [Test]
        public void Should_be_immutable()
        {
            var messageType = CreateMessage().GetType();
            var infos = messageType.GetProperties().Where(p => p.CanWrite);
            var writableProperites = infos.Count();
            if (writableProperites > 0)
            {
                Assert.IsTrue(false,
                            String.Format("Message of type {1} has writable property {0}. Not immutable",
                                          infos.First().Name, messageType.Name));
            }
            var fields = messageType.GetFields();
            foreach (var info in fields)
            {
                if (info.Attributes != FieldAttributes.InitOnly)
                {
                    Assert.IsTrue(false,
                                String.Format("{0} was not readonly in message of type {1}", info.Name, messageType.Name));
                }
            }
        }
    }
}