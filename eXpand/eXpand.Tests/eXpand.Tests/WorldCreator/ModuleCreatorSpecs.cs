using System;
using System.Collections.Generic;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;
using System.Linq;

namespace eXpand.Tests.WorldCreator
{
    [Subject(typeof(AssemblyRouter))]
    public class When_Having_2_ClassInfos_With_2_assemblies:With_In_Memory_DataStore {
        static XPCollection<PersistentClassInfo> _col;

        static List<List<IPersistentClassInfo>> persistentClassInfos;

        Establish context = () => {

            var classInfo1 = new PersistentClassInfo(Session.DefaultSession) { Name = "C1" };
            Isolate.WhenCalled(() => classInfo1.AssemblyName).WillReturn("A1");
            classInfo1.Save();
            var classInfo2 = new PersistentClassInfo(Session.DefaultSession) { Name = "C2" };
            Isolate.WhenCalled(() => classInfo2.AssemblyName).WillReturn("A2");
            classInfo2.Save();
            _col = new XPCollection<PersistentClassInfo>(Session.DefaultSession);
        };

        Because of = () => {        
            persistentClassInfos= AssemblyRouter.GetLists(_col.Cast<IPersistentClassInfo>().ToList());
        };

        It should_return_2_lists_one_for_each_assembly = () => persistentClassInfos.Count.ShouldEqual(2);
    }

    [Subject(typeof(ModuleCreator))]
    public class When_Creating_Dynamic_Module:With_Type_Builder
    {
        static XPCollection<PersistentClassInfo> _col;

        static List<Type> _types;

        Establish context = () => {
            new PersistentClassInfo(Session.DefaultSession){Name = "TestClass"}.Save();
            _col = new XPCollection<PersistentClassInfo>(Session.DefaultSession);
        };

        Because of = () => { _types = new ModuleCreator().DefineModule(AssemblyRouter.GetLists(_col.Cast<IPersistentClassInfo>().ToList())); };

        It should_return_a_list_of_module_base_descenants = () =>_types.Count.ShouldEqual(1) ;
    }

}
