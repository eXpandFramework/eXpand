using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using Machine.Specifications;

namespace eXpand.Tests.eXpand.WorldCreator{
    [Subject(typeof(CodeTemplate))]
    public class When_changing_template_type_of_a_CodeTemplate:With_Isolations {
        static CodeTemplate _currentObject;

        Establish context = () => {
            var artifactHandler = new TestAppLication<CodeTemplate>().Setup(null,template => _currentObject=template);
            
            artifactHandler.WithArtiFacts(WCArtifacts).CreateDetailView().
                CreateFrame();
        };

        Because of = () => {_currentObject.TemplateType=TemplateType.Class; };


        It should_find_all_default_template_from_module_resources_and_display_them = () => _currentObject.TemplateCode.Length.ShouldBeGreaterThan(0);
    }
}