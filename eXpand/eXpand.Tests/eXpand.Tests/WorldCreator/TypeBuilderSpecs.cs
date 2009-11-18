using System;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator.ClassTypeBuilder;
using eXpand.Persistent.Base.General;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using Machine.Specifications;
using System.Linq;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.WorldCreator
{
    [Subject(typeof(PersistentClassInfoTypeBuilder), "specs")]
    [Isolated]
    public class When_Creating_An_Assembly : With_AssemblyNameBuilder
    {
        Because of = () => AssemblyNameBuilder.WithAssemblyName("TestAssembly"+new Random().Next(Int32.MinValue,Int32.MaxValue));
        It should_Create_An_Assembly_In_Current_AppDomain = () => AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.FullName == "TestAssembly, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null").FirstOrDefault().ShouldNotBeNull();
        
    }
    [Subject(typeof(PersistentClassInfoTypeBuilder), "specs")]
    [Isolated]
    public class When_Defining_A_DynamicType : With_Type_Builder
    {
        static Type type;
        Because of = () => {type=TypeDefineBuilder.Define(new PersistentClassInfo(Session.DefaultSession) {Name = "TestClass"});
        };

        It should_Create_A_Dynamic_Type =() => type.FullName.ShouldEqual("TestAssembly.TestClass");

        It should_have_A_Constructor_with_Session_as_parameter = () => type.GetConstructor(new []{typeof(Session)}).ShouldNotBeNull();
    }
    [Subject(typeof(PersistentClassInfoTypeBuilder), "specs")]
    [Isolated]
    public class When_Requesting_A_Property_With_Reflection : With_DynamicCore_Property{
        
        Because of = () => {
            PropertyInfo = Type.GetProperty("TestProperty");
        };

        It should_find_a_property = () => PropertyInfo.ShouldNotBeNull();

        It should_be_of_Correct_Type = () => PropertyInfo.PropertyType.FullName.ShouldEqual(typeof(bool).FullName);
    }
    [Subject(typeof(PersistentClassInfoTypeBuilder), "specs")]
    [Isolated]
    public class When_Invoking_A_Property:With_DynamicCore_Property {
        static object value;

        Because of = () => {
            object o= Activator.CreateInstance(Type, Session.DefaultSession);
            value=PropertyInfo.GetValue(o, null);
        };

        It should_have_value = () => value.ShouldNotBeNull();

    }
    [Subject(typeof(PersistentClassInfoTypeBuilder), "specs")]
    [Isolated]
    public class When_Creating_A_reference_Property_of_dynamic_type:With_Type_Builder {
        static List<TypeInfo> _types;
        static PropertyInfo PropertyInfo;

        Because of = () => {
            var userClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "User" };
            const string name = "TestClass";
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = name };
            persistentClassInfo.OwnMembers.Add(new PersistentReferenceMemberInfo(Session.DefaultSession) { Name = "TestProperty", ReferenceTypeAssemblyQualifiedName = "TestAssembly.User, TestAssembly, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" });

            _types = TypeDefineBuilder.Define(new List<IPersistentClassInfo> { persistentClassInfo, userClassInfo });
        };

        It should_find_the_property_thourgh_reflection = () => {
            PropertyInfo = _types.Where(type => type.Type.Name == "TestClass").Single().Type.GetProperty("TestProperty");
            PropertyInfo.ShouldNotBeNull();
        };


        It should_be_ofthe_correct_referenced_type =
            () =>
            PropertyInfo.PropertyType.AssemblyQualifiedName.ShouldEqual(
                "TestAssembly.User, TestAssembly, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
    }
    [Subject(typeof(PersistentClassInfoTypeBuilder), "specs")]
    [Isolated]
    public class When_Creating_A_reference_Property_of_existent_type:With_Type_Builder {
        static Type _type;
        static PropertyInfo PropertyInfo;

        Because of = () => {

            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClass" };
            persistentClassInfo.OwnMembers.Add(new PersistentReferenceMemberInfo(Session.DefaultSession) { Name = "TestProperty", ReferenceType = typeof(User) });

            _type = TypeDefineBuilder.Define(persistentClassInfo);
        };

        It should_find_the_property_thourgh_reflection = () => {
            PropertyInfo = _type.GetProperty("TestProperty");
            PropertyInfo.ShouldNotBeNull();
        };

        It should_be_ofthe_correct_referenced_type =() =>PropertyInfo.PropertyType.AssemblyQualifiedName.ShouldEqual(typeof(User).AssemblyQualifiedName);
    }

    [Subject(typeof(PersistentClassInfoTypeBuilder), "specs")]
    [Isolated]
    public class When_Invoking_A_Dynamic_Reference_Property:With_DynamicReference_Property {
        static User User;
        static object o;

        Because of = () => {
            o = Activator.CreateInstance(Type, Session.DefaultSession);
            User = new User(Session.DefaultSession);
            PropertyInfo.SetValue(o,User, null);
        };

        It should_have_the_correct_value = () => PropertyInfo.GetValue(o, null).ShouldEqual(User);
    }

    [Subject(typeof(PersistentClassInfoTypeBuilder), "specs")]
    [Isolated]
    public class When_Creating_A_Collection_Property:With_Type_Builder {
        static Type Type;

        Because of = () => {
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClass"  };
            persistentClassInfo.OwnMembers.Add(new PersistentCollectionMemberInfo(Session.DefaultSession) { Name = "TestProperty" });

            Type = TypeDefineBuilder.Define(persistentClassInfo);
        };

        It should_find_the_propertyInfo_through_reflection = () => Type.GetProperty("TestProperty").ShouldNotBeNull();
    }
    [Subject(typeof(PersistentClassInfoTypeBuilder), "specs")]
    [Isolated]
    public class When_Invoking_A_Dynamic_Collection_Property:With_DynamicCollection_Property {
        static object value;

        Because of = () => {
            object o=Activator.CreateInstance(Type, Session.DefaultSession);
            value = PropertyInfo.GetValue(o, null);
        };

        It should_not_have_value = () => value.ShouldBeNull();
    }
    [Subject(typeof(PersistentClassInfoTypeBuilder), "specs")]
    [Isolated]
    public class When_Applying_Attributes_To_Properties:With_Type_Builder {
        public static Type Type;
        protected static PropertyInfo PropertyInfo;

        static IPersistentClassInfo PersistentClassInfo;

        Establish context = () =>
        {
            PersistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClass" };
            var colInfo = new PersistentCollectionMemberInfo(Session.DefaultSession) {Name = "TestProperty"};
            colInfo.TypeAttributes.Add(new PersistentAssociationAttribute(Session.DefaultSession) { ElementType = typeof(User) });
            PersistentClassInfo.OwnMembers.Add(colInfo);
        };

        Because of = () => { Type = TypeDefineBuilder.Define(PersistentClassInfo); };

        It should_be_discovarable_thourgh_reflection =
            () =>
            Type.GetProperty("TestProperty").GetCustomAttributes(typeof (AssociationAttribute), false).Count().
                ShouldEqual(1);
    }

    [Subject(typeof(PersistentClassInfoTypeBuilder), "specs")]
    [Isolated]
    public class When_Applying_Attributes_To_Dynamic_Classes:With_Type_Builder {
        static PersistentClassInfo PersistentClassInfo;
        static Type _type;

        Establish context = () => {
            PersistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClass" + MethodBase.GetCurrentMethod().Name };
            PersistentClassInfo.TypeAttributes.Add(new PersistentCustomAttribute(Session.DefaultSession));
        };

        Because of = () => {_type= TypeDefineBuilder.Define(PersistentClassInfo); };

        It should_be_discoverable_through_reflection =() => _type.GetCustomAttributes(false).OfType<CustomAttribute>().Count().ShouldEqual(1);
    }
    [Subject(typeof(PersistentClassInfoTypeBuilder), "specs")]
    [Isolated]
    public class When_Associating_1_DynamicType_With_1_Existent:With_Type_Builder
    {
        static List<IPersistentClassInfo> _list;

        static List<TypeInfo> _types;

        Establish context = () => {
            var customerClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "Customer" };
            var ordersMemberInfo = new PersistentCollectionMemberInfo(Session.DefaultSession) { Name = "Orders" };
            ordersMemberInfo.TypeAttributes.Add(new PersistentAssociationAttribute(Session.DefaultSession) { AssemblyQualifiedName = "TestAssembly.Order, TestAssembly, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" });
            customerClassInfo.OwnMembers.Add(ordersMemberInfo);

            var orderClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "Order" };
            var customerMemberInfo = new PersistentReferenceMemberInfo(Session.DefaultSession) { Name = "Customer", ReferenceTypeAssemblyQualifiedName = "TestAssembly.Customer, TestAssembly, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" };
            customerMemberInfo.TypeAttributes.Add(new PersistentAssociationAttribute(Session.DefaultSession) { AssemblyQualifiedName = "TestAssembly.Order, TestAssembly, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" });
            orderClassInfo.OwnMembers.Add(customerMemberInfo);
            _list = new List<IPersistentClassInfo> {customerClassInfo, orderClassInfo};
        };

        Because of = () =>_types= TypeDefineBuilder.Define(_list);

        It should_create_2_types_In_Current_domain =
            () =>
            _types[0].Type.Assembly.GetTypes().Where(type => typeof (XPBaseObject).IsAssignableFrom(type)).Count().
                ShouldEqual(2);
    }
    [Subject(typeof(PersistentClassInfoTypeBuilder), "specs")]
    [Isolated]
    public class When_Changing_A_Dynamic_property:With_DynamicCore_Property {
        static bool changed;
        static XPBaseObject baseObject;

        Establish context = () => {
            baseObject = (XPBaseObject) Activator.CreateInstance(Type, Session.DefaultSession);
            baseObject.Changed += (sender, args) => changed = true;
        };

        Because of = () => baseObject.SetMemberValue("TestProperty", true);

        It should_raize_changed_event = () => changed.ShouldEqual(true);
    }
    [Subject(typeof(PersistentClassInfoTypeBuilder), "specs")]
    [Isolated]
    public class When_Dynamic_Type_Inherits_Existent_Type:With_Type_Builder {
        static PersistentClassInfo persistentClassInfo;
        static Type type;
        Establish context = () => { persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) {Name = "Customer", BaseType = typeof (User)}; };
        Because of = () => { type = TypeDefineBuilder.Define(persistentClassInfo); };
        It should_have_as_base_Class_the_existent_one = () => type.BaseType.ShouldEqual(typeof (User));
    }
    [Subject(typeof(PersistentClassInfoTypeBuilder), "specs")]
    [Isolated]
    public class When_Dynamic_Type_inherits_a_Dynamic_type:With_Type_Builder {
        static List<IPersistentClassInfo> list;
        static List<TypeInfo> types;

        Establish context = () => {
            var userClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "User" };
            var customerClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "Customer", BaseTypeAssemblyQualifiedName = "TestAssembly.User, TestAssembly, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" };
            list = new List<IPersistentClassInfo> {customerClassInfo, userClassInfo};
        };

        Because of = () => {
            types=TypeDefineBuilder.Define(list);
        };

        It should_have_as_base_class_the_dynamic_one =
            () =>
            types[1].Type.BaseType.AssemblyQualifiedName.ShouldEqual(
                "TestAssembly.User, TestAssembly, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
    }
    [Subject(typeof(PersistentClassInfoTypeBuilder), "specs")]
    [Isolated]
    public class When_DynamicType_Implement_An_Interface:With_Type_Builder {
        static PersistentClassInfo userClassInfo;

        static Type _type;

        Establish context = () => {
            userClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "User" };
            userClassInfo.Interfaces.Add(new InterfaceInfo(Session.DefaultSession) { Name = typeof(IHidden).FullName, Assembly = new AssemblyName(typeof(IHidden).Assembly.FullName + "").Name });
        };

        Because of = () => { _type = TypeDefineBuilder.Define(userClassInfo); };

        It should_be_Discoverable_thourgh_reflection =() => _type.GetInterfaces().Where(type => type == typeof (IHidden)).ShouldNotBeNull();
        It should_create_non_existent_properties = () => _type.GetProperty("Hidden").ShouldNotBeNull();
    }
    [Isolated]
    [Subject(typeof(PersistentClassInfoTypeBuilder), "specs")]
    public class When_Creating_Types_In_A_Non_Xaf_Context : With_Type_Builder{


        static Type dynamicType;

        static ITypeDefineBuilder typeDefineBuilder;

        static PersistentClassInfo persistentClassInfo;

        Establish context = () => {
            persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "Test" };
            persistentClassInfo.Save();
            typeDefineBuilder = PersistentClassInfoTypeBuilder.BuildClass().WithAssemblyName(persistentClassInfo.AssemblyName + new Random().Next(Int32.MinValue, Int32.MaxValue));
        };

        Because of = () => {
            dynamicType = typeDefineBuilder.Define(persistentClassInfo);
        };

        It should_create_a_Dynamic_Type = () => dynamicType.ShouldNotBeNull();
    }
}
