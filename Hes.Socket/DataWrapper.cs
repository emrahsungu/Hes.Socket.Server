namespace Hes.Socket {

    using System;

    public class DataWrapper {

        /// <summary></summary>
        /// <param name="count"></param>
        public DataWrapper(int count) {
            Buffer = new byte[count];
        }

        public DateTime Start { get; set; }

        /// <summary></summary>
        public bool ReadFinished => ReadSofar == TotalAmountToRead;

        /// <summary></summary>
        public int TotalAmountToRead => Buffer.Length;

        /// <summary></summary>
        public int ReadSofar { get; set; }

        /// <summary></summary>
        public byte[] Buffer { get; }
    }
}