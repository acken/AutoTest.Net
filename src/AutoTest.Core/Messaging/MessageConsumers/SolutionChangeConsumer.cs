using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Messages;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.Crawlers;
using AutoTest.Core.Configuration;
using System.IO;

namespace AutoTest.Core.Messaging.MessageConsumers
{
    class SolutionChangeConsumer : ISolutionChangeConsumer
    {
        private IConfiguration _configuration;
        private ISolutionParser _crawler;

        public SolutionChangeConsumer(ISolutionParser crawler, IConfiguration configuration)
        {
            _crawler = crawler;
            _configuration = configuration;
        }

        #region ISolutionChangeConsumer Members

        public void Consume(ChangedFile file)
        {
            if (!File.Exists(_configuration.WatchToken))
                return;
            if (file.Extension.ToLower().Equals(".sln"))
                _crawler.Crawl(file.FullName);
        }

        #endregion
    }
}
