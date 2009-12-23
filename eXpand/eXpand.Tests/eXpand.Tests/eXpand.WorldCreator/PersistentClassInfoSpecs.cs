using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using Machine.Specifications;

namespace eXpand.Tests.eXpand.WorldCreator {

    [Subject(typeof(PersistentClassInfo), "Initialization")]
    public class When_creating_PersistentClassInfo_and_default_template_exists : With_Isolations{
        static IFrameCreationHandler frameCreationHandler;
        static PersistentClassInfo _persistentClassInfo;

        static CodeTemplate _codeTemplate;

        Establish context = () => {
            var artifactHandler = new TestAppLication<PersistentClassInfo>().Setup(null, info => {
                _persistentClassInfo=info;
                info.PersistentAssemblyInfo=new PersistentAssemblyInfo(info.Session);
            });
            _codeTemplate = new CodeTemplate(new Session(artifactHandler.UnitOfWork.DataLayer)) { IsDefault = true, TemplateType = TemplateType.Class };
            _codeTemplate.Save();
            frameCreationHandler = artifactHandler.WithArtiFacts(WCArtifacts).CreateDetailView();
        };

        Because of = () => frameCreationHandler.CreateFrame().RaiseControlsCreated();

        It should_assign_that_template_to_classInfo =
            () =>
            _persistentClassInfo.CodeTemplateInfo.CodeTemplate.ShouldEqual(_codeTemplate).
                ShouldNotBeNull();
    }
    [Subject(typeof(PersistentClassInfo), "Initialization")]
    public class When_creating_a_PersistentClassInfo :With_Isolations
    {


        static PersistentClassInfo _persistentClassInfo;

        static IFrameCreationHandler _frameCreationHandler;
        static CodeTemplate _codeTemplate;

        Establish context = () =>
        {

            var artifactHandler = new TestAppLication<PersistentClassInfo>().Setup(null, info => {
                _persistentClassInfo = info;
                info.PersistentAssemblyInfo = new PersistentAssemblyInfo(info.Session);
            });
            _frameCreationHandler = artifactHandler.WithArtiFacts(WCArtifacts).CreateDetailView();
        };

        Because of = () => _frameCreationHandler.CreateFrame().RaiseControlsCreated();


        It should_create_a_default_classInfo_template_if_not_exists = () =>
        {
            _codeTemplate = _persistentClassInfo.CodeTemplateInfo.CodeTemplate;
            _codeTemplate.ShouldNotBeNull();
            _codeTemplate.Session.IsNewObject(_codeTemplate).ShouldBeTrue();
            _codeTemplate.TemplateType.ShouldEqual(TemplateType.Class);
        };

        It should_set_template_as_default = () => _codeTemplate.IsDefault.ShouldBeTrue();
    }

}