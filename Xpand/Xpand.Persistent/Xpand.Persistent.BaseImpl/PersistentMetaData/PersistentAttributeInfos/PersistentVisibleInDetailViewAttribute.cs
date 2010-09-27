using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [DefaultProperty("PersistentVisibleInDetailViewName")]
    public class PersistentVisibleInDetailViewAttribute : PersistentAttributeInfo {
        public PersistentVisibleInDetailViewAttribute(Session session)
            : base(session) {
        }
        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        public string PersistentVisibleInDetailViewName {
            get { return "PersistentVisibleInDetailView"; }
        }

        private bool _visible;
        public bool Visible {
            get {
                return _visible;
            }
            set {
                SetPropertyValue("Visible", ref _visible, value);
            }
        }
        public override AttributeInfo Create() {
            return
                new AttributeInfo(typeof(VisibleInDetailViewAttribute).GetConstructor(new[] { typeof(bool) }), Visible);
        }
    }
}
