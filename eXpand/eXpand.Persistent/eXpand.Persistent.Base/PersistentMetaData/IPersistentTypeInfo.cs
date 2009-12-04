using System.Collections.Generic;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentTypeInfo {
        string Name { get; set; }
        IList<IPersistentAttributeInfo> TypeAttributes { get; }
        Session Session { get; }
        string GeneratedCode { get; set; }
        ICodeTemplate CodeTemplate { get; set; }
        ITemplateInfo TemplateInfo { get; set; }
    }
}