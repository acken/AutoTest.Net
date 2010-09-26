using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NUnit.Framework;
using AutoTest.Core.FileSystem;
using NUnit.Framework.Extensions;
using AutoTest.Core.Configuration;
using Rhino.Mocks;

namespace AutoTest.Test.Core.FileSystem
{
    [TestFixture]
    public class WatchValidatorTest
    {
        private WatchValidator _validator;
		private IConfiguration _configuration;

        [SetUp]
        public void SetUp()
        {
			_configuration = MockRepository.GenerateMock<IConfiguration>();
            _validator = new WatchValidator(_configuration);
        }
        
        [Test]
        public void Should_return_true_if_normal_file()
        {
			_configuration.Stub(c => c.WatchIgnoreList).Return(new string[] {});
            _validator
                .ShouldPublish(Path.GetTempFileName())
                .ShouldBeTrue();
        }

        [Test]
        public void Should_invalidate_bin_debug()
        {
            _validator
                .ShouldPublish(getInfo("something{0}bin{0}Debug{0}"))
                .ShouldBeFalse();
        }

        [Test]
        public void Should_invalidate_bin_release()
        {
            _validator
                .ShouldPublish(getInfo("something{0}bin{0}Release{0}"))
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
                .ShouldPublish(getInfo("something{0}obj{0}Debug{0}"))
                .ShouldBeFalse();
        }

        [Test]
        public void Should_invalidate_obj_release()
        {
            _validator
                .ShouldPublish(getInfo("something{0}obj{0}Release{0}"))
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
		
		[Test]
		public void Should_ignore_directory_with_pattern_name()
		{
			_configuration.Stub(c => c.WatchIgnoreList).Return(new string[] { "meh" });
			_validator.ShouldPublish("/Somedirectory/meh/AndAnotherOne").ShouldBeFalse();
		}
		
		[Test]
		public void Should_match_pattern_to_path_end()
		{
			_configuration.Stub(c => c.WatchIgnoreList).Return(new string[] { "xmloutput.xml" });
			_validator.ShouldPublish("/Somedirectory/hoi/AndAnotherOne/xmloutput.xml").ShouldBeFalse();
		}
		
		[Test]
		public void Should_match_sub_directories_through_glob()
		{
			_configuration.Stub(c => c.WatchIgnoreList).Return(new string[] { "myFolder/*" });
			_validator.ShouldPublish("/Somedirectory/hoi/myFolder/somexmlfile.xml").ShouldBeFalse();
		}
		
		[Test]
		public void Should_glob_case_sensitive()
		{
			_configuration.Stub(c => c.WatchIgnoreList).Return(new string[] { "myFolder/*" });
			_validator.ShouldPublish("/Somedirectory/hoi/myfolder/somexmlfile.xml").ShouldBeTrue();
		}

        private string getInfo(string path)
        {
            return string.Format(path, Path.DirectorySeparatorChar);
        }
    }
}
