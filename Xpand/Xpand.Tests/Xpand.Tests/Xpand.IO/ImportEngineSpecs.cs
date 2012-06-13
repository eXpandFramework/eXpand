using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Machine.Specifications;
using Xpand.ExpressApp.IO.Core;
using Xpand.ExpressApp.IO.PersistentTypesHelpers;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using Xpand.Persistent.Base.ImportExport;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.BaseImpl.ImportExport;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using Xpand.Xpo;

namespace Xpand.Tests.Xpand.IO {
    [Subject(typeof(ImportEngine))]
    public class When_importing_1_Customer_with_1_ref_User_2_Orders_add_user_not_serializable : With_Customer_Orders {
        static XPBaseObject _order1;
        static User _user;
        static XPBaseObject _customer;

        static Stream _manifestResourceStream;

        Establish context = () => {
            _user = (User)XPObjectSpace.CreateObject(typeof(User));
            _user.SetMemberValue("oid", new Guid("{B11AFD0E-6B2B-44cf-A986-96909A93291A}"));
            XPObjectSpace.Session.GetClassInfo(OrderType).CreateMember("Ammount", typeof(int));
            XPObjectSpace.CommitChanges();
            _manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Xpand.Tests.Xpand.IO.Resources.1toMany.xml");
            if (_manifestResourceStream != null)
                _manifestResourceStream = new MemoryStream(Encoding.UTF8.GetBytes(new StreamReader(_manifestResourceStream).ReadToEnd().Replace("B11AFD0E-6B2B-44cf-A986-96909A93291A", _user.Oid.ToString())));

        };

        Because of = () => new ImportEngine(ErrorHandling.CreateErrorObjects).ImportObjects(_manifestResourceStream, XPObjectSpace);

        It should_create_1_new_customer_object = () => {
            _customer = XPObjectSpace.FindObject(CustomerType, null) as XPBaseObject;
            _customer.ShouldNotBeNull();
        };

        It should_fill_all_customer_simple_properties_with_property_element_values = () => {
            _customer.GetMemberValue("oid").ToString().ShouldEqual("B11AFD0E-6B2B-44cf-A986-96909A93291D".ToLower());
            _customer.GetMemberValue("Name").ShouldEqual("Apostolis");
        };

        It should_set_customer_user_property_same_as_one_found_in_datastore = () => _customer.GetMemberValue("User").ShouldEqual(_user);

        It should_not_import_donotserialized_strategy_user_object = () => XPObjectSpace.Session.GetCount(typeof(User)).ShouldEqual(1);

        It should_create_2_new_order_objects = () => XPObjectSpace.Session.GetCount(OrderType).ShouldEqual(2);

        It should_fill_all_order_properties_with_property_element_values = () => {
            _order1 = ((XPBaseObject)XPObjectSpace.GetObjectByKey(OrderType, new Guid("{B11AFD0E-6B2B-44cf-A986-96909A93291E}")));
            _order1.Reload();
            _order1.ShouldNotBeNull();
            _order1.GetMemberValue("Ammount").ShouldEqual(200);
            var order2 = ((XPBaseObject)XPObjectSpace.GetObjectByKey(OrderType, new Guid("{B11AFD0E-6B2B-44cf-A986-96909A93291F}")));
            order2.ShouldNotBeNull();
            order2.GetMemberValue("Ammount").ShouldEqual(100);
        };

        It should_set_customer_property_of_order_same_as_new_created_customer = () => _order1.GetMemberValue("Customer").ShouldEqual(_customer);


    }
    [Subject(typeof(ExportEngine))]
    [Ignore]
    public class When_importing_an_object_with_value_converter : With_Isolations {
        static XPObjectSpace _XPObjectSpace;
        static Stream _manifestResourceStream;

        static ModelDifferenceObject _differenceObject;

        Establish context = () => {
            _manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("eXpand.Tests.eXpand.IO.Resources.WithValueConverter.xml");
            _XPObjectSpace = (XPObjectSpace)new XPObjectSpaceProvider(new MemoryDataStoreProvider()).CreateObjectSpace();
        };

        Because of = () => new ImportEngine().ImportObjects(_manifestResourceStream, _XPObjectSpace);

        It should_iumport_the_converter_from_storage_value = () => {
            //            var persistentApplication = _XPObjectSpace.FindObject<PersistentApplication>(null);
            throw new NotImplementedException();
            //            persistentApplication.Model.RootNode.Name.ShouldEqual("Application");            
        };
    }

