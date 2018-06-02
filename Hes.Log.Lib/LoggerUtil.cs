namespace Hes.Log.Lib {

    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using log4net;
    using log4net.Config;

    public static class LoggerUtil {
        private static readonly ConcurrentDictionary<Type, ILogger> Loggers = new ConcurrentDictionary<Type, ILogger>();

        /// <summary>
        /// Static constructor for initializing the class.
        /// </summary>
        static LoggerUtil() {
            XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.config"));
        }

        /// <summary>
        /// Get logger for given type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ILogger GetLogger(Type type) {
            return Loggers.GetOrAdd(type, _ => new Logger(LogManager.GetLogger(_)));
        }
    }
}