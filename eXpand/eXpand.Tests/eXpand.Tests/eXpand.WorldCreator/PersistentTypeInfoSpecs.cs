using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.SystemModule;
using eXpand.ExpressApp.WorldCreator;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;
using eXpand.Xpo;

namespace eXpand.Tests.eXpand.WorldCreator {
    [Subject(typeof(PersistentTypeInfo))]
    public class When_peristent_type_code_template_change:With_In_Memory_DataStore {
        static CodeTemplateInfo _codeTemplateInfo;

        static CodeTemplate _codeTemplate;

        Establish context = () => {

            var factory = new ControllerFactory<CodeInfoCodeTemplateModifierController,CodeTemplateInfo>();
            var typeInfoCodeTemplateModifierController = factory.CreateAndActivate();
            var worldCreatorModule = new WorldCreatorModule();
            Isolate.WhenCalled(() => worldCreatorModule.TypesInfo.TemplateInfoType).WillReturn(typeof(TemplateInfo));
            Isolate.WhenCalled(() => typeInfoCodeTemplateModifierController.Application.Modules.FindModule(typeof(void))).WillReturn(worldCreatorModule);
            _codeTemplateInfo =  factory.CurrentObject;
            _codeTemplate = new CodeTemplate(_codeTemplateInfo.Session){TemplateCode ="TemplateCode" };
        };
        
        Because of = () => { _codeTemplateInfo.CodeTemplate=_codeTemplate;};

        It should_delegate_all_props_from_code_template_to_persistent_type_templateinfo_object=() => _codeTemplateInfo.TemplateInfo.TemplateCode.ShouldEqual("TemplateCode");
    }
    [Subject(typeof(PersistentTypeInfo),"Creation")]
    public class When_A_persistentTypeInfo_object_is_creating:With_In_Memory_DataStore {
        static TrackObjectConstructionsViewController _trackObjectConstructionsViewController;

        static PersistentTypeInfo _persistentTypeInfo;

        Establish context = () => {
            
            var viewControllerFactory = new ControllerFactory<InitPersistentTypeInfoViewController,PersistentCoreTypeMemberInfo>();
            var initPersistentTypeInfoViewController = viewControllerFactory.Create(ViewType.ListView);
            _persistentTypeInfo = viewControllerFactory.CurrentObject;
            Isolate.WhenCalled(() => _persistentTypeInfo.Init(null)).IgnoreCall();
            var worldCreatorModule = new WorldCreatorModule();
            Isolate.WhenCalled(() => worldCreatorModule.TypesInfo.PersistentTypesInfoType).WillReturn(typeof(PersistentClassInfo));
            Isolate.WhenCalled(() => initPersistentTypeInfoViewController.Application.Modules.FindModule(typeof(WorldCreatorModule))).WillReturn(worldCreatorModule);
            var frame = initPersistentTypeInfoViewController.Frame;
            _trackObjectConstructionsViewController = new TrackObjectConstructionsViewController();
            Isolate.WhenCalled(() => frame.GetController<TrackObjectConstructionsViewController>()).WillReturn(_trackObjectConstructionsViewController);
            viewControllerFactory.Activate();
        };

        Because of = () => Isolate.Invoke.Event(() => _trackObjectConstructionsViewController.ObjectCreated += null, null, new ObjectCreatedEventArgs(_persistentTypeInfo));

        It should_initialize_its_template = () => Isolate.Verify.WasCalledWithAnyArguments(() => _persistentTypeInfo.Init(null));
    }
    [Subject(typeof(PersistentTypeInfo))]
    public class When_Saving_PersistentTypes : With_In_Memory_DataStore
    {
        static PersistentReferenceMemberInfo _referenceMemberInfo;
        static GenerateCodeController _generateCodeController;

        Establish context = () => {
            var controllerFactory = new ControllerFactory<GenerateCodeController, PersistentReferenceMemberInfo>();
            _generateCodeController = controllerFactory.CreateAndActivate();
            _referenceMemberInfo=(PersistentReferenceMemberInfo) _generateCodeController.View.CurrentObject;
            var codeTemplate = new CodeTemplate(_referenceMemberInfo.Session){TemplateType = TemplateType.ReadWriteMember};
            codeTemplate.SetDefaults();
            _referenceMemberInfo.CodeTemplateInfo.TemplateInfo=codeTemplate;
            _referenceMemberInfo.ReferenceType = typeof(User) ;
        };

        Because of = () => _generateCodeController.View.ObjectSpace.CommitChanges();

        It should_persist_the_referenceType =
            () =>
            ((PersistentReferenceMemberInfo)new Session().GetObject(_referenceMemberInfo)).ReferenceType.ShouldEqual(
                typeof(User));

        It should_generate_the_class_code = () => _referenceMemberInfo.CodeTemplateInfo.GeneratedCode.ShouldNotBeNull();
    }

    [Subject(typeof(PersistentTypeInfo))]
    public class When_Deleting_PersistentTypes:With_In_Memory_DataStore {
        static PersistentClassInfo _persistentClassInfo;

        Establish context = () => {
            _persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { CodeTemplateInfo = { TemplateInfo = new TemplateInfo(Session.DefaultSession) } };
            _persistentClassInfo.Save();
        };
        Because of = () => _persistentClassInfo.Delete();
        It should_delete_CodeTemplateInfo_as_well=() => _persistentClassInfo.CodeTemplateInfo.IsDeleted.ShouldBeTrue();
        It should_delete_TemplateInfo_as_well=() => _persistentClassInfo.CodeTemplateInfo.TemplateInfo.IsDeleted.ShouldBeTrue();
    }

    internal class MyClass {
        
    }
    public class MyClass1:BaseObject {

        private MyClass2 _myClass2;
        [Aggregated]
        public MyClass2 MyClass2
        {
            get
            {
                return _myClass2;
            }
            set
            {
                SetPropertyValue("MyClass2", ref _myClass2, value);
            }
        }
    }

    public class MyClass2:BaseObject {
        
    }
}