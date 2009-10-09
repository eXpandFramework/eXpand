using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.ModelDifference.Web.Controllers{
    public partial class WebListEditorSettingsStoreViewController : DevExpress.ExpressApp.Web.WebListEditorSettingsStoreViewController
    {
        public const string SaveStateInDataStoreAttributeName = "SaveStateInDataStore";
        public const string SaveListViewStateInDataStoreAttributeName = "SaveListViewStateInDataStore";
        public WebListEditorSettingsStoreViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }
        protected override DevExpress.ExpressApp.Web.IKeyValueStorage CreateKeyValueStorage(bool storeStateToCookie)
        {
            if (Application.Model.RootNode.FindChildNode("Options").GetAttributeBoolValue(
                    SaveListViewStateInDataStoreAttributeName) ||View.Info.GetAttributeBoolValue(SaveStateInDataStoreAttributeName)){
                    DataStoreKeyValueStorage.Instance.RegisterKeyForCookies(View.Id,ObjectSpace.Session,Application.GetType().FullName);
                return DataStoreKeyValueStorage.Instance;
            }
            return base.CreateKeyValueStorage(storeStateToCookie);
        }
        public override void UpdateModel(Dictionary dictionary)
        {
            base.UpdateModel(dictionary);
            DictionaryNode optionsNode = dictionary.RootNode.GetChildNode(XafApplication.OptionsNodeName);
            optionsNode.SetAttribute(SaveListViewStateInDataStoreAttributeName, false);
        }
        public override Schema GetSchema()
        {
            var schema = new Schema(new DictionaryXmlReader().ReadFromString(
                                        @"<Element Name=""Application"">
					                        <Element Name=""Options"">
							                    <Attribute Name=""" + SaveListViewStateInDataStoreAttributeName + @""" Choice=""True,False"" />
					                        </Element>
                                            <Element Name=""Views"">
                                                <Element Name=""ListView"">
                                                    <Attribute Name=""" + SaveStateInDataStoreAttributeName + @""" Choice=""True,False"" DefaultValueExpr=""SourceNode=Options; SourceAttribute=@"+SaveListViewStateInDataStoreAttributeName + @""" />
                                                </Element>
                                            </Element>
				                        </Element>"));
            Schema schema2 = base.GetSchema();
            schema2.CombineWith(schema);
            
            return schema2;
        }
    }
}