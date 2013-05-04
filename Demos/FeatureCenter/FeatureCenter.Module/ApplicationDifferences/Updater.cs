using System;
using System.IO;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;
using Xpand.ExpressApp.ModelDifference.Security.Improved;
using Xpand.Xpo.Collections;
using Xpand.ExpressApp.Security.Core;

namespace FeatureCenter.Module.ApplicationDifferences {

    public class Updater : FCUpdater {
        private const string ModelCombine = "ModelCombine";

        public Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }



        public override void UpdateDatabaseAfterUpdateSchema() {
            var session = ((XPObjectSpace)ObjectSpace).Session;
            if (new QueryModelDifferenceObject(session).GetActiveModelDifference(ModelCombine, FeatureCenterModule.Application) == null) {
                new ModelDifferenceObject(session).InitializeMembers(ModelCombine, FeatureCenterModule.Application);
                var role = (XpandRole)ObjectSpace.GetRole(ModelCombine);
                role.CanEditModel = true;
                var permissionData = ObjectSpace.CreateObject<ModelCombineOperationPermissionData>();
                permissionData.Difference = ModelCombine;
                role.Permissions.Add(permissionData);

                var user = ObjectSpace.GetUser(ModelCombine, "", role);
                var collection = (XPBaseCollection)((XafMemberInfo)XafTypesInfo.Instance.FindTypeInfo(role.GetType()).FindMember("Users")).GetValue(role);
                collection.BaseAdd(user);
                ObjectSpace.CommitChanges();
            }
            var modelDifferenceObjects = new XpandXPCollection<ModelDifferenceObject>(session, o => o.PersistentApplication.Name == "FeatureCenter");
            foreach (var modelDifferenceObject in modelDifferenceObjects) {
                modelDifferenceObject.PersistentApplication.Name =
                    Path.GetFileNameWithoutExtension(modelDifferenceObject.PersistentApplication.ExecutableName);
            }
            ObjectSpace.CommitChanges();
        }

    }
}
