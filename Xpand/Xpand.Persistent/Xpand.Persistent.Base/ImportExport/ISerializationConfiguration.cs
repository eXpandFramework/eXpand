using System;
using System.Collections.Generic;
using DevExpress.Xpo;

namespace Xpand.Persistent.Base.ImportExport {
    public interface ISerializationConfiguration 
    {
        Type TypeToSerialize { get; set; }
        IList<IClassInfoGraphNode> SerializationGraph { get; }
        ISerializationConfigurationGroup SerializationConfigurationGroup { get; set; }
    }
}