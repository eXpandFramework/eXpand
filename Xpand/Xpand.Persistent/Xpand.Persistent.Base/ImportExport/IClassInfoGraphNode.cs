namespace Xpand.Persistent.Base.ImportExport {
    public interface IClassInfoGraphNode 
    {
        NodeType NodeType { get; set; }
        string Name { get; set; }
        ISerializationConfiguration SerializationConfiguration { get; set; }
        SerializationStrategy SerializationStrategy { get; set; }
        bool Key { get; set; }
        string TypeName { get; set; }
    }
}