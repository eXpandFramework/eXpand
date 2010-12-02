using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base.Security;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;
using Xpand.ExpressApp.ModelDifference.Security;

namespace FeatureCenter.Module.ApplicationDifferences {

    public class Updater : Xpand.Persistent.BaseImpl.Updater {
        private const string ModelCombine = "ModelCombine";

        public Updater(ObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            if (new QueryModelDifferenceObject(ObjectSpace.Session).GetActiveModelDifference(ModelCombine) == null) {
                new ModelDifferenceObject(ObjectSpace.Session).InitializeMembers(ModelCombine);
                ICustomizableRole role = EnsureRoleExists(ModelCombine, GetPermissions);
                IUserWithRoles user = EnsureUserExists(ModelCombine, ModelCombine, role);
                role.AddPermission(new ModelCombinePermission(ApplicationModelCombineModifier.Allow) { Difference = ModelCombine });
                role.Users.Add(user);
                ObjectSpace.CommitChanges();
            }
        }
        protected override System.Collections.Generic.List<System.Security.IPermission> GetPermissions(ICustomizableRole customizableRole) {
            var permissions = base.GetPermissions(customizableRole);
            if (customizableRole.Name == ModelCombine)
                permissions.Add(new EditModelPermission(ModelAccessModifier.Allow));
            return permissions;
        }
    }
}
