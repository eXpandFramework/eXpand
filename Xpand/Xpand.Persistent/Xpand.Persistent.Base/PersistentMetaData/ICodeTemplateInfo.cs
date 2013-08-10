using DevExpress.Xpo;

namespace Xpand.Persistent.Base.PersistentMetaData {
    public interface ICodeTemplateInfo : IXPSimpleObject {
        ICodeTemplate CodeTemplate { get; set; }
        ITemplateInfo TemplateInfo { get; set; }
    }
}