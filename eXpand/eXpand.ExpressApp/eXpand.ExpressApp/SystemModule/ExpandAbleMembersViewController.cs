using System;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace eXpand.ExpressApp.SystemModule {
    public class ExpandAbleMembersViewController : ViewController {
        public ExpandAbleMembersViewController() {
            TargetViewType = ViewType.DetailView;
            TargetObjectType = typeof (PersistentBase);
        }

        protected override void OnActivated() {
            base.OnActivated();
            ConstractExpandObjectMembers(View.ObjectSpace.Session, (PersistentBase) View.CurrentObject);
        }

        public void ConstractExpandObjectMembers(Session session, PersistentBase persistentBase) {
            if (persistentBase != null && session.IsNewObject(persistentBase)) {
                foreach (XPMemberInfo memberInfo in persistentBase.ClassInfo.ObjectProperties) {
                    if (memberInfo.HasAttribute(typeof (ExpandObjectMembersAttribute))) {
                        if (((ExpandObjectMembersAttribute)
                             memberInfo.GetAttributeInfo(typeof (ExpandObjectMembersAttribute))).ExpandingMode !=
                            ExpandObjectMembers.Never && memberInfo.GetValue(persistentBase) == null)
                            memberInfo.SetValue(persistentBase,
                                                Activator.CreateInstance(memberInfo.MemberType,
                                                                         new object[] {persistentBase.Session}));
                    }
                }
            }
        }
    }
}