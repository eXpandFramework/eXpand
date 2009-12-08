using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using Machine.Specifications;

namespace eXpand.Tests.eXpand.WorldCreator {
    [Subject(typeof(PersistentClassInfo), "Initialization")]
    public class When_initializing_a_PersistentClassInfo_and_default_template_exists:With_In_Memory_DataStore {
        static PersistentClassInfo _persistentClassInfo;

        static CodeTemplate _codeTemplate;

        Establish context = () => {
            _persistentClassInfo = new PersistentClassInfo(Session.DefaultSession);
            _codeTemplate = new CodeTemplate(Session.DefaultSession){IsDefault = true,TemplateType = TemplateType.Class};
            _codeTemplate.Save();            
        };

        Because of = () => _persistentClassInfo.Init(typeof(CodeTemplate), CodeDomProvider.CSharp);

        It should_assign_that_template_to_classInfo =
            () =>
            _persistentClassInfo.CodeTemplateInfo.CodeTemplate.ShouldEqual(_codeTemplate).
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
            _persistentClassInfo.Init(typeof(CodeTemplate), CodeDomProvider.CSharp);
            _persistentClassInfo.Save();
        };


        It should_create_a_default_classInfo_template_if_not_exists = () =>
        {
            _codeTemplate = _persistentClassInfo.CodeTemplateInfo.CodeTemplate;
            _codeTemplate.Session.IsNewObject(_codeTemplate).ShouldBeFalse();
            _codeTemplate.TemplateType.ShouldEqual(TemplateType.Class);
        };

        It should_set_template_as_default = () => _codeTemplate.IsDefault.ShouldBeTrue();
    }



}