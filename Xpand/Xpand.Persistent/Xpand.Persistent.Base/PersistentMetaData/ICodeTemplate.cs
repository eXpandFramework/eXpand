namespace Xpand.Persistent.Base.PersistentMetaData {
    public interface ICodeTemplate:ITemplateInfo {
        CodeDomProvider CodeDomProvider { get; set; }
        TemplateType TemplateType { get; set; }
        bool IsDefault { get; set; }
        void SetDefaults();
    }

    public enum TemplateType {
        None,
        Class,
        XPReadWritePropertyMember,
        XPOneToOnePropertyMember,
        XPCollectionMember,
        InterfaceReadWriteMember,
        Struct,
        ReadWriteMember,
        FieldMember
    }
}