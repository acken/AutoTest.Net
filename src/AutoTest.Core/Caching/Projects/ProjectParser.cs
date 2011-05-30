using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using AutoTest.Core.FileSystem;
using AutoTest.Core.DebugLog;
using System.Xml;

namespace AutoTest.Core.Caching.Projects
{
    class ProjectParser : IProjectParser
    {
        private const string CSHARP_PROJECT_EXTENTION = ".csproj";
        private const string VB_PROJECT_EXTENTION = ".vbproj";

        private IFileSystemService _fsService;
        private string _projectFile;
        private string _fileContent;
        private XmlDocument _xml = null;
        private XmlNamespaceManager _nsManager = null;

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
            setOutputPath(newDocument);
            setFrameworkVersion(newDocument);
            setProductVersion(newDocument);
			setBinaryReferences(newDocument);
            setReferences(newDocument);
            setReferencedBy(newDocument, existingDocument);
            setFiles(newDocument);
            return newDocument;
        }

        private void readFile()
        {
            _fileContent = _fsService.ReadFileAsText(_projectFile);
            _xml = new XmlDocument();
            tryOpen(_xml, _fileContent);
        }

        private void tryOpen(XmlDocument document, string xml)
        {
            try
            {
                document.LoadXml(xml);
                if (xml.Contains("http://schemas.microsoft.com/developer/msbuild/2003"))
                {
                    _nsManager = new XmlNamespaceManager(document.NameTable);
                    _nsManager.AddNamespace("b", "http://schemas.microsoft.com/developer/msbuild/2003");
                }
            }
            catch
            {
            }
        }

        private void setAssembly(ProjectDocument newDocument)
        {
            var assemblyName = _xml.SelectSingleNode("b:Project/b:PropertyGroup/b:AssemblyName", _nsManager);
            if (assemblyName == null)
                throw new Exception("Could not read assembly name. Invalid project file.");
            var outputType = _xml.SelectSingleNode("b:Project/b:PropertyGroup/b:OutputType", _nsManager); ;
            if (outputType == null)
                throw new Exception("Could not read output type. Invalid project file.");
            string fileType = "dll";
            if (outputType.InnerText.ToLower().Contains("exe"))
                fileType = "exe";
            newDocument.SetAssemblyName(string.Format("{0}.{1}", assemblyName.InnerText, fileType));
        }

        private void setOutputPath(ProjectDocument newDocument)
        {
			newDocument.SetOutputPath(Path.Combine("bin", "AutoTest.Net") + Path.DirectorySeparatorChar);
        }

        private void setFrameworkVersion(ProjectDocument newDocument)
        {
            newDocument.SetFramework(getNode("b:Project/b:PropertyGroup/b:TargetFrameworkVersion"));
        }

        private void setProductVersion(ProjectDocument newDocument)
        {
            newDocument.SetVSVersion(getNode("b:Project/b:PropertyGroup/b:ProductVersion"));
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
            var references = getNodes("b:Project/b:ItemGroup/b:ProjectReference", "Include")
                .Select(x => new PathParser(x.Replace('\\', Path.DirectorySeparatorChar)).ToAbsolute(Path.GetDirectoryName(_projectFile)))
                .Where(x => _fsService.FileExists(x)).ToArray();
            document.AddReference(references);
        }
		
		private void setBinaryReferences(ProjectDocument document)
        {
            getNodes("b:Project/b:ItemGroup/b:Reference", "Include")
                .Where(x => _fsService.FileExists(x)).ToList()
                .ForEach(x => document.AddBinaryReference(x));
        }

        private void setReferencedBy(ProjectDocument newDocument, ProjectDocument existingDocument)
        {
            if (existingDocument != null)
                newDocument.AddReferencedBy(existingDocument.ReferencedBy);
            newDocument.HasBeenReadFromFile();
        }

        private void setFiles(ProjectDocument newDocument)
        {
            var projectPath = Path.GetDirectoryName(_projectFile);
            getNodes("b:Project/b:ItemGroup/b:Compile", "Include")
                .ForEach(x => newDocument.AddFile(new ProjectFile(new PathParser(x.Replace('\\', Path.DirectorySeparatorChar)).ToAbsolute(projectPath), FileType.Compile, _projectFile)));
            getNodes("b:Project/b:ItemGroup/b:EmbeddedResource", "Include")
                .ForEach(x => newDocument.AddFile(new ProjectFile(new PathParser(x.Replace('\\', Path.DirectorySeparatorChar)).ToAbsolute(projectPath), FileType.Resource, _projectFile)));
            getNodes("b:Project/b:ItemGroup/b:None", "Include")
                .ForEach(x => newDocument.AddFile(new ProjectFile(new PathParser(x.Replace('\\', Path.DirectorySeparatorChar)).ToAbsolute(projectPath), FileType.None, _projectFile)));
        }

        private List<string> getNodes(string nodeName)
        {
            var list = new List<string>();
            var nodes = _xml.SelectNodes(nodeName, _nsManager);
            foreach (XmlNode node in nodes)
                list.Add(node.InnerText);
            return list;
        }

        private List<string> getNodes(string nodeName, string attribute)
        {
            var list = new List<string>();
            var nodes = _xml.SelectNodes(nodeName, _nsManager);
            foreach (XmlNode node in nodes)
            {
                var attr = node.Attributes[attribute];
                if (attr != null)
                    list.Add(attr.InnerText);
            }
            return list;
        }

        private string getNode(string nodeName)
        {
            var node = _xml.SelectSingleNode(nodeName, _nsManager);
            if (node == null)
                return "";
            return node.InnerText;
        }
    }
}
