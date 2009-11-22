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

namespace eXpand.Tests.WorldCreator
{
    public abstract class With_AssemblyNameBuilder
    {
        protected static IAssemblyNameBuilder AssemblyNameBuilder;
        Establish context = () => { AssemblyNameBuilder = PersistentClassInfoTypeBuilder.BuildClass(); };
    }
    public abstract class With_DynamicCore_Property : With_Type_Builder
    {
        protected static Type Type;
        protected static PropertyInfo PropertyInfo;
        protected static PersistentClassInfo ClassInfo;
        Establish context = () =>
        {
            ClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClass" };
            ClassInfo.OwnMembers.Add(new PersistentCoreTypeMemberInfo(Session.DefaultSession) { Name = "TestProperty", DataType = XPODataType.Boolean });
            Type = TypeDefineBuilder.Define(ClassInfo);
            PropertyInfo = Type.GetProperty("TestProperty");
        };

        
    }

    public abstract class With_In_Memory_DataStore {
        Establish context = () => {
            Session.DefaultSession.Disconnect();
            ReflectionHelper.Reset();
            XafTypesInfo.Reset(true);
            var dataStore = new InMemoryDataStore( AutoCreateOption.DatabaseAndSchema);
            XpoDefault.DataLayer = new SimpleDataLayer(XafTypesInfo.XpoTypeInfoSource.XPDictionary, dataStore);
        };    
    }
    public abstract class With_Type_Builder:With_In_Memory_DataStore
    {
        protected static ITypeDefineBuilder TypeDefineBuilder;
        Establish context = () => {
            TypeDefineBuilder = PersistentClassInfoTypeBuilder.BuildClass().WithAssemblyName("TestAssembly" );
        };
    }
    public abstract class With_DynamicReference_Property : With_Type_Builder
    {
        protected static Type Type;

        protected static PropertyInfo PropertyInfo;

        Establish context = () =>
        {
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClass" };
            persistentClassInfo.OwnMembers.Add(new PersistentReferenceMemberInfo(Session.DefaultSession) { Name = "TestProperty", ReferenceType = typeof(User) });

            Type = TypeDefineBuilder.Define(persistentClassInfo);
            PropertyInfo = Type.GetProperty("TestProperty");
        };
    }
    public abstract class With_DynamicCollection_Property : With_Type_Builder
    {
        public static Type Type;
        protected static PropertyInfo PropertyInfo;

        Establish context = () =>
        {
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClass" };
            persistentClassInfo.OwnMembers.Add(new PersistentCollectionMemberInfo(Session.DefaultSession) { Name = "TestProperty" });

            Type = TypeDefineBuilder.Define(persistentClassInfo);
            PropertyInfo = Type.GetProperty("TestProperty");
        };
    }


}
