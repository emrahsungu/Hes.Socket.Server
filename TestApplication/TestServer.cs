namespace TestApplication {

    using Hes.Socket;

    public class Peer : PeerBase {

        public Peer(InitRequest initRequest) : base(initRequest) {
        }

        protected override void OnDisconnect(DisconnectReason disconnectReason) {
        }

        protected override void OnDataFromClient(byte[] data) {
        }
    }

    public class Server : ServerBase {

        protected override PeerBase CreatePeer(InitRequest initRequest) {
            return new Peer(initRequest);
        }
    }
}