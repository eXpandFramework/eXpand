using System;

namespace Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos {
    public static class PersistentAttributeInfoExtensions {
        public static Version Version(this IPersistentAssemblyInfo info) {
            var xpandVersion = new Version(XpandAssemblyInfo.Version);
            return new Version(xpandVersion.Major, xpandVersion.Minor, Int32.Parse(xpandVersion.Build.ToString() + xpandVersion.Revision), info.Revision);
        }
    }
}
