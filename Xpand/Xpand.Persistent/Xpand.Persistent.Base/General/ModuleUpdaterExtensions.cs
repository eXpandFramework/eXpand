using System;
using System.IO;
using DevExpress.ExpressApp.Updating;
using Xpand.Extensions.AppDomainExtensions;

namespace Xpand.Persistent.Base.General {
    public static class ModuleUpdaterExtensions {
        public static string XpandRootPath(this ModuleUpdater moduleUpdater) {
            string applicationBase = Path.GetDirectoryName(AppDomain.CurrentDomain.ApplicationPath()) + "";
            while (!File.Exists(Path.Combine(applicationBase, "Xpand.build"))) {
                applicationBase = Path.GetFullPath(applicationBase + @"..\");
            }
            return applicationBase;
        }
    }
}
