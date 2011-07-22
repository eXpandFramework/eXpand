using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Builders;
using Xpand.ExpressApp.ModelDifference.DictionaryStores;
using Xpand.ExpressApp.ModelDifference.NodeUpdaters;
using Xpand.ExpressApp.SystemModule;


namespace Xpand.ExpressApp.ModelDifference {
    public sealed class ModelDifferenceModule : XpandModuleBase {
        XpoUserModelDictionaryDifferenceStore _xpoUserModelDictionaryDifferenceStore;

        public ModelDifferenceModule() {
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.CloneObject.CloneObjectModule));
            RequiredModuleTypes.Add(typeof(XpandSystemModule));
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);

            if (Application != null && Application.Security != null) {
                if (Application.Security is ISecurityComplex)
                    RoleDifferenceObjectBuilder.CreateDynamicRoleMember((ISecurityComplex)Application.Security);

                UserDifferenceObjectBuilder.CreateDynamicUserMember(Application.Security.UserType);
            } else {
                CreateDesignTimeCollection(typesInfo, typeof(UserModelDifferenceObject), "Users");
                CreateDesignTimeCollection(typesInfo, typeof(RoleModelDifferenceObject), "Roles");
            }
        }


        public override void Setup(XafApplication application) {
            base.Setup(application);
            if (application != null && !DesignMode) {
                if (!(application is ISupportModelsManager))
                    throw new NotImplementedException("Implement " + typeof(ISupportModelsManager).FullName + " at your " + Application.GetType().FullName + " descenant");
                application.CreateCustomUserModelDifferenceStore += ApplicationOnCreateCustomUserModelDifferenceStore;
            }
        }

        void ApplicationOnCreateCustomUserModelDifferenceStore(object sender, DevExpress.ExpressApp.CreateCustomModelDifferenceStoreEventArgs args) {
            args.Handled = true;
            _xpoUserModelDictionaryDifferenceStore = _xpoUserModelDictionaryDifferenceStore ?? new XpoUserModelDictionaryDifferenceStore(Application);
            args.Store = _xpoUserModelDictionaryDifferenceStore;
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new BOModelNodesUpdater());
            updaters.Add(new BOModelMemberNodesUpdater());
        }

    }
}