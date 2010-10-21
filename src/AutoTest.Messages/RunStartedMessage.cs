using System;
using System.IO;
using System.Collections.Generic;
namespace AutoTest.Messages
{
	public class RunStartedMessage : IMessage, ICustomBinarySerializable
    {
        private ChangedFile[] _files;

        public ChangedFile[] Files { get { return _files; } }

        public RunStartedMessage(ChangedFile[] files)
        {
            _files = files;
        }

		#region ICustomBinarySerializable implementation
		public void WriteDataTo (BinaryWriter writer)
		{
			writer.Write((int) _files.Length);
			foreach (var file in _files)
				file.WriteDataTo(writer);
		}

		public void SetDataFrom (BinaryReader reader)
		{
			var files = new List<ChangedFile>();
			var count = reader.ReadInt32();
			for (int i = 0; i < count; i++)
			{
				var file = new ChangedFile();
				file.SetDataFrom(reader);
				files.Add(file);
			}
			_files = files.ToArray();
		}
		#endregion
}
}

