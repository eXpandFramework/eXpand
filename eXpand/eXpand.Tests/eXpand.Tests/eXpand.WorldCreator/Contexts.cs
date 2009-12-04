using System;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using eXpand.Xpo;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.WorldCreator
{
    public abstract class with_classInfo_with_interfaceInfos<InterfaceType> : With_In_Memory_DataStore
    {
        protected static PersistentClassInfo _persistentClassInfo;

        protected static TypesInfo _typesInfo;

        Establish context = () =>
        {
            _persistentClassInfo = new PersistentClassInfo(Session.DefaultSession);
            var interfaceInfos = _persistentClassInfo.Interfaces;
            var interfaceInfo = new InterfaceInfo(Session.DefaultSession);
            Isolate.WhenCalled(() => interfaceInfo.Type).WillReturn(typeof(InterfaceType));
            interfaceInfos.Add(interfaceInfo);
            _typesInfo = Isolate.Fake.Instance<TypesInfo>();
            Isolate.WhenCalled(() => _typesInfo.PersistentCoreTypeInfoType).WillReturn(typeof(PersistentCoreTypeMemberInfo));
            Isolate.WhenCalled(() => _typesInfo.PersistentReferenceInfoType).WillReturn(typeof(PersistentReferenceMemberInfo));
            Isolate.WhenCalled(() => _typesInfo.CodeTemplateType).WillReturn(typeof(CodeTemplate));
        };

    }
    public abstract class with_TypesInfo
    {
        Establish context = () =>
        {
            Isolate.CleanUp();
            typesInfo = Isolate.Fake.Instance<TypesInfo>();
            Isolate.Swap.NextInstance<TypesInfo>().With(typesInfo);
            Isolate.WhenCalled(() => typesInfo.ExtendedCollectionMemberInfoType).WillReturn(typeof(ExtendedCollectionMemberInfo));
            Isolate.WhenCalled(() => typesInfo.ExtendedCoreMemberInfoType).WillReturn(typeof(ExtendedCoreTypeMemberInfo));
            Isolate.WhenCalled(() => typesInfo.ExtendedReferenceMemberInfoType).WillReturn(typeof(ExtendedReferenceMemberInfo));
            Isolate.WhenCalled(() => typesInfo.IntefaceInfoType).WillReturn(typeof(InterfaceInfo));
            Isolate.WhenCalled(() => typesInfo.PersistentTypesInfoType).WillReturn(typeof(PersistentClassInfo));
        };

        protected static TypesInfo typesInfo;
    }

    public abstract class With_Types : With_In_Memory_DataStore
    {
        protected static TypesInfo TypesInfo;

        Establish context = () =>
        {
            TypesInfo = Isolate.Fake.Instance<TypesInfo>();
            Isolate.WhenCalled(() => TypesInfo.ExtendedCoreMemberInfoType).WillReturn(typeof(ExtendedCoreTypeMemberInfo));
            Isolate.WhenCalled(() => TypesInfo.ExtendedCoreMemberInfoType).WillReturn(typeof(ExtendedCoreTypeMemberInfo));
            Isolate.WhenCalled(() => TypesInfo.ExtendedReferenceMemberInfoType).WillReturn(typeof(ExtendedReferenceMemberInfo));
            Isolate.WhenCalled(() => TypesInfo.ExtendedCollectionMemberInfoType).WillReturn(typeof(ExtendedCollectionMemberInfo));
        };
    }

    public abstract class With_DynamicCore_Property 
    {
        protected static Type Type;
        protected static PropertyInfo PropertyInfo;
        protected static PersistentClassInfo ClassInfo;
        Establish context = () =>
        {
            ClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClass" };
            ClassInfo.OwnMembers.Add(new PersistentCoreTypeMemberInfo(Session.DefaultSession) { Name = "TestProperty", DataType = XPODataType.Boolean });
//            Type = TypeDefineBuilder.Define(ClassInfo);
            PropertyInfo = Type.GetProperty("TestProperty");
        };

        
    }

    public abstract class With_In_Memory_DataStore {
        Establish context = () => {
            Isolate.CleanUp();
            Session.DefaultSession.Disconnect();
            ReflectionHelper.Reset();
            XafTypesInfo.Reset(true);
            var dataStore = new InMemoryDataStore( AutoCreateOption.DatabaseAndSchema);
            XpoDefault.DataLayer = new SimpleDataLayer(XafTypesInfo.XpoTypeInfoSource.XPDictionary, dataStore);
        };    
    }
//    public abstract class With_Type_Builder:With_In_Memory_DataStore
//    {
//        protected static ITypeDefineBuilder TypeDefineBuilder;
//        Establish context = () => {
//            TypeDefineBuilder = PersistentClassInfoTypeBuilder.BuildClass().WithAssemblyName("TestAssembly" );
//        };
//    }
    public abstract class With_DynamicReference_Property 
    {
        protected static Type Type;

        protected static PropertyInfo PropertyInfo;

        Establish context = () =>
        {
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClass" };
            persistentClassInfo.OwnMembers.Add(new PersistentReferenceMemberInfo(Session.DefaultSession) { Name = "TestProperty", ReferenceType = typeof(User) });

//            Type = TypeDefineBuilder.Define(persistentClassInfo);
            PropertyInfo = Type.GetProperty("TestProperty");
        };
    }
    public abstract class With_DynamicCollection_Property 
    {
        public static Type Type;
        protected static PropertyInfo PropertyInfo;

        Establish context = () =>
        {
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClass" };
            persistentClassInfo.OwnMembers.Add(new PersistentCollectionMemberInfo(Session.DefaultSession) { Name = "TestProperty" });

//            Type = TypeDefineBuilder.Define(persistentClassInfo);
            PropertyInfo = Type.GetProperty("TestProperty");
        };
    }


}
