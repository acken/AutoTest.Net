using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Messages;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.Crawlers;

namespace AutoTest.Core.Messaging.MessageConsumers
{
    class SolutionChangeConsumer : ISolutionChangeConsumer
    {
        private ISolutionParser _crawler;

        public SolutionChangeConsumer(ISolutionParser crawler)
        {
            _crawler = crawler;
        }

        #region ISolutionChangeConsumer Members

        public void Consume(ChangedFile[] files)
        {
            foreach (var file in files)
            {
                if (file.Extension.ToLower().Equals(".sln"))
                    _crawler.Crawl(file.FullName);
            }
        }

        #endregion
    }
}
