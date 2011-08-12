using System.IO;
using AutoTest.Messages;

namespace AutoTest.Messages
{
    public class CacheBuildMessage : ICustomBinarySerializable
    {
        public string Project { get; private set; }
        public BuildMessage BuildItem { get; private set; }

        public CacheBuildMessage(string project, BuildMessage result)
        {
            Project = project;
            BuildItem = result;
        }

        public override bool Equals(object obj)
        {
            return GetHashCode().Equals(obj.GetHashCode());
        }

        public override int GetHashCode()
        {
            return string.Format("{0}|{1}", Project, BuildItem.GetHashCode()).GetHashCode();
        }

        public void SetDataFrom(BinaryReader reader)
        {
            Project = reader.ReadString();
            BuildItem = new BuildMessage();
            BuildItem.SetDataFrom(reader);
        }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(Project);
            BuildItem.WriteDataTo(writer);
        }
    }
}
