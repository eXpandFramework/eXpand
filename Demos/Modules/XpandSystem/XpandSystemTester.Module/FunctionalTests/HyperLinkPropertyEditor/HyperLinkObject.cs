using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;

namespace XpandSystemTester.Module.FunctionalTests.HyperLinkPropertyEditor {
    [DefaultClassOptions]
    [DefaultProperty("Url")]
    public class HyperLinkObject:BaseObject {
        public HyperLinkObject(Session session) : base(session){
        }
        [EditorAlias(EditorAliases.HyperLinkPropertyEditor)]
        public string Url { get; set; }
        

        
    }
}
