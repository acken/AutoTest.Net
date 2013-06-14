using System;
using AutoTest.Core.Messaging;
using AutoTest.Core.Configuration;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using AutoTest.Core.DebugLog;
using AutoTest.Messages;
namespace AutoTest.Core.Launchers
{
	public class EditorEngineLauncher :
		IConsumerOf<BuildRunMessage>,
		IConsumerOf<TestRunMessage>,
		IConsumerOf<RunStartedMessage>,
		IConsumerOf<RunFinishedMessage>
	{
		private IMessageBus _bus;		
		private string _path = null;
		private SocketClient _client = null;
		
		public EditorEngineLauncher(IMessageBus bus)
		{
			_bus = bus;
		}
		
		public void Connect(string path)
		{
			_path = path;
			if (_client != null &&_client.IsConnected)
				_client.Disconnect();
			_client = null;
			isConnected();
		}
		
		public void GoTo(string file, int line, int column)
		{
			if (!isConnected())
				return;
			send(string.Format("goto {0}|{1}|{2}", file, line, column));
		}

		public void Consume(BuildRunMessage message) {
			if (!isConnected()) return;
			var state = "succeeded";
			if (message.Results.ErrorCount > 0)
				state = "failed";
			send(
				string.Format("autotest.net build \"{0}\" {1}",
					message.Results.Project,
					state));
		}
		
		public void Consume(TestRunMessage message) {
			if (!isConnected()) return;
			var state = "succeeded";
			if (message.Results.Failed.Length > 0)
				state = "failed";
			send(
				string.Format("autotest.net testrun \"{0}\" {1} {2}",
					message.Results.Assembly,
					message.Results.Runner.ToString(),
					state));
		}
		
		public void Consume(RunStartedMessage message) {
			if (!isConnected()) return;
			send("autotest.net runstarted");
		}
		
		public void Consume(RunFinishedMessage message) {
			if (!isConnected()) return;
			send(
				string.Format("autotest.net runfinished {0} {1}",
					message.Report.NumberOfBuildsFailed,
					message.Report.NumberOfTestsFailed));
		}
		
		private bool isConnected()
		{
			try
			{
				if (_client != null && _client.IsConnected)
					return true;
				var instance = new EngineLocator().GetInstance(_path);
				if (instance == null)
					return false;
				_client = new SocketClient();
				_client.IncomingMessage += Handle_clientIncomingMessage;
				_client.Connect(instance.Port);
				if (_client.IsConnected)
					return true;
				_client = null;
				return false;
			}
			catch (Exception ex)
			{
				Debug.WriteError(ex.ToString());
				return false;
			}
		}

		void Handle_clientIncomingMessage(object sender, IncomingMessageArgs e)
		{
			Debug.WriteDebug("Dispatching editor message: " + e.Message);
			_bus.Publish(new ExternalCommandMessage("EditorEngine", e.Message));
		}

		private void send(string message)
		{
			Debug.WriteDebug("Sending to editor engine: " + message);
			_client.Send(message);
		}
	}
	
	class EngineLocator
	{
		public Instance GetInstance(string path)
		{
			var instances = getInstances(path);
			return instances.Where(x => path.StartsWith(x.Key) && canConnectTo(x))
				.OrderByDescending(x => x.Key.Length)
				.FirstOrDefault();
		}
		
		private IEnumerable<Instance> getInstances(string path)
		{
			var dir = Path.Combine(Path.GetTempPath(), "EditorEngine");
			if (Directory.Exists(dir))
			{
				foreach (var file in Directory.GetFiles(dir, "*.pid"))
				{
					var instance = Instance.Get(file, File.ReadAllLines(file));
					if (instance != null)
						yield return instance;
				}
			}
		}
		
		private bool canConnectTo(Instance info)
		{
			var client = new SocketClient();
			client.Connect(info.Port);
			var connected = client.IsConnected;
			client.Disconnect();
			if (!connected)
				File.Delete(info.File);
			return connected;
		}
	}
}

