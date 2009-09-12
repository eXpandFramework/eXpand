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
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using System.Linq;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.ModelDifference.DictionaryStores;

namespace eXpand.ExpressApp.ModelDifference{
    
    public sealed partial class ModelDifferenceModule : ModuleBase
    {
        private bool applicationModelUpdated;

        public const string CreateCustomModelStore = "CreateCustomModelStore";
        public const string CreateCustomUserModelStore = "CreateCustomUserModelStore";

        


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
            RoleDifferenceObjectBuilder.CreateDynamicMembers();
            if (!UserDifferenceObjectBuilder.CreateDynamicMembers()){
                createDesignTimeCollection(typesInfo, typeof(UserModelDifferenceObject), "Users");
                createDesignTimeCollection(typesInfo, typeof(RoleModelDifferenceObject), "Roles");
            }
            
        }

        private void createDesignTimeCollection(ITypesInfo typesInfo, Type classType, string propertyName){
            XPClassInfo info = XafTypesInfo.XpoTypeInfoSource.XPDictionary.GetClassInfo(classType);
            info.CreateMember(propertyName,typeof (XPCollection),true);
            typesInfo.RefreshInfo(classType);
        }


        public override void Setup(XafApplication application)
        {
            
            base.Setup(application);
            
            application.SetupComplete += (sender, args) =>{
                                             if (!applicationModelUpdated){
                                                 PersistentApplication persistentApplication;
                                                 using (var objectSpace = application.CreateObjectSpace()){
                                                     persistentApplication =
                                                            new QueryPersistentApplication(objectSpace.Session).Find(Application.GetType().FullName) ??
                                                            new PersistentApplication(objectSpace.Session);
                                                     persistentApplication.Model = application.Model;
                                                     persistentApplication.UniqueName = application.GetType().FullName;
                                                     if (string.IsNullOrEmpty(persistentApplication.Name))
                                                         persistentApplication.Name =application.Title;
                                                     objectSpace.CommitChanges();
                                                 }
                                                 applicationModelUpdated = true;
                                             }
                                             
                                         };            
            
            application.CreateCustomUserModelDifferenceStore+=ApplicationOnCreateCustomUserModelDifferenceStore;
            
        }



        [CoverageExclude]
        public override Schema GetSchema()
        {
            string s = @"<Element Name=""Application"">;
                            <Element Name=""" +typeof(ModelDifferenceModule).Name+ @""">
                                    <Attribute Name=""" + CreateCustomUserModelStore + @""" Choice=""False,True""/>
                                    <Attribute Name=""" + CreateCustomModelStore + @""" Choice=""False,True""/>
                            </Element>
                    </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(s));
        }



//        protected abstract XpoDictionaryDifferenceStore getModelDifferenceStore(Session updatingSession);

        private void ApplicationOnCreateCustomUserModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs args)
        {
            args.Handled = true;
            args.Store =new XpoUserModelDictionaryDifferenceStore(Application.ObjectSpaceProvider.CreateUpdatingSession(), Application);
            

        }

        public class CultureDescription
        {
            private readonly string name;
            private readonly string caption;
            public CultureDescription(CultureInfo culture)
            {
                name = culture.Name;
                caption = string.Concat(new[] { name, " (", culture.DisplayName, " - ", culture.NativeName, ")" });
            }
            public override string ToString()
            {
                return caption;
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            public override bool Equals(object obj)
            {
                var objCulture = obj as CultureDescription;
                if (objCulture != null)
                {
                    return name == objCulture.Name;
                }
                return base.Equals(obj);
            }
            public string Caption
            {
                get { return caption; }
            }
            public string Name
            {
                get { return name; }
            }
        }

        public string GetAllCultures()
        {
            var list=new List<string>{DictionaryAttribute.DefaultLanguage};
            
            list.AddRange(CultureInfo.GetCultures(CultureTypes.AllCultures).Select(info => new CultureDescription(info).ToString()));
            string s = "";
            foreach (var list1 in list){
                s += list1 + ";";
            }
            return s.TrimEnd(';');
        }
    }
}