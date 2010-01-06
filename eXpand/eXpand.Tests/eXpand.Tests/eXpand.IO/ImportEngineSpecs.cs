using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.IO.Core;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using Machine.Specifications;
using eXpand.Xpo;

namespace eXpand.Tests.eXpand.IO {
    [Subject(typeof(ImportEngine))]
    public class When_importing_1_Customer_with_1_ref_User_2_Orders_add_user_not_serializable:With_Customer_Orders
    {
        static XPBaseObject _order1;
        static User _user;
        static XPBaseObject _customer;
        static int _count;
        static Stream _manifestResourceStream;

        Establish context = () => {
            _user = (User) ObjectSpace.CreateObject(typeof (User));
            _user.SetMemberValue("oid", new Guid("{B11AFD0E-6B2B-44cf-A986-96909A93291A}"));
            ObjectSpace.Session.GetClassInfo(OrderType).CreateMember("Ammount", typeof (int));
            ObjectSpace.CommitChanges();
            _manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("eXpand.Tests.eXpand.IO.Resources.1toMany.xml");
            if (_manifestResourceStream != null)
                _manifestResourceStream = new MemoryStream(Encoding.UTF8.GetBytes(new StreamReader(_manifestResourceStream).ReadToEnd().Replace("B11AFD0E-6B2B-44cf-A986-96909A93291A", _user.Oid.ToString())));
        };

        Because of = () => {_count= new ImportEngine().ImportObjects(_manifestResourceStream,ObjectSpace); };

        It should_create_1_new_customer_object=() => {
            _customer = ObjectSpace.FindObject(CustomerType, null) as XPBaseObject;
            _customer.ShouldNotBeNull();
        };

        It should_fill_all_customer_simple_properties_with_property_element_values=() => {
            _customer.GetMemberValue("oid").ToString().ShouldEqual("B11AFD0E-6B2B-44cf-A986-96909A93291D".ToLower());
            _customer.GetMemberValue("Name").ShouldEqual("Apostolis");
        };

        It should_set_customer_user_property_same_as_one_found_in_datastore=() => _customer.GetMemberValue("User").ShouldEqual(_user);

        It should_not_import_donotserialized_strategy_user_object=() => ObjectSpace.Session.GetCount(typeof(User)).ShouldEqual(1);
        
        It should_create_2_new_order_objects=() => ObjectSpace.Session.GetCount(OrderType).ShouldEqual(2);

        It should_fill_all_order_properties_with_property_element_values=() => {
            _order1 = ((XPBaseObject) ObjectSpace.GetObjectByKey(OrderType,new Guid("{B11AFD0E-6B2B-44cf-A986-96909A93291E}")));
            _order1.Reload();
            _order1.ShouldNotBeNull();
            _order1.GetMemberValue("Ammount").ShouldEqual(200);
            var order2 = ((XPBaseObject)ObjectSpace.GetObjectByKey(OrderType, new Guid("{B11AFD0E-6B2B-44cf-A986-96909A93291F}")));
            order2.ShouldNotBeNull();
            order2.GetMemberValue("Ammount").ShouldEqual(100);
        };

        It should_set_customer_property_of_order_same_as_new_created_customer=() => _order1.GetMemberValue("Customer").ShouldEqual(_customer);

        It should_return_0_unimported_objects=() => _count.ShouldEqual(0);
    }
    [Subject(typeof(ImportEngine))]
    public class When_importing_an_object_that_is_invalid:With_Isolations {
//        static ObjectSpace _objectSpace;
//        static Stream _manifestResourceStream;

        Establish context = () => {
//            
        };

        It should_should;
//        Because of = () => new ImportEngine().ImportObjects(_manifestResourceStream, _objectSpace);

//        It should_not_import_it=() => _objectSpace.GetObjectsCount(typeof(User), null).ShouldEqual(0);
    }
    [Subject(typeof(ImportEngine))]
    public class When_importing_a_null_reference_property:With_Isolations {
        static ObjectSpace _objectSpace;
        static Type _customerType;
        static Stream _manifestResourceStream;

