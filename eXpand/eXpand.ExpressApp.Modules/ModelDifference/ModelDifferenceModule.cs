using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Builders;
using eXpand.ExpressApp.ModelDifference.DictionaryStores;


namespace eXpand.ExpressApp.ModelDifference
{
    public sealed class ModelDifferenceModule : XpandModuleBase{
        public ModelDifferenceModule(){
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.CloneObject.CloneObjectModule));
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            base.CustomizeTypesInfo(typesInfo);

            if (Application != null && Application.Security != null)
            {
                if (Application.Security is ISecurityComplex)
                    RoleDifferenceObjectBuilder.CreateDynamicMembers((ISecurityComplex)Application.Security);

                UserDifferenceObjectBuilder.CreateDynamicMembers(Application.Security.UserType);
            }
            else
            {
                createDesignTimeCollection(typesInfo, typeof(UserModelDifferenceObject), "Users");
                createDesignTimeCollection(typesInfo, typeof(RoleModelDifferenceObject), "Roles");
            }
        }

        private void createDesignTimeCollection(ITypesInfo typesInfo, Type classType, string propertyName)
        {
            XPClassInfo info = XafTypesInfo.XpoTypeInfoSource.XPDictionary.GetClassInfo(classType);
            if (info.FindMember(propertyName) == null)
            {
                info.CreateMember(propertyName, typeof(XPCollection), true);
                typesInfo.RefreshInfo(classType);
            }
        }

        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            if (!(Application is ISupportModelsManager))
                throw new NotImplementedException("Implement " + typeof(ISupportModelsManager).FullName + " at your " + Application.GetType().FullName);
            application.CreateCustomUserModelDifferenceStore +=ApplicationOnCreateCustomUserModelDifferenceStore;
        }

        void ApplicationOnCreateCustomUserModelDifferenceStore(object sender, DevExpress.ExpressApp.CreateCustomModelDifferenceStoreEventArgs args) {
            args.Handled = true;
            args.Store = new XpoUserModelDictionaryDifferenceStore(Application);
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
        {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new BOModelNodesUpdater());
        }

    }

    public class BOModelNodesUpdater : ModelNodesGeneratorUpdater<ModelBOModelClassNodesGenerator>
    {
        public override void UpdateNode(ModelNode node)
        {
            var classNode = ((IModelBOModel)node)[typeof(RoleModelDifferenceObject).FullName];
            if (SecuritySystem.UserType != null && !(SecuritySystem.Instance is ISecurityComplex) && classNode != null)
            {
                node.Remove((ModelNode)classNode);
            }
        }
    }
}