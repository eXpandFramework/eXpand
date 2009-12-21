using eXpand.ExpressApp.IO.Core;
using Machine.Specifications;

namespace eXpand.Tests.eXpand.IO
{
    [Subject(typeof(ExportEngine))]
    public class When_Exporting_1_Customer_with_1_ref_User_2_Orders_add_user_not_serializable
    {
        Establish context = () => { };
        It should_create_an_xml_at_serialization_configuration_path;
        It should_have_serializedObjects_as_root_element;
        It should_have_2_Orders_an_1_Customer_serialized_elements_as_childs;
        It should_not_have_User_child_Serialized_element;
        It should_have_2_simple_property_elements_as_Serialized_element_childs;
        It should_have_1_key_property_element_as_Serialized_element_child;
        It should_have_1_object_property_with_value_the_username_of_user;
        It should_have_1_collection_property__with_name_Orders_as_Serialized_element_child;
        It should_have_2_ref_properties_under_orders_serialized_element;
    }
}
