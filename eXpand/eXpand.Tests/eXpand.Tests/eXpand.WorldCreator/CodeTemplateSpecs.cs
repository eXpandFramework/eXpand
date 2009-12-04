using eXpand.ExpressApp.WorldCreator;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using Machine.Specifications;

namespace eXpand.Tests.eXpand.WorldCreator{
    [Subject(typeof(CodeTemplate))]
    public class When_changing_template_type_of_a_CodeTemplate:With_In_Memory_DataStore {
        static CodeTemplate _codeTemplate;

        Establish context = () => {
            _codeTemplate =
                (CodeTemplate)
                new ViewControllerFactory().CreateAndActivateController<CodeTemplateTypeModifierController, CodeTemplate>().View.CurrentObject;
        };

        Because of = () => {_codeTemplate.TemplateType=TemplateType.Class; };

        It should_find_all_default_template_usings_from_module_resources_and_display_them = () => _codeTemplate.Usings.ShouldNotBeNull();
        It should_find_all_default_template_from_module_resources_and_display_them = () => _codeTemplate.TemplateCode.ShouldNotBeNull();
    }
}