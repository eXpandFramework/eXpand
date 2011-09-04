using System.Collections.Generic;

namespace Xpand.Persistent.Base.ImportExport {
    public interface ISerializationConfigurationGroup {
        string Name { get; set; }
        IList<ISerializationConfiguration> Configurations { get; }
    }
}