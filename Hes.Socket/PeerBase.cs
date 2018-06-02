namespace Hes.Socket {

    using System;
    using System.Net.Sockets;
    using Listeners;

    public abstract class PeerBase {

        /// <summary>
        ///
        /// </summary>
        private readonly TcpSocket _listener;

        protected PeerBase(InitRequest initRequest) {
            ConnectionId = initRequest.ConnectionId;
            switch (initRequest.Socket.SocketType) {
                case SocketType.Stream:
                    _listener = new TcpSocket(this, initRequest.Socket);
                    _listener.Start();
                    break;
            }
        }

        public void SendData(byte[] data) {
            _listener.SendData(data);
        }

        public IFiber RequestFiber { get; } = new SimpleFiber();

        /// <summary>
        /// Connection Id.
        /// </summary>
        public int ConnectionId { get; }

        /// <summary>
        /// Is peer connected.
        /// </summary>
        public bool IsConnected => _listener.IsConnected;

        /// <summary>
        ///
        /// </summary>
        /// <param name="disconnectReason"></param>
        protected internal abstract void OnDisconnect(DisconnectReason disconnectReason);

        protected internal abstract void OnDataFromClient(byte[] data,TimeSpan timeSpan);
    }
}