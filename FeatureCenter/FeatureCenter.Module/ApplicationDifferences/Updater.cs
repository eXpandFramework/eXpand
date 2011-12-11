using System;
using System.Collections.Generic;
using System.IO;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base.Security;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;
using Xpand.ExpressApp.ModelDifference.Security;
using Xpand.Xpo.Collections;

namespace FeatureCenter.Module.ApplicationDifferences {

    public class Updater : FCUpdater {
        private const string ModelCombine = "ModelCombine";

        public Updater(IObjectSpace objectSpace, Version currentDBVersion, Xpand.Persistent.BaseImpl.Updater updater)
            : base(objectSpace, currentDBVersion, updater) {
        }



        public override void UpdateDatabaseAfterUpdateSchema() {
            var session = ((ObjectSpace)ObjectSpace).Session;
            if (new QueryModelDifferenceObject(session).GetActiveModelDifference(ModelCombine, FeatureCenterModule.Application) == null) {
                new ModelDifferenceObject(session).InitializeMembers(ModelCombine, FeatureCenterModule.Application);
                var role = Updater.EnsureRoleExists(ModelCombine, customizableRole => GetPermissions(customizableRole, Updater));
                var user = Updater.EnsureUserExists(ModelCombine, ModelCombine, role);
                if (!Updater.IsNewSecuritySystem)
                    ((ICustomizableRole)role).Users.Add((IUser)user);
                else {
                    ((SecurityRole)role).Users.Add((SecurityUserWithRolesBase)user);
                }
                ObjectSpace.CommitChanges();
            }
            var modelDifferenceObjects = new XpandXPCollection<ModelDifferenceObject>(session, o => o.PersistentApplication.Name == "FeatureCenter");
            foreach (var modelDifferenceObject in modelDifferenceObjects) {
                modelDifferenceObject.PersistentApplication.Name =
                    Path.GetFileNameWithoutExtension(modelDifferenceObject.PersistentApplication.ExecutableName);
            }
            ObjectSpace.CommitChanges();
        }
        protected List<object> GetPermissions(object customizableRole, Xpand.Persistent.BaseImpl.Updater updater) {
            var permissions = updater.GetPermissions(customizableRole);
            if (!updater.IsNewSecuritySystem) {
                if (((ICustomizableRole)customizableRole).Name == ModelCombine) {
                    permissions.Add(new EditModelPermission(ModelAccessModifier.Allow));
                    permissions.Add(new ModelCombinePermission(ApplicationModelCombineModifier.Allow) { Difference = ModelCombine });
                }
            } else {
                GetPermissions(permissions, ((SecurityRole)customizableRole));
            }
            return permissions;
        }

        private void GetPermissions(List<object> permissions, SecurityRole securityRole) {
            if (securityRole.Name == ModelCombine) {
                var modelPermission = ObjectSpace.CreateObject<ModelOperationPermissionData>();
                modelPermission.Save();
                securityRole.BeginUpdate();
                securityRole.Permissions.GrantRecursive(typeof(object), SecurityOperations.Read);
                securityRole.Permissions.GrantRecursive(typeof(object), SecurityOperations.Write);
                securityRole.Permissions.GrantRecursive(typeof(object), SecurityOperations.Create);
                securityRole.Permissions.GrantRecursive(typeof(object), SecurityOperations.Delete);
                securityRole.Permissions.GrantRecursive(typeof(object), SecurityOperations.Navigate);
                securityRole.EndUpdate();
                permissions.Add(modelPermission);
            }
        }
    }
}
