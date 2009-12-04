namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface ICodeTemplate:ITemplateInfo {
        TemplateType TemplateType { get; set; }
        bool IsDefault { get; set; }
        void SetDefaults();
    }

    public enum TemplateType {
        Class=1,
        ReadWriteMember=2,
        ReadOnlyMember=3
    }
}