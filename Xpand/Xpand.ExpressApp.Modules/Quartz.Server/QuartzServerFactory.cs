using System;
using Common.Logging;
using Fasterflect;

namespace Xpand.Quartz.Server {
    /// <summary>
    /// Factory class to create Quartz server implementations from.
    /// </summary>
    public class QuartzServerFactory {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(QuartzServerFactory));

        /// <summary>
        /// Creates a new instance of an Quartz.NET server core.
        /// </summary>
        /// <returns></returns>
        public static IQuartzServer CreateServer() {
            string typeName = Configuration.ServerImplementationType;

            Type t = Type.GetType(typeName, true);

            _logger.Debug("Creating new instance of server type '" + typeName + "'");
            var retValue = (IQuartzServer)t.CreateInstance();
            _logger.Debug("Instance successfully created");
            return retValue;
        }
    }

}