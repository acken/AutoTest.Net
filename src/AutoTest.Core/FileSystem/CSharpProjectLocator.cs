namespace AutoTest.Core.FileSystem
{
    using System.IO;
    using log4net;
    using Messaging;

    public class CSharpProjectLocator : IConsumerOf<FileChangeMessage>
    {
        readonly IMessageBus _bus;
        readonly IDirectoryCrawler _crawler;
        static readonly ILog _logger = LogManager.GetLogger(typeof(CSharpProjectLocator));

        public CSharpProjectLocator(IMessageBus bus, IDirectoryCrawler crawler)
        {
            _bus = bus;
            _crawler = crawler;
        }

        public void Consume(FileChangeMessage message)
        {
            //only watching .cs files right now! OH NO!!! 
            //if (message.Extension != ".cs" || message.Extension != ".vb") return;
            if(message.Extension != ".cs") return;
            var dir = Directory.GetParent(message.FullName);
            var parent = _crawler.FindParent(dir.FullName, f => f.Extension.Equals(".csproj"));
            //if (parent == null) _crawler.FindParent(dir.FullName, f => f.Extension.Equals(".vbproj"));
            if(parent != null)
            {
                _logger.DebugFormat("Project Locator found a signification change in file \"{0}\". Publishing {1}.", message.Name, typeof(ProjectChangeMessage).Name);
                _bus.Publish(new ProjectChangeMessage(parent));
            }
        }
    }
}