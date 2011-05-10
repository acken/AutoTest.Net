using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using AutoTest.Core.FileSystem;
using AutoTest.Core.DebugLog;

namespace AutoTest.Core.Caching.Projects
{
    class ProjectParser : IProjectParser
    {
        private const string ASSEMBLYNAME_NODE = "AssemblyName";
        private const string OUTPUT_TYPE = "OutputType";
        private const string CSHARP_PROJECT_EXTENTION = ".csproj";
        private const string VB_PROJECT_EXTENTION = ".vbproj";
        private const string PROJECT_REFERENCE_START = "<ProjectReference Include=\"";
        private const string PROJECT_REFERENCE_END = "\">";
		private const string BINARY_REFERENCE_START = "<Reference Include=\"";
        private const string BINARY_REFERENCE_END = "\"";
        private const string CONFIGURATION_START = "<Configuration Condition";
        private const string CONFIGURATION_START_CONTENT_STARTS_HERE = "\">";
        private const string CONFIGURATION_END = "</Configuration>";
		private const string PLATFORM_NON_DEFAULT_START = "|$(Platform)' == 'Debug|";
		private const string PLATFORM_NON_DEFAULT_END = "'";
        private const string PLATFORM_START = "<Platform Condition";
        private const string PLATFORM_START_CONTENT_STARTS_HERE = "\">";
        private const string PLATFORM_END = "</Platform>";
        private const string PROPERTYGROUP_START = "<PropertyGroup Condition=";
        private const string PROPERTYGROUP_END = "</PropertyGroup>";
        private const string OUTPUTPATH_NODE = "OutputPath";
        private const string FRAMEWORK_NODE = "TargetFrameworkVersion";
        private const string PRODUCTVERSION_NODE = "ProductVersion";

        private IFileSystemService _fsService;
        private string _projectFile;
        private string _fileContent;

        public ProjectParser(IFileSystemService fsService)
        {
            _fsService = fsService;
        }

        public ProjectDocument Parse(string projectFile, ProjectDocument existingDocument)
        {
            _projectFile = projectFile;
            readFile();
            var newDocument = new ProjectDocument(getType());
            setAssembly(newDocument);
            setConfiguration(newDocument);
            setPlatform(newDocument);
            setOutputPath(newDocument);
            setFrameworkVersion(newDocument);
            setProductVersion(newDocument);
			setBinaryReferences(newDocument);
            setReferences(newDocument);
            setReferencedBy(newDocument, existingDocument);
            return newDocument;
        }

        private void setAssembly(ProjectDocument newDocument)
        {
            var assemblyName = getNode(ASSEMBLYNAME_NODE);
            if (assemblyName.Length == 0)
                throw new Exception("Could not read assembly name. Invalid project file.");
            var fileType = getNode(OUTPUT_TYPE).ToLower();
            if (fileType.Contains("exe"))
                fileType = "exe";
            else
                fileType = "dll";
            newDocument.SetAssemblyName(string.Format("{0}.{1}", assemblyName, fileType));
        }

        private void setOutputPath(ProjectDocument newDocument)
        {
			newDocument.SetOutputPath(Path.Combine("bin", "AutoTest.Net") + Path.DirectorySeparatorChar);
//            var configuration = string.Format("{0}|{1}", newDocument.BuildConfiguration, newDocument.Platform);
//            var configurations = getNodes(PROPERTYGROUP_START, PROPERTYGROUP_END);
//            for (int i = 0; i < configurations.Length; i++)
//            {
//                var content = configurations[i];
//                if (content.Contains(configuration))
//                {
//                    newDocument.SetOutputPath(
//                        getNode(OUTPUTPATH_NODE, content)
//                        .Replace("\\", Path.DirectorySeparatorChar.ToString()));
//                    break;
//                }
//            }
        }

        private void setFrameworkVersion(ProjectDocument newDocument)
        {
            newDocument.SetFramework(getNode(FRAMEWORK_NODE));
        }

        private void setProductVersion(ProjectDocument newDocument)
        {
            newDocument.SetVSVersion(getNode(PRODUCTVERSION_NODE));
        }

