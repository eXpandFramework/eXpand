using System.Linq;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.WorldCreator {
    [Subject(typeof(PersistentMemberInfo), "Initialization")]
    public class When_initializing_a_PersistentMemberInfo_and_default_template_exists:With_In_Memory_DataStore {
        static PersistentMemberInfo _persistentMemberInfo;

        static CodeTemplate _codeTemplate;

        Establish context = () => {
            _persistentMemberInfo = Isolate.Fake.Instance<PersistentMemberInfo>(Members.CallOriginal,ConstructorWillBe.Called,Session.DefaultSession);
            _codeTemplate = new CodeTemplate(Session.DefaultSession){IsDefault = true,TemplateType = TemplateType.Member};
            _codeTemplate.Save();            
        };

        Because of = () => _persistentMemberInfo.Init(typeof(CodeTemplate));

        It should_assign_that_template_to_classInfo = () => _persistentMemberInfo.CodeTemplate.ShouldEqual(_codeTemplate);
    }
    [Subject(typeof(PersistentMemberInfo), "Initialization")]
    public class When_initializing_a_PersistentMemberInfo
    {
        static PersistentMemberInfo _persistentMemberInfo;

        static CodeTemplate _codeTemplate;

        Establish context = () => { _persistentMemberInfo = Isolate.Fake.Instance<PersistentMemberInfo>(Members.CallOriginal, ConstructorWillBe.Called, Session.DefaultSession); };

        Because of = () => _persistentMemberInfo.Init(typeof(CodeTemplate));
        It should_create_a_default_classInfo_template_if_not_exists = () =>
        {
            var codeTemplates = new XPCollection<CodeTemplate>(Session.DefaultSession);
            codeTemplates.Count.ShouldEqual(1);
            _codeTemplate = codeTemplates[0];
            _codeTemplate.TemplateType.ShouldEqual(TemplateType.Member);
        };
        It should_set_template_as_default = () => _codeTemplate.IsDefault.ShouldBeTrue();
        It should_have_the_default_member_template = () => _codeTemplate.TypeInfos.Where(info => info.Oid == _persistentMemberInfo.Oid).FirstOrDefault().ShouldNotBeNull();
        It should_have_template_code_for_that_template = () => _codeTemplate.TemplateCode.ShouldNotBeNull();
        It should_be_able_to_sync_code_with_template_at_a_later_time;

    }

}