        Establish context = () =>
        {
            _objectSpace = new ObjectSpaceProvider(new MemoryDataStoreProvider()).CreateObjectSpace();
            var persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(_objectSpace, GetUniqueAssemblyName());
            persistentAssemblyBuilder.CreateClasses(new[] { "Customer" }).
                CreateReferenceMembers(info => new[] { typeof(User) });
            _objectSpace.CommitChanges();
            var compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));
            _customerType = compileModule.Assembly.GetTypes().Where(type => type.Name == "Customer").Single();
            _manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("eXpand.Tests.eXpand.IO.Resources.NullRefProperty.xml");
        };

        Because of = () => new ImportEngine().ImportObjects(_manifestResourceStream, _objectSpace);
        It should_import_parent_object=() => _objectSpace.GetObjectsCount(_customerType, null).ShouldEqual(1);
        It should_not_import_it = () => _objectSpace.GetObjectsCount(typeof(Address), null).ShouldEqual(0);
    }
    [Subject(typeof(ExportEngine))]
    public class When_importing_an_object_with_value_converter:With_Isolations
    {
        static ObjectSpace _objectSpace;
        static Stream _manifestResourceStream;
        
        static DifferenceObject _differenceObject;

        Establish context = () => {
            _manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("eXpand.Tests.eXpand.IO.Resources.WithValueConverter.xml");
            _objectSpace = new ObjectSpaceProvider(new MemoryDataStoreProvider()).CreateObjectSpace();
        };

        Because of = () => new ImportEngine().ImportObjects(_manifestResourceStream, _objectSpace);

        It should_iumport_the_converter_from_storage_value=() => {
            var persistentApplication = _objectSpace.FindObject<PersistentApplication>(null);
            persistentApplication.Model.RootNode.Name.ShouldEqual ("dictionaryXmlValue");            
        };
    }
    [Subject(typeof(ImportEngine))]
    public class When_importing_an_object_with_key_that_exists_and_differs_than_natural_key:With_Isolations {
        static Type _customerType;
        static ObjectSpace _objectSpace;
        static XPBaseObject _customer;
        static Stream _manifestResourceStream;

        Establish context = () =>
        {
            _manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("eXpand.Tests.eXpand.IO.Resources.ExistentKeyObject.xml");
            _objectSpace = new ObjectSpaceProvider(new MemoryDataStoreProvider()).CreateObjectSpace();
            var persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(_objectSpace, GetUniqueAssemblyName());
            persistentAssemblyBuilder.CreateClasses(new[] { "Customer" }).CreateSimpleMembers<string>(info => new[] { "Name","Age" });
            _objectSpace.CommitChanges();
            var compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));
            _customerType = compileModule.Assembly.GetTypes().Where(type => type.Name == "Customer").Single();
            _customer = (XPBaseObject)_objectSpace.CreateObject(_customerType);
            _customer.SetMemberValue("Name", "test");
            _customer.SetMemberValue("Age", "1");
            _objectSpace.CommitChanges();
        };

        Because of = () => new ImportEngine().ImportObjects(_manifestResourceStream, _objectSpace);

        It should_not_create_it = () => _objectSpace.GetObjectsCount(_customerType, null).ShouldEqual(1);
        It should_overide_its_values=() => _customer.GetMemberValue("Age").ShouldEqual("2");
    }

    [Subject(typeof(ImportEngine))]
    public class When_importing_an_object_with_key_that_exists:With_Isolations {
        static Type _customerType;
        static ObjectSpace _objectSpace;
        static XPBaseObject _customer;
        static Stream _manifestResourceStream;

        Establish context = () =>
        {
            _manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("eXpand.Tests.eXpand.IO.Resources.ExistentNaturalKeyObject.xml");
            _objectSpace = new ObjectSpaceProvider(new MemoryDataStoreProvider()).CreateObjectSpace();
            var persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(_objectSpace, GetUniqueAssemblyName());
            persistentAssemblyBuilder.CreateClasses(new[] { "Customer" }).CreateSimpleMembers<string>(info => new[] { "Name" });
            _objectSpace.CommitChanges();
            var compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));
            _customerType = compileModule.Assembly.GetTypes().Where(type => type.Name == "Customer").Single();
            _customer = (XPBaseObject)_objectSpace.CreateObject(_customerType);
            _customer.SetMemberValue("Name", "test");
            _customer.SetMemberValue("oid", new Guid("B11AFD0E-6B2B-44cf-A986-96909A93291D"));
            _objectSpace.CommitChanges();
        };

        Because of = () => new ImportEngine().ImportObjects(_manifestResourceStream, _objectSpace);

        It should_not_create_it = () => _objectSpace.GetObjectsCount(_customerType, null).ShouldEqual(1);
        It should_overide_its_values = () => _customer.GetMemberValue("Name").ShouldEqual("newName");
    }

    [Subject(typeof(ImportEngine))]
    public class When_importing_an_object_with_natural_key_that_exists_as_deleted:With_Isolations {
        static XPBaseObject _findObject;
        static Guid _guid;
        static Type _customerType;
        static ObjectSpace _objectSpace;
        static XPBaseObject _customer;
        static Stream _manifestResourceStream;

        Establish context = () =>
        {
            _manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("eXpand.Tests.eXpand.IO.Resources.ExistentNaturalKeyObject.xml");
            _objectSpace = new ObjectSpaceProvider(new MemoryDataStoreProvider()).CreateObjectSpace();
            var persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(_objectSpace, GetUniqueAssemblyName());
            persistentAssemblyBuilder.CreateClasses(new[] { "Customer" }).CreateSimpleMembers<string>(info => new[] { "Name" });
            _objectSpace.CommitChanges();
            var compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));
            _customerType = compileModule.Assembly.GetTypes().Where(type => type.Name == "Customer").Single();
            _customer = (XPBaseObject)_objectSpace.CreateObject(_customerType);
            _customer.SetMemberValue("Name", "test");
            _guid = new Guid("B11AFD0E-6B2B-44cf-A986-96909A93291D");
            _customer.SetMemberValue("oid", _guid);
            _objectSpace.CommitChanges();
            _customer.Delete();
            _objectSpace.CommitChanges();
        };

        Because of = () => new ImportEngine().ImportObjects(_manifestResourceStream, _objectSpace);

        It should_create_it=() => {
            var session = new Session(_objectSpace.Session.DataLayer);
            _findObject = (XPBaseObject)session.FindObject(_customerType, new BinaryOperator("Oid", _guid));
            _findObject.ShouldNotBeNull();
        };

        It should_purge_deleted_object = () => _findObject.IsDeleted.ShouldBeFalse();
    }

    [Subject(typeof(ImportEngine))]
    public class When_importing_a_customers_orders_many_to_many:With_Isolations {
        static Type _orderType;
        static Type _customerType;
        static Stream _manifestResourceStream;
        static ObjectSpace _objectSpace;

        Establish context = () => {
            var typeHandler = ModelBuilder<ICustomer, IOrder>.Build().ManyToMany();
            _customerType = typeHandler.T1Type;
            _orderType = typeHandler.T2Type;
            _objectSpace = typeHandler.ObjectSpace;
            _manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("eXpand.Tests.eXpand.IO.Resources.ManyToMany.xml");
        };

        Because of = () => new ImportEngine().ImportObjects(_manifestResourceStream, _objectSpace);

        It should_create_1_customer=() => _objectSpace.GetObjectsCount(_customerType, null).ShouldEqual(1);
        It should_create_2_orders = () => _objectSpace.GetObjectsCount(_orderType, null).ShouldEqual(2);
        It should_create_2_customer_orders=() =>
                                           ((IList)
                                            ((XPBaseObject) _objectSpace.FindObject(_customerType, null)).GetMemberValue("Orders")).Count.ShouldEqual(2);
        It should_create_1_order1_customer=() =>
                                           ((IList)
                                            ((XPBaseObject) new XPCollection(_objectSpace.Session,_orderType)[0]).GetMemberValue("Customers")).Count.ShouldEqual(1);
        It should_create_1_order2_customer = () => ((IList)
                                            ((XPBaseObject)new XPCollection(_objectSpace.Session, _orderType)[1]).GetMemberValue("Customers")).Count.ShouldEqual(1);
    }
}