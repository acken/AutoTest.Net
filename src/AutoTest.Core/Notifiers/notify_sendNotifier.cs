using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
namespace AutoTest.Core.Notifiers
{
	class notify_sendNotifier : ISendNotifications
	{
		#region ISendNotifications implementation
		public void Notify (string msg, NotificationType type)
		{
			runNotification(msg, type);
		}
		
		public bool IsSupported()
		{
			try
			{
				var process = new Process();
				process.StartInfo = new ProcessStartInfo("locate", "notify-send");
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.Start();
				var output = process.StandardOutput.ReadToEnd();
				var lines = output.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				if (lines.Length == 0)
					return false;
				return File.Exists(lines[0]);
			}
			catch
			{
				return false;
			}
		}
		#endregion
		
		private void runNotification(string msg, NotificationType type) {
			var bleh = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath);
			string icon = bleh;
			switch (type) {
				case NotificationType.Green:
					icon += "/Icons/circleWIN.png";
					break;
				case NotificationType.Yellow:
					icon += "/Icons/circleWAIL.png";
					break;
				case NotificationType.Red:
					icon += "/Icons/circleFAIL.png";
					break;
			}
			string args = "\"AutoTest.NET\" \"" + msg + "\" --icon=\"" + icon + "\"";
			var process = new Process();
            process.StartInfo = new ProcessStartInfo("notify-send", args);
			process.StartInfo.CreateNoWindow = true;
            process.Start(); 
		}
	}
}

