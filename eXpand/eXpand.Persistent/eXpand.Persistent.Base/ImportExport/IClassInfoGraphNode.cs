using DevExpress.Persistent.Base.General;

namespace eXpand.Persistent.Base.ImportExport {
    public interface IClassInfoGraphNode 
    {
        NodeType NodeType { get; set; }
        new string Name { get; set; }
        ISerializationConfiguration SerializationConfiguration { get; set; }
        SerializationStrategy SerializationStrategy { get; set; }
        bool Key { get; set; }
        bool NaturalKey { get; set; }
        string TypeName { get; set; }
    }
}