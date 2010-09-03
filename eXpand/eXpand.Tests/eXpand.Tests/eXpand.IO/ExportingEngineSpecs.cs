using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
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
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.ImportExport;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using Machine.Specifications;
using System.Linq;
using TypeMock.ArrangeActAssert;
using eXpand.Xpo;
using ObjectSpaceProvider = DevExpress.ExpressApp.ObjectSpaceProvider;

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
            _serializationConfiguration = new SerializationConfiguration(ObjectSpace.Session)
            {
                TypeToSerialize = CustomerType,
                SerializationConfigurationGroup = new SerializationConfigurationGroup(Session)
            };
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
            _serializationConfiguration.SerializationGraph.Where(node => node.Name == "User").Single().SerializationStrategy = SerializationStrategy.DoNotSerialize;
            ObjectSpace.CommitChanges();
        };

        Because of = () => {
            _xDocument = new ExportEngine().Export(new List<XPBaseObject> { _customer }, _serializationConfiguration.SerializationConfigurationGroup);
            _xDocument.Save("Customer.xml");
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
            () => _customerElement.ObjectKeyProperties().Count().ShouldEqual(1);

        It should_have_0_object_property_with_value_the_oid_of_user=() => {
            var objectProperties = _customerElement.Properties(NodeType.Object);
            objectProperties.Count().ShouldEqual(0);
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
    public class When_exporting_with_reference_object_with_no_existent_configuration:With_Isolations {
        static SerializationConfigurationGroup _serializationConfigurationGroup;
        static Type _customerType;
        static ObjectSpace _objectSpace;
        static XPBaseObject _order;
        static XElement _root;
        static SerializationConfiguration _serializationConfiguration;

        Establish context = () => {
            var modelBuilder = ModelBuilder<ICustomer,IOrder>.Build();
            var oneToMany = modelBuilder.OneToMany();
            _customerType = oneToMany.T1Type;
            var modelInstancesHandler = oneToMany.CreateInstances();
            _order = (XPBaseObject) modelInstancesHandler.T2Instance;
            _objectSpace = oneToMany.ObjectSpace;
            _serializationConfigurationGroup = _objectSpace.CreateObject<SerializationConfigurationGroup>();
            _serializationConfiguration = new SerializationConfiguration(_objectSpace.Session)
            {
                TypeToSerialize = _order.GetType(),
                SerializationConfigurationGroup = _serializationConfigurationGroup
            };
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
            _objectSpace.CommitChanges();
            _objectSpace.Session.FindObject<SerializationConfiguration>(configuration => configuration.TypeToSerialize==_customerType).Delete();
            _objectSpace.Session.FindObject<SerializationConfiguration>(configuration => configuration.TypeToSerialize==_order.GetType()).Delete();
            _objectSpace.CommitChanges();
        };

        Because of = () => new ExportEngine().Export(new List<XPBaseObject> { _order }, _serializationConfigurationGroup);

        It should_create_the_non_existent_configuration =
            () => {
                _objectSpace.Session.FindObject<SerializationConfiguration>(
                    PersistentCriteriaEvaluationBehavior.InTransaction,
                    configuration => configuration.TypeToSerialize == _customerType).IsDeleted.ShouldBeFalse();
                _objectSpace.Session.FindObject<SerializationConfiguration>(
                    PersistentCriteriaEvaluationBehavior.InTransaction,
                    configuration => configuration.TypeToSerialize == _order.GetType()).IsDeleted.ShouldBeFalse();
            };
    }
    [Subject(typeof(ExportEngine))]
    public class When_exporting_object_with_Null_referenced_object:With_Isolations {
        static XElement _property;
        static XElement _root;
        static SerializationConfiguration _serializationConfiguration;
        static XPBaseObject _customer;

        Establish context = () => {
            var objectSpace = ObjectSpaceInMemory.CreateNew();

            PersistentAssemblyBuilder persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(objectSpace, GetUniqueAssemblyName());
            IClassInfoHandler classInfoHandler = persistentAssemblyBuilder.CreateClasses(new[]{"Customer"});
            classInfoHandler.CreateReferenceMembers(info => new[]{typeof(User)});            
            objectSpace.CommitChanges();
            Type compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));
            var customerType = compileModule.Assembly.GetTypes().Where(type => type.Name == "Customer").Single();
            _customer = (XPBaseObject) objectSpace.CreateObject(customerType);
            objectSpace.CommitChanges();
            _serializationConfiguration = new SerializationConfiguration(objectSpace.Session)
            {
                TypeToSerialize = customerType,
                SerializationConfigurationGroup = objectSpace.CreateObject<SerializationConfigurationGroup>()
            };
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
        };


        Because of = () => {
            XDocument document = new ExportEngine().Export(new[]{_customer}, _serializationConfiguration.SerializationConfigurationGroup);
            _root = document.Root;
        };

        It should_create_a_property_with_name_same_as_referenced_object_type =
            () => {
                _property = _root.SerializedObjects(_customer.GetType()).ObjectProperty(typeof (User));
                _property.ShouldNotBeNull();
            };
        It should_set_null_value_to_that_property=() => _property.Value.ShouldEqual(String.Empty);
    }
    [Subject(typeof(ExportEngine))]
    public class When_a_many_to_many_collection_has_non_serialize_strategy : With_Isolations
    {
        static SerializationConfigurationGroup _serializationConfigurationGroup;
        static XElement _root;
        static ObjectSpace _objectSpace;
        static SerializationConfiguration _serializationConfiguration;
        static XPBaseObject _customer;

        Establish context = () => {
            var typeHandler = ModelBuilder<ICustomer, IOrder>.Build().ManyToMany();
            var modelInstancesHandler = typeHandler.CreateInstances();
            _objectSpace = typeHandler.ObjectSpace;
            _customer = (XPBaseObject)modelInstancesHandler.T1instance;
            _serializationConfiguration = new SerializationConfiguration(_objectSpace.Session) { TypeToSerialize = _customer.GetType() };
            _serializationConfigurationGroup = _objectSpace.CreateObject<SerializationConfigurationGroup>();
            _serializationConfiguration.SerializationConfigurationGroup=_serializationConfigurationGroup;
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
            
            var classInfoGraphNode = _serializationConfiguration.SerializationGraph.Where(node => node.Name=="Orders").Single();
            classInfoGraphNode.SerializationStrategy=SerializationStrategy.DoNotSerialize;
            _objectSpace.CommitChanges();
            
        };

        Because of = () => {
            var xDocument = new ExportEngine().Export(new[] { _customer }, _serializationConfigurationGroup);
            _root = xDocument.Root;
        };

        It should_not_create_an_property_element_with_the_name_of_the_collection =
            () => _root.SerializedObjects(_customer.GetType()).Properties(NodeType.Collection).Count().ShouldEqual(0);
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
            _serializationConfiguration = new SerializationConfiguration(_objectSpace.Session)
            {
                TypeToSerialize = _customer.GetType(),
                SerializationConfigurationGroup = _objectSpace.CreateObject<SerializationConfigurationGroup>()
            };
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
            _objectSpace.CommitChanges();
        };

        Because of = () => {
            var xDocument = new ExportEngine().Export(new[] {_customer}, _serializationConfiguration.SerializationConfigurationGroup);
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
    [Subject(typeof(ExportEngine))][Ignore]
    public class When_exporting_an_object_with_value_converter:With_Isolations {
        static ModelDifferenceObject _differenceObject;
        static XElement _root;
        static SerializationConfiguration _serializationConfiguration;


        Establish context = () => {
            var objectSpace = new ObjectSpaceProvider(new MemoryDataStoreProvider()).CreateObjectSpace();
            _differenceObject = Isolate.Fake.Instance<ModelDifferenceObject>(Members.CallOriginal,ConstructorWillBe.Called,objectSpace.Session);
//            _differenceObject.Model=new Dictionary(new DictionaryNode("dictionaryXmlValue"),new Schema(new DictionaryNode("shemaNode")));
            _serializationConfiguration = new SerializationConfiguration(objectSpace.Session) { TypeToSerialize = _differenceObject.GetType() };
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
        };

        Because of = () => {
//            _root = new ExportEngine().Export(new[] {_differenceObject}, null).Root;
        };

        It should_export_the_storage_converter_value =
            () => {
//                var serializedObjects = _root.SerializedObjects(_differenceObject.GetType());
//                var value = serializedObjects.Properties(NodeType.Simple).Property(_differenceObject.GetPropertyName(x => x.ModelApplication)).Value;
//                value.ShouldContain("dictionaryXmlValue");

            };    
    }

    public class When_group_does_not_have_a_configuration_for_the_object_type:With_Isolations {
        static SerializationConfigurationGroup _serializationConfigurationGroup;
        static ObjectSpace _objectSpace;
        static XPBaseObject _customer;

        Establish context = () => {
            var persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(_objectSpace, GetUniqueAssemblyName());
            persistentAssemblyBuilder.CreateClasses(new[] { "Customer" });
            _objectSpace = persistentAssemblyBuilder.ObjectSpace;
            _objectSpace.CommitChanges();
            Type compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));
            var customerType = compileModule.Assembly.GetTypes().Where(type => type.Name == "Customer").Single();
            _customer = (XPBaseObject)_objectSpace.CreateObject(customerType);
            _serializationConfigurationGroup = _objectSpace.CreateObject<SerializationConfigurationGroup>();
        };

        Because of = () => new ExportEngine().Export(new[]{_customer},_serializationConfigurationGroup);

        It should_create_a_configuration =
            () => _objectSpace.FindObject<SerializationConfiguration>(null, true).ShouldNotBeNull();

        It should_add_it_to_the_serialization_group =
            () => _serializationConfigurationGroup.SerializationConfigurations.Count.ShouldEqual(1);
    }
    [Subject(typeof(ExportEngine))]
    public class When_exporting_a_type_with_a_simple_property_not_serializable:With_Isolations {
        static XElement _root;
        static XPBaseObject _customer;
        static ObjectSpace _objectSpace;
        static SerializationConfiguration _serializationConfiguration;

        Establish context = () => {
            var persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(_objectSpace, GetUniqueAssemblyName());
            persistentAssemblyBuilder.CreateClasses(new[] {"Customer"});
            _objectSpace = persistentAssemblyBuilder.ObjectSpace;
            _objectSpace.CommitChanges();
            Type compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));
            var customerType = compileModule.Assembly.GetTypes().Where(type => type.Name == "Customer").Single();
            _customer = (XPBaseObject) _objectSpace.CreateObject(customerType);
            _serializationConfiguration = new SerializationConfiguration(_objectSpace.Session)
            {
                TypeToSerialize = customerType,
                SerializationConfigurationGroup = _objectSpace.CreateObject<SerializationConfigurationGroup>()
            };
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
            _serializationConfiguration.SerializationGraph.Where(node => node.Name=="oid").Single().SerializationStrategy=SerializationStrategy.DoNotSerialize;
        };

        Because of = () => {
            var document = new ExportEngine().Export(new []{_customer}, _serializationConfiguration.SerializationConfigurationGroup);
            _root = document.Root;
        };

        It should_not_serialize_that_proparty =
            () => _root.SerializedObjects(_customer.GetType()).Properties(NodeType.Simple).Count().ShouldEqual(0);
    }

    [Subject(typeof(ExportEngine))]
    public class When_exporting_customer_orders_persistent_assembly_info:With_Isolations {
        static XElement _root;
        static SerializationConfiguration _serializationConfiguration;
        static IPersistentAssemblyInfo _persistentAssemblyInfo;

        Establish context = () => {
            var modelBuilder = ModelBuilder<ICustomer,IOrder>.Build();
            modelBuilder.OneToMany();
            _persistentAssemblyInfo = modelBuilder.PersistentAssemblyBuilder.PersistentAssemblyInfo;
            var session = _persistentAssemblyInfo.Session;
            _serializationConfiguration = new SerializationConfiguration(session)
            {
                TypeToSerialize = typeof(PersistentAssemblyInfo),
                SerializationConfigurationGroup = new SerializationConfigurationGroup(session)
            };
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
            ObjectSpace.FindObjectSpace(_persistentAssemblyInfo).CommitChanges();
        };

        Because of = () => {
            _root = new ExportEngine().Export(new[] { _persistentAssemblyInfo }.OfType<XPBaseObject>(), ObjectSpace.FindObjectSpace(_persistentAssemblyInfo).CreateObject<SerializationConfigurationGroup>()).Root;
        };

        It should_contain_serialized_element_of_persistentAssemblyInfo_type =
            () => _root.SerializedObjects(typeof (PersistentAssemblyInfo)).Count().ShouldEqual(1);

        It should_create_2_persistentassociation_root_serialized_elements =
            () => _root.SerializedObjects(typeof (PersistentAssociationAttribute)).Count().ShouldEqual(2);

        It should_create_all_properties_of_persistentAssociation_attribute =
            () =>
            _root.SerializedObjects(typeof (PersistentAssociationAttribute)).FirstOrDefault().Properties().Count().ShouldEqual(7);

    }

    [Subject(typeof(ExportEngine),"Non Default Keys")]
    public class When_objects_has_a_reference_object_as_key_that_has_non_default_key {
        It should_set_the_value_of_ref_object_non_default_key_as_valye_of_SerializedObjectRef_Key_element;
    }
    
    [Subject(typeof(ExportEngine))]
    public class When_object_has_a_byte_array_property:With_Isolations {
        static string _xml;
        static XElement _root;
        static Analysis _analysis;

        Establish context = () => {
            ObjectSpace objectSpace = ObjectSpaceInMemory.CreateNew();
            _analysis = objectSpace.CreateObject<Analysis>();
            Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("eXpand.Tests.eXpand.IO.Resources.PivotContent.xml");
            if (manifestResourceStream != null) {
                _xml = new StreamReader(manifestResourceStream).ReadToEnd();
                _analysis.PivotGridSettingsContent = Encoding.UTF8.GetBytes(_xml);
            }
        };

        Because of = () => {
            var xDocument = new ExportEngine().Export(new List<XPBaseObject> { _analysis }, ObjectSpace.FindObjectSpace(_analysis).CreateObject<SerializationConfigurationGroup>());
            xDocument.Save(@"c:\my.xml");
            _root = xDocument.Root;
        };

        It should_serialize_bytes_to_xml =
            () => {
                string value =
                    _root.SerializedObjects(typeof (Analysis)).FirstOrDefault().Property("PivotGridSettingsContent").
                        Value;
                Encoding.UTF8.GetString(Convert.FromBase64String(value)).ShouldEqual(_xml);
            };
    }

    [Subject(typeof(ExportEngine))]
    public class When_object_has_An_image_property:With_Isolations {
        static ObjectSpace _objectSpace;
        static Color _color;
        static XElement _root;
        static ImagePropertyObject _imagePropertyObject;

        Establish context = () => {
            _objectSpace = ObjectSpaceInMemory.CreateNew();
            _imagePropertyObject = _objectSpace.CreateObject<ImagePropertyObject>();
            var bitmap = new Bitmap(1,1);
            bitmap.SetPixel(0,0,Color.Red);
            _color = bitmap.GetPixel(0,0);

            _imagePropertyObject.Photo = bitmap;
            _objectSpace.CommitChanges();
            _imagePropertyObject.Reload();
        };

        Because of = () => {
            _root = new ExportEngine().Export(new[]{_imagePropertyObject}, _objectSpace.CreateObject<SerializationConfigurationGroup>()).Root;
        };

        It should_serialize_the_image_property =
            () => {
                var serializedObject = _root.SerializedObjects(typeof (ImagePropertyObject)).FirstOrDefault();
                var xElement = serializedObject.Property("Photo");
                var bytes = Convert.FromBase64String(xElement.Value);
                var image = Image.FromStream(new MemoryStream(bytes));
                image.Width.ShouldEqual(1);
                image.Height.ShouldEqual(1);
                var bitmap = new Bitmap(image);
                bitmap.GetPixel(0,0).ShouldEqual(_color);
            };
    }


    [Subject(typeof(ExportEngine))]
    public class When_exporting_object_with_date_time_property:With_Isolations {
        static ObjectSpace _objectSpace;
        static XElement _root;
        static DateTimePropertyObject _dateTimePropertyObject;
        static DateTime _dateTime;

        Establish context = () => {
            _objectSpace = ObjectSpaceInMemory.CreateNew();
            _dateTime = DateTime.Now;
            _dateTimePropertyObject = _objectSpace.CreateObject<DateTimePropertyObject>();
            _dateTimePropertyObject.Date = _dateTime;
            _objectSpace.CommitChanges();
        };

        Because of = () => {
            _root = new ExportEngine().Export(new[]{_dateTimePropertyObject}, _objectSpace.CreateObject<SerializationConfigurationGroup>()).Root;
        };

        It should_export_the_full_date_time = () => {
            var serializedObject = _root.SerializedObjects(typeof(DateTimePropertyObject)).ToList()[0];
            _dateTime.Ticks.ToString().ShouldEqual(serializedObject.Property("Date").Value);
        };
    }
}
