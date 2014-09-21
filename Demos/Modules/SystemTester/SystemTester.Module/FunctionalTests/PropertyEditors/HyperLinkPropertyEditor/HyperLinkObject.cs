using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;

namespace SystemTester.Module.FunctionalTests.PropertyEditors.HyperLinkPropertyEditor {
    [DefaultClassOptions]
    [DefaultProperty("Url")]
    [NavigationItem("PropertyEditors")]
    public class HyperLinkObject:BaseObject {
        public HyperLinkObject(Session session) : base(session){
        }
        [EditorAlias(EditorAliases.HyperLinkPropertyEditor)]
        public string Url { get; set; }
    }
}
