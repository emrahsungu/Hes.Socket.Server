namespace Hes.Socket.Listeners {

    public interface ISocketWrapper {
        bool IsConnected { get; }

        void SendData(byte[] data);

        void Start();
    }
}