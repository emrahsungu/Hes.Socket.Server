namespace TestApplication {

    using System;
    using System.Text;
    using Hes.Socket;

    public class Peer : PeerBase {
        private int messageCounter;

        private DateTime Init;

        public Peer(InitRequest initRequest) : base(initRequest) {
        }

        protected override void OnDisconnect(DisconnectReason disconnectReason) {
        }

        protected override void OnDataFromClient(byte[] data,TimeSpan timeSpan) {
            if(messageCounter == 0) {
                Init=DateTime.UtcNow;
                ;
            }
            var message = Encoding.UTF8.GetBytes($"{messageCounter++}: TimeSpan:{timeSpan.TotalMilliseconds}; Avg Msg:{(double)(messageCounter-1) /((DateTime.UtcNow-Init).Seconds)}");
            SendData(message);
        }
    }
}