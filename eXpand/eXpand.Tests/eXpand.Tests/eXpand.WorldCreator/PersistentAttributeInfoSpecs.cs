using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using Machine.Specifications;

namespace eXpand.Tests.eXpand.WorldCreator
{
    [Subject(typeof(PersistentAttributeInfo))]
    public class When_saving_ClassInfo_attribute:With_In_Memory_DataStore
    {
        static GenerateCodeController _generateCodeController;

        static PersistentAttributeInfo _persistentAttributeInfo;

        Establish context = () => {
            var controllerFactory = new ControllerFactory<GenerateCodeController, PersistentDefaultClassOptionsAttribute>();
            controllerFactory.CreateAndActivate();
            _generateCodeController = controllerFactory.Controller;
            _persistentAttributeInfo = ((PersistentDefaultClassOptionsAttribute)_generateCodeController.View.CurrentObject);
            var persistentClassInfo = new PersistentClassInfo(_persistentAttributeInfo.Session){PersistentAssemblyInfo = new PersistentAssemblyInfo(_persistentAttributeInfo.Session)};
            var codeTemplate = new CodeTemplate(_persistentAttributeInfo.Session) { TemplateType = TemplateType.Class };
            codeTemplate.SetDefaults();
            persistentClassInfo.CodeTemplateInfo.TemplateInfo = codeTemplate;
            _persistentAttributeInfo.Owner = persistentClassInfo;

        };

        Because of = () => ((UnitOfWork) _persistentAttributeInfo.Session).CommitChanges();

        It should_generate_code_for_the_referenced_class_info=() =>
                                                              ((IPersistentTemplatedTypeInfo) _persistentAttributeInfo.Owner).CodeTemplateInfo.GeneratedCode.ShouldNotBeNull();
    }
    [Subject(typeof(PersistentAttributeInfo))]
    public class When_saving_memberInfo_attribute : With_In_Memory_DataStore
    {
        static GenerateCodeController _generateCodeController;
        static PersistentAttributeInfo _persistentAttributeInfo;

        Establish context = () =>
        {
            var controllerFactory = new ControllerFactory<GenerateCodeController, PersistentDefaultClassOptionsAttribute>();
            controllerFactory.CreateAndActivate();
            _generateCodeController = controllerFactory.Controller;
            _persistentAttributeInfo = ((PersistentDefaultClassOptionsAttribute)_generateCodeController.View.CurrentObject);
            var persistentCoreTypeMemberInfo = new PersistentCoreTypeMemberInfo(_persistentAttributeInfo.Session);
            var codeTemplate = new CodeTemplate(_persistentAttributeInfo.Session) { TemplateType = TemplateType.ReadWriteMember };
            codeTemplate.SetDefaults();
            persistentCoreTypeMemberInfo.CodeTemplateInfo.TemplateInfo = codeTemplate;
            _persistentAttributeInfo.Owner = persistentCoreTypeMemberInfo;
        };

        Because of = () => ((UnitOfWork)_persistentAttributeInfo.Session).CommitChanges();

        It should_generate_code_for_the_referenced_class_info=() =>
                                                              ((IPersistentTemplatedTypeInfo) _persistentAttributeInfo.Owner).CodeTemplateInfo.GeneratedCode.ShouldNotBeNull();
    }
}
