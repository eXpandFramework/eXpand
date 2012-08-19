using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata.Helpers;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;
using Xpand.ExpressApp.IO.Core;
using Xpand.ExpressApp.IO.PersistentTypesHelpers;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using Xpand.Persistent.Base.ImportExport;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.BaseImpl.ImportExport;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using Xpand.Xpo;


namespace Xpand.Tests.Xpand.IO {
    [Subject(typeof(ExportEngine))]
    public class When_Exporting_1_Customer_with_1_ref_User_2_Orders_and_user_not_serialized : With_Customer_Orders {
        private const int GcRecord = 1;
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
            _user = (XPBaseObject)XPObjectSpace.CreateObject(typeof(User));
            _customer = (XPBaseObject)XPObjectSpace.CreateObject(CustomerType);
            _customer.SetMemberValue("Name", "CustomerName");
            _customer.SetMemberValue("User", _user);
            _order1 = (XPBaseObject)XPObjectSpace.CreateObject(OrderType);
            _order1.SetMemberValue("Customer", _customer);
            _order2 = (XPBaseObject)XPObjectSpace.CreateObject(OrderType);
            _order2.SetMemberValue("Customer", _customer);
            _serializationConfiguration = new SerializationConfiguration(XPObjectSpace.Session) {
                TypeToSerialize = CustomerType,
                SerializationConfigurationGroup = new SerializationConfigurationGroup(UnitOfWork)
            };
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
            _serializationConfiguration.SerializationGraph.Single(node => node.Name == "User").SerializationStrategy = SerializationStrategy.DoNotSerialize;
            XPObjectSpace.CommitChanges();
        };

        Because of = () => {
            _xDocument = new ExportEngine().Export(new List<XPBaseObject> { _customer }, _serializationConfiguration.SerializationConfigurationGroup);
            _xDocument.Save("Customer.xml");
        };

        It should_create_an_xml_document = () => {
            _xDocument.ShouldNotBeNull();
            _root = _xDocument.Root;
        };

        It should_have_serializedObjects_as_root_element = () => _root.Name.ShouldEqual("SerializedObjects");

        It should_have_2_Orders_an_1_Customer_serialized_elements_as_childs_under_root = () => {
            _root.SerializedObjects(_customer.GetType()).Count().ShouldEqual(1);
            _root.SerializedObjects(_order1.GetType()).Count().ShouldEqual(2);
        };

        It should_not_have_User_child_Serialized_element_under_root = () => _root.SerializedObjects().Count().ShouldEqual(3);

        It should_have_2_simple_property_elements_as_customer_Serialized_element_childs = () => {

            _customerElement = _root.SerializedObjects(_customer.GetType()).Single();
            _customerElement.Properties(NodeType.Simple).Count().ShouldEqual(2 + GcRecord);
        };


        It should_have_1_key_property_element_as_customer_Serialized_element_child =
            () => _customerElement.ObjectKeyProperties().Count().ShouldEqual(1);

        It should_have_0_object_property_with_value_the_oid_of_user = () => {
            var objectProperties = _customerElement.Properties(NodeType.Object);
            objectProperties.Count().ShouldEqual(0);
        };

        It should_have_1_collection_property__with_name_Orders_as_Serialized_element_child = () => {
            _ordersElement = _customerElement.Properties(NodeType.Collection).Property("Orders");
            _ordersElement.ShouldNotBeNull();
        };

