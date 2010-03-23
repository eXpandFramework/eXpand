using System;
using System.Collections.Generic;
using System.Globalization;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Builders;
using System.Linq;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.ModelDifference.DictionaryStores;

namespace eXpand.ExpressApp.ModelDifference{
    
    public sealed partial class ModelDifferenceModule : ModuleBase
    {
        

        public ModelDifferenceModule()
        {
            InitializeComponent();
        }
        
        public override void UpdateModel(Dictionary model)
        {
            base.UpdateModel(model);
            AddCultures(model);
            if (SecuritySystem.UserType!= null&& !(SecuritySystem.Instance is ISecurityComplex)){
                RemoveXpoRoleModelDictionaryDifferenceStoreBONode(model);
            }
        }

        public void AddCultures(Dictionary model){
            var propertyInfoNodeWrapper = new ApplicationNodeWrapper(model).BOModel.FindClassByType(typeof(ModelDifferenceObject)).AllProperties.Where(wrapper => wrapper.Name == "PreferredAspect").Single();
            propertyInfoNodeWrapper.Node.SetAttribute("PredefinedValues", GetAllCultures());
        }

        private void RemoveXpoRoleModelDictionaryDifferenceStoreBONode(Dictionary model)
        {
            BOModelNodeWrapper boModelNodeWrapper = new ApplicationNodeWrapper(model).BOModel;
            ClassInfoNodeWrapper wrapper = boModelNodeWrapper.Classes.Where(nodeWrapper => nodeWrapper.ClassTypeInfo.Type==typeof(RoleModelDifferenceObject)).FirstOrDefault();
            if (wrapper != null) 
                boModelNodeWrapper.Node.RemoveChildNode(wrapper.Node);
        }
        
        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            base.CustomizeTypesInfo(typesInfo);
            if (Application != null && Application.Security != null){
                if (Application.Security is ISecurityComplex)
                    RoleDifferenceObjectBuilder.CreateDynamicMembers((ISecurityComplex) Application.Security);
                UserDifferenceObjectBuilder.CreateDynamicMembers(Application.Security.UserType);
            }
            else{
                createDesignTimeCollection(typesInfo, typeof (UserModelDifferenceObject), "Users");
                createDesignTimeCollection(typesInfo, typeof (RoleModelDifferenceObject), "Roles");
            }
        }

        private void createDesignTimeCollection(ITypesInfo typesInfo, Type classType, string propertyName){
            XPClassInfo info = XafTypesInfo.XpoTypeInfoSource.XPDictionary.GetClassInfo(classType);
            if (info.FindMember(propertyName)== null) {
                info.CreateMember(propertyName,typeof (XPCollection),true);
                typesInfo.RefreshInfo(classType);
            }
        }


        public override void Setup(XafApplication application)
        {
            
            base.Setup(application);
            application.CreateCustomUserModelDifferenceStore+=ApplicationOnCreateCustomUserModelDifferenceStore;
        }




        private void ApplicationOnCreateCustomUserModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs args)
        {
            args.Handled = true;
            args.Store =new XpoUserModelDictionaryDifferenceStore( Application);
        }

        public string GetAllCultures()
        {
            var list=new List<string>{DictionaryAttribute.DefaultLanguage};
            list.AddRange(CultureInfo.GetCultures(CultureTypes.AllCultures).Select(info => new CultureDescription(info).ToString()));
            string s = list.Aggregate("", (current, list1) => current + (list1 + ";"));
            return s.TrimEnd(';');
        }


    }
}