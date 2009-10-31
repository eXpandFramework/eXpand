using System;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator.ClassTypeBuilder;
using eXpand.Persistent.Base.PersistentMetaData;
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
        public void DynamicType_Reference_Property_Creation()
        {
            var userClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "User" };
            var name = "TestClass" + MethodBase.GetCurrentMethod().Name;
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = name };
            persistentClassInfo.OwnMembers.Add(new PersistentReferenceMemberInfo(Session.DefaultSession) { Name = "TestProperty", ReferenceTypeAssemblyQualifiedName = "TestAssembly.User, TestAssembly, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" });

            var types = _builder.WithAssemblyName("TestAssembly").Define(new List<IPersistentClassInfo> {persistentClassInfo,userClassInfo});

            var property = types.Where(type => type.Type.Name == name).Single().Type.GetProperty("TestProperty");
            Assert.IsNotNull(property);
            Assert.AreEqual("TestAssembly.User, TestAssembly, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", property.PropertyType.AssemblyQualifiedName);
        }
        [Test]
        public void ExistentType_Reference_Property_Creation()
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

            PropertyInfo property = type.GetProperty("TestProperty");
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

        [Test]
        public void Associate_2_DynamicTypes()
        {
            var customerClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "Customer" };
            var ordersMemberInfo = new PersistentCollectionMemberInfo(Session.DefaultSession) { Name = "Orders" };
            ordersMemberInfo.TypeAttributes.Add(new PersistentAssociationAttribute(Session.DefaultSession) { AssemblyQualifiedName = "TestAssembly.Order, TestAssembly, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" });
            customerClassInfo.OwnMembers.Add(ordersMemberInfo);

            var orderClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "Order" };
            var customerMemberInfo = new PersistentReferenceMemberInfo(Session.DefaultSession) { Name = "Customer", ReferenceTypeAssemblyQualifiedName = "TestAssembly.Customer, TestAssembly, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" };
            customerMemberInfo.TypeAttributes.Add(new PersistentAssociationAttribute(Session.DefaultSession) { AssemblyQualifiedName = "TestAssembly.Order, TestAssembly, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" });
            orderClassInfo.OwnMembers.Add(customerMemberInfo);
            var classDefineBuilder = _builder.WithAssemblyName("TestAssembly");

            var types = classDefineBuilder.Define(new List<IPersistentClassInfo> {customerClassInfo,orderClassInfo});

            Assert.AreEqual(2, types.Count);
            Assert.AreEqual("TestAssembly.Customer, TestAssembly, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", types[0].Type.AssemblyQualifiedName);
            Assert.AreEqual("TestAssembly.Order, TestAssembly, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", types[1].Type.AssemblyQualifiedName);
        }
        [Test]
        public void Associate_1_DynamicType_With_1_Existent()
        {
            var customerClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "Customer" };
            customerClassInfo.TypeAttributes.Add(new PersistentAssociationAttribute(Session.DefaultSession) { ElementType = typeof(User) });
            var classDefineBuilder = _builder.WithAssemblyName("TestAssembly");

            var types = classDefineBuilder.Define(customerClassInfo);

            Assert.AreEqual("TestAssembly.Customer, TestAssembly, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", types.AssemblyQualifiedName);
        }
        [Test]
        public void On_Change_Will_Be_Called_When_Setting_A_Property()
        {
            var customerClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "Customer" };
            customerClassInfo.OwnMembers.Add(new PersistentCoreTypeMemberInfo(Session.DefaultSession){Name = "Test",DataType = XPODataType.String});
            IClassDefineBuilder classDefineBuilder = _builder.WithAssemblyName("TestAssembly");
            var type = classDefineBuilder.Define(customerClassInfo);
            classDefineBuilder.AssemblyBuilder.Save("Test.dll");
            var instance = Activator.CreateInstance(type,Session.DefaultSession);
            var xpBaseObject = ((XPBaseObject) instance);

            bool changed = false;
            xpBaseObject.Changed += (sender, args) => changed = true;
            xpBaseObject.SetMemberValue("Test","testValue");

            Assert.AreEqual("testValue", xpBaseObject.GetMemberValue("Test"));
            Assert.IsTrue(changed);
        }
        [Test]
        public void DynamicType_Can_Inherit_An_Existent_Type() {
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) {Name = "Customer", BaseType = typeof (User)};

            var type = _builder.WithAssemblyName("TestAssembly").Define(persistentClassInfo);

            Assert.AreEqual(typeof(User), type.BaseType);
        }
        [Test]
        public void DynamicType_Can_Inherit_A_Dynamic_Type()
        {
            var userClassInfo = new PersistentClassInfo(Session.DefaultSession) {Name = "User"};
            var customerClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "Customer", BaseTypeAssemblyQualifiedName = "TestAssembly.User, TestAssembly, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" };

            var types = _builder.WithAssemblyName("TestAssembly").Define(new List<IPersistentClassInfo> {customerClassInfo,userClassInfo});

            var custtomerAssemblyQualifiedName = types.Where(type => type.Type.Name=="Customer").Single().Type.BaseType.AssemblyQualifiedName;
            Assert.AreEqual("TestAssembly.User, TestAssembly, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", custtomerAssemblyQualifiedName);
        }
    }



}
