using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.IO.Core;
using eXpand.ExpressApp.IO.PersistentTypesHelpers;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using eXpand.Persistent.Base.ImportExport;
using eXpand.Persistent.BaseImpl.ImportExport;
using eXpand.Tests.eXpand.WorldCreator;
using Machine.Specifications;
using System.Linq;
using eXpand.Utils.Helpers;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.IO
{
    [Subject(typeof(ExportEngine))]
    public class When_Exporting_1_Customer_with_1_ref_User_2_Orders_and_user_not_serialized:With_Customer_Orders
    {
        static SerializationConfiguration _serializationConfiguration;
        static XPBaseObject _order2;
        static XPBaseObject _order1;
        static XElement _ordersElement;
        static XPBaseObject _user;
        static XElement _customerElement;
        static XElement _root;
        static XDocument _xDocument;
        static XPBaseObject _customer;

        Establish context = () => {
            _user = (XPBaseObject)ObjectSpace.CreateObject(typeof(User));
            _customer = (XPBaseObject) ObjectSpace.CreateObject(CustomerType);
            _customer.SetMemberValue("Name","CustomerName");
            _customer.SetMemberValue("User",_user);
            _order1 = (XPBaseObject)ObjectSpace.CreateObject(OrderType);
            _order1.SetMemberValue("Customer",_customer);
            _order2 = (XPBaseObject) ObjectSpace.CreateObject(OrderType);
            _order2.SetMemberValue("Customer",_customer);
            _serializationConfiguration = new SerializationConfiguration(ObjectSpace.Session) { TypeToSerialize = CustomerType };
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
            _serializationConfiguration.SerializationGraph[0].Children.Where(node => node.Name == "User").Single().SerializationStrategy = SerializationStrategy.DoNotSerialize;
            ObjectSpace.CommitChanges();
        };

        Because of = () => {
            _xDocument = new ExportEngine().Export(new List<object> { _customer }, _serializationConfiguration);
        };

        It should_create_an_xml_document=() => {
            _xDocument.ShouldNotBeNull();
            _root = _xDocument.Root;
        };

        It should_have_serializedObjects_as_root_element=() => _root.Name.ShouldEqual("SerializedObjects");

        It should_have_2_Orders_an_1_Customer_serialized_elements_as_childs_under_root = () => {
            _root.SerializedObjects(_customer.GetType()).Count().ShouldEqual(1);
            _root.SerializedObjects(_order1.GetType()).Count().ShouldEqual(2);
        };

        It should_not_have_User_child_Serialized_element_under_root=() => _root.SerializedObjects().Count().ShouldEqual(3);

        It should_have_2_simple_property_elements_as_customer_Serialized_element_childs=() => {

            _customerElement = _root.SerializedObjects(_customer.GetType()).Single();
            _customerElement.Properties(NodeType.Simple).Count().ShouldEqual(2);
        };


        It should_have_1_key_property_element_as_customer_Serialized_element_child =
            () => {
                _customerElement.ObjectKeyProperties().Count().ShouldEqual(1);
                _customerElement.ObjectNaturalKeyProperty().ShouldNotBeNull();
            };

        It should_have_1_object_property_with_value_the_oid_of_user=() => {
            var objectProperties = _customerElement.Properties(NodeType.Object);
            objectProperties.Count().ShouldEqual(1);
            objectProperties.KeyElements("oid").Single().Value.ShouldEqual(_user.GetMemberValue("Oid").ToString());
        };

        It should_have_1_collection_property__with_name_Orders_as_Serialized_element_child=() => {
            _ordersElement = _customerElement.Properties(NodeType.Collection).Property("Orders");
            _ordersElement.ShouldNotBeNull();
        };

        It should_have_2_ref_properties_with_serialized_strategy_and_value_under_orders_property_collection_element=() => {
            var serializedObjectRefs = _ordersElement.SerializedObjectRefs(_order1.GetType());
            var objectRefs = serializedObjectRefs.SerializedObjectRefs(SerializationStrategy.SerializeAsObject);
            var count = objectRefs.Count();
            count.ShouldEqual(2);
            objectRefs.Where(xElement => xElement.Value == _order1.GetMemberValue("Oid").ToString()).FirstOrDefault().ShouldNotBeNull();
        };
    }

    [Subject(typeof(ExportEngine))]
    public class When_exporting_object_with_Null_referenced_object:With_Isolations {
        static XElement _property;
        static XElement _root;
        static SerializationConfiguration _serializationConfiguration;
        static object _customer;

        Establish context = () => {
            var objectSpace = new TestAppLication<ClassInfoGraphNode>().Setup().ObjectSpace;

            PersistentAssemblyBuilder persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(objectSpace, GetUniqueAssemblyName());
            IClassHandler classHandler = persistentAssemblyBuilder.CreateClasses(new[]{"Customer"});
            classHandler.CreateReferenceMembers(info => new[]{typeof(User)});            
            objectSpace.CommitChanges();
            Type compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));
            var customerType = compileModule.Assembly.GetTypes().Where(type => type.Name == "Customer").Single();
            _customer = objectSpace.CreateObject(customerType);
            objectSpace.CommitChanges();
            _serializationConfiguration = new SerializationConfiguration(objectSpace.Session) { TypeToSerialize = customerType };
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
        };

        Because of = () => {
            XDocument document = new ExportEngine().Export(new[]{_customer},_serializationConfiguration);
            _root = document.Root;
        };

        It should_create_a_property_with_name_same_as_referenced_object_type =
            () => {
                _property = _root.SerializedObjects(_customer.GetType()).ObjectProperty(typeof (User));
                _property.ShouldNotBeNull();
            };
        It should_set_null_value_to_that_property=() => _property.Value.ShouldEqual(String.Empty);
    }

    public interface ICustomer
    {
    }

    public interface IOrder
    {
    }
    [Subject(typeof(ExportEngine))]
    public class When_exporting_Customers_Orders_many_to_many:With_Isolations {
        static XElement _customersElement;
        static XElement _orderElement;
        static XElement _ordersElement;
        static XPBaseObject _order;
        static XElement _customerElement;
        static XElement _root;
        static SerializationConfiguration _serializationConfiguration;
        static XPBaseObject _customer;
        static ObjectSpace _objectSpace;

        Establish context = () => {
            var typeHandler = ModelBuilder<ICustomer,IOrder>.Build().ManyToMany();
            var modelInstancesHandler = typeHandler.CreateInstances();
            _customer = (XPBaseObject) modelInstancesHandler.T1instance;
            _order = (XPBaseObject) modelInstancesHandler.T2Instance;
            _objectSpace=typeHandler.ObjectSpace;
            _serializationConfiguration = new SerializationConfiguration(_objectSpace.Session){TypeToSerialize = _customer.GetType()};
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
            _objectSpace.CommitChanges();
        };

        Because of = () => {
            var xDocument = new ExportEngine().Export(new[] {_customer}, _serializationConfiguration);
            _root = xDocument.Root;
        };

        It should_create_2_serialized_elements = () => _root.SerializedObjects().Count().ShouldEqual(2);

        It should_create_a_customer_root_element=() => {
            _customerElement = _root.SerializedObjects(_customer.GetType()).FirstOrDefault();
            _customerElement.ShouldNotBeNull();
        };

        It should_create_a_collection_property_with_order_type_and_name_orders_under_customer_element=() => {
            _ordersElement = _customerElement.Properties(NodeType.Collection).Property("Orders");
            _ordersElement.ShouldNotBeNull();            
        };

        It should_create_a_ref_object_of_type_order_under_orders_collection_property =
            () => _ordersElement.SerializedObjectRefs(_order.GetType()).FirstOrDefault().ShouldNotBeNull();

        It should_create_a_order_root_element=() => {
            _orderElement = _root.SerializedObjects(_order.GetType()).FirstOrDefault();
            _orderElement.ShouldNotBeNull();
        };

        It should_not_create_a_collection_property_with_customer_type_and_name_customers_under_order_element=() => {
            _customersElement = _ordersElement.Properties(NodeType.Collection).Property("Customers");
            _customersElement.ShouldBeNull();
        };
    }
    [Subject(typeof(ExportEngine))]
    public class When_exporting_an_object_with_value_converter:With_Isolations {
        static DifferenceObject _differenceObject;
        static XElement _root;
        static SerializationConfiguration _serializationConfiguration;


        Establish context = () => {
            var objectSpace = new ObjectSpaceProvider(new MemoryDataStoreProvider()).CreateObjectSpace();
            _differenceObject = Isolate.Fake.Instance<DifferenceObject>(Members.CallOriginal,ConstructorWillBe.Called,objectSpace.Session);
            _differenceObject.Model=new Dictionary(new DictionaryNode("dictionaryXmlValue"),new Schema(new DictionaryNode("shemaNode")));
            _serializationConfiguration = new SerializationConfiguration(objectSpace.Session) { TypeToSerialize = _differenceObject.GetType() };
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
        };

        Because of = () => {
            _root = new ExportEngine().Export(new[] {_differenceObject}, _serializationConfiguration).Root;
        };

        It should_export_the_storage_converter_value =
            () => {
                var serializedObjects = _root.SerializedObjects(_differenceObject.GetType());
                var value = serializedObjects.Properties(NodeType.Simple).Property(_differenceObject.GetPropertyName(x => x.Model)).Value;
                value.ShouldContain("dictionaryXmlValue");

            };    
    }

}
