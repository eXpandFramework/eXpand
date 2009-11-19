using System;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.WorldCreator;
using eXpand.ExpressApp.WorldCreator.ClassTypeBuilder;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.WorldCreator
{
    [Subject(typeof(WorldCreatorModule))][Isolated]
    public class When_Settingup_Module:With_In_Memory_DataStore
    {
        static bool existentCreated;
        static ApplicationModulesManager applicationModulesManager;

//        static bool loadDiffs = false;

        Establish context = () => {
            var existentTypesMemberCreator = Isolate.Fake.Instance<ExistentTypesMemberCreator>();
            Isolate.Swap.NextInstance<ExistentTypesMemberCreator>().With(existentTypesMemberCreator);
            Session defaultSession = Session.DefaultSession;
            Isolate.WhenCalled(() => existentTypesMemberCreator.CreateMembers(defaultSession, null)).DoInstead(callContext => {existentCreated = true;});
            var typesInfo = Isolate.Fake.Instance<TypesInfo>();
            Isolate.Swap.NextInstance<TypesInfo>().With(typesInfo);
            Isolate.WhenCalled(() => typesInfo.PersistentTypesInfoType).WillReturn(typeof(PersistentClassInfo));
            var persistentClassInfo = new PersistentClassInfo(defaultSession){Name = "TestClass"};
            persistentClassInfo.Save();
            var wcModule = new WorldCreatorModule();
            Isolate.WhenCalled(() => wcModule.GetAdditionalClasses()).WillReturn(new List<Type>{typeof(IPersistentClassInfo)});
            applicationModulesManager = new ApplicationModulesManager();
//            Isolate.WhenCalled((bool loadDiifs) => applicationModulesManager.AddModule(null, loadDiifs)).AndArgumentsMatch(b => true).CallOriginal();
            wcModule.Application = Isolate.Fake.Instance<XafApplication>(Members.CallOriginal);
            wcModule.Setup(applicationModulesManager);

        };

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

            xafApplication = Isolate.Fake.Instance<XafApplication>(Members.CallOriginal);
            var wcModule = new WorldCreatorModule { Application = xafApplication };
            wcModule.Setup(new ApplicationModulesManager());
            var xpoObjectMerger = Isolate.Fake.Instance<XpoObjectMerger>();
            Isolate.Swap.NextInstance<XpoObjectMerger>().With(xpoObjectMerger);
            Isolate.WhenCalled(() => xpoObjectMerger.MergeTypes(null, null, null, null)).DoInstead(callContext => merged= true);
        };


        Because of = () => Isolate.Invoke.Event(()=>xafApplication.SetupComplete+= null, null,new EventArgs());

        
        It should_merge_any_mergable_dynamic_type = () => merged.ShouldEqual(true);

    }

}
