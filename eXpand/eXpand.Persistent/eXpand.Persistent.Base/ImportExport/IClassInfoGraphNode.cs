using DevExpress.Persistent.Base.General;
using System;

namespace eXpand.Persistent.Base.ImportExport {
    public enum NodeType {
        Simple,
        Object,
        Collection
    }
    public interface IClassInfoGraphNode :ITreeNode
    {
        NodeType NodeType { get; set; }
        new string Name { get; set; }
        ISerializationConfiguration SerializationConfiguration { get; set; }
        SerializationStrategy SerializationStrategy { get; set; }
        bool Key { get; set; }
    }
}