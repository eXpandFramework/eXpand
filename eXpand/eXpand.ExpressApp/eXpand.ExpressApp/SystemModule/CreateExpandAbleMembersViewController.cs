using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace eXpand.ExpressApp.SystemModule {
    public interface IModelClassCreateExpandAbleMembers
    {
        [DefaultValue(true)]
        [Description("Creates automatically any ref objects that are null when a detailview of a new persistent object is shown")]
        bool CreateExpandAbleMembers { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassCreateExpandAbleMembers), "ModelClass")]
    public interface IModelDetailViewCreateExpandAbleMembers : IModelClassCreateExpandAbleMembers
    {
        
    }
    public class CreateExpandAbleMembersViewController : ViewController,IModelExtender {
        public CreateExpandAbleMembersViewController() {
            TargetViewType = ViewType.DetailView;
            TargetObjectType = typeof (PersistentBase);
        }

        protected override void OnActivated() {
            base.OnActivated();
            if (((IModelDetailViewCreateExpandAbleMembers) View.Model).CreateExpandAbleMembers)
                ConstractExpandObjectMembers(View.ObjectSpace.Session, (PersistentBase) View.CurrentObject);
        }

        public virtual void ConstractExpandObjectMembers(Session session, PersistentBase persistentBase) {
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

        #region IModelExtender Members

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelClass, IModelClassCreateExpandAbleMembers>();
            extenders.Add<IModelDetailView, IModelDetailViewCreateExpandAbleMembers>();
        }

        #endregion
    }
}