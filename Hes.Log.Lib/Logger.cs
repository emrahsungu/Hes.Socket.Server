namespace Hes.Log.Lib {

    using System;
    using System.Runtime.CompilerServices;
    using log4net;

    public class Logger : ILogger {
        private readonly ILog _logger;

        /// <summary>
        ///
        /// </summary>
        /// <param name="logger"></param>
        public Logger(ILog logger) {
            _logger = logger;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        public void Debug(object message) {
            _logger.Debug(message);
        }

        public void Debug(object message, Exception ex) {
            _logger.Debug(message, ex);
        }

        public void Error(object message, [CallerFilePath] string callerFilePath = null, [CallerMemberName] string callerMemberName = null) {
            _logger.Error(message);
        }

        public void Error(object message, Exception ex, [CallerFilePath] string callerFilePath = null, [CallerMemberName] string callerMemberName = null) {
            _logger.Error(message, ex);
        }

        public void Fatal(object message) {
            _logger.Fatal(message);
        }

        public void Fatal(object message, Exception ex) {
            _logger.Fatal(message, ex);
        }

        public void Info() {
            throw new NotImplementedException();
        }

        public void Warn() {
            throw new NotImplementedException();
        }
    }
}