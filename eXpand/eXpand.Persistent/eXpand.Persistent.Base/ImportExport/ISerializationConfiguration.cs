using System;
using System.Collections.Generic;

namespace eXpand.Persistent.Base.ImportExport {
    public interface ISerializationConfiguration
    {
        Type TypeToSerialize { get; set; }
        IList<IClassInfoGraphNode> SerializationGraph { get; }
    }
}