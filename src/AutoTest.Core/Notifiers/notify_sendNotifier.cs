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
			string args = "--icon=\"" + icon + "\" \"" + msg + "\"";
			var process = new Process();
            process.StartInfo = new ProcessStartInfo("notify-send", args);
            process.Start(); 
		}
	}
}

