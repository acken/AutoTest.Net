using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NUnit.Framework;
using AutoTest.Core.FileSystem;
using NUnit.Framework.Extensions;

namespace AutoTest.Test.Core.FileSystem
{
    [TestFixture]
    public class WatchValidatorTest
    {
        private WatchValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new WatchValidator();
        }
        
        [Test]
        public void Should_return_true_if_normal_file()
        {
            _validator
                .ShouldPublish(Path.GetTempFileName())
                .ShouldBeTrue();
        }

        [Test]
        public void Should_invalidate_bin_debug()
        {
            _validator
                .ShouldPublish(getInfo("something{0}bin{0}debug{0}"))
                .ShouldBeFalse();
        }

        [Test]
        public void Should_invalidate_bin_release()
        {
            _validator
                .ShouldPublish(getInfo("something{0}bin{0}release{0}"))
                .ShouldBeFalse();
        }

        [Test]
        public void Should_invalidate_bin_x86()
        {
            _validator
                .ShouldPublish(getInfo("something{0}bin{0}x86{0}"))
                .ShouldBeFalse();
        }

        [Test]
        public void Should_invalidate_obj_debug()
        {
            _validator
                .ShouldPublish(getInfo("something{0}obj{0}debug{0}"))
                .ShouldBeFalse();
        }

        [Test]
        public void Should_invalidate_obj_release()
        {
            _validator
                .ShouldPublish(getInfo("something{0}obj{0}release{0}"))
                .ShouldBeFalse();
        }

        [Test]
        public void Should_invalidate_obj_x86()
        {
            _validator
                .ShouldPublish(getInfo("something{0}obj{0}x86{0}"))
                .ShouldBeFalse();
        }

        [Test]
        public void Should_invalidate_directories()
        {
            _validator
                .ShouldPublish(Path.GetDirectoryName(Path.GetTempFileName()))
                .ShouldBeFalse();
        }

        [Test]
        public void Should_invalidate_monos_filelistabsolute_file()
        {
            _validator.ShouldPublish(getInfo("something{0}obj{0}SomeProject.csproj.FileListAbsolute.txt"))
                .ShouldBeFalse();
        }

        [Test]
        public void Should_invalidate_monos_fileswrittenabsolute_file()
        {
            _validator.ShouldPublish(getInfo("something{0}obj{0}SomeProject.csproj.FilesWrittenAbsolute.txt"))
                .ShouldBeFalse();
        }

        private string getInfo(string path)
        {
            return string.Format(path, Path.DirectorySeparatorChar);
        }
    }
}
