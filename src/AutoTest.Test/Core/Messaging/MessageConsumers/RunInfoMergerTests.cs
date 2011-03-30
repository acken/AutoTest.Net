using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.Caching.Projects;

namespace AutoTest.Test.Core.Messaging.MessageConsumers
{
    [TestFixture]
    public class RunInfoMergerTests
    {
        private List<RunInfo> _list;
        private List<RunInfo> _newList;

        [SetUp]
        public void Setup()
        {
            _list = new List<RunInfo>();
            _newList = new List<RunInfo>();
        }

        [Test]
        public void When_item_does_not_exist_in_target_it_should_add()
        {
            _newList.Add(getRunInfo("NewProject"));
            Assert.That(new RunInfoMerger(_list).MergeWith(_newList).Count, Is.EqualTo(1));
        }

        [Test]
        public void When_item_exists_in_target_it_should_do_nothing()
        {
            var info = getRunInfo("NewProject");
            _list.Add(info);
            _newList.Add(info);
            Assert.That(new RunInfoMerger(_list).MergeWith(_newList).Count, Is.EqualTo(1));
        }

        [Test]
        public void It_should_only_add_run_infos_set_to_build()
        {
            _newList.Add(getRunInfo("NewProject1"));
            _newList.Add(getNotBuiltRunInfo("NewProject2"));
            Assert.That(new RunInfoMerger(_list).MergeWith(_newList).Count, Is.EqualTo(1));
        }

        private RunInfo getRunInfo(string key)
        {
            var info = new RunInfo(new Project(key, new ProjectDocument(ProjectType.CSharp)));
            info.ShouldBuild();
            return info;
        }

        private RunInfo getNotBuiltRunInfo(string key)
        {
            return new RunInfo(new Project(key, new ProjectDocument(ProjectType.CSharp)));
        }
    }
}
