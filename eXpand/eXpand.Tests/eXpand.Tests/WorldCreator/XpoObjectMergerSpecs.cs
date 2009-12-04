using System;
using System.Collections.Generic;
using System.Data;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using Machine.Specifications;
using TypeMock;
using TypeMock.ArrangeActAssert;
using TypeMock.Extensions;
using eXpand.Utils.Helpers;

namespace eXpand.Tests.WorldCreator
{
    [Subject(typeof(XpoObjectMerger))][Isolated]
    public class When_Merging_Dynamic_Types:With_In_Memory_DataStore {
        static XpoObjectMerger xpoObjectMerger;

        static List<Type> types;

        static IDbCommand dbCommand;

        static bool execCommand;

        static UnitOfWork unitOfWork;

        static int updateSchemaCalls;

        Establish context = () => {
            xpoObjectMerger = new XpoObjectMerger();
            var swapAll = Isolate.Fake.InstanceAndSwapAll<PersistentClassInfo>(Members.CallOriginal,ConstructorWillBe.Called,Session.DefaultSession);
            Isolate.WhenCalled(() => swapAll.AssemblyName).WillReturn(swapAll.AssemblyName+"Merging");
            var parentClassInfo = new PersistentClassInfo(Session.DefaultSession){Name = "Parent"};
            parentClassInfo.Save();
            
            
            ITypeDefineBuilder typeDefineBuilder = PersistentClassInfoTypeBuilder.BuildClass().WithAssemblyName(parentClassInfo.AssemblyName);
            types = new List<Type>();
            Type parentType = typeDefineBuilder.Define(parentClassInfo);
            types.Add(parentType);
            var childClassInfo = new PersistentClassInfo(Session.DefaultSession)
                                      {Name = "Child", BaseType = parentType, MergedObjectType = parentType};
            childClassInfo.Save();
            Type childType = typeDefineBuilder.Define(childClassInfo);
            types.Add(childType);
            dbCommand = Isolate.Fake.Instance<IDbCommand>();
            
            Isolate.WhenCalled(() => dbCommand.ExecuteNonQuery()).DoInstead(callContext => {
                execCommand = true;
                return 0;   
            });
            unitOfWork = new UnitOfWork(Session.DefaultSession.DataLayer);
            Isolate.WhenCalled(() => unitOfWork.UpdateSchema()).CallOriginal();
        };

        Because of = () => xpoObjectMerger.MergeTypes(unitOfWork,typeof(PersistentClassInfo),types ,dbCommand);

        It should_updateXpdictionary_Schema_for_each_type = () => MockManager.CalledCounter<UnitOfWork>(unitOfWork.GetMemberInfo(x=>x.UpdateSchema()).Name).ShouldEqual(3);

        It should_call_an_update_sql_statement = () => {
            execCommand.ShouldEqual(true);
            dbCommand.CommandText.ShouldEqual("UPDATE Parent SET ObjectType=2 WHERE ObjectType IS NULL OR ObjectType=3");
        };

        It should_create_an_ObjectType_Column_to_Parent_if_parent_has_no_records =
            () => Session.DefaultSession.GetClassInfo(types[1]).FindMember("ObjectType").ShouldNotBeNull();
    }

}
