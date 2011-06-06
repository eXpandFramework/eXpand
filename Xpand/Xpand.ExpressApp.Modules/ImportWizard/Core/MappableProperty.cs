using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.ImportWizard.Core {
    [CreatableItem(false)]
    public class MappableProperty : XPObject {
        public string Name { get; set; }
        public string DisplayName { set; get; }
        public bool Mapped { get; set; }
        public MappableProperty(Session session)
            : base(session) {
            Mapped = false;
        }

    }
}