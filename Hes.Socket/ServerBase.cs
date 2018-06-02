namespace Hes.Socket {

    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using Log.Lib;

    public abstract class ServerBase {

        /// <summary>
        /// Logger for current class.
        /// </summary>
        private static readonly ILogger Logger = LoggerUtil.GetLogger(typeof(ServerBase));

        /// <summary>
        /// ConnectionId counter.
        /// </summary>
        private int _connectionIdCounter;

        public void Test() {
            var manualresetEvent = new ManualResetEvent(false);

            new Thread(async () => {
                var l = new TcpListener(new IPEndPoint(IPAddress.Loopback, 5000));
                l.Start();
                manualresetEvent.Set();
                while(true) {
                    var socket = await l.AcceptSocketAsync();
                    var connectionId = Interlocked.Increment(ref _connectionIdCounter);
                    var initrequest = new InitRequest(connectionId, socket);
                    var peer = CreatePeer(initrequest);
                    Console.WriteLine($"PeerId:{connectionId}");
                }
            }).Start();

            var udpClient = new UdpClient(new IPEndPoint(IPAddress.Loopback, 5001));
            new Thread(async () => {
                while(true) {
                    var socket = await udpClient.ReceiveAsync();
                    if(socket.Buffer.Length != 60000) Logger.Error("");

                    var datagram = (Datagram) socket.Buffer;
                    Console.WriteLine(datagram.AckNo);
                    Logger.Debug($"EndPoint:{socket.RemoteEndPoint}");
                }
            }).Start();

            new Thread(async () => {
                while(true) {
                    var socket = await udpClient.ReceiveAsync();
                    if(socket.Buffer.Length != 60000) Logger.Error("");

                    var datagram = (Datagram) socket.Buffer;
                    Console.WriteLine(datagram.AckNo);

                    Logger.Debug($"EndPoint:{socket.RemoteEndPoint}");
                }
            }).Start();

            manualresetEvent.WaitOne();
        }

        protected abstract PeerBase CreatePeer(InitRequest initRequest);
    }
}