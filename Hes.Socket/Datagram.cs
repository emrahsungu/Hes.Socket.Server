namespace Hes.Socket {

    using System;
    using System.Linq;

    public struct Datagram {

        /// <summary>
        /// 1
        /// </summary>
        public int Session { get; set; }

        /// <summary>2</summary>
        public DatagramType Type { get; set; }

        /// <summary>
        /// 3 - The number that the client recieved so far
        /// </summary>
        public int AckNo { get; set; }

        public byte[] Data { get; set; }

        public static implicit operator byte[] (Datagram datagram)
        {
            var session = BitConverter.GetBytes(datagram.Session);
            var type = BitConverter.GetBytes((int)datagram.Type);
            var ackNo = BitConverter.GetBytes(datagram.AckNo);
            return session.Concat(type).Concat(ackNo).Concat(datagram.Data).ToArray();
        }

        public static implicit operator Datagram(byte[] data)
        {
            var session = BitConverter.ToInt32(data, 0);
            var type = (DatagramType) BitConverter.ToInt32(data, sizeof(int));
            var ackNo = BitConverter.ToInt32(data, 2 * sizeof(int));
            var actualData = data.Skip(3 * sizeof(int)).ToArray();
            return new Datagram {Session = session, Type = type, AckNo = ackNo, Data = actualData};
        }
    }
}