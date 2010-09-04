using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.SystemModule {
    public interface IModelClassCreateExpandAbleMembers
    {
        [Category("eXpand")]
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
                ConstractExpandObjectMembers(View.ObjectSpace.Session);
        }

        public virtual void ConstractExpandObjectMembers(Session session) {
            if (View.CurrentObject != null && session.IsNewObject(View.CurrentObject)) {
                foreach (var memberInfo in View.ObjectTypeInfo.Members) {
                    var expandObjectMembersAttribute = memberInfo.FindAttribute<ExpandObjectMembersAttribute>();
                    if (expandObjectMembersAttribute != null &&
                        expandObjectMembersAttribute.ExpandingMode != ExpandObjectMembers.Never) {
                        if (memberInfo.GetValue(View.CurrentObject) == null) {
                            memberInfo.SetValue(View.CurrentObject, ObjectSpace.CreateObject(memberInfo.MemberType));
                        }
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