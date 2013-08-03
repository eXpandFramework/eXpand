using System;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [DefaultProperty("PersistentVisibleInLookupListViewName")]
    [System.ComponentModel.DisplayName("Lisible In LookupView")]
    [CreateableItem(typeof(IPersistentMemberInfo))]
    [CreateableItem(typeof(IExtendedMemberInfo))]
    public class PersistentVisibleInLookupListViewAttribute : PersistentAttributeInfo {
        public PersistentVisibleInLookupListViewAttribute(Session session)
            : base(session) {
        }
        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        public string PersistentVisibleInLookupListViewName {
            get { return "PersistentVisibleInLookupListView"; }
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
        public override AttributeInfoAttribute Create() {
            return
                new AttributeInfoAttribute(typeof(VisibleInLookupListViewAttribute).GetConstructor(new[] { typeof(bool) }), Visible);
        }
    }
}
