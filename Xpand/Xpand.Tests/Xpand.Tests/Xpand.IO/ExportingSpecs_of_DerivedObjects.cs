using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using Machine.Specifications;
using Xpand.ExpressApp.IO.Core;
using Xpand.ExpressApp.IO.PersistentTypesHelpers;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using Xpand.Persistent.Base.ImportExport;
using Xpand.Persistent.BaseImpl.ImportExport;

namespace Xpand.Tests.Xpand.IO {
    [Subject(typeof(ExportEngine), "Derived objects")]
    public class When_exporting_objects_that_derive_from_exporting_type : With_Isolations {
        static XElement _element;
        static XElement _root;
        static SerializationConfiguration _serializationConfiguration;
        static object _derivedCustomer;
        static XPObjectSpace _XPObjectSpace;

        Establish context = () => {
            _XPObjectSpace = (XPObjectSpace)new XPObjectSpaceProvider(new MemoryDataStoreProvider()).CreateObjectSpace();
            var persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(_XPObjectSpace, GetUniqueAssemblyName());
            var classHandler = persistentAssemblyBuilder.CreateClasses(new[] { "Customer", "DerivedCustomer" });
            classHandler.CreateSimpleMembers<string>(classInfo => classInfo.Name == "DerivedCustomer" ? new[] { "DerivedName" } : null);
            classHandler.SetInheritance(info => info.Name == "DerivedCustomer" ? persistentAssemblyBuilder.PersistentAssemblyInfo.PersistentClassInfos[0] : null);
            _XPObjectSpace.CommitChanges();

            var compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));

            Type _derivedCustomerType = compileModule.Assembly.GetTypes().Where(type => type.Name == "DerivedCustomer").Single();
            Type customerType = compileModule.Assembly.GetTypes().Where(type => type.Name == "Customer").Single();
            _serializationConfiguration = new SerializationConfiguration(_XPObjectSpace.Session) {
                TypeToSerialize = customerType,
                SerializationConfigurationGroup = _XPObjectSpace.CreateObject<SerializationConfigurationGroup>()
            };
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
            _derivedCustomer = _XPObjectSpace.CreateObject(_derivedCustomerType);
        };


        Because of = () => {
            _root = new ExportEngine().Export(new[] { _derivedCustomer }.OfType<XPBaseObject>(), _serializationConfiguration.SerializationConfigurationGroup).Root;
        };

        It should_export_derived_type_instead = () => {
            _element = _root.SerializedObjects(_derivedCustomer.GetType()).Single();
            _element.ShouldNotBeNull();
        };

        It should_export_derived_type_properties =
            () => _element.Properties(NodeType.Simple).Property("DerivedName").ShouldNotBeNull();
    }
    [Subject(typeof(ExportEngine), "Derived objects")]
    public class When_exporting_an_object_with_associated_property_with_value_a_derived_property_type_object : With_Isolations {
        static XElement _root;
        static XPBaseObject _order;
        static Type _derivedCustomerType;
        static XPObjectSpace _XPObjectSpace;

        static SerializationConfiguration _serializationConfiguration;

        Establish context = () => {
            ITypeHandler<ICustomer, IOrder> oneToMany = ModelBuilder<ICustomer, IOrder>.Build().OneToMany();
            _XPObjectSpace = oneToMany.XPObjectSpace;
            var existentConfiguration =
                (SerializationConfiguration)_XPObjectSpace.CreateObject(typeof(SerializationConfiguration));
            existentConfiguration.TypeToSerialize = oneToMany.T2Type;


            var persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(_XPObjectSpace, GetUniqueAssemblyName());
            var classHandler = persistentAssemblyBuilder.CreateClasses(new[] { "DerivedCustomer" });
            classHandler.SetInheritance(info => oneToMany.T1Type);

            _XPObjectSpace.CommitChanges();

            var compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));
            _derivedCustomerType = compileModule.Assembly.GetTypes().Where(type => type.Name == "DerivedCustomer").Single();
            _order = (XPBaseObject)_XPObjectSpace.CreateObject(oneToMany.T2Type);
            _order.SetMemberValue("Customer", _XPObjectSpace.CreateObject(_derivedCustomerType));

            _serializationConfiguration = new SerializationConfiguration(_XPObjectSpace.Session) {
                TypeToSerialize = oneToMany.T1Type,
                SerializationConfigurationGroup = _XPObjectSpace.CreateObject<SerializationConfigurationGroup>()
            };

            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);

            _XPObjectSpace.CommitChanges();

        };

        Because of = () => {
            _root = new ExportEngine().Export(new[] { _order }, _serializationConfiguration.SerializationConfigurationGroup).Root;
        };

        It should_export_derived_type_instead = () => _root.SerializedObjects(_derivedCustomerType).Count().ShouldEqual(1);
    }
    [Subject(typeof(ExportEngine), "Derived objects")]
    public class When_exporting_an_object_with_associated_collection_with_childs_a_derived_collection_type_objects : With_Isolations {
        static XPBaseObject _customer;
        static XElement _root;
        static Type _derivedOrderType;
        static XPBaseObject _derivedOrder;
        static SerializationConfiguration _serializationConfiguration;
        static XPObjectSpace _XPObjectSpace;

        Establish context = () => {
            ITypeHandler<ICustomer, IOrder> oneToMany = ModelBuilder<ICustomer, IOrder>.Build().OneToMany();
            _XPObjectSpace = oneToMany.XPObjectSpace;
            var existentConfiguration = (SerializationConfiguration)_XPObjectSpace.CreateObject(typeof(SerializationConfiguration));
            existentConfiguration.TypeToSerialize = oneToMany.T1Type;


            var persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(_XPObjectSpace, GetUniqueAssemblyName());
            var classHandler = persistentAssemblyBuilder.CreateClasses(new[] { "DerivedOrder" });
            classHandler.SetInheritance(info => oneToMany.T2Type);

            _XPObjectSpace.CommitChanges();

            var compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));
            _derivedOrderType = compileModule.Assembly.GetTypes().Where(type => type.Name == "DerivedOrder").Single();
            _derivedOrder = (XPBaseObject)_XPObjectSpace.CreateObject(_derivedOrderType);
            _customer = (XPBaseObject)_XPObjectSpace.CreateObject(oneToMany.T1Type);
            _derivedOrder.SetMemberValue("Customer", _customer);

            _serializationConfiguration = new SerializationConfiguration(_XPObjectSpace.Session) {
                TypeToSerialize = oneToMany.T1Type,
                SerializationConfigurationGroup = _XPObjectSpace.CreateObject<SerializationConfigurationGroup>()
            };
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
            _XPObjectSpace.CommitChanges();

        };

        Because of = () => { _root = new ExportEngine().Export(new[] { _customer }, _serializationConfiguration.SerializationConfigurationGroup).Root; };

        It should_export_derived_type_instead = () => _root.SerializedObjects(_derivedOrderType).Count().ShouldEqual(1);
    }
}