        private void setPlatform(ProjectDocument newDocument)
        {
			var platform = getNode(PLATFORM_START, PLATFORM_START_CONTENT_STARTS_HERE, PLATFORM_END);
			if (platform == null || platform.Length.Equals(0))
				platform = getNode(PLATFORM_NON_DEFAULT_START, PLATFORM_NON_DEFAULT_END, 0);
            newDocument.SetPlatform(platform);
        }

        private void setConfiguration(ProjectDocument newDocument)
        {
            newDocument.SetConfiguration(
                getNode(CONFIGURATION_START,
                        CONFIGURATION_START_CONTENT_STARTS_HERE,
                        CONFIGURATION_END));
        }

        private void readFile()
        {
            _fileContent = _fsService.ReadFileAsText(_projectFile);
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

        private void setReferences(ProjectDocument document)
        {
            var regExp = new Regex(string.Format("{0}.*?{1}", PROJECT_REFERENCE_START, PROJECT_REFERENCE_END));
            var matches = regExp.Matches(_fileContent);
            for (int i = 0; i < matches.Count; i++)
            {
                var reference = getReference(matches[i].Value);
                if (!_fsService.FileExists(reference))
                    continue;
                document.AddReference(reference);
            }
        }
		
		private void setBinaryReferences(ProjectDocument document)
        {
            var regExp = new Regex(string.Format("{0}.*?{1}", BINARY_REFERENCE_START, BINARY_REFERENCE_END));
            var matches = regExp.Matches(_fileContent);
            for (int i = 0; i < matches.Count; i++)
			{
				var patternLength = BINARY_REFERENCE_START.Length + BINARY_REFERENCE_END.Length;
				var reference = matches[i].Value;
                document.AddBinaryReference(reference.Substring(BINARY_REFERENCE_START.Length, reference.Length - patternLength));
			}
        }

        private string[] getNodes(string start, string end)
        {
            return getNodes(start, end, _fileContent);
        }

        private string[] getNodes(string startTag, string endTag, string content)
        {
            int start = 0, end;
            List<string> nodes = new List<string>();
            while ((start = content.IndexOf(startTag, start)) >= 0)
            {
                start += startTag.Length;
                end = content.IndexOf(endTag, start);
                nodes.Add(content.Substring(start, end - start));
            }
            return nodes.ToArray();
        }

        private string getNode(string nodeName)
        {
            return getNode(nodeName, _fileContent);
        }

        private string getNode(string nodeName, string content)
        {
            var regExp = new Regex(string.Format("<{0}>.*?</{0}>", nodeName));
            var match = regExp.Match(content);
            return getInnerText(match.Value);
        }

        private string getInnerText(string node)
        {
            int start = node.IndexOf(">") + 1;
            int end = node.IndexOf("<", start);
            if (start == -1 || end == -1)
                return "";
            return node.Substring(start, end - start);
        }

        private string getNode(string startTag, string contentStartsHereTag, string contentEndsHereTag)
        {
            int start = _fileContent.IndexOf(startTag) + startTag.Length;
            if (start == -1)
                return "";
			return getNode(contentStartsHereTag, contentEndsHereTag, start);
        }
				
		private string getNode(string startTag, string endTag, int startIndex)
        {
			var start = _fileContent.IndexOf(startTag, startIndex) + startTag.Length;
            if (start - startTag.Length == -1)
                return "";
            int end = _fileContent.IndexOf(endTag, start);
            if (end == -1)
                return "";
            return _fileContent.Substring(start, end - start);
        }

        private string getReference(string match)
        {
            var path = match.Substring(
                PROJECT_REFERENCE_START.Length,
                match.Length - (PROJECT_REFERENCE_START.Length + PROJECT_REFERENCE_END.Length))
                .Replace("\\", Path.DirectorySeparatorChar.ToString());
            if (path.Substring(0, 2).Equals(".."))
                return new PathParser(path).ToAbsolute(_projectFile);
            return path;
        }

        private void setReferencedBy(ProjectDocument newDocument, ProjectDocument existingDocument)
        {
            if (existingDocument != null)
                newDocument.AddReferencedBy(existingDocument.ReferencedBy);
            newDocument.HasBeenReadFromFile();
        }
    }
}
