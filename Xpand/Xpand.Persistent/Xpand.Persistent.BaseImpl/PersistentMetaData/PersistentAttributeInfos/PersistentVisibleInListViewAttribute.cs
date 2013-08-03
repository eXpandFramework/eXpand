using System;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [DefaultProperty("PersistentVisibleInListViewName")]
    [System.ComponentModel.DisplayName("Visible In ListView")]
    [CreateableItem(typeof(IPersistentMemberInfo))]
    [CreateableItem(typeof(IExtendedMemberInfo))]
    public class PersistentVisibleInListViewAttribute : PersistentAttributeInfo {
        public PersistentVisibleInListViewAttribute(Session session)
            : base(session) {
        }
        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        public string PersistentVisibleInListViewName {
            get { return "PersistentVisibleInListView"; }
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
            var constructorInfo = typeof(VisibleInListViewAttribute).GetConstructor(new[] { typeof(bool) });
            return new AttributeInfoAttribute(constructorInfo, Visible);
        }
    }
}
