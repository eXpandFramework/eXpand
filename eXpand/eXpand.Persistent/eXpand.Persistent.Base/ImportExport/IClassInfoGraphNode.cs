using DevExpress.Persistent.Base.General;
using System;

namespace eXpand.Persistent.Base.ImportExport {
    public interface IClassInfoGraphNode : ITreeNode {
        ISerializationConfiguration SerializationConfiguration { get; set; }
        new string Name { get; set; }
        SerializationStrategy SerializationStrategy { get; set; }
    }
}