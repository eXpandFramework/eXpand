using System.Collections.Generic;
using System.Xml.Linq;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.IO.Core;
using eXpand.ExpressApp.IO.PersistentTypesHelpers;
using eXpand.Persistent.Base.ImportExport;
using Machine.Specifications;
using System.Linq;
using eXpand.Utils.Helpers;

namespace eXpand.Tests.eXpand.IO
{
    [Subject(typeof(ExportEngine))]
    public class When_Exporting_1_Customer_with_1_ref_User_2_Orders_add_user_not_serializable:With_Customer_Orders_Serialization_Config
    {
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
            
            new ClassInfoGraphNodeBuilder().Generate(SerializationConfiguration);
            SerializationConfiguration.SerializationGraph[0].Children.Where(node => node.Name=="User").Single().SerializationStrategy=SerializationStrategy.DoNotSerialize;
            ObjectSpace.CommitChanges();
        };

        Because of = () => {
            _xDocument = ExportEngine.Export(_customer,SerializationConfiguration);
        };

        It should_create_an_xml_document=() => {
            _xDocument.ShouldNotBeNull();
            _root = _xDocument.Root;
        };

        It should_have_serializedObjects_as_root_element=() => _root.Name.ShouldEqual("SerializedObjects");

        It should_have_2_Orders_an_1_Customer_serialized_elements_as_childs = () => {
            _root.Descendants("SerializedObject").Where(element => hasAsttributes(element, "type", "Customer")).Count().ShouldEqual(1);
            _root.Descendants("SerializedObject").Where(element => hasAsttributes(element, "type", "Order")).Count().ShouldEqual(2);
        };

        It should_not_have_User_child_Serialized_element=() => _root.Descendants("SerializedObject").Count().ShouldEqual(3);

        It should_have_2_simple_property_elements_as_customer_Serialized_element_childs=() => {
            _customerElement = _root.Descendants("SerializedObject").Where(element => hasAsttributes(element, "type","Customer")).Single();
            _customerElement.Descendants("Property").
                Where(xElement => hasAsttributes(xElement, "type", "simple")).Count().ShouldEqual(2);
        };

        static bool hasAsttributes (XElement element,string name, string value) {
            XAttribute xAttribute = element.Attribute(name);
            if (xAttribute != null) return xAttribute.Value == value;
            return false; 
        }

        It should_have_1_key_property_element_as_customer_Serialized_element_child =
            () =>_customerElement.Descendants("Property").Where(element => hasAsttributes(element, "isKey", "true")).Count().ShouldEqual(1);

        It should_have_1_object_property_with_value_the_oid_of_user=() => {
            IEnumerable<XElement> xElements = _customerElement.Descendants("Property").Where(element => hasAsttributes(element, "type", "object"));
            xElements.Count().ShouldEqual(1);
            xElements.Single().Value.ShouldEqual(_user.GetMemberValue("Oid").ToString());
        };

        It should_have_1_collection_property__with_name_Orders_as_Serialized_element_child=() => {
            IEnumerable<XElement> xElements =_customerElement.Descendants("Property").Where(
                    element =>hasAsttributes(element, "type", "collection") && hasAsttributes(element, "name", "Orders"));
            xElements.Count().ShouldEqual(1);
            _ordersElement = xElements.Single();
        };

        It should_have_2_ref_properties_with_serialized_strategy_and_value_under_orders_property_collection_element=() => {
            IEnumerable<XElement> xElements =
                _ordersElement.Descendants("SerializedObjectRef").Where(
                    element => hasAsttributes(element, "type", "Order") && hasAsttributes(element, "strategy", "SerializeAsObject"));
            xElements.Count().ShouldEqual(2);
            xElements.Where(xElement => xElement.Value==_order1.GetMemberValue("Oid").ToString()).FirstOrDefault().ShouldNotBeNull();
        };
    }
}
