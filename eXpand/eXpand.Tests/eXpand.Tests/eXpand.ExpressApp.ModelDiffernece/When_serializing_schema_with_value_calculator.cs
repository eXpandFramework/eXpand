using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.BaseImpl;
using eXpand.ExpressApp.AdditionalViewControlsProvider;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects.ValueConverters;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece
{
    public class When_serializing_schema_with_value_calculator
    {
        static AdditionalViewControlsRulesNodeWrapper _additionalViewControlsRulesNodeWrapper;
        static Dictionary _convertFromStorageType;
        static DictionaryValueConverter _dictionaryValueConverter;
        static object _convertToStorageType;

        Establish context = () => {
            const string s = @"<Element Name=""Application""><Element Name=""AdditionalViewControls"">
	<Element Name=""Rules"">
	  <Element Name=""AdditionalViewControlRule"" KeyAttribute=""ID"" Multiple=""True"">
	      <Attribute Name=""ID""  Required=""True""/>
	      <Attribute Name=""ExecutionContextGroup"" Required=""True"" DefaultValueExpr=""SourceNode=AdditionalViewControls\Contexts; SourceAttribute=@CurrentGroup""             
	          RefNodeName=""AdditionalViewControls/Contexts/*""/>
	  </Element>
	</Element>
	<Element Name=""Contexts"">
	    <Attribute Name=""CurrentGroup"" RefNodeName=""/Application/AdditionalViewControls/Contexts/*""/>
	    <Element Name=""ContextGroup"" Multiple=""True"" KeyAttribute=""ID"">
	        <Attribute Name=""ID"" Required=""True"" IsReadOnly=""True""/>
	        <Attribute Name=""Description"" IsLocalized=""True""/>
	        <Element Name=""ViewChanging"" KeyAttribute=""ID"" Multiple=""False"">
	            <Attribute Name=""ID"" Required=""True"" IsReadOnly=""True""/>
	            <Attribute Name=""Description"" IsLocalized=""True""/>
	        </Element>
	    </Element>
	</Element>
</Element></Element>";
            var schema = new Schema(new DictionaryXmlReader().ReadFromString(s));
            var commonSchema = Schema.GetCommonSchema();
            commonSchema.CombineWith(schema);

            var applicationNodeWrapper = new ApplicationNodeWrapper(new Dictionary(new DictionaryNode(ApplicationNodeWrapper.NodeName), schema));
            var viewControlsProviderModuleBase = Isolate.Fake.Instance<AdditionalViewControlsProviderModuleBase>(Members.CallOriginal);
            viewControlsProviderModuleBase.UpdateModel(applicationNodeWrapper.Dictionary);
            _additionalViewControlsRulesNodeWrapper = new AdditionalViewControlsRulesNodeWrapper(applicationNodeWrapper.Dictionary.RootNode.FindChildNode(AdditionalViewControlsRulesNodeWrapper.NodeNameAttribute).FindChildNode("Rules"));
            _additionalViewControlsRulesNodeWrapper.AddRule(new AdditionalViewControlsAttribute("sd","",AdditionalViewControlsProviderPosition.Bottom,
                                                                                              null, null, null,
                                                                                              null, null),
                                                          XafTypesInfo.CastTypeToTypeInfo(typeof (User)),
                                                          typeof (AdditionalViewControlsRuleNodeWrapper));
            var generateDetailViewItemsNode = new DetailViewItemsFactory().GenerateDetailViewItemsNode();
            applicationNodeWrapper.Dictionary.RootNode.AddChildNode(generateDetailViewItemsNode);
            _dictionaryValueConverter = new DictionaryValueConverter();
            _convertToStorageType = _dictionaryValueConverter.ConvertToStorageType(applicationNodeWrapper.Dictionary);
        };


        Because of = () => {
            _convertFromStorageType = (Dictionary) _dictionaryValueConverter.ConvertFromStorageType(_convertToStorageType);
        };

        It should_calcuted_properties_that_are_calculabe = () => {
            var dictionaryNode = _convertFromStorageType.RootNode.FindChildNode(AdditionalViewControlsRulesNodeWrapper.NodeNameAttribute).FindChildNode("Rules").ChildNodes[0];
            dictionaryNode.GetAttributeValue("ExecutionContextGroup").ShouldEqual("Default");
        };

        It should_contains_nodes_created_at_application_setup_such_us_detailviewItems_node =
            () => _convertFromStorageType.RootNode.FindChildNode("DetailViewItems").ShouldNotBeNull();
    }
}
