using System;
using System.Collections.Generic;
using DevExpress.Xpo;

namespace eXpand.Persistent.Base.ImportExport {
    public interface ISerializationConfiguration
    {
        string Name { get; set; }
        Session Session { get; }
        Type TypeToSerialize { get; set; }
        IList<IClassInfoGraphNode> SerializationGraph { get; }
    }
}