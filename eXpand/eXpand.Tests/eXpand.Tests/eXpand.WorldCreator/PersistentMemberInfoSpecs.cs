using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.WorldCreator {
    
    [Subject(typeof(PersistentMemberInfo), "Initialization")]
    public class When_initializing_a_PersistentMemberInfo_and_default_template_exists:With_In_Memory_DataStore {
        static PersistentMemberInfo _persistentMemberInfo;

        static CodeTemplate _codeTemplate;

        Establish context = () => {
            _persistentMemberInfo = Isolate.Fake.Instance<PersistentMemberInfo>(Members.CallOriginal,ConstructorWillBe.Called,Session.DefaultSession);
            _codeTemplate = new CodeTemplate(Session.DefaultSession){IsDefault = true,TemplateType = TemplateType.ReadWriteMember};
            _codeTemplate.Save();            
        };

        Because of = () => _persistentMemberInfo.Init(typeof(CodeTemplate), CodeDomProvider.CSharp);

        It should_assign_that_template_to_classInfo = () => _persistentMemberInfo.CodeTemplateInfo.CodeTemplate.ShouldEqual(_codeTemplate);
    }
    [Subject(typeof(PersistentMemberInfo), "Initialization")]
    public class When_initializing_a_read_write_PersistentMemberInfo:With_In_Memory_DataStore
    {
        static PersistentMemberInfo _persistentMemberInfo;

        static CodeTemplate _codeTemplate;

        Establish context = () => { _persistentMemberInfo = Isolate.Fake.Instance<PersistentMemberInfo>(Members.CallOriginal, ConstructorWillBe.Called, Session.DefaultSession); };

        Because of = () => _persistentMemberInfo.Init(typeof(CodeTemplate), CodeDomProvider.CSharp);
        It should_create_a_default_classInfo_template_if_not_exists = () =>
        {
            _codeTemplate = _persistentMemberInfo.CodeTemplateInfo.CodeTemplate;
            _codeTemplate.ShouldNotBeNull();
            _codeTemplate.IsDefault.ShouldBeTrue();    
            _codeTemplate.TemplateType.ShouldEqual(TemplateType.ReadWriteMember);
        };

    }
    [Subject(typeof(PersistentMemberInfo), "Initialization")]
    public class When_initializing_a_read_only_PersistentMemberInfo:With_In_Memory_DataStore
    {
        static PersistentMemberInfo _persistentMemberInfo;

        static CodeTemplate _codeTemplate;

        Establish context = () => { _persistentMemberInfo = new PersistentCollectionMemberInfo(Session.DefaultSession); };

        Because of = () => _persistentMemberInfo.Init(typeof(CodeTemplate), CodeDomProvider.CSharp);

        It should_create_a_default_classInfo_template_if_not_exists = () =>
        {
            _codeTemplate = _persistentMemberInfo.CodeTemplateInfo.CodeTemplate;
            _codeTemplate.ShouldNotBeNull();
            _codeTemplate.IsDefault.ShouldBeTrue();    
            _codeTemplate.TemplateType.ShouldEqual(TemplateType.ReadOnlyMember);
        };

    }
    [Subject(typeof(PersistentMemberInfo))]
    public class When_saving_a_memberinfo_associated_with_classInfo:With_In_Memory_DataStore {
        
        static ControllerFactory<GenerateCodeController, PersistentCoreTypeMemberInfo> _controllerFactory;

        Establish context = () => {
            _controllerFactory = new ControllerFactory<GenerateCodeController, PersistentCoreTypeMemberInfo>();
            _controllerFactory.CreateAndActivate();
            GenerateCodeController generateCodeController = _controllerFactory.Controller;
            var persistentCoreTypeMemberInfo = ((PersistentCoreTypeMemberInfo)generateCodeController.View.CurrentObject);
            persistentCoreTypeMemberInfo.CodeTemplateInfo.TemplateInfo.TemplateCode = "TemplateCode";
            var persistentClassInfo = new PersistentClassInfo(_controllerFactory.UnitOfWork) {
                                                                                                 CodeTemplateInfo = {TemplateInfo = {TemplateCode = "ClassTemplateCode"}},
                                                                                                 PersistentAssemblyInfo =
                                                                                                     new PersistentAssemblyInfo(_controllerFactory.UnitOfWork)
                                                                                             };
            persistentCoreTypeMemberInfo.Owner = persistentClassInfo;
        };

        Because of = () => _controllerFactory.UnitOfWork.CommitChanges();

        It should_generate_code_for_the_memberInfo =
            () => _controllerFactory.CurrentObject.CodeTemplateInfo.GeneratedCode.ShouldStartWith("TemplateCode");
        It should_generate_associate_classinfo_generatedcode =
            () => _controllerFactory.CurrentObject.Owner.CodeTemplateInfo.GeneratedCode.ShouldStartWith("ClassTemplateCode"); 
    }
}