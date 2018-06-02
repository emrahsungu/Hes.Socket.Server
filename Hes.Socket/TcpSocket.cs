namespace Hes.Socket {

    using System;
    using System.Linq;
    using System.Net.Sockets;
    using Log.Lib;

    public enum DisconnectReason {
        DisconnectedByClient,
        InternalServerError
    }

    public class TcpSocket {

        /// <summary>
        /// Payload size.
        /// </summary>
        private const int PayloadDelimiter = 4;

        /// <summary>
        /// Logger for current class.
        /// </summary>
        private static readonly ILogger Logger = LoggerUtil.GetLogger(typeof(TcpSocket));

        /// <summary></summary>
        private readonly PeerBase _peerBase;

        /// <summary>
        /// Underlying socket.
        /// </summary>
        private readonly Socket _socket;

        /// <summary>
        /// Wrapper around a Tcp Socket.
        /// </summary>
        /// <param name="peerBase"></param>
        /// <param name="socket"></param>
        public TcpSocket(PeerBase peerBase, Socket socket) {
            _peerBase = peerBase;
            _socket = socket;
        }

        public bool IsConnected => _socket.Connected;

        /// <summary>
        /// Start listening for data from client.
        /// </summary>
        public void Start() {
            ReadPayload();
        }

        /// <summary>
        /// Tries to read payload
        /// </summary>
        private void ReadPayload() {
            try {
                BeginReceive(new DataWrapper(PayloadDelimiter), PayloadCallback);
            }

            catch(SocketException socketException) {
                Logger.Error($"MethodName: {nameof(ReadPayload)}", socketException);
                HandleError(DisconnectReason.InternalServerError);
            }
            catch(ObjectDisposedException objectDisposedException) {
                Logger.Error($"MethodName: {nameof(ReadPayload)}", objectDisposedException);
                HandleError(DisconnectReason.InternalServerError);
            }
            catch(Exception exception) {
                Logger.Error($"MethodName: {nameof(ReadPayload)}", exception);
                HandleError(DisconnectReason.InternalServerError);
            }
        }

        /// <summary>
        /// Callback for reading payload.
        /// </summary>
        /// <param name="asyncResult"></param>
        private void PayloadCallback(IAsyncResult asyncResult) {
            try {
                var dataWrapper = (DataWrapper) asyncResult.AsyncState;
                var readFromSocket = _socket.EndReceive(asyncResult);

                if(readFromSocket <= 0) {
                    HandleError(DisconnectReason.DisconnectedByClient);
                    return;
                }

                dataWrapper.ReadSofar += readFromSocket;
                if(dataWrapper.ReadFinished == false) {
                    _socket.BeginReceive(dataWrapper.Buffer, dataWrapper.ReadSofar, dataWrapper.TotalAmountToRead - dataWrapper.ReadSofar, SocketFlags.None, PayloadCallback, dataWrapper);
                    return;
                }

                //Calculate message size
                var messageSize = BitConverter.ToInt32(dataWrapper.Buffer, 0);
                var dataWrapperForMessage = new DataWrapper(messageSize);

                //Start to get real message
                _socket.BeginReceive(dataWrapperForMessage.Buffer, 0, dataWrapperForMessage.Buffer.Length, SocketFlags.None, MessageCallback, dataWrapperForMessage);
            }

            catch(SocketException socketException) {
                Logger.Error($"MethodName: {nameof(PayloadCallback)}", socketException);
                HandleError(DisconnectReason.InternalServerError);
            }
            catch(ObjectDisposedException objectDisposedException) {
                Logger.Error($"MethodName: {nameof(PayloadCallback)}", objectDisposedException);
                HandleError(DisconnectReason.InternalServerError);
            }
            catch(Exception exception) {
                Logger.Error($"MethodName: {nameof(PayloadCallback)}", exception);
                HandleError(DisconnectReason.InternalServerError);
            }
        }

        /// <summary></summary>
        /// <param name="asyncResult"></param>
        private void MessageCallback(IAsyncResult asyncResult) {
            var dataWrapper = (DataWrapper) asyncResult.AsyncState;
            var readFromSocket = _socket.EndReceive(asyncResult);
            if(readFromSocket > 0) {
                dataWrapper.ReadSofar += readFromSocket;
                if(dataWrapper.ReadFinished) {
                    var buffer = dataWrapper.Buffer;
                    var operationCode = (Operation) BitConverter.ToInt16(buffer, 0);
                    switch(operationCode) {
                        case Operation.Data:
                            _peerBase.RequestFiber.Enqueue(() => _peerBase.OnDataFromClient(dataWrapper.Buffer.Skip(2).ToArray()));
                            break;

                        case Operation.Ping:
                            break;
                    }

                    //Start all over again
                    ReadPayload();
                    return;
                }

                _socket.BeginReceive(dataWrapper.Buffer, dataWrapper.ReadSofar, dataWrapper.TotalAmountToRead - dataWrapper.ReadSofar, SocketFlags.None, MessageCallback, dataWrapper);
                return;
            }

            HandleError(DisconnectReason.DisconnectedByClient);
        }

        private void BeginReceive(DataWrapper dataWrapper, AsyncCallback callBack) {
            _socket.BeginReceive(dataWrapper.Buffer, dataWrapper.ReadSofar, dataWrapper.TotalAmountToRead - dataWrapper.ReadSofar, SocketFlags.None, callBack, dataWrapper);
        }

        /// <summary>
        /// I think this method is supposed to be called only once.
        /// </summary>
        /// <param name="disconnectReason"></param>
        private void HandleError(DisconnectReason disconnectReason) {
            switch(disconnectReason) {
                case DisconnectReason.DisconnectedByClient:
                    _socket.Dispose();
                    _peerBase.RequestFiber.Enqueue(() => _peerBase.OnDisconnect(DisconnectReason.DisconnectedByClient));
                    break;

                case DisconnectReason.InternalServerError:
                    _socket.Dispose();
                    _peerBase.RequestFiber.Enqueue(() => _peerBase.OnDisconnect(DisconnectReason.InternalServerError));
                    break;
            }
        }
    }
}