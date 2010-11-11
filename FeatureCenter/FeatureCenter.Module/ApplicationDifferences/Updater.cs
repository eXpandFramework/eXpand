using System;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base.Security;
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
                ICustomizableRole role = EnsureRoleExists(ModelCombine,GetPermissions);
                IUserWithRoles user = EnsureUserExists( ModelCombine, ModelCombine,role);
                role.AddPermission(new ModelCombinePermission(ApplicationModelCombineModifier.Allow) { Difference = ModelCombine });
                role.Users.Add(user);
                Session.Save(role);
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
