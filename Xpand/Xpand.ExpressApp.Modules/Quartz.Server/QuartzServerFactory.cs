using System;
using System.Configuration;
using System.IO;
using Common.Logging;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.ModelDifference.Core;
using Xpand.Persistent.Base.General;

namespace Xpand.Quartz.Server {
    /// <summary>
    /// Factory class to create Quartz server implementations from.
    /// </summary>
    public class QuartzServerFactory {
        private static readonly ILog logger = LogManager.GetLogger(typeof(QuartzServerFactory));

        /// <summary>
        /// Creates a new instance of an Quartz.NET server core.
        /// </summary>
        /// <returns></returns>
        public static IQuartzServer CreateServer() {
            string typeName = Configuration.ServerImplementationType;

            Type t = Type.GetType(typeName, true);

            logger.Debug("Creating new instance of server type '" + typeName + "'");
            var retValue = (IQuartzServer)Activator.CreateInstance(t);
            logger.Debug("Instance successfully created");
            return retValue;
        }
    }

    public class XafApplicationFactory {
        public static XafApplication GetApplication(string modulePath) {
            var fullPath = Path.GetFullPath(modulePath);
            var moduleName = Path.GetFileName(fullPath);
            var directoryName = Path.GetDirectoryName(fullPath);
            var xafApplication = ApplicationBuilder.Create()
                .UsingTypesInfo(s => XafTypesInfo.Instance)
                .FromModule(moduleName)
                .FromAssembliesPath(directoryName)
                .Build();
            xafApplication.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            xafApplication.Setup();
            var objectSpaceProvider = ((IXpandObjectSpaceProvider)xafApplication.ObjectSpaceProvider);
            if (objectSpaceProvider.WorkingDataLayer == null) {
                using (objectSpaceProvider.CreateObjectSpace()) {
                }
            }
            return xafApplication;
        }

    }
}