using DevExpress.Xpo;
using Xpand.ExpressApp.Security.Permissions;

namespace Xpand.Persistent.BaseImpl.Security.PermissionPolicyData{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public abstract class ModifierPermissionPolicyData : PermissionPolicyData, IModifier {
        protected ModifierPermissionPolicyData(Session session)
            : base(session) {
        }
        Modifier _modifier;

        public Modifier Modifier{
            get { return _modifier; }
            set { SetPropertyValue("Modifier", ref _modifier, value); }
        }
    }
}
