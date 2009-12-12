using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.WorldCreator;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;
using System.Linq;

namespace eXpand.Tests.eXpand.WorldCreator
{
    [Subject(typeof(WorldCreatorModule))]
    [Isolated]
    public class When_dynamic_module_cannot_be_created:With_In_Memory_DataStore {
        static WorldCreatorModule _worldCreatorModule;

        static ApplicationModulesManager _applicationModulesManager;

        static PersistentAssemblyInfo _persistentAssemblyInfo;

        Establish context = () => {
            _worldCreatorModule = new WorldCreatorModule();
            _applicationModulesManager = new ApplicationModulesManager();
            _persistentAssemblyInfo = new PersistentAssemblyInfo(UnitOfWork){Name = "TestAssembly"};
            new PersistentClassInfo(UnitOfWork){
                                                   Name = "TestClass",
                                                   PersistentAssemblyInfo = _persistentAssemblyInfo
                                               };
            UnitOfWork.CommitChanges();
            var compilerResults = Isolate.Fake.Instance<CompilerResults>(Members.CallOriginal,ConstructorWillBe.Called);
            Isolate.Swap.NextInstance<CompilerResults>().With(compilerResults);
            Isolate.WhenCalled(() => compilerResults.CompiledAssembly).WillThrow(new Exception("test"));
            Isolate.WhenCalled(() => compilerResults.Errors).WillReturn(new CompilerErrorCollection(new[]{new CompilerError("",2,2,"","test"), }));
            Isolate.WhenCalled(() => CodeEngine.GenerateCode(_persistentAssemblyInfo)).WillReturn(null);
        };

        Because of = () => _worldCreatorModule.AddDynamicModules(_applicationModulesManager, UnitOfWork,typeof(PersistentAssemblyInfo) );


        It should_save_errors_in_persistent_assembly_info = () => {
            _persistentAssemblyInfo.Reload();
            _persistentAssemblyInfo.CompileErrors.IndexOf("test").ShouldBeGreaterThan(-1); };

        It should_load_previous__assembly_version_if_exists;
    }
    [Subject(typeof(WorldCreatorModule))][Isolated]
    public class When_Settingup_Module:With_In_Memory_DataStore
    {
        static bool _modulesAdded;
        static bool existentCreated;
        static WorldCreatorModule _worldCreatorModule;
        static ApplicationModulesManager applicationModulesManager;


        Establish context = () => {
            var existentTypesMemberCreator = Isolate.Fake.Instance<ExistentTypesMemberCreator>();
            Isolate.Swap.NextInstance<ExistentTypesMemberCreator>().With(existentTypesMemberCreator);
            Session defaultSession = UnitOfWork;
            Isolate.WhenCalled(() => existentTypesMemberCreator.CreateMembers(defaultSession, null)).DoInstead(callContext => {existentCreated = true;});
            
            var persistentAssemblyInfo = new PersistentAssemblyInfo(defaultSession){Name = "TestAssembly"};
            new PersistentClassInfo(defaultSession) {
                                                        Name = "TestClass",
                                                        PersistentAssemblyInfo = persistentAssemblyInfo
                                                    };
            
            UnitOfWork.CommitChanges();

            Isolate.WhenCalled(() => CodeEngine.GenerateCode(persistentAssemblyInfo)).WillReturn(null);
            _worldCreatorModule = new WorldCreatorModule();
            Isolate.WhenCalled(() => _worldCreatorModule.AddDynamicModules(null, null, null)).DoInstead(methodCallContext => _modulesAdded=true);
            Isolate.WhenCalled(() => _worldCreatorModule.GetAdditionalClasses()).WillReturn(new List<Type>{typeof(IPersistentClassInfo)});
            applicationModulesManager = new ApplicationModulesManager();
            _worldCreatorModule.Application = Isolate.Fake.Instance<XafApplication>(Members.CallOriginal);
        };

        Because of = () => _worldCreatorModule.Setup(applicationModulesManager);

        It should_contain_dynamic_modules_within_Application_Modules_collection =
            () => _modulesAdded.ShouldBeTrue();

        

        It should_create_Existent_Classes_Member = () => existentCreated.ShouldBeTrue();
    }


    [Subject(typeof(WorldCreatorModule))]
    [Isolated]
    public class When_Updating_Model : With_Isolations
    {
        static WorldCreatorModule wcModule;

        static Dictionary dictionary;

        Establish context = () => {

            wcModule = new WorldCreatorModule
            {
                                        Application = Isolate.Fake.Instance<XafApplication>(Members.CallOriginal)
                                    };
            Isolate.WhenCalled(() => wcModule.DefinedModules).WillReturn(new List<Type> { typeof(ExtendedCoreTypeMemberInfo) });

            var types = TypesInfo.Instance.GetType().GetProperties().Where(propertyInfo => propertyInfo.PropertyType==typeof(Type)).Select(info =>((Type)info.GetValue(TypesInfo.Instance,null)));
            dictionary = ApplicationNodeWrapperExtensions.Create(types.ToArray()).Dictionary;

            AssemblyResourceImageSource.CreateAssemblyResourceImageSourceNode(dictionary.RootNode.GetChildNode("ImageSources"), new AssemblyName(typeof(ExtendedCoreTypeMemberInfo).Assembly.FullName+"").Name);
        };

        Because of = () => wcModule.UpdateModel(dictionary);

        It should_display_owner_column_to_extended_members_list_view =
            () =>
            new ApplicationNodeWrapper(dictionary).Views.GetListViews(typeof (ExtendedCoreTypeMemberInfo))[0].Columns.
                FindColumnInfo("Owner").VisibleIndex.ShouldBeGreaterThan(-1);

        It should_remove_dynamic_assemblies_from_Assemblies_image_Node =
            () => dictionary.RootNode.GetChildNode("ImageSources").ChildNodes.Count.ShouldEqual(0);

        It should_enable_cloning_for_all_persistent_classes =
            () =>
            new ApplicationNodeWrapper(dictionary).BOModel.FindClassByType(typeof (PersistentClassInfo)).Node.
                GetAttributeBoolValue("IsClonable").ShouldBeTrue(); 
    }
    [Subject(typeof(WorldCreatorModule))]
    [Isolated]
    public class When_Application_Setup_Is_Completed:With_Isolations {
        static XafApplication xafApplication;

        static bool merged;

        Establish context = () => {
            xafApplication = Isolate.Fake.Instance<XafApplication>(Members.CallOriginal);
            var wcModule = new WorldCreatorModule { Application = xafApplication };
            Isolate.WhenCalled(() => CodeEngine.GenerateCode(Isolate.Fake.Instance<IPersistentAssemblyInfo>())).WillReturn(null);
            wcModule.Setup(new ApplicationModulesManager());
            var xpoObjectMerger = Isolate.Fake.Instance<XpoObjectMerger>();
            Isolate.Swap.NextInstance<XpoObjectMerger>().With(xpoObjectMerger);
            Isolate.WhenCalled(() => xpoObjectMerger.MergeTypes(null, null, null)).DoInstead(callContext => merged= true);
        };


        Because of = () => Isolate.Invoke.Event(()=>xafApplication.SetupComplete+= null, null,new EventArgs());

        
        It should_merge_any_mergable_dynamic_type = () => merged.ShouldEqual(true);

    }

}
