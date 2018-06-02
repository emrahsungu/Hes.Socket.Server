namespace Hes.Socket {

    using System.Net.Sockets;

    public class InitRequest {

        public InitRequest(int connectionId, Socket socket) {
            ConnectionId = connectionId;
            Socket = socket;
        }

        public int ConnectionId { get; }
        public Socket Socket { get; }
    }
}