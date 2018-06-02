namespace Hes.Socket {

    using System;

    public interface IFiber {

        void Enqueue(Action action);

    }

}