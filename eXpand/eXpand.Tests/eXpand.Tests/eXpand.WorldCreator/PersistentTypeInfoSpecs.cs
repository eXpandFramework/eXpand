using eXpand.ExpressApp.WorldCreator;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using Machine.Specifications;

namespace eXpand.Tests.eXpand.WorldCreator {
    [Subject(typeof(PersistentTypeInfo))]
    public class When_peristent_type_code_template_change:With_Isolations {
        static CodeTemplateInfo _codeTemplateInfo;

        static PersistentClassInfo _persistentClassInfo;
        static CodeTemplate _codeTemplate;

        Establish context = () => {
            new TestAppLication<PersistentClassInfo>().Setup(null,info => {
                info.PersistentAssemblyInfo=new PersistentAssemblyInfo(info.Session);
                _persistentClassInfo = info;
            }).WithArtiFacts(WCArtifacts).CreateDetailView().CreateFrame().RaiseControlsCreated();
            _codeTemplate = new CodeTemplate(_persistentClassInfo.Session){TemplateCode ="TemplateCode" };
        };


        Because of = () => { _persistentClassInfo.CodeTemplateInfo.CodeTemplate=_codeTemplate;};

        It should_delegate_all_props_from_code_template_to_persistent_type_templateinfo_object = () => _persistentClassInfo.CodeTemplateInfo.TemplateInfo.TemplateCode.ShouldEqual("TemplateCode");
    }

    [Subject(typeof(PersistentTypeInfo))]
    public class When_Deleting_PersistentTypes:With_Isolations {
        static PersistentClassInfo _persistentClassInfo;

        Establish context = () => {
            var artifactHandler = new TestAppLication<PersistentClassInfo>().Setup(null,info => {
                info.PersistentAssemblyInfo = new PersistentAssemblyInfo(info.Session);
                _persistentClassInfo = info;
            });
            artifactHandler.WithArtiFacts(WCArtifacts).CreateDetailView().CreateFrame().RaiseControlsCreated();
            artifactHandler.UnitOfWork.CommitChanges();
        };

        Because of = () => _persistentClassInfo.Delete();

        It should_delete_CodeTemplateInfo_as_well=() => _persistentClassInfo.CodeTemplateInfo.IsDeleted.ShouldBeTrue();
        It should_delete_TemplateInfo_as_well=() => _persistentClassInfo.CodeTemplateInfo.TemplateInfo.ShouldBeNull();
    }
}