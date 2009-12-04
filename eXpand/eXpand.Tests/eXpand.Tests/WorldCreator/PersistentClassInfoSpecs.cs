using System.Linq;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using Machine.Specifications;

namespace eXpand.Tests.WorldCreator {
    [Subject(typeof(PersistentClassInfo), "Initialization")]
    public class When_initializing_a_PersistentClassInfo_and_default_template_exists:With_In_Memory_DataStore {
        static PersistentClassInfo _persistentClassInfo;

        static CodeTemplate _codeTemplate;

        Establish context = () => {
            _persistentClassInfo = new PersistentClassInfo(Session.DefaultSession);
            _codeTemplate = new CodeTemplate(Session.DefaultSession){IsDefault = true,TemplateType = TemplateType.Class};
            _codeTemplate.Save();            
        };

        Because of = () => _persistentClassInfo.Init(typeof(CodeTemplate));

        It should_assign_that_template_to_classInfo =
            () =>
            _persistentClassInfo.CodeTemplate.ShouldEqual(_codeTemplate).
                ShouldNotBeNull();
    }
    [Subject(typeof(PersistentClassInfo), "Initialization")]
    public class When_initializing_a_PersistentClassInfo : With_In_Memory_DataStore
    {


        static PersistentClassInfo _persistentClassInfo;

        static CodeTemplate _codeTemplate;

        Establish context = () =>
        {
            _persistentClassInfo = new PersistentClassInfo(Session.DefaultSession);
        };

        Because of = () =>
        {
            _persistentClassInfo.Init(typeof(CodeTemplate));
            _persistentClassInfo.Save();
        };


        It should_create_a_default_classInfo_template_if_not_exists = () =>
        {
            var codeTemplates = new XPCollection<CodeTemplate>(Session.DefaultSession);
            codeTemplates.Count.ShouldEqual(1);
            _codeTemplate = codeTemplates[0];
            _codeTemplate.TemplateType.ShouldEqual(TemplateType.Class);
        };

        It should_set_template_as_default = () => _codeTemplate.IsDefault.ShouldBeTrue();
        It should_have_the_default_classinfo_template = () => _codeTemplate.TypeInfos.Where(info => info.Oid == _persistentClassInfo.Oid).FirstOrDefault().ShouldNotBeNull();
        It should_have_template_code_for_that_template = () => _codeTemplate.TemplateCode.ShouldNotBeNull();
        It should_have_references_for_that_template = () => _codeTemplate.References.ShouldNotBeNull();
        It should_be_able_to_sync_code_with_template_at_a_later_time;
    }

}