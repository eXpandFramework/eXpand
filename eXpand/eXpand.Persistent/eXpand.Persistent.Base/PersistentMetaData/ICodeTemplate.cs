using System.Collections.Generic;

namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface ICodeTemplate {
        IList<IPersistentTypeInfo> TypeInfos { get; }
        TemplateType TemplateType { get; set; }
        bool IsDefault { get; set; }
        string TemplateCode { get; set; }
        string References { get; set; }
    }

    public enum TemplateType {
        Class,
        Member
    }
}