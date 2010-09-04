namespace Xpand.Persistent.Base.PersistentMetaData {
    public interface ICodeTemplateInfo
    {
        ICodeTemplate CodeTemplate { get; set; }
        ITemplateInfo TemplateInfo { get; set; }
    }
}