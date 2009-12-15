using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using Machine.Specifications;

namespace eXpand.Tests.eXpand.WorldCreator {
    
    [Subject(typeof(PersistentMemberInfo), "Initialization")]
    public class When_creating_a_PersistentMemberInfo_and_default_template_exists:With_Isolations {
        static IFrameCreationHandler _frameCreationHandler;
        static PersistentCoreTypeMemberInfo _persistentCoreTypeMemberInfo;

        static CodeTemplate _codeTemplate;

        Establish context = () => {
            var artifactHandler = new TestAppLication<PersistentCoreTypeMemberInfo>().Setup(null,info => {
                _persistentCoreTypeMemberInfo=info;
                info.Owner=new PersistentClassInfo(info.Session){PersistentAssemblyInfo = new PersistentAssemblyInfo(info.Session)};
            });
            _frameCreationHandler = artifactHandler.WithArtiFacts(() => new[]{typeof(WorldCreatorModule)}).CreateDetailView();
            _codeTemplate = new CodeTemplate(new Session(artifactHandler.UnitOfWork.DataLayer)) { IsDefault = true, TemplateType = TemplateType.ReadWriteMember };
            _codeTemplate.Save();            
        };

        Because of = () => _frameCreationHandler.CreateFrame().RaiseControlsCreated();

        It should_assign_that_template_to_classInfo = () => _persistentCoreTypeMemberInfo.CodeTemplateInfo.CodeTemplate.ShouldEqual(_codeTemplate);
    }
    [Subject(typeof(PersistentMemberInfo), "Initialization")]
    public class When_creating_a_read_write_PersistentMemberInfo:With_Isolations
    {
        static PersistentMemberInfo _persistentMemberInfo;

        static IFrameCreationHandler _frameCreationHandler;
        static CodeTemplate _codeTemplate;

        Establish context = () => {
            _frameCreationHandler = new TestAppLication<PersistentCoreTypeMemberInfo>().Setup(null,info => {
                _persistentMemberInfo = info;
                info.Owner = new PersistentClassInfo(info.Session) { PersistentAssemblyInfo = new PersistentAssemblyInfo(info.Session) };
            }).WithArtiFacts(() => new[]{typeof(WorldCreatorModule)}).CreateDetailView();
        };

        Because of = () => _frameCreationHandler.CreateFrame().RaiseControlsCreated();

        It should_create_a_default_classInfo_template_if_not_exists = () =>
        {
            _codeTemplate = _persistentMemberInfo.CodeTemplateInfo.CodeTemplate;
            _codeTemplate.ShouldNotBeNull();
            _codeTemplate.IsDefault.ShouldBeTrue();    
            _codeTemplate.TemplateType.ShouldEqual(TemplateType.ReadWriteMember);
        };
    }
    [Subject(typeof(PersistentMemberInfo), "Initialization")]
    public class When_initializing_a_read_only_PersistentMemberInfo:With_Isolations
    {
        static IFrameCreationHandler _frameCreationHandler;
        static PersistentMemberInfo _persistentMemberInfo;
        static CodeTemplate _codeTemplate;

        Establish context = () =>
        {
            _frameCreationHandler = new TestAppLication<PersistentCollectionMemberInfo>().Setup(null, info =>
            {
                _persistentMemberInfo = info;
                info.Owner = new PersistentClassInfo(info.Session) { PersistentAssemblyInfo = new PersistentAssemblyInfo(info.Session) };
            }).WithArtiFacts(() => new[] { typeof(WorldCreatorModule) }).CreateDetailView();
        };


        Because of = () => _frameCreationHandler.CreateFrame().RaiseControlsCreated();

        It should_create_a_default_classInfo_template_if_not_exists = () =>
        {
            _codeTemplate = _persistentMemberInfo.CodeTemplateInfo.CodeTemplate;
            _codeTemplate.ShouldNotBeNull();
            _codeTemplate.IsDefault.ShouldBeTrue();    
            _codeTemplate.TemplateType.ShouldEqual(TemplateType.ReadOnlyMember);
        };
    }
//    [Subject(typeof(PersistentMemberInfo))]
//    public class When_saving_a_memberinfo_associated_with_classInfo:With_In_Memory_DataStore {
//        
//        static ControllerFactory<GenerateCodeController, PersistentCoreTypeMemberInfo> _controllerFactory;
//
//        Establish context = () => {
//            _controllerFactory = new ControllerFactory<GenerateCodeController, PersistentCoreTypeMemberInfo>();
//            _controllerFactory.CreateAndActivate();
//            GenerateCodeController generateCodeController = _controllerFactory.Controller;
//            var persistentCoreTypeMemberInfo = ((PersistentCoreTypeMemberInfo)generateCodeController.View.CurrentObject);
//            persistentCoreTypeMemberInfo.CodeTemplateInfo.TemplateInfo.TemplateCode = "TemplateCode";
//            var persistentClassInfo = new PersistentClassInfo(_controllerFactory.UnitOfWork) {
//                                                                                                 CodeTemplateInfo = {TemplateInfo = {TemplateCode = "ClassTemplateCode"}},
//                                                                                                 PersistentAssemblyInfo =
//                                                                                                     new PersistentAssemblyInfo(_controllerFactory.UnitOfWork)
//                                                                                             };
//            persistentCoreTypeMemberInfo.Owner = persistentClassInfo;
//        };
//
//        Because of = () => _controllerFactory.UnitOfWork.CommitChanges();
//
//        It should_generate_code_for_the_memberInfo =
//            () => _controllerFactory.CurrentObject.CodeTemplateInfo.GeneratedCode.ShouldStartWith("TemplateCode");
//        It should_generate_associate_classinfo_generatedcode =
//            () => _controllerFactory.CurrentObject.Owner.CodeTemplateInfo.GeneratedCode.ShouldStartWith("ClassTemplateCode"); 
//    }
}