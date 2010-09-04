namespace Xpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentTemplatedTypeInfo : IPersistentTypeInfo
    {
        ICodeTemplateInfo CodeTemplateInfo { get; set; }

    }
}