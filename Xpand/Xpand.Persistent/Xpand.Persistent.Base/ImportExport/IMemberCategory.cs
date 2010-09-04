using DevExpress.Persistent.Base.General;

namespace eXpand.Persistent.Base.ImportExport {
    public interface IMemberCategory : ITreeNode
    {
        new string Name { get; set; }
    }
}