        It should_have_2_ref_properties_with_serialized_strategy_and_value_under_orders_property_collection_element = () => {
            var serializedObjectRefs = _ordersElement.SerializedObjectRefs(_order1.GetType());
            var objectRefs = serializedObjectRefs.SerializedObjectRefs(SerializationStrategy.SerializeAsObject).ToList();
            var count = objectRefs.Count();
            count.ShouldEqual(2);
            objectRefs.FirstOrDefault(xElement => xElement.Value == _order1.GetMemberValue("Oid").ToString()).ShouldNotBeNull();
        };
    }

    [Subject(typeof(ExportEngine))]
    public class When_exporting_with_reference_object_with_no_existent_configuration : With_Isolations {
        static SerializationConfigurationGroup _serializationConfigurationGroup;
        static Type _customerType;
        static XPObjectSpace _XPObjectSpace;
        static XPBaseObject _order;
        static XElement _root;
        static SerializationConfiguration _serializationConfiguration;

        Establish context = () => {
            var modelBuilder = ModelBuilder<ICustomer, IOrder>.Build();
            var oneToMany = modelBuilder.OneToMany();
            _customerType = oneToMany.T1Type;
            var modelInstancesHandler = oneToMany.CreateInstances();
            _order = (XPBaseObject)modelInstancesHandler.T2Instance;
            _XPObjectSpace = oneToMany.XPObjectSpace;
            _serializationConfigurationGroup = _XPObjectSpace.CreateObject<SerializationConfigurationGroup>();
            _serializationConfiguration = new SerializationConfiguration(_XPObjectSpace.Session) {
                TypeToSerialize = _order.GetType(),
                SerializationConfigurationGroup = _serializationConfigurationGroup
            };
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
            _XPObjectSpace.CommitChanges();
            _XPObjectSpace.Session.FindObject<SerializationConfiguration>(configuration => configuration.TypeToSerialize == _customerType).Delete();
            _XPObjectSpace.Session.FindObject<SerializationConfiguration>(configuration => configuration.TypeToSerialize == _order.GetType()).Delete();
            _XPObjectSpace.CommitChanges();
        };

        Because of = () => new ExportEngine().Export(new List<XPBaseObject> { _order }, _serializationConfigurationGroup);

        It should_create_the_non_existent_configuration =
            () => {
                _XPObjectSpace.Session.FindObject<SerializationConfiguration>(
                    PersistentCriteriaEvaluationBehavior.InTransaction,
                    configuration => configuration.TypeToSerialize == _customerType).IsDeleted.ShouldBeFalse();
                _XPObjectSpace.Session.FindObject<SerializationConfiguration>(
                    PersistentCriteriaEvaluationBehavior.InTransaction,
                    configuration => configuration.TypeToSerialize == _order.GetType()).IsDeleted.ShouldBeFalse();
            };
    }
    [Subject(typeof(ExportEngine))]
    public class When_exporting_object_with_Null_referenced_object : With_Isolations {
        static XElement _property;
        static XElement _root;
        static SerializationConfiguration _serializationConfiguration;
        static XPBaseObject _customer;

        Establish context = () => {
            var XPObjectSpace = ((XPObjectSpace)ObjectSpaceInMemory.CreateNew());

            PersistentAssemblyBuilder persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(XPObjectSpace, GetUniqueAssemblyName());
            IClassInfoHandler classInfoHandler = persistentAssemblyBuilder.CreateClasses(new[] { "Customer" });
            classInfoHandler.CreateReferenceMembers(info => new[] { typeof(User) });
            XPObjectSpace.CommitChanges();
            Type compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));
            var customerType = compileModule.Assembly.GetTypes().Single(type => type.Name == "Customer");
            _customer = (XPBaseObject)XPObjectSpace.CreateObject(customerType);
            XPObjectSpace.CommitChanges();
            _serializationConfiguration = new SerializationConfiguration(XPObjectSpace.Session) {
                TypeToSerialize = customerType,
                SerializationConfigurationGroup = XPObjectSpace.CreateObject<SerializationConfigurationGroup>()
            };
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
        };


        Because of = () => {
            XDocument document = new ExportEngine().Export(new[] { _customer }, _serializationConfiguration.SerializationConfigurationGroup);
            _root = document.Root;
        };

        It should_create_a_property_with_name_same_as_referenced_object_type =
            () => {
                _property = _root.SerializedObjects(_customer.GetType()).Single().ObjectProperty("User");
                _property.ShouldNotBeNull();
            };
        It should_set_null_value_to_that_property = () => _property.Value.ShouldEqual(String.Empty);
    }
    [Subject(typeof(ExportEngine))]
    public class When_a_many_to_many_collection_has_non_serialize_strategy : With_Isolations {
        static SerializationConfigurationGroup _serializationConfigurationGroup;
        static XElement _root;
        static XPObjectSpace _XPObjectSpace;
        static SerializationConfiguration _serializationConfiguration;
        static XPBaseObject _customer;

        Establish context = () => {
            var typeHandler = ModelBuilder<ICustomer, IOrder>.Build().ManyToMany();
            var modelInstancesHandler = typeHandler.CreateInstances();
            _XPObjectSpace = typeHandler.XPObjectSpace;
            _customer = (XPBaseObject)modelInstancesHandler.T1instance;
            _serializationConfiguration = new SerializationConfiguration(_XPObjectSpace.Session) { TypeToSerialize = _customer.GetType() };
            _serializationConfigurationGroup = _XPObjectSpace.CreateObject<SerializationConfigurationGroup>();
            _serializationConfiguration.SerializationConfigurationGroup = _serializationConfigurationGroup;
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);

            var classInfoGraphNode = _serializationConfiguration.SerializationGraph.Single(node => node.Name == "Orders");
            classInfoGraphNode.SerializationStrategy = SerializationStrategy.DoNotSerialize;
            _XPObjectSpace.CommitChanges();

        };

        Because of = () => {
            var xDocument = new ExportEngine().Export(new[] { _customer }, _serializationConfigurationGroup);
            _root = xDocument.Root;
        };

        It should_not_create_an_property_element_with_the_name_of_the_collection =
            () => _root.SerializedObjects(_customer.GetType()).Properties(NodeType.Collection).Count().ShouldEqual(0);
    }
    [Subject(typeof(ExportEngine))]
    public class When_exporting_Customers_Orders_many_to_many : With_Isolations {
        static XElement _customersElement;
        static XElement _orderElement;
        static XElement _ordersElement;
        static XPBaseObject _order;
        static XElement _customerElement;
        static XElement _root;
        static SerializationConfiguration _serializationConfiguration;
        static XPBaseObject _customer;
        static XPObjectSpace _XPObjectSpace;

        Establish context = () => {
            var typeHandler = ModelBuilder<ICustomer, IOrder>.Build().ManyToMany();
            var modelInstancesHandler = typeHandler.CreateInstances();
            _customer = (XPBaseObject)modelInstancesHandler.T1instance;
            _order = (XPBaseObject)modelInstancesHandler.T2Instance;
            _XPObjectSpace = typeHandler.XPObjectSpace;
            _serializationConfiguration = new SerializationConfiguration(_XPObjectSpace.Session) {
                TypeToSerialize = _customer.GetType(),
                SerializationConfigurationGroup = _XPObjectSpace.CreateObject<SerializationConfigurationGroup>()
            };
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
            _XPObjectSpace.CommitChanges();
        };

        Because of = () => {
            var xDocument = new ExportEngine().Export(new[] { _customer }, _serializationConfiguration.SerializationConfigurationGroup);
            _root = xDocument.Root;
        };

        It should_create_2_serialized_elements = () => _root.SerializedObjects().Count().ShouldEqual(2);

        It should_create_a_customer_root_element = () => {
            _customerElement = _root.SerializedObjects(_customer.GetType()).FirstOrDefault();
            _customerElement.ShouldNotBeNull();
        };

        It should_create_a_collection_property_with_order_type_and_name_orders_under_customer_element = () => {
            _ordersElement = _customerElement.Properties(NodeType.Collection).Property("Orders");
            _ordersElement.ShouldNotBeNull();
        };

        It should_create_a_ref_object_of_type_order_under_orders_collection_property =
            () => _ordersElement.SerializedObjectRefs(_order.GetType()).FirstOrDefault().ShouldNotBeNull();

        It should_create_a_order_root_element = () => {
            _orderElement = _root.SerializedObjects(_order.GetType()).FirstOrDefault();
            _orderElement.ShouldNotBeNull();
        };

        It should_not_create_a_collection_property_with_customer_type_and_name_customers_under_order_element = () => {
            _customersElement = _ordersElement.Properties(NodeType.Collection).Property("Customers");
            _customersElement.ShouldBeNull();
        };
    }
    [Subject(typeof(ExportEngine))]
    [Ignore("Not implemented")]
    public class When_exporting_an_object_with_value_converter : With_Isolations {
        static ModelDifferenceObject _differenceObject;
        static XElement _root;
        static SerializationConfiguration _serializationConfiguration;


        Establish context = () => {
            var XPObjectSpace = ((XPObjectSpace)new XPObjectSpaceProvider(new MemoryDataStoreProvider()).CreateObjectSpace());
            _differenceObject = Isolate.Fake.Instance<ModelDifferenceObject>(Members.CallOriginal, ConstructorWillBe.Called, XPObjectSpace.Session);
            //            _differenceObject.Model=new Dictionary(new DictionaryNode("dictionaryXmlValue"),new Schema(new DictionaryNode("shemaNode")));
            _serializationConfiguration = new SerializationConfiguration(XPObjectSpace.Session) { TypeToSerialize = _differenceObject.GetType() };
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

    public class When_group_does_not_have_a_configuration_for_the_object_type : With_Isolations {
        static SerializationConfigurationGroup _serializationConfigurationGroup;
        static XPObjectSpace _XPObjectSpace;
        static XPBaseObject _customer;

        Establish context = () => {
            _XPObjectSpace = (XPObjectSpace)ObjectSpaceInMemory.CreateNew();
            var persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(_XPObjectSpace, GetUniqueAssemblyName());
            persistentAssemblyBuilder.CreateClasses(new[] { "Customer" });
            _XPObjectSpace = (XPObjectSpace)persistentAssemblyBuilder.ObjectSpace;
            _XPObjectSpace.CommitChanges();
            Type compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));
            var customerType = compileModule.Assembly.GetTypes().Single(type => type.Name == "Customer");
            _customer = (XPBaseObject)_XPObjectSpace.CreateObject(customerType);
            _serializationConfigurationGroup = _XPObjectSpace.CreateObject<SerializationConfigurationGroup>();
        };

        Because of = () => new ExportEngine().Export(new[] { _customer }, _serializationConfigurationGroup);

        It should_create_a_configuration =
            () => _XPObjectSpace.FindObject<SerializationConfiguration>(null, true).ShouldNotBeNull();

        It should_add_it_to_the_serialization_group =
            () => _serializationConfigurationGroup.SerializationConfigurations.Count.ShouldEqual(1);
    }
    [Subject(typeof(ExportEngine))]
    public class When_exporting_a_type_with_a_simple_property_not_serializable : With_Isolations {
        private const int GcRecord = 1;
        static XElement _root;
        static XPBaseObject _customer;
        static XPObjectSpace _XPObjectSpace;
        static SerializationConfiguration _serializationConfiguration;

        Establish context = () => {
            _XPObjectSpace = (XPObjectSpace)ObjectSpaceInMemory.CreateNew();
            var persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(_XPObjectSpace, GetUniqueAssemblyName());
            persistentAssemblyBuilder.CreateClasses(new[] { "Customer" });
            _XPObjectSpace = (XPObjectSpace)persistentAssemblyBuilder.ObjectSpace;
            _XPObjectSpace.CommitChanges();
            Type compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));
            var customerType = compileModule.Assembly.GetTypes().Single(type => type.Name == "Customer");
            _customer = (XPBaseObject)_XPObjectSpace.CreateObject(customerType);
            _serializationConfiguration = new SerializationConfiguration(_XPObjectSpace.Session) {
                TypeToSerialize = customerType,
                SerializationConfigurationGroup = _XPObjectSpace.CreateObject<SerializationConfigurationGroup>()
            };
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
            _serializationConfiguration.SerializationGraph.Single(node => node.Name == "Oid").SerializationStrategy = SerializationStrategy.DoNotSerialize;
        };

        Because of = () => {
            var document = new ExportEngine().Export(new[] { _customer }, _serializationConfiguration.SerializationConfigurationGroup);
            _root = document.Root;
        };

        It should_not_serialize_that_proparty =
            () => _root.SerializedObjects(_customer.GetType()).Properties(NodeType.Simple).Count().ShouldEqual(GcRecord);
    }

    [Subject(typeof(ExportEngine))]
    public class When_exporting_customer_orders_persistent_assembly_info : With_Isolations {
        private const int GcRecord = 1;
        static XElement _root;
        static SerializationConfiguration _serializationConfiguration;
        static IPersistentAssemblyInfo _persistentAssemblyInfo;

        Establish context = () => {
            var modelBuilder = ModelBuilder<ICustomer, IOrder>.Build();
            modelBuilder.OneToMany();
            _persistentAssemblyInfo = modelBuilder.PersistentAssemblyBuilder.PersistentAssemblyInfo;
            var session = _persistentAssemblyInfo.Session;
            _serializationConfiguration = new SerializationConfiguration(session) {
                TypeToSerialize = typeof(PersistentAssemblyInfo),
                SerializationConfigurationGroup = new SerializationConfigurationGroup(session)
            };
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
            XPObjectSpace.FindObjectSpaceByObject(_persistentAssemblyInfo).CommitChanges();
        };

        Because of = () => {
            _root = new ExportEngine().Export(new[] { _persistentAssemblyInfo }.OfType<XPBaseObject>(), XPObjectSpace.FindObjectSpaceByObject(_persistentAssemblyInfo).CreateObject<SerializationConfigurationGroup>()).Root;
        };

        It should_contain_serialized_element_of_persistentAssemblyInfo_type =
            () => _root.SerializedObjects(typeof(PersistentAssemblyInfo)).Count().ShouldEqual(1);

        It should_create_2_persistentassociation_root_serialized_elements =
            () => _root.SerializedObjects(typeof(PersistentAssociationAttribute)).Count().ShouldEqual(2);

        It should_create_all_properties_of_persistentAssociation_attribute =
            () =>
            _root.SerializedObjects(typeof(PersistentAssociationAttribute)).FirstOrDefault().Properties().Count().ShouldEqual(7 + GcRecord);

    }

    [Subject(typeof(ExportEngine), "Non Default Keys")]
    public class When_objects_has_a_reference_object_as_key_that_has_non_default_key {
        It should_set_the_value_of_ref_object_non_default_key_as_valye_of_SerializedObjectRef_Key_element;
    }

    [Subject(typeof(ExportEngine))]
    public class When_object_has_a_byte_array_property : With_Isolations {
        static string _xml;
        static XElement _root;
        static Analysis _analysis;

        Establish context = () => {
            var XPObjectSpace = (XPObjectSpace)ObjectSpaceInMemory.CreateNew();
            _analysis = XPObjectSpace.CreateObject<Analysis>();
            Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Xpand.Tests.Xpand.IO.Resources.PivotContent.xml");
            if (manifestResourceStream != null) {
                _xml = new StreamReader(manifestResourceStream).ReadToEnd();
                _analysis.PivotGridSettingsContent = Encoding.UTF8.GetBytes(_xml);
            }
        };

        Because of = () => {
            var xDocument = new ExportEngine().Export(new List<XPBaseObject> { _analysis }, XPObjectSpace.FindObjectSpaceByObject(_analysis).CreateObject<SerializationConfigurationGroup>());
            xDocument.Save(@"c:\my.xml");
            _root = xDocument.Root;
        };

        It should_serialize_bytes_to_xml =
            () => {
                string value =
                    _root.SerializedObjects(typeof(Analysis)).FirstOrDefault().Property("PivotGridSettingsContent").
                        Value;

                Encoding.UTF8.GetString(Convert.FromBase64String(value)).ShouldEqual(_xml);
            };
    }

    [Subject(typeof(ExportEngine))]
    public class When_object_has_An_image_property : With_Isolations {
        static XPObjectSpace _XPObjectSpace;
        static Color _color;
        static XElement _root;
        static ImagePropertyObject _imagePropertyObject;

        Establish context = () => {
            _XPObjectSpace = (XPObjectSpace)ObjectSpaceInMemory.CreateNew();
            _imagePropertyObject = _XPObjectSpace.CreateObject<ImagePropertyObject>();
            var bitmap = new Bitmap(1, 1);
            bitmap.SetPixel(0, 0, Color.Red);
            _color = bitmap.GetPixel(0, 0);

            _imagePropertyObject.Photo = bitmap;
            _XPObjectSpace.CommitChanges();
            _imagePropertyObject.Reload();
        };

        Because of = () => {
            _root = new ExportEngine().Export(new[] { _imagePropertyObject }, _XPObjectSpace.CreateObject<SerializationConfigurationGroup>()).Root;
        };

        It should_serialize_the_image_property =
            () => {
                var serializedObject = _root.SerializedObjects(typeof(ImagePropertyObject)).FirstOrDefault();
                var xElement = serializedObject.Property("Photo");
                var bytes = Convert.FromBase64String(xElement.Value);
                var image = Image.FromStream(new MemoryStream(bytes));
                image.Width.ShouldEqual(1);
                image.Height.ShouldEqual(1);
                var bitmap = new Bitmap(image);
                bitmap.GetPixel(0, 0).ShouldEqual(_color);
            };
    }


    [Subject(typeof(ExportEngine))]
    public class When_exporting_object_with_date_time_property : With_Isolations {
        static XPObjectSpace _XPObjectSpace;
        static XElement _root;
        static DateTimePropertyObject _dateTimePropertyObject;
        static DateTime _dateTime;

        Establish context = () => {
            _XPObjectSpace = (XPObjectSpace)ObjectSpaceInMemory.CreateNew();
            _dateTime = DateTime.Now;
            _dateTimePropertyObject = _XPObjectSpace.CreateObject<DateTimePropertyObject>();
            _dateTimePropertyObject.Date = _dateTime;
            _XPObjectSpace.CommitChanges();
        };

        Because of = () => {
            _root = new ExportEngine().Export(new[] { _dateTimePropertyObject }, _XPObjectSpace.CreateObject<SerializationConfigurationGroup>()).Root;
        };

        It should_export_the_full_date_time = () => {
            var serializedObject = _root.SerializedObjects(typeof(DateTimePropertyObject)).ToList()[0];
            _dateTime.Ticks.ToString(CultureInfo.InvariantCulture).ShouldEqual(serializedObject.Property("Date").Value);
        };
    }
    [Subject(typeof(ExportEngine))]
    public class When_object_property_is_null : With_Isolations {
        static XPObjectSpace _XPObjectSpace;

        static XDocument _document;
        static Analysis _analysis;

        Establish context = () => {
            _XPObjectSpace = (XPObjectSpace)ObjectSpaceInMemory.CreateNew();
            _analysis = _XPObjectSpace.CreateObject<Analysis>();
            _XPObjectSpace.CommitChanges();
        };

        Because of = () => {
            _document = new ExportEngine().Export(new[] { _analysis }, _XPObjectSpace.CreateObject<SerializationConfigurationGroup>());
        };

        It should_export_the_property_with_null_value = () => {
            var element = _document.Root.SerializedObjects(typeof(Analysis)).First();
            element.ObjectProperty(GCRecordField.StaticName).Value.ShouldEqual(string.Empty);
        };
    }
    [Subject(typeof(ExportEngine))]
    public class When_object_property_contains_quote : With_Isolations {
        static XPObjectSpace _XPObjectSpace;

        static XDocument _document;
        static Analysis _analysis;

        Establish context = () => {
            _XPObjectSpace = (XPObjectSpace)ObjectSpaceInMemory.CreateNew();
            _analysis = _XPObjectSpace.CreateObject<Analysis>();
            _analysis.Name = @"3211¬_M1";
            _XPObjectSpace.CommitChanges();
        };

        Because of = () => {
            _document = new ExportEngine().Export(new[] { _analysis }, _XPObjectSpace.CreateObject<SerializationConfigurationGroup>());
        };

        It should_export_the_property_with_null_value = () => {
            var element = _document.Root.SerializedObjects(typeof(Analysis)).First();
            element.ObjectProperty("Name").Value.ShouldEqual(@"3211¬_M1");

            var xmlWriterSettings = new XmlWriterSettings {
                OmitXmlDeclaration = true, Indent = true, NewLineChars = "\r\n", CloseOutput = true,
            };
            var outputFileName = new FileStream(@"c:\test2.xml", FileMode.Create, FileAccess.ReadWrite);
            using (XmlWriter textWriter = XmlWriter.Create(outputFileName, xmlWriterSettings)) {
                _document.Save(textWriter);
                textWriter.Close();
            }
        };
    }
    [Subject(typeof(ExportEngine))]
    public class When_object_property_is_IConvertable : With_Isolations {
        static SerializationConfiguration _serializationConfiguration;

        static XPBaseObject _customer;

        static XDocument _document;

        Establish context = () => {
            var XPObjectSpace = ((XPObjectSpace)ObjectSpaceInMemory.CreateNew());
            PersistentAssemblyBuilder persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(XPObjectSpace, GetUniqueAssemblyName());
            IClassInfoHandler classInfoHandler = persistentAssemblyBuilder.CreateClasses(new[] { "Customer" });
            classInfoHandler.CreateSimpleMembers(DBColumnType.Double, info => new[] { "Cost" });
            XPObjectSpace.CommitChanges();
            Type compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));
            var customerType = compileModule.Assembly.GetTypes().Single(type => type.Name == "Customer");
            _serializationConfiguration = new SerializationConfiguration(XPObjectSpace.Session) {
                TypeToSerialize = customerType,
                SerializationConfigurationGroup = XPObjectSpace.CreateObject<SerializationConfigurationGroup>()
            };
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
            _customer = (XPBaseObject)XPObjectSpace.CreateObject(customerType);
            _customer.SetMemberValue("Cost", 1.2);
        };

        Because of = () => {
            _document = new ExportEngine().Export(new[] { _customer }, _serializationConfiguration.SerializationConfigurationGroup);
        };

        It should_export_an_invariant_culture_formatted_value =
            () =>
            _document.Root.SerializedObjects(_customer.GetType()).Single().Property("Cost").Value.ShouldEqual(
                (1.2).ToString(CultureInfo.InvariantCulture));
    }

    public class When_Enum_values_already_existis_in_the_db : With_Isolations {
        static XPObjectSpace _XPObjectSpace;
        static XDocument _exportRecords;
        static PEnumClass _pEnumClass;



        Establish context = () => {
            _XPObjectSpace = CreateRecords();
            _exportRecords = ExportRecords(_XPObjectSpace);
            _XPObjectSpace = (XPObjectSpace)ObjectSpaceInMemory.CreateNew();
            new ImportEngine().ImportObjects(_exportRecords.ToString(), (UnitOfWork)_XPObjectSpace.Session);
            var pEnumClass = _XPObjectSpace.FindObject<PEnumClass>(null);
            pEnumClass.MyEnum = null;
            _XPObjectSpace.CommitChanges();
        };

        Because of = () => {
            new ImportEngine().ImportObjects(_exportRecords.ToString(), (UnitOfWork)_XPObjectSpace.Session);
            ((UnitOfWork)_XPObjectSpace.Session).CommitChanges();
            _XPObjectSpace.CommitChanges();
        };

        It should_should = () => _XPObjectSpace.FindObject<PEnumClass>(null).MyEnum.ShouldEqual(MyEnum.Val2);

        static XDocument ExportRecords(XPObjectSpace XPObjectSpace) {
            ISerializationConfiguration serializationConfiguration = XPObjectSpace.CreateObject<SerializationConfiguration>();
            serializationConfiguration.TypeToSerialize = typeof(PEnumClass);
            serializationConfiguration.SerializationConfigurationGroup =
                XPObjectSpace.CreateObject<SerializationConfigurationGroup>();
            new ClassInfoGraphNodeBuilder().Generate(serializationConfiguration);
            XDocument document = new ExportEngine().Export(new[] { _pEnumClass }, serializationConfiguration.SerializationConfigurationGroup);
            return document;
        }

        static XPObjectSpace CreateRecords() {
            var dataSet = new DataSet();
            var XPObjectSpace = ((XPObjectSpace)ObjectSpaceInMemory.CreateNew(dataSet));

            _pEnumClass = XPObjectSpace.CreateObject<PEnumClass>();
            _pEnumClass.MyEnum = MyEnum.Val2;
            XPObjectSpace.CommitChanges();
            return XPObjectSpace;
        }
    }

    class When : With_Isolations {
        static PEnumClass _pEnumClass;

        static XPObjectSpace CreateRecords() {
            XafTypesInfo.Instance.RegisterEntity(typeof(PEnumClass));
            var dataSet = new DataSet();
            var XPObjectSpace = ((XPObjectSpace)ObjectSpaceInMemory.CreateNew(dataSet));

            _pEnumClass = XPObjectSpace.CreateObject<PEnumClass>();
            XPObjectSpace.CommitChanges();
            return XPObjectSpace;
        }

        Establish context = () => {
            XPObjectSpace XPObjectSpace = CreateRecords();
            ISerializationConfiguration serializationConfiguration = XPObjectSpace.CreateObject<SerializationConfiguration>();
            serializationConfiguration.TypeToSerialize = typeof(PEnumClass);
            serializationConfiguration.SerializationConfigurationGroup =
                XPObjectSpace.CreateObject<SerializationConfigurationGroup>();
            new ClassInfoGraphNodeBuilder().Generate(serializationConfiguration);
            XDocument xDocument = new ExportEngine().Export(new List<XPBaseObject> { _pEnumClass }, serializationConfiguration.SerializationConfigurationGroup);

            _pEnumClass.Delete();
            XPObjectSpace.CommitChanges();
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xDocument.ToString()));
            new ImportEngine(ErrorHandling.ThrowException).ImportObjects(memoryStream, (UnitOfWork)XPObjectSpace.Session);

            _pEnumClass.Reload();
            Debug.Print("");

        };

        Because of = () => { };
        It should_should = () => { };
    }
}
