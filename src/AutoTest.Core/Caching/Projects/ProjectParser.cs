using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using AutoTest.Core.FileSystem;

namespace AutoTest.Core.Caching.Projects
{
    class ProjectParser : IProjectParser
    {
        private const string NUNIT_REFERENCE = "<reference include=\"nunit.framework";
        private const string MSTEST_REFERENCE = "<reference include=\"microsoft.visualstudio.qualitytools.unittestframework";
        private const string CSHARP_PROJECT_EXTENTION = ".csproj";
        private const string VB_PROJECT_EXTENTION = ".vbproj";
        private const string PROJECT_REFERENCE_START = "<ProjectReference Include=\"";
        private const string PROJECT_REFERENCE_END = "\">";

        private IFileSystemService _fsService;
        private string _projectFile;
        private string _fileContent;
        private string _fileContentLower;
        private ProjectType _type;
        private bool _containsNUnitTests;
        private string[] _references = new string[] {};

        public ProjectParser(IFileSystemService fsService)
        {
            _fsService = fsService;
        }

        public ProjectDocument Parse(string projectFile, ProjectDocument existingDocument)
        {
            _projectFile = projectFile;
            readFile();
            var newDocument = new ProjectDocument(getType());
            setContainsTests(newDocument);
            setReferences(newDocument);
            setReferencedBy(newDocument, existingDocument);
            return newDocument;
        }

        private void readFile()
        {
            _fileContent = _fsService.ReadFileAsText(_projectFile);
            _fileContentLower = _fileContent.ToLower();
        }

        private ProjectType getType()
        {
            switch (Path.GetExtension(_projectFile).ToLower())
            {
                case CSHARP_PROJECT_EXTENTION:
                    return ProjectType.CSharp;
                case VB_PROJECT_EXTENTION:
                    return ProjectType.VisualBasic;
            }
            return ProjectType.None;
        }

        private void setContainsTests(ProjectDocument document)
        {
            if (_fileContentLower.Contains(NUNIT_REFERENCE))
                document.SetAsNUnitTestContainer();
            if (_fileContentLower.Contains(MSTEST_REFERENCE))
                document.SetAsMSTestContainer();
        }

        private void setReferences(ProjectDocument document)
        {
            var regExp = new Regex(string.Format("{0}.*?{1}", PROJECT_REFERENCE_START, PROJECT_REFERENCE_END));
            var matches = regExp.Matches(_fileContent);
            _references = new string[matches.Count];
            for (int i = 0; i < matches.Count; i++)
                document.AddReference(getReference(matches[i].Value));
        }

        private string getReference(string match)
        {
            var path = match.Substring(
                PROJECT_REFERENCE_START.Length,
                match.Length - (PROJECT_REFERENCE_START.Length + PROJECT_REFERENCE_END.Length));
            if (path.Substring(0, 2).Equals(".."))
                return getAbsolutePath(path);
            return path;
        }

        private void setReferencedBy(ProjectDocument newDocument, ProjectDocument existingDocument)
        {
            if (existingDocument != null)
                newDocument.AddReferencedBy(existingDocument.ReferencedBy);
            newDocument.HasBeenReadFromFile();
        }

        private string getAbsolutePath(string relativePath)
        {
            var path = Path.Combine(Path.GetDirectoryName(_projectFile), relativePath);
            return Path.GetFullPath(path);
        }
    }
}
