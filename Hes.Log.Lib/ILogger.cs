namespace Hes.Log.Lib {

    using System;
    using System.Runtime.CompilerServices;

    public interface ILogger {

        /// <summary>
        /// Debug log.
        /// </summary>
        /// <param name="message"></param>
        void Debug(object message);

        /// <summary>
        /// Debug log with exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        void Debug(object message, Exception ex);

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        void Fatal(object message);

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        void Fatal(object message, Exception ex);

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        void Error(object message,[CallerFilePath] string callerFilePath = null, [CallerMemberName] string callerMemberName = null);

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        void Error(object message, Exception ex, [CallerFilePath]string callerFilePath = null, [CallerMemberName]string callerMemberName = null);

        void Warn();

        void Info();
    }
}