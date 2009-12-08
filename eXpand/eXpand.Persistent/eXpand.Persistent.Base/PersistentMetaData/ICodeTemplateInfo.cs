namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface ICodeTemplateInfo
    {
        ICodeTemplate CodeTemplate { get; set; }
        ITemplateInfo TemplateInfo { get; set; }
        string GeneratedCode { get; set; }
        IPersistentAssemblyInfo PersistentAssemblyInfo { get; set; }
    }
}