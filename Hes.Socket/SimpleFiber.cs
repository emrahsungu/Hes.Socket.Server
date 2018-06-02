namespace Hes.Socket {

    using System;
    using System.Threading;

    public class SimpleFiber : IFiber {

        public void Enqueue(Action action) {
            ThreadPool.QueueUserWorkItem(x => action(), null);
        }

    }

}