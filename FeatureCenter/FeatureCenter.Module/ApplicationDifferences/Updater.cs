using System;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;
using Xpand.ExpressApp.ModelDifference.Security;

namespace FeatureCenter.Module.ApplicationDifferences
{
    
    public class Updater:Xpand.Persistent.BaseImpl.Updater
    {
        private const string ModelCombine = "ModelCombine";
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) {
            
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
            if (new QueryModelDifferenceObject(Session).GetActiveModelDifference(ModelCombine)== null)
            {
                var modelDifferenceObject = new ModelDifferenceObject(Session).InitializeMembers(ModelCombine);
                modelDifferenceObject.Save();
                Role role = EnsureRoleExists(ModelCombine,GetPermissions);
                User user = EnsureUserExists( ModelCombine, ModelCombine,role);
                role.AddPermission(new ModelCombinePermission(ApplicationModelCombineModifier.Allow) { Difference = ModelCombine });
                role.Users.Add(user);
                role.Save();
            }
        }
        protected override System.Collections.Generic.List<System.Security.IPermission> GetPermissions(Role role) {
            var permissions = base.GetPermissions(role);
            if (role.Name==ModelCombine)
                permissions.Add(new EditModelPermission(ModelAccessModifier.Allow));
            return permissions;
        }
    }
}
