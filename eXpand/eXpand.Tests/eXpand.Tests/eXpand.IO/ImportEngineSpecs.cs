using System;
using System.IO;
using System.Reflection;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.IO.Core;
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
            XafTypesInfo.CastTypeToTypeInfo(OrderType).CreateMember("Ammount", typeof (int));
            ObjectSpace.CommitChanges();
            _manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("eXpand.Tests.eXpand.IO.tt.xml");
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

    public class When_importing_an_object_that_is_invalid {
        It should_not_import_it;
        It should_return_1_unimported_object;
    }

    public class When_importing_an_object_with_key_that_exists {
        It should_not_import_it;
        It should_return_1_unimported_object;
    }

    public class When_importing_a_customers_orders_many_to_many {
        It should_should;
    }
}