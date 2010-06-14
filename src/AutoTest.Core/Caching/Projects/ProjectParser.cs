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
        private const string NUNIT_REFERENCE = "<Reference Include=\"nunit.framework";
        private const string CSHARP_PROJECT_EXTENTION = ".csproj";
        private const string VB_PROJECT_EXTENTION = ".vbproj";
        private const string PROJECT_REFERENCE_START = "<ProjectReference Include=\"";
        private const string PROJECT_REFERENCE_END = "\">";

        private IFileSystemService _fsService;
        private string _projectFile;

        private string _fileContent;
        private ProjectType _type;
        private bool _containsTests;
        private string[] _references = new string[] {};

        public ProjectParser(IFileSystemService fsService)
        {
            _fsService = fsService;
        }

        public ProjectDocument Parse(string projectFile, ProjectDocument document)
        {
            _projectFile = projectFile;
            readFile();
            setType();
            setContainsTests();
            setReferences();
            return getDocument(document);
        }

        private void readFile()
        {
            _fileContent = _fsService.ReadFileAsText(_projectFile);
        }

        private void setType()
        {
            switch (Path.GetExtension(_projectFile).ToLower())
            {
                case CSHARP_PROJECT_EXTENTION:
                    _type = ProjectType.CSharp;
                    break;
                case VB_PROJECT_EXTENTION:
                    _type = ProjectType.VisualBasic;
                    break;
            }
        }

        private void setContainsTests()
        {
            _containsTests = _fileContent.Contains(NUNIT_REFERENCE);
        }

        private ProjectDocument getDocument(ProjectDocument existingDocument)
        {
            var document = new ProjectDocument(_type, _containsTests);
            document.AddReference(_references);
            if (existingDocument != null)
                document.AddReferencedBy(existingDocument.ReferencedBy);
            document.HasBeenReadFromFile();
            return document;
        }

        private void setReferences()
        {
            var regExp = new Regex(string.Format("{0}.*?{1}", PROJECT_REFERENCE_START, PROJECT_REFERENCE_END));
            var matches = regExp.Matches(_fileContent);
            _references = new string[matches.Count];
            for (int i = 0; i < matches.Count; i++)
                _references[i] = getReference(matches[i].Value);
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

        private string getAbsolutePath(string relativePath)
        {
            var path = Path.Combine(Path.GetDirectoryName(_projectFile), relativePath);
            return Path.GetFullPath(path);
        }
    }
}
