using System;
namespace AutoTest.Messages
{
	[Serializable]
	public class BuildMessage
	{
		public string File { get; set; }
        public int LineNumber { get; set; }
        public int LinePosition { get; set; }
        public string ErrorMessage { get; set; }

        public override bool Equals(object obj)
        {
            var other = (BuildMessage)obj;
            return this.GetHashCode().Equals(other.GetHashCode());
        }

        public override int GetHashCode()
        {
            return string.Format("{0}|{1}|{2}|{3}",
                                 File,
                                 LineNumber,
                                 LinePosition,
                                 ErrorMessage).GetHashCode();
        }
	}
}

