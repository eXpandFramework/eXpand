using System;
using System.ComponentModel;
using System.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.DictionaryDifferenceStore.DictionaryStores;
using XpoUserModelDictionaryDifferenceStore=
    eXpand.ExpressApp.DictionaryDifferenceStore.BaseObjects.XpoUserModelDictionaryDifferenceStore;

namespace eXpand.ExpressApp.DictionaryDifferenceStore
{
    [Description(
        "Changes Model DataStore to database and assigns to a user/role"),
     ToolboxTabName("eXpressApp"), EditorBrowsable(EditorBrowsableState.Always), Browsable(true), ToolboxItem(true)]
    public sealed partial class DictionaryDifferenceModule : ModuleBase
    {
        public const string CreateCustomModelStore = "CreateCustomModelStore";
        public const string CreateCustomUserModelStore = "CreateCustomUserModelStore";

        private IObjectSpaceProvider objectSpaceProvider;

        public DictionaryDifferenceModule()
        {
            InitializeComponent();
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            base.CustomizeTypesInfo(typesInfo);
            
            XPDictionary xpDictionary = XafTypesInfo.XpoTypeInfoSource.XPDictionary;
            createBasicUserProperties(xpDictionary);
            makeModifiedOnTime(xpDictionary);

        }

        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            application.CreateCustomObjectSpaceProvider+=ApplicationOnCreateCustomObjectSpaceProvider;
            application.CreateCustomUserModelDifferenceStore+=ApplicationOnCreateCustomUserModelDifferenceStore;
            application.CreateCustomModelDifferenceStore+=ApplicationOnCreateCustomModelDifferenceStore;
        }

        public override Schema GetSchema()
        {
            string s = @"<Element Name=""Application"">;
                            <Element Name=""" +typeof(DictionaryDifferenceModule).Name+ @""">
                                    <Attribute Name=""" + CreateCustomUserModelStore + @""" Choice=""False,True""/>
                                    <Attribute Name=""" + CreateCustomModelStore + @""" Choice=""False,True""/>
                            </Element>
                    </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(s));
        }

        private void ApplicationOnCreateCustomObjectSpaceProvider(object sender, CreateCustomObjectSpaceProviderEventArgs args)
        {
            objectSpaceProvider = args.ObjectSpaceProvider;
        }

        private void ApplicationOnCreateCustomUserModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs args)
        {
            if (objectSpaceProvider == null)
                throw new NoNullAllowedException("Custom objectSpaceProvider");
            args.Handled = true;
            args.Store =
                new DictionaryStores.XpoUserModelDictionaryDifferenceStore(objectSpaceProvider.CreateUpdatingSession(),
                                                      Application);

        }

        private void ApplicationOnCreateCustomModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs args)
        {
            if (objectSpaceProvider== null)
                throw new NoNullAllowedException("Custom objectSpaceProvider");
            args.Handled = true;
            args.Store =
                new XpoModelDictionaryDifferenceStore(objectSpaceProvider.CreateUpdatingSession(),
                                                      Application);
        }


        private void makeModifiedOnTime(XPDictionary xpDictionary)
        {
            XPMemberInfo memberInfo = xpDictionary.GetClassInfo(typeof (AuditDataItemPersistent)).GetMember("ModifiedOn");
            memberInfo.AddAttribute(new CustomAttribute("DisplayFormat", "{0: ddd, dd MMMM yyyy hh:mm:ss tt}"));
            memberInfo.AddAttribute(new CustomAttribute("EditMask", "ddd, dd MMMM yyyy hh:mm:ss tt"));
        }

        private void createBasicUserProperties(XPDictionary xpDictionary)
        {
            
            if (SecuritySystem.UserType != null)
            {
                XPClassInfo xpClassInfo = xpDictionary.GetClassInfo(SecuritySystem.UserType);
                if (xpClassInfo.FindMember(typeof (XpoUserModelDictionaryDifferenceStore).Name) == null)
                    addUserToModelAssociation(xpClassInfo);
            }
            addModelToUserAssociation(xpDictionary);
            if (SecuritySystem.UserType != null) XafTypesInfo.Instance.RefreshInfo(SecuritySystem.UserType);
            XafTypesInfo.Instance.RefreshInfo(typeof(XpoUserModelDictionaryDifferenceStore));
        }

        private void addModelToUserAssociation(XPDictionary xpDictionary)
        {
            XPClassInfo xpClassInfo = xpDictionary.GetClassInfo(typeof(XpoUserModelDictionaryDifferenceStore));


            Type type = fetchType();
            if (xpClassInfo.FindMember("Users")== null)
                xpClassInfo.CreateMember("Users", typeof(XPCollection), true,
                                     new AssociationAttribute(
                                         XpoUserModelDictionaryDifferenceStore.BasicUsersAssociation,
                                         type));
            
        }

        private Type fetchType()
        {
            try
            {
                return SecuritySystem.Instance.UserType;
            }
            catch (Exception)
            {
                return typeof(User);
            }
        }

        private void addUserToModelAssociation(XPClassInfo xpClassInfo)
        {
            string propertyName = typeof(XpoUserModelDictionaryDifferenceStore).Name;
            if (xpClassInfo.FindMember(propertyName) == null)
                xpClassInfo.CreateMember(propertyName,
                                         typeof (XpoUserModelDictionaryDifferenceStore),
                                         new AssociationAttribute(
                                             XpoUserModelDictionaryDifferenceStore.BasicUsersAssociation));
            
        }
    }
}