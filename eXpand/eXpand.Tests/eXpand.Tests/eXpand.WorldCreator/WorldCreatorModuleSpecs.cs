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
using TypesInfo = eXpand.ExpressApp.WorldCreator.Core.TypesInfo;

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
            _persistentAssemblyInfo = new PersistentAssemblyInfo(Session.DefaultSession);
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession){
                Name = "TestClass",
                PersistentAssemblyInfo = _persistentAssemblyInfo
            };
            persistentClassInfo.Save();
            var compilerResults = Isolate.Fake.Instance<CompilerResults>(Members.CallOriginal,ConstructorWillBe.Called);
            Isolate.Swap.NextInstance<CompilerResults>().With(compilerResults);
            Isolate.WhenCalled(() => compilerResults.CompiledAssembly).WillThrow(new Exception("test"));
            Isolate.WhenCalled(() => compilerResults.Errors).WillReturn(new CompilerErrorCollection(new[]{new CompilerError("",2,2,"","test"), }));
            Isolate.WhenCalled(() => CodeEngine.GenerateCode(_persistentAssemblyInfo)).WillReturn(null);
        };

        Because of = () => _worldCreatorModule.CreateDynamicTypes(_applicationModulesManager, new UnitOfWork(Session.DefaultSession.DataLayer),typeof(PersistentAssemblyInfo) );


        It should_save_errors_in_persistent_assembly_info = () => {
            _persistentAssemblyInfo.Reload();
            _persistentAssemblyInfo.CompileErrors.IndexOf("test").ShouldBeGreaterThan(-1); };
    }
    [Subject(typeof(WorldCreatorModule))][Isolated]
    public class When_Settingup_Module:With_In_Memory_DataStore
    {
        static bool existentCreated;
        static WorldCreatorModule _worldCreatorModule;
        static ApplicationModulesManager applicationModulesManager;

//        static bool loadDiffs = false;

        Establish context = () => {
            var existentTypesMemberCreator = Isolate.Fake.Instance<ExistentTypesMemberCreator>();
            Isolate.Swap.NextInstance<ExistentTypesMemberCreator>().With(existentTypesMemberCreator);
            Session defaultSession = Session.DefaultSession;
            Isolate.WhenCalled(() => existentTypesMemberCreator.CreateMembers(defaultSession, null)).DoInstead(callContext => {existentCreated = true;});
            var typesInfo = Isolate.Fake.Instance<TypesInfo>();
            Isolate.Swap.NextInstance<TypesInfo>().With(typesInfo);
            Isolate.WhenCalled(() => typesInfo.PersistentAssemblyInfoType).WillReturn(typeof(PersistentAssemblyInfo));
            var persistentAssemblyInfo = new PersistentAssemblyInfo(defaultSession){Name = "TestAssembly"};
            var persistentClassInfo = new PersistentClassInfo(defaultSession) {
                                                                                  Name = "TestClass",
                                                                                  PersistentAssemblyInfo = persistentAssemblyInfo
                                                                              };
            
            persistentClassInfo.Save();
            Isolate.WhenCalled(() => CodeEngine.GenerateCode(persistentAssemblyInfo)).WillReturn(null);
            _worldCreatorModule = new WorldCreatorModule();
            Isolate.WhenCalled(() => _worldCreatorModule.GetAdditionalClasses()).WillReturn(new List<Type>{typeof(IPersistentClassInfo)});
            applicationModulesManager = new ApplicationModulesManager();
//            Isolate.WhenCalled((bool loadDiifs) => applicationModulesManager.AddModule(typeof(int), loadDiifs)).AndArgumentsMatch(b => true).CallOriginal();
            _worldCreatorModule.Application = Isolate.Fake.Instance<XafApplication>(Members.CallOriginal);
        };

        Because of = () => _worldCreatorModule.Setup(applicationModulesManager);

        It should_contain_dynamic_modules_within_Application_Modules_collection =
            () => applicationModulesManager.Modules.Count.ShouldEqual(2);

        It should_not_load_modelDiffs_from_dynamic_assemblies; //= () => loadDiffs.ShouldBeTrue();

        It should_create_Existent_Classes_Member = () => existentCreated.ShouldEqual(true);


//        It should_update_dictionary_schema = () => {
//            throw new NotImplementedException();
//            Type orDefault =applicationModulesManager.Modules[1].GetType().Assembly.GetTypes().Where(type => type.FullName.EndsWith("TestClass")).FirstOrDefault();
//            Activator.CreateInstance(orDefault, Session.DefaultSession);
//        };
        
    }



    [Subject(typeof(WorldCreatorModule))]
    [Isolated]
    public class When_Updating_Model : with_TypesInfo
    {
        static WorldCreatorModule wcModule;

        static Dictionary dictionary;

        Establish context = () => {

            wcModule = new WorldCreatorModule
            {
                                        Application = Isolate.Fake.Instance<XafApplication>(Members.CallOriginal)
                                    };
            Isolate.WhenCalled(() => wcModule.TypesInfo).WillReturn(typesInfo);
            Isolate.WhenCalled(() => wcModule.DefinedModules).WillReturn(new List<Type> { typeof(ExtendedCoreTypeMemberInfo) });
            dictionary =ApplicationNodeWrapperExtensions.Create(new[] {
                                                                  typeof (ExtendedCoreTypeMemberInfo),
                                                                  typeof (ExtendedReferenceMemberInfo),
                                                                  typeof (ExtendedCollectionMemberInfo)
                                                              }).Dictionary;

            AssemblyResourceImageSource.CreateAssemblyResourceImageSourceNode(dictionary.RootNode.GetChildNode("ImageSources"), new AssemblyName(typeof(ExtendedCoreTypeMemberInfo).Assembly.FullName+"").Name);
        };

        Because of = () => wcModule.UpdateModel(dictionary);

        It should_display_owner_column_to_extended_members_list_view =
            () =>
            new ApplicationNodeWrapper(dictionary).Views.GetListViews(typeof (ExtendedCoreTypeMemberInfo))[0].Columns.
                FindColumnInfo("Owner").VisibleIndex.ShouldBeGreaterThan(-1);

        It should_remove_dynamic_assemblies_from_Assemblies_image_Node =
            () => dictionary.RootNode.GetChildNode("ImageSources").ChildNodes.Count.ShouldEqual(0);
    }
    [Subject(typeof(WorldCreatorModule))]
    [Isolated]
    public class When_Application_Setup_Is_Completed:with_TypesInfo {
        static XafApplication xafApplication;

        static bool merged;

        Establish context = () => {
            Isolate.WhenCalled(() => typesInfo.PersistentAssemblyInfoType).WillReturn(typeof(PersistentAssemblyInfo));
            xafApplication = Isolate.Fake.Instance<XafApplication>(Members.CallOriginal);
            var wcModule = new WorldCreatorModule { Application = xafApplication };
            Isolate.WhenCalled(() => CodeEngine.GenerateCode(Isolate.Fake.Instance<IPersistentAssemblyInfo>())).WillReturn(null);
            wcModule.Setup(new ApplicationModulesManager());
            var xpoObjectMerger = Isolate.Fake.Instance<XpoObjectMerger>();
            Isolate.Swap.NextInstance<XpoObjectMerger>().With(xpoObjectMerger);
            Isolate.WhenCalled(() => xpoObjectMerger.MergeTypes(null, null, null, null)).DoInstead(callContext => merged= true);
        };


        Because of = () => Isolate.Invoke.Event(()=>xafApplication.SetupComplete+= null, null,new EventArgs());

        
        It should_merge_any_mergable_dynamic_type = () => merged.ShouldEqual(true);

    }

}
