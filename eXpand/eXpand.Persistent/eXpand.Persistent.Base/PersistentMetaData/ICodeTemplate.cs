namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface ICodeTemplate:ITemplateInfo {
        CodeDomProvider CodeDomProvider { get; set; }
        TemplateType TemplateType { get; set; }
        bool IsDefault { get; set; }
        void SetDefaults();
    }

    public enum TemplateType {
        Class=1,
        ReadWriteMember=2,
        ReadOnlyMember=3,
        InterfaceReadWriteMember=4
    }
}