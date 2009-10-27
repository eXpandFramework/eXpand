using System;
using System.Reflection;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator.ClassTypeBuilder;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Xpo;
using MbUnit.Framework;
using System.Linq;

namespace eXpand.Tests.WorldCreator
{
    [TestFixture]
    public class PersistentClassTypeBuilderTests : XpandBaseFixture
    {
        private IClassAssemblyNameBuilder _builder;

        [SetUp]
        public override void Setup() {
            base.Setup();
            _builder = PersistentClassTypeBuilder.BuildClass();
        }
        [Test]
        public void DynamicAssembly_Can_Create() {
            _builder.WithAssemblyName("TestAssembly").Define(new PersistentClassInfo(Session.DefaultSession) { Name = MethodBase.GetCurrentMethod().Name });

            var singleOrDefault = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.FullName == "TestAssembly, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null").FirstOrDefault();

            Assert.IsNotNull(singleOrDefault);
        }
        [Test]
        public void Type_Can_Be_Created()
        {
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClass" + MethodBase.GetCurrentMethod().Name };

            var type = _builder.WithAssemblyName("TestAssembly").Define(persistentClassInfo);

            Assert.AreEqual("TestAssembly.TestClass" + MethodBase.GetCurrentMethod().Name, type.FullName);
        }
        [Test]
        public void Ctor_Creation()
        {
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClass" + MethodBase.GetCurrentMethod().Name };

            var type = _builder.WithAssemblyName("TestAssembly").Define(persistentClassInfo);

            Activator.CreateInstance(type, Session.DefaultSession);
        }
        
        [Test]
        public void Core_Property_Creation()
        {
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClass" + MethodBase.GetCurrentMethod().Name };
            persistentClassInfo.OwnMembers.Add(new PersistentCoreTypeMemberInfo(Session.DefaultSession){Name = "TestProperty",DataType = XPODataType.Boolean});

            var type = _builder.WithAssemblyName("TestAssembly").Define(persistentClassInfo);

            var property = type.GetProperty("TestProperty");
            Assert.IsNotNull(property);
            Assert.AreEqual(typeof(bool), property.PropertyType);
        }
        
        [Test]
        public void Core_Property_GetSet_Invocation()
        {
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClass" + MethodBase.GetCurrentMethod().Name };
            persistentClassInfo.OwnMembers.Add(new PersistentCoreTypeMemberInfo(Session.DefaultSession) { Name = "TestProperty", DataType = XPODataType.Boolean });

            var type = _builder.WithAssemblyName("TestAssembly").Define(persistentClassInfo);

            var instance = Activator.CreateInstance(type, Session.DefaultSession);
            var property = instance.GetType().GetProperty("TestProperty");
            property.SetValue(instance, true, null);
            Assert.AreEqual(true, property.GetValue(instance, null));
        }
        [Test]
        public void Reference_Property_Creation()
        {
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClass" + MethodBase.GetCurrentMethod().Name };
            persistentClassInfo.OwnMembers.Add(new PersistentReferenceMemberInfo(Session.DefaultSession) { Name = "TestProperty", ReferenceType =typeof(User) });

            var type = _builder.WithAssemblyName("TestAssembly").Define(persistentClassInfo);

            var property = type.GetProperty("TestProperty");
            Assert.IsNotNull(property);
            Assert.AreEqual(typeof(User), property.PropertyType);
        }
        [Test]
        public void Reference_Property_GetSet_Invocation()
        {
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClass" + MethodBase.GetCurrentMethod().Name };
            persistentClassInfo.OwnMembers.Add(new PersistentReferenceMemberInfo(Session.DefaultSession) { Name = "TestProperty", ReferenceType = typeof(User) });

            var type = _builder.WithAssemblyName("TestAssembly").Define(persistentClassInfo);

            var instance = Activator.CreateInstance(type, Session.DefaultSession);
            var property = instance.GetType().GetProperty("TestProperty");
            var user = new User(Session.DefaultSession);
            property.SetValue(instance, user, null);
            Assert.AreEqual(user, property.GetValue(instance, null));
        }
        [Test]
        public void Collection_Property_Creation()
        {
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClass" + MethodBase.GetCurrentMethod().Name };
            persistentClassInfo.OwnMembers.Add(new PersistentCollectionMemberInfo(Session.DefaultSession) { Name = "TestProperty"});

            var type = _builder.WithAssemblyName("TestAssembly").Define(persistentClassInfo);

            var property = type.GetProperty("TestProperty");
            Assert.IsNotNull(property);
            Assert.IsNull(property.GetSetMethod());
            Assert.AreEqual(typeof(XPCollection), property.PropertyType);
        }

        [Test]
        public void Collection_Property_Get_Invocation()
        {
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClass" + MethodBase.GetCurrentMethod().Name };
            persistentClassInfo.OwnMembers.Add(new PersistentCollectionMemberInfo(Session.DefaultSession) { Name = "TestProperty" });

            var type = _builder.WithAssemblyName("TestAssembly").Define(persistentClassInfo);

            var instance = Activator.CreateInstance(type, Session.DefaultSession);
            var property = instance.GetType().GetProperty("TestProperty");
            Assert.IsNull(property.GetValue(instance, null));

        }
        [Test]
        public void Attributes_Can_Be_Applied_On_Properties()
        {
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClass" + MethodBase.GetCurrentMethod().Name };
            var coreTypeMemberInfo = new PersistentCoreTypeMemberInfo(Session.DefaultSession) { Name = "TestProperty", DataType = XPODataType.String };
            coreTypeMemberInfo.TypeAttributes.Add(new PersistentAssociationAttribute(Session.DefaultSession) { ElementType = typeof(User) });
            persistentClassInfo.OwnMembers.Add(coreTypeMemberInfo);

            var type = _builder.WithAssemblyName("TestAssembly").Define(persistentClassInfo);

            var property = type.GetProperty("TestProperty");
            Assert.AreEqual(1, property.GetCustomAttributes(false).OfType<AssociationAttribute>().Count());
        }
        [Test]
        public void Attributes_Can_Be_Applied_On_Classes()
        {
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClass" + MethodBase.GetCurrentMethod().Name };
            persistentClassInfo.TypeAttributes.Add(new PersistentCustomAttribute(Session.DefaultSession));

            var type = _builder.WithAssemblyName("TestAssembly").Define(persistentClassInfo);

            Assert.AreEqual(1, type.GetCustomAttributes(false).OfType<CustomAttribute>().Count());
        }

    }

    
}
