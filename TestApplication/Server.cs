namespace TestApplication {

    using Hes.Socket;

    public class Server : ServerBase {

        protected override PeerBase CreatePeer(InitRequest initRequest) {
            return new Peer(initRequest);
        }
    }

}