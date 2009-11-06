using DevExpress.Persistent.Base.General;

namespace eXpand.Persistent.Base.Taxonomies{
    public interface IBaseTerm : ITreeNode{
        new string Name { get; set; }
        string Key { get; set; }
        string FullPath { get; }
        string RelativePath { get; }
        string Indexer { get; set; }
        int Level { get; }
        IBaseTaxonomy BaseTaxonomy { get; set; }
        IBaseTerm ParentTerm { get; set; }
        void EvaluateTermPropertyValues(bool recursive);
    }
}