    [Subject(typeof(ImportEngine))]
    public class When_importing_an_object_with_key_that_exists : With_Isolations {
        static Type _customerType;
        static XPObjectSpace _XPObjectSpace;
        static XPBaseObject _customer;
        static Stream _manifestResourceStream;

        Establish context = () => {
            _manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Xpand.Tests.Xpand.IO.Resources.ExistentNaturalKeyObject.xml");
            _XPObjectSpace = (XPObjectSpace)new XPObjectSpaceProvider(new MemoryDataStoreProvider()).CreateObjectSpace();
            var persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(_XPObjectSpace, GetUniqueAssemblyName());
            persistentAssemblyBuilder.CreateClasses(new[] { "Customer" }).CreateSimpleMembers<string>(info => new[] { "Name" });
            _XPObjectSpace.CommitChanges();
            var compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));
            _customerType = compileModule.Assembly.GetTypes().Where(type => type.Name == "Customer").Single();
            _customer = (XPBaseObject)_XPObjectSpace.CreateObject(_customerType);
            _customer.SetMemberValue("Name", "test");
            _customer.SetMemberValue("oid", new Guid("B11AFD0E-6B2B-44cf-A986-96909A93291D"));
            _XPObjectSpace.CommitChanges();
        };

        Because of = () => new ImportEngine().ImportObjects(_manifestResourceStream, _XPObjectSpace);

        It should_not_create_it = () => _XPObjectSpace.GetObjectsCount(_customerType, null).ShouldEqual(1);
        It should_overide_its_values = () => _customer.GetMemberValue("Name").ShouldEqual("newName");
    }

    [Subject(typeof(ImportEngine))]
    public class When_importing_an_object_with_key_that_exists_as_deleted : With_Isolations {
        static XPBaseObject _findObject;
        static Guid _guid;
        static Type _customerType;
        static XPObjectSpace _XPObjectSpace;
        static XPBaseObject _customer;
        static Stream _manifestResourceStream;

        Establish context = () => {
            _manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Xpand.Tests.Xpand.IO.Resources.ExistentNaturalKeyObject.xml");
            _XPObjectSpace = (XPObjectSpace)new XPObjectSpaceProvider(new MemoryDataStoreProvider()).CreateObjectSpace();
            var persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(_XPObjectSpace, GetUniqueAssemblyName());
            persistentAssemblyBuilder.CreateClasses(new[] { "Customer" }).CreateSimpleMembers<string>(info => new[] { "Name" });
            _XPObjectSpace.CommitChanges();
            var compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));
            _customerType = compileModule.Assembly.GetTypes().Where(type => type.Name == "Customer").Single();
            _customer = (XPBaseObject)_XPObjectSpace.CreateObject(_customerType);
            _customer.SetMemberValue("Name", "test");
            _guid = new Guid("B11AFD0E-6B2B-44cf-A986-96909A93291D");
            _customer.SetMemberValue("oid", _guid);
            _XPObjectSpace.CommitChanges();
            _customer.Delete();
            _XPObjectSpace.CommitChanges();
            _XPObjectSpace.Session.DropIdentityMap();
        };

        Because of = () => new ImportEngine().ImportObjects(_manifestResourceStream, _XPObjectSpace);

        It should_create_it = () => {
            var session = new Session(_XPObjectSpace.Session.DataLayer);
            _findObject = (XPBaseObject)session.FindObject(_customerType, new BinaryOperator("Oid", _guid), true);
            _findObject.ShouldNotBeNull();
        };

        It should_purge_deleted_object = () => _findObject.IsDeleted.ShouldBeFalse();
    }

    [Subject(typeof(ImportEngine))]
    public class When_importing_a_customers_orders_many_to_many : With_Isolations {
        static Type _orderType;
        static Type _customerType;
        static Stream _manifestResourceStream;
        static XPObjectSpace _XPObjectSpace;

        Establish context = () => {
            var typeHandler = ModelBuilder<ICustomer, IOrder>.Build().ManyToMany();
            _customerType = typeHandler.T1Type;
            _orderType = typeHandler.T2Type;
            _XPObjectSpace = typeHandler.XPObjectSpace;
            _manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Xpand.Tests.Xpand.IO.Resources.ManyToMany.xml");
        };

        Because of = () => new ImportEngine().ImportObjects(_manifestResourceStream, _XPObjectSpace);

        It should_create_1_customer = () => _XPObjectSpace.GetObjectsCount(_customerType, null).ShouldEqual(1);
        It should_create_2_orders = () => _XPObjectSpace.GetObjectsCount(_orderType, null).ShouldEqual(2);
        It should_create_2_customer_orders = () =>
                                           ((IList)
                                            ((XPBaseObject)_XPObjectSpace.FindObject(_customerType, null)).GetMemberValue("Orders")).Count.ShouldEqual(2);
        It should_create_1_order1_customer = () =>
                                           ((IList)
                                            ((XPBaseObject)new XPCollection(_XPObjectSpace.Session, _orderType)[0]).GetMemberValue("Customers")).Count.ShouldEqual(1);
        It should_create_1_order2_customer = () => ((IList)
                                            ((XPBaseObject)new XPCollection(_XPObjectSpace.Session, _orderType)[1]).GetMemberValue("Customers")).Count.ShouldEqual(1);
    }
    [Subject(typeof(ImportEngine))]
    public class When_importing_object_with_byte_array : With_Isolations {
        static XDocument document;
        static byte[] _pivotGridSettingsContent;
        static string _xml;
        static Session _session;

        Establish context = () => {
            var XPObjectSpace = (XPObjectSpace)ObjectSpaceInMemory.CreateNew();
            _session = XPObjectSpace.Session;
            var analysis = XPObjectSpace.CreateObject<Analysis>();
            Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("eXpand.Tests.eXpand.IO.Resources.PivotContent.xml");
            if (manifestResourceStream != null) {
                _xml = new StreamReader(manifestResourceStream).ReadToEnd();
                _pivotGridSettingsContent = Encoding.UTF8.GetBytes(_xml);
                analysis.PivotGridSettingsContent = _pivotGridSettingsContent;
            }
            document = new ExportEngine().Export(new List<XPBaseObject> { analysis }, XPObjectSpace.CreateObject<SerializationConfigurationGroup>());
        };

        Because of = () => new ImportEngine().ImportObjects(document.ToString(), new XPObjectSpace((UnitOfWork)_session));

        It should_import_correct_bytes = () => _session.FindObject<Analysis>(null).PivotGridSettingsContent.ShouldEqual(_pivotGridSettingsContent);
    }
    [Subject(typeof(ImportEngine))]
    public class When_importing_customer_user_orders_persistentAssemblyInfo : With_Isolations {
        static XDocument _document;
        static IPersistentAssemblyInfo _persistentAssemblyInfo;
        static XPObjectSpace _XPObjectSpace;
        static Stream _manifestResourceStream;

        Establish context = () => {
            var modelBuilder = ModelBuilder<ICustomer, IOrder>.Build();
            modelBuilder.OneToMany();
            _persistentAssemblyInfo = modelBuilder.PersistentAssemblyBuilder.PersistentAssemblyInfo;
            _XPObjectSpace = (XPObjectSpace)XPObjectSpace.FindObjectSpaceByObject(_persistentAssemblyInfo);
            var configuration = new SerializationConfiguration(_persistentAssemblyInfo.Session) {
                TypeToSerialize = typeof(PersistentAssemblyInfo),
                SerializationConfigurationGroup = _XPObjectSpace.CreateObject<SerializationConfigurationGroup>()
            };

            new ClassInfoGraphNodeBuilder().Generate(configuration);
            _XPObjectSpace.CommitChanges();
            _document = new ExportEngine().Export(new[] { _persistentAssemblyInfo }.OfType<XPBaseObject>(), configuration.SerializationConfigurationGroup);
        };

        Because of = () => new ImportEngine().ImportObjects(_document.ToString(), _XPObjectSpace);

        It should_create_a_persistent_assemblyInfo = () => {
            _persistentAssemblyInfo = _XPObjectSpace.FindObject<PersistentAssemblyInfo>(null);
            _persistentAssemblyInfo.ShouldNotBeNull();
        };

        It should_set_codetemplateinfo_property_for_classinfos =
            () => _persistentAssemblyInfo.PersistentClassInfos[1].CodeTemplateInfo.ShouldNotBeNull();

        It should_be_combile_able = () => new CompileEngine().CompileModule(_persistentAssemblyInfo, null).ShouldNotBeNull();
    }
    [Subject(typeof(ImportEngine))]
    public class When_importing_from_xml_with_special_characters : With_Isolations {
        static XPObjectSpace _XPObjectSpace;
        static Type _testClassType;
        static MemoryStream _memoryStream;

        Establish context = () => {
            _XPObjectSpace = (XPObjectSpace)ObjectSpaceInMemory.CreateNew();
            PersistentAssemblyBuilder persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(_XPObjectSpace, GetUniqueAssemblyName());
            persistentAssemblyBuilder.CreateClasses(new[] { "TestClass" }).CreateSimpleMembers<string>(info => new[] { "TestProperty" });
            var compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));
            _testClassType = compileModule.Assembly.GetTypes().Where(type => type.Name == "TestClass").Single();
            var testClass = (XPBaseObject)_XPObjectSpace.CreateObject(_testClassType);
            testClass.SetMemberValue("TestProperty", "<Application></Application>");

            XDocument document = new ExportEngine().Export(new[] { testClass }, _XPObjectSpace.CreateObject<SerializationConfigurationGroup>());
            testClass.Delete();
            _memoryStream = new MemoryStream();
            document.Save(new StreamWriter(_memoryStream));
            _memoryStream.Position = 0;

        };

        Because of = () => {
            var unitOfWork = new UnitOfWork(_XPObjectSpace.Session.DataLayer);
            new ImportEngine().ImportObjects(_memoryStream, _XPObjectSpace);
            unitOfWork.CommitChanges();
        };

        It should_decode_them =
            () => {
                var baseObject = (XPBaseObject)_XPObjectSpace.FindObject(_testClassType, null);
                baseObject.GetMemberValue("TestProperty").ShouldEqual("<Application></Application>");
            };
    }
    [Subject(typeof(ImportEngine))]
    [Ignore]
    public class When_importing_modeldifference_object {
        static Exception _exception;
        static UnitOfWork _unitOfWork;
        static Stream _manifestResourceStream;

        Establish context = () => {
            _unitOfWork = new UnitOfWork(((XPObjectSpace)ObjectSpaceInMemory.CreateNew()).Session.DataLayer);
            _manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("eXpand.Tests.eXpand.IO.Resources.ModelDifferenceObject.xml");

        };

        Because of = () => {
            _exception = Catch.Exception(() => new ImportEngine().ImportObjects(_manifestResourceStream, new XPObjectSpace(_unitOfWork)));
        };

        It should_raize_no_errors = () => _exception.ShouldBeNull();
    }
    [Subject(typeof(ImportEngine))]
    public class When_an_object_has_an_image_property : With_Isolations {
        static UnitOfWork _unitOfWork;
        static Stream _manifestResourceStream;

        Establish context = () => {
            _manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Xpand.Tests.Xpand.IO.Resources.ObjectWithImageProperty.xml");
            _unitOfWork = new UnitOfWork(((XPObjectSpace)ObjectSpaceInMemory.CreateNew()).Session.DataLayer);
        };

        Because of = () => new ImportEngine().ImportObjects(_manifestResourceStream, new XPObjectSpace(_unitOfWork));

        It should_create_the_serialized_image = () => {
            var findObject = _unitOfWork.FindObject<ImagePropertyObject>(null);
            findObject.Photo.Width.ShouldEqual(1);
            findObject.Photo.Height.ShouldEqual(1);
        };
    }
    [Subject(typeof(ImportEngine))]
    public class When_an_object_has_a_datetime_property : With_Isolations {
        static XPObjectSpace _XPObjectSpace;
        static MemoryStream _memoryStream;

        Establish context = () => {
            const string xml = @"<SerializedObjects>
                  <SerializedObject type=""DateTimePropertyObject"">
                    <Property type=""simple"" name=""Date"" isKey=""false"">634038486102582525</Property>
                    <Property type=""simple"" name=""oid"" isKey=""true"">7b806eb9-e459-4117-b48f-fa98f8a1b9d2</Property>
                  </SerializedObject>
                </SerializedObjects>";
            _memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            _XPObjectSpace = (XPObjectSpace)ObjectSpaceInMemory.CreateNew();
        };

        Because of = () => new ImportEngine().ImportObjects(_memoryStream, _XPObjectSpace);

        It should_deserialize_full_date =
            () =>
            _XPObjectSpace.FindObject<DateTimePropertyObject>(null).Date.Ticks.ShouldEqual(634038486102582525);
    }
    [Subject(typeof(ImportEngine))]
    public class When_source_object_is_deleted_and_target_is_not : With_Isolations {
        static MemoryStream _memoryStream;
        static UnitOfWork _unitOfWork;

        Establish context = () => {
            _unitOfWork = new UnitOfWork(((XPObjectSpace)ObjectSpaceInMemory.CreateNew()).Session.DataLayer);
            new Analysis(_unitOfWork) { Name = "target" };
            _unitOfWork.CommitChanges();

            const string xml = @"<SerializedObjects>
                  <SerializedObject type=""Analysis"">
                    <Property type=""simple"" name=""GCRecord"" isKey=""false"">-1</Property>
                    <Property type=""simple"" name=""Name"" isKey=""true"">target</Property>
                  </SerializedObject>
                </SerializedObjects>";
            _memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        };

        Because of = () => new ImportEngine().ImportObjects(_memoryStream, new XPObjectSpace(_unitOfWork));

        It should_match_deleted_state_of_source =
            () => _unitOfWork.FindObject<Analysis>(null, true).IsDeleted.ShouldEqual(true);
    }

    [Subject(typeof(ImportEngine))]
    [Ignore("")]
    public class When_target_object_is_deleted_and_source_is_not {
        static MemoryStream _memoryStream;
        static UnitOfWork _unitOfWork;

        Establish context = () => {
            XafTypesInfo.Instance.RegisterEntity(typeof(Analysis));
            _unitOfWork = new UnitOfWork(((XPObjectSpace)ObjectSpaceInMemory.CreateNew()).Session.DataLayer);
            var analysis = new Analysis(_unitOfWork) { Name = "target" };
            analysis.Delete();
            _unitOfWork.CommitChanges();

            const string xml = @"<SerializedObjects>
                  <SerializedObject type=""Analysis"">
                    <Property type=""simple"" name=""GCRecord"" isKey=""false""></Property>
                    <Property type=""simple"" name=""Name"" isKey=""true"">target</Property>
                  </SerializedObject>
                </SerializedObjects>";
            _memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        };

        Because of = () => new ImportEngine().ImportObjects(_memoryStream, new XPObjectSpace(_unitOfWork));
        It should_match_deleted_state_of_source =
            () => _unitOfWork.FindObject<Analysis>(null).ShouldNotBeNull();
    }
    [Subject(typeof(ImportEngine))]
    public class When_nullable_enum_property_with_null_value {
        static UnitOfWork _unitOfWork;


        static Stream _manifestResourceStream;


        Establish context = () => {
            XafTypesInfo.Instance.RegisterEntity(typeof(PEnumClass));
            _unitOfWork = new UnitOfWork(((XPObjectSpace)ObjectSpaceInMemory.CreateNew()).Session.DataLayer);
            _manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Xpand.Tests.Xpand.IO.Resources.NullAbleEnum.xml");
        };

        Because of = () => new ImportEngine().ImportObjects(_manifestResourceStream, new XPObjectSpace(_unitOfWork));

        It should_import_null_value = () => _unitOfWork.FindObject<PEnumClass>(null).MyEnum.ShouldBeNull();
    }
    [Subject(typeof(ImportEngine))]
    public class When_reference_property_is_null_and_has_serialialize_as_value : With_Customer_Orders {
        static XPBaseObject _order1;
        static User _user;
        static XPBaseObject _customer;

        static Stream _manifestResourceStream;

        Establish context = () => {
            _user = (User)XPObjectSpace.CreateObject(typeof(User));
            _user.SetMemberValue("oid", new Guid("{B11AFD0E-6B2B-44cf-A986-96909A93291A}"));
            XPObjectSpace.Session.GetClassInfo(OrderType).CreateMember("Ammount", typeof(int));
            XPObjectSpace.CommitChanges();
            _manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Xpand.Tests.Xpand.IO.Resources.NullValuesImport.xml");
            if (_manifestResourceStream != null)
                _manifestResourceStream = new MemoryStream(Encoding.UTF8.GetBytes(new StreamReader(_manifestResourceStream).ReadToEnd().Replace("B11AFD0E-6B2B-44cf-A986-96909A93291A", _user.Oid.ToString())));

        };
        Because of = () => new ImportEngine(ErrorHandling.CreateErrorObjects).ImportObjects(_manifestResourceStream, XPObjectSpace);

        It should_remain_null_the_property_value = () => {
            _customer = (XPBaseObject)XPObjectSpace.FindObject(CustomerType, null);
            _customer.GetMemberValue("User").ShouldEqual(null);
        };


    }
    [Subject(typeof(ImportEngine))]
    public class When_updating_a_ref_property_with_a_null_value_and_serializeasvalue : With_Customer_Orders {
        static XPBaseObject _order1;
        static User _user;
        static XPBaseObject _customer;

        static Stream _manifestResourceStream;

        Establish context = () => {
            _user = (User)XPObjectSpace.CreateObject(typeof(User));
            _user.SetMemberValue("oid", new Guid("{B11AFD0E-6B2B-44cf-A986-96909A93291A}"));
            XPObjectSpace.Session.GetClassInfo(OrderType).CreateMember("Ammount", typeof(int));
            XPObjectSpace.CommitChanges();
            _manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Xpand.Tests.Xpand.IO.Resources.NotNullRefValuesImport.xml");
            if (_manifestResourceStream != null)
                _manifestResourceStream = new MemoryStream(Encoding.UTF8.GetBytes(new StreamReader(_manifestResourceStream).ReadToEnd().Replace("B11AFD0E-6B2B-44cf-A986-96909A93291A", _user.Oid.ToString())));

            new ImportEngine(ErrorHandling.CreateErrorObjects).ImportObjects(_manifestResourceStream, XPObjectSpace);

            _manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Xpand.Tests.Xpand.IO.Resources.NullValuesImport.xml");
            if (_manifestResourceStream != null)
                _manifestResourceStream = new MemoryStream(Encoding.UTF8.GetBytes(new StreamReader(_manifestResourceStream).ReadToEnd().Replace("B11AFD0E-6B2B-44cf-A986-96909A93291A", _user.Oid.ToString())));

        };

        Because of = () => new ImportEngine(ErrorHandling.CreateErrorObjects).ImportObjects(_manifestResourceStream, XPObjectSpace);

        It should_null_the_ref_property_value = () => {
            _customer = (XPBaseObject)XPObjectSpace.FindObject(CustomerType, null);
            _customer.GetMemberValue("User").ShouldBeNull();
        };


    }
    [Subject(typeof(ImportEngine))]
    public class When_updating_a_ref_property_with_a_null_value_and_serializeasobject : With_Customer_Orders {
        static XPBaseObject _order1;
        static User _user;
        static XPBaseObject _customer;

        static Stream _manifestResourceStream;

        Establish context = () => {
            _user = (User)XPObjectSpace.CreateObject(typeof(User));
            _user.SetMemberValue("oid", new Guid("{B11AFD0E-6B2B-44cf-A986-96909A93291A}"));
            XPObjectSpace.Session.GetClassInfo(OrderType).CreateMember("Ammount", typeof(int));
            XPObjectSpace.CommitChanges();
            _manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Xpand.Tests.Xpand.IO.Resources.NotNullRefValuesImport.xml");
            if (_manifestResourceStream != null)
                _manifestResourceStream = new MemoryStream(Encoding.UTF8.GetBytes(new StreamReader(_manifestResourceStream).ReadToEnd().Replace("B11AFD0E-6B2B-44cf-A986-96909A93291A", _user.Oid.ToString())));

            new ImportEngine(ErrorHandling.CreateErrorObjects).ImportObjects(_manifestResourceStream, XPObjectSpace);

            _manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Xpand.Tests.Xpand.IO.Resources.NullValuesImportAsObject.xml");
            if (_manifestResourceStream != null)
                _manifestResourceStream = new MemoryStream(Encoding.UTF8.GetBytes(new StreamReader(_manifestResourceStream).ReadToEnd().Replace("B11AFD0E-6B2B-44cf-A986-96909A93291A", _user.Oid.ToString())));

        };

        Because of = () => new ImportEngine(ErrorHandling.CreateErrorObjects).ImportObjects(_manifestResourceStream, XPObjectSpace);

        It should_null_the_ref_property_value = () => {
            _customer = (XPBaseObject)XPObjectSpace.FindObject(CustomerType, null);
            _customer.GetMemberValue("User").ShouldBeNull();
        };


    }

}