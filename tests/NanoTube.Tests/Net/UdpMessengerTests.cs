using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NanoTube.Net;
using Xunit;

namespace NanoTube.Tests.Net
{
	public class UdpMessengerTests
	{
		private const string LocalIPAddress = "127.0.0.1";
		private const int Port = 8125;

		[Fact]
		public void CanSend()
		{
			var udpListenerTask = StartListeningForBytes();

			var messenger = new UdpMessenger(LocalIPAddress, Port);
			messenger.SendMetrics(new [] {"test"});

			if (!udpListenerTask.Wait(500))
			{
				Assert.True(false, "Timed out while waiting for response from task");
			}

			Assert.True(udpListenerTask.Result.Length > 0);
		}

		private Task<byte[]> StartListeningForBytes()
		{
			var receiveTask = new Task<byte[]>(
				() => {
					using (var listener = new UdpClient(Port))
					{
						var groupEndpoint = new IPEndPoint(IPAddress.Any, Port);
						return listener.Receive(ref groupEndpoint);
					}
				});
			receiveTask.Start();

			return receiveTask;
		}
	}
}
