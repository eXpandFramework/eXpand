using System;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;
using Xpand.ExpressApp.ModelDifference.Security;

namespace FeatureCenter.Module.ApplicationDifferences
{
    
    public class Updater:ModuleUpdater
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
                Role role = Module.Updater.EnsureRoleExists(Session, ModelCombine);
                User user = Module.Updater.EnsureUserExists(Session, ModelCombine, ModelCombine);
                role.AddPermission(new ModelCombinePermission(ApplicationModelCombineModifier.Allow) { Difference = ModelCombine });
                Module.Updater.ApplyModelEditingPermission(role);
                role.Users.Add(user);
                role.Save();
            }
        }
    }
}
