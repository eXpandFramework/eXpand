using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator.ClassTypeBuilder;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using eXpand.Xpo;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;
using System.Linq;

namespace eXpand.Tests.WorldCreator
{
    [TestFixture]
    public class TestFixture1 : XpandBaseFixture
    {
        private IClassAssemblyNameBuilder _builder;

        [SetUp]
        public override void Setup() {
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
        public void Core_Property_Get_Invocation()
        {
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClass" + MethodBase.GetCurrentMethod().Name };
            persistentClassInfo.OwnMembers.Add(new PersistentCoreTypeMemberInfo(Session.DefaultSession) { Name = "TestProperty", DataType = XPODataType.Boolean });

            var type = _builder.WithAssemblyName("TestAssembly").Define(persistentClassInfo);

            var instance = Activator.CreateInstance(type, Session.DefaultSession);
            var property = instance.GetType().GetProperty("TestProperty");
            property.SetValue(instance, true, null);
            Assert.AreEqual(true, property.GetValue(instance, null));
        }
    }
    
}
