using System.Collections.Generic;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.Metadata;

namespace Xpand.Xpo {
    public interface ISupportChangedMembers : ISessionProvider {
        HashSet<string> ChangedProperties { get; set; }
    }
}
