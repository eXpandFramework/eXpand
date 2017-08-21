using System;
using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.Security.Permissions {
    public enum Modifier {
        Allow,
        Deny
    }

    public interface IModifier {
        Modifier Modifier { get; set; }
    }

    [Serializable]
    public abstract class ModifierOperationRequest : OperationPermissionRequestBase, IModifier {
        protected ModifierOperationRequest(ModifierPermission permission, string operationName)
            : base(operationName) {
            Modifier = permission.Modifier;
        }

        public override object GetHashObject() {
            return GetType().Name;
        }

        public Modifier Modifier { get; set; }
    }

    public abstract class ModifierRequestProcessor<TModifierPermission> : PermissionRequestProcessorBase<ModifierOperationRequest>, ICustomPermissionRequestProccesor where TModifierPermission : ModifierPermission {
        public IPermissionDictionary Permissions{ get; set; }
        public override bool IsGranted(ModifierOperationRequest modifierOperationRequest) {
            var permission = Permissions.FindFirst<TModifierPermission>();
            return permission != null && permission.Modifier == modifierOperationRequest.Modifier;
        }
    }

    public interface ICustomPermissionRequestProccesor{
        IPermissionDictionary Permissions { get; set; }
    }


    [MapInheritance(MapInheritanceType.ParentTable)]
    public abstract class ModifierPermissionData : XpandPermissionData, IModifier {
        protected ModifierPermissionData(Session session)
            : base(session) {
        }
        Modifier _modifier;

        public Modifier Modifier {
            get { return _modifier; }
            set { SetPropertyValue("Modifier", ref _modifier, value); }
        }
    }

    public abstract class ModifierPermission : OperationPermissionBase, IModifier {
        protected ModifierPermission(string operation) : base(operation) {
        }
        public Modifier Modifier { get; set; }
    }
}
