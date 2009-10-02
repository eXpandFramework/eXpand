using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.ModelDifference.Web.Controllers{
    public partial class WebListEditorSettingsStoreViewController : DevExpress.ExpressApp.Web.WebListEditorSettingsStoreViewController
    {
        public const string SaveStateInDataStoreAttributeName = "SaveStateInDataStoreAttributeName";
        public WebListEditorSettingsStoreViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }
        protected override DevExpress.ExpressApp.Web.IKeyValueStorage CreateKeyValueStorage(bool storeStateToCookie)
        {
            if (Application.Model.RootNode.FindChildNode("Options").GetAttributeBoolValue(SaveStateInDataStoreAttributeName)){
                if (storeStateToCookie)
                    DataStoreKeyValueStorage.Instance.RegisterKeyForCookies(View.Id,ObjectSpace.Session,Application.GetType().FullName);
                return DataStoreKeyValueStorage.Instance;
            }
            return base.CreateKeyValueStorage(storeStateToCookie);
        }
        public override Schema GetSchema()
        {
            var schema = new Schema(new DictionaryXmlReader().ReadFromString(
                                        @"<Element Name=""Application"">
					<Element Name=""Options"">
							<Attribute Name=""" +SaveStateInDataStoreAttributeName+ @""" Choice=""True,False"" />
					</Element>
				</Element>"));
            Schema schema2 = base.GetSchema();
            schema2.CombineWith(schema);
            
            return schema2;
        }
    }
}