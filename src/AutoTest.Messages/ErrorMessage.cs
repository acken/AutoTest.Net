using System;
using System.Text;
namespace AutoTest.Messages
{
	public class ErrorMessage : IMessage
    {
        private string _error;

        public string Error { get { return _error; } }

        public ErrorMessage(string error)
        {
            _error = error;
        }

        public ErrorMessage(Exception exception)
        {
            var builder = new StringBuilder();
            addException(builder, exception);
            _error = builder.ToString();
        }

        private void addException(StringBuilder builder, Exception exception)
        {
            if (exception.Source != null)
                builder.AppendLine(exception.Source);
            if (exception.Message.Length > 0)
                builder.AppendLine(exception.Message);
            if (exception.StackTrace != null)
                builder.AppendLine(exception.StackTrace);
            if (exception.InnerException == null)
                return;
            builder.AppendLine("");
            addException(builder, exception.InnerException);
        }
    }
}

