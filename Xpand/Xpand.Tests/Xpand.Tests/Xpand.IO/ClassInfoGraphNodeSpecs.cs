using System;
using System.IO;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;
using Xpand.ExpressApp.IO.PersistentTypesHelpers;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using Xpand.Persistent.BaseImpl.ImportExport;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using Machine.Specifications;
using System.Linq;

using Xpand.Persistent.Base.ImportExport;
using Xpand.Xpo;

namespace Xpand.Tests.Xpand.IO {
    [Subject(typeof(ClassInfoGraphNode))]
    public class When_creating_a_graph_for_type_with_associated_collection_and_associated_type_derived_types_exist_in_the_domain:With_Isolations
    {
        static Type _derivedOrderType;
        static SerializationConfiguration _serializationConfiguration;
        static ObjectSpace _objectSpace;

        Establish context = () => {
            
            ITypeHandler<ICustomer, IOrder> oneToMany = ModelBuilder<ICustomer, IOrder>.Build().OneToMany();
            _objectSpace = oneToMany.ObjectSpace;

            _serializationConfiguration = new SerializationConfiguration(_objectSpace.Session)
            {
                TypeToSerialize = oneToMany.T1Type,
                SerializationConfigurationGroup = _objectSpace.CreateObject<SerializationConfigurationGroup>()
            };

            var persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(_objectSpace,GetUniqueAssemblyName());
            var classHandler = persistentAssemblyBuilder.CreateClasses(new[] { "DerivedOrder" });
            classHandler.SetInheritance(info => oneToMany.T2Type);

            _objectSpace.CommitChanges();

            var compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));
            _derivedOrderType = compileModule.Assembly.GetTypes().Where(type => type.Name == "DerivedOrder").Single();
            var existentConfiguration =
                (SerializationConfiguration)_objectSpace.CreateObject(typeof(SerializationConfiguration));
            existentConfiguration.TypeToSerialize = _derivedOrderType;
            existentConfiguration.SerializationConfigurationGroup=_serializationConfiguration.SerializationConfigurationGroup;
        };

        Because of = () =>
        {
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
            _objectSpace.CommitChanges();
        };

        It should_create_graph_for_derived_types =
            () =>
            new XPQuery<SerializationConfiguration>(_objectSpace.Session).Where(
                configuration => configuration.TypeToSerialize == _derivedOrderType).Count().ShouldEqual(1);
    }
    [Subject(typeof(ClassInfoGraphNode))]
    public class When_creating_a_graph_for_type_with_associated_object_and_associated_type_derived_types_exist_in_the_domain:With_Isolations
    {
        static Type _derivedCustomerType;
        static SerializationConfiguration _serializationConfiguration;
        static ObjectSpace _objectSpace;

        Establish context = () => {

            ITypeHandler<ICustomer, IOrder> oneToMany = ModelBuilder<ICustomer, IOrder>.Build().OneToMany();
            _objectSpace = oneToMany.ObjectSpace;
            var existentConfiguration =
                (SerializationConfiguration)_objectSpace.CreateObject(typeof(SerializationConfiguration));
            existentConfiguration.TypeToSerialize = oneToMany.T2Type;
            _serializationConfiguration = new SerializationConfiguration(_objectSpace.Session)
            {
                TypeToSerialize = oneToMany.T2Type,
                SerializationConfigurationGroup = _objectSpace.CreateObject<SerializationConfigurationGroup>()
            };

            var persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(_objectSpace,GetUniqueAssemblyName());
            var classHandler = persistentAssemblyBuilder.CreateClasses(new[]{"DerivedCustomer"});
            classHandler.SetInheritance(info =>oneToMany.T1Type );

            _objectSpace.CommitChanges();

            var compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));
            _derivedCustomerType = compileModule.Assembly.GetTypes().Where(type => type.Name == "DerivedCustomer").Single();
        };
        Because of = () =>
        {
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
            _objectSpace.CommitChanges();
        };

        It should_create_graph_for_derived_types = () => new XPQuery<SerializationConfiguration>(_objectSpace.Session).Where(configuration => configuration.TypeToSerialize==_derivedCustomerType).Count().ShouldEqual(1);
    }
    [Subject(typeof(ClassInfoGraphNode))]
    public class When_creating_a_graph_for_type_and_type_derived_types_exist_in_the_domain:With_Isolations
    {
        static SerializationConfiguration _serializationConfiguration;
        static Type _t1Type;
        static Type _t2Type;
        static ObjectSpace _objectSpace;

        Establish context = () => {
            _objectSpace = new ObjectSpaceProvider(new MemoryDataStoreProvider()).CreateObjectSpace();
            var persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(_objectSpace, GetUniqueAssemblyName());
            var classHandler = persistentAssemblyBuilder.CreateClasses(new[] {"Customer", "DerivedCustomer"});
            classHandler.SetInheritance(info=>info.Name=="DerivedCustomer"?persistentAssemblyBuilder.PersistentAssemblyInfo.PersistentClassInfos[0]:null);

            _objectSpace.CommitChanges();

            var compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));
            _t1Type = compileModule.Assembly.GetTypes().Where(type => type.Name == "Customer").Single();
            _t2Type = compileModule.Assembly.GetTypes().Where(type => type.Name == "DerivedCustomer").Single();
            _serializationConfiguration = new SerializationConfiguration(_objectSpace.Session)
            {
                TypeToSerialize = _t1Type,
                SerializationConfigurationGroup = _objectSpace.CreateObject<SerializationConfigurationGroup>()
            };
        };


        Because of = () => {
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
            _objectSpace.CommitChanges();
        };
        It should_create_graph_for_derived_types=() => new XPQuery<SerializationConfiguration>(_objectSpace.Session).Where(
                                                           configuration => configuration.TypeToSerialize == _t2Type).Count().ShouldEqual(1);
    }
    [Subject(typeof(ClassInfoGraphNode))]
    public class When_creating_a_graph__for_type_with_associated_collection_and_associated_type_graph_exists : With_Isolations
    {
        static SerializationConfiguration _serializationConfiguration;
        static ObjectSpace _objectSpace;

        Establish context = () =>
        {
            ITypeHandler<ICustomer, IOrder> oneToMany = ModelBuilder<ICustomer, IOrder>.Build().OneToMany();
            _objectSpace = oneToMany.ObjectSpace;
            var existentConfiguration =
                (SerializationConfiguration)_objectSpace.CreateObject(typeof(SerializationConfiguration));
            var serializationConfigurationGroup = _objectSpace.CreateObject<SerializationConfigurationGroup>();
            existentConfiguration.TypeToSerialize = oneToMany.T2Type;
            existentConfiguration.SerializationConfigurationGroup=serializationConfigurationGroup;
            _serializationConfiguration = new SerializationConfiguration(_objectSpace.Session)
            {
                TypeToSerialize = oneToMany.T1Type,
                SerializationConfigurationGroup = serializationConfigurationGroup
            };
        };

        Because of = () =>
        {
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
            _objectSpace.CommitChanges();
        };

        It should_not_create_any_more_graphs_for_associated_type=() => {
            const int customer_order = 2;
            _objectSpace.Session.GetCount(typeof(SerializationConfiguration)).ShouldEqual(customer_order);
        };
    }
    [Subject(typeof(ClassInfoGraphNode))]
    public class When_creating_a_graph__for_type_with_associated_object_and_associated_type_graph_exists:With_Isolations {
        static ObjectSpace _objectSpace;
        static SerializationConfiguration _serializationConfiguration;

        Establish context = () => {
            ITypeHandler<ICustomer, IOrder> oneToMany = ModelBuilder<ICustomer, IOrder>.Build().OneToMany();
            _objectSpace = oneToMany.ObjectSpace;
            var existentConfiguration =
                (SerializationConfiguration) _objectSpace.CreateObject(typeof (SerializationConfiguration));
            var serializationConfigurationGroup = _objectSpace.CreateObject<SerializationConfigurationGroup>();
            existentConfiguration.TypeToSerialize=oneToMany.T1Type;
            existentConfiguration.SerializationConfigurationGroup=serializationConfigurationGroup;
            _serializationConfiguration = new SerializationConfiguration(_objectSpace.Session)
            {
                TypeToSerialize = oneToMany.T2Type,
                SerializationConfigurationGroup = serializationConfigurationGroup
            };
        };

        Because of = () => {
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
            _objectSpace.CommitChanges();
        };

        It should_not_create_any_more_graphs_for_associated_type=() => {
            const int customer_order = 2;
            _objectSpace.Session.GetCount(typeof (SerializationConfiguration)).ShouldEqual(customer_order);
        };
    }

    [Subject(typeof(ClassInfoGraphNode))]
    public class When_creating_a_graph_for_type_with_associated_collection:With_Isolations {
        static ObjectSpace _objectSpace;
        static SerializationConfiguration _serializationConfiguration2;
        static Type _t2Type;
        static ISerializationConfiguration _serializationConfiguration;

        Establish context = () => {
            ITypeHandler<ICustomer, IOrder> oneToMany = ModelBuilder<ICustomer,IOrder>.Build().OneToMany();
            _objectSpace = oneToMany.ObjectSpace;
            var serializationConfigurationGroup = _objectSpace.CreateObject<SerializationConfigurationGroup>();            
            serializationConfigurationGroup.Name = "dummy";
            _serializationConfiguration = new SerializationConfiguration(_objectSpace.Session) { TypeToSerialize = oneToMany.T1Type,SerializationConfigurationGroup = serializationConfigurationGroup};
            _t2Type = oneToMany.T2Type;
        };

        Because of = () => new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);

        It should_create_nodes_for_all_properties=() => {
            const int oid_and_orders = 2;
            _serializationConfiguration.SerializationGraph.Count().ShouldEqual(oid_and_orders);
        };

        It should_create_graph_for_the_associated_collection_type=() => {
            _serializationConfiguration2 =
                _serializationConfiguration.Session.FindObject<SerializationConfiguration>(PersistentCriteriaEvaluationBehavior.InTransaction, 
                    configuration => configuration.TypeToSerialize == _t2Type);
            _serializationConfiguration2.ShouldNotBeNull();
        };


        It should_be_able_to_validate_graph_for_the_associated_collection_type =
            () => {
                var ruleSetValidationResult = Validator.RuleSet.ValidateTarget(_serializationConfiguration2,
                                                                               ContextIdentifier.Save);
                ruleSetValidationResult.State.ShouldEqual(ValidationState.Valid);
            };
        It should_create_nodes_for_all_properties_of_associated_collection_type =() => {
            const int customer_and_oid = 2;
            _serializationConfiguration2.SerializationGraph.Count.ShouldEqual(customer_and_oid);
        };
    }
    [Subject(typeof(ClassInfoGraphNode))]
    public class When_generating_graph_for_self_reference_object:With_Isolations {
        static ObjectSpace _objectSpace;
        static SerializationConfiguration _serializationConfiguration;

        Establish context = () =>
        {
            
            _objectSpace = ObjectSpaceInMemory.CreateNew();            
            var persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(_objectSpace, GetUniqueAssemblyName());
            var classHandler = persistentAssemblyBuilder.CreateClasses(new[] { "CustomerSelfRef" });
            var persistentClassInfo = persistentAssemblyBuilder.PersistentAssemblyInfo.PersistentClassInfos[0];
            classHandler.CreateRefenenceMember(persistentClassInfo, "Parent",persistentClassInfo);
            classHandler.CreateCollectionMember(persistentClassInfo, "Collection",persistentClassInfo);

            _objectSpace.CommitChanges();
            var compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder,Path.GetDirectoryName(Application.ExecutablePath));
            var customerType = compileModule.Assembly.GetTypes().Where(type => type.Name == "CustomerSelfRef").Single();
            _serializationConfiguration = new SerializationConfiguration(_objectSpace.Session)
            {
                TypeToSerialize = customerType,
                SerializationConfigurationGroup = _objectSpace.CreateObject<SerializationConfigurationGroup>()
            };
        };


        Because of = () => new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);

        It should_not_create_graph_for_property_of_the_same_type =
            () => {
                _objectSpace.CommitChanges();
                _serializationConfiguration.Session.GetCount(typeof (SerializationConfiguration)).ShouldEqual(1);
            };
    }
    [Subject(typeof(ClassInfoGraphNode))]
    public class When_creating_a_graph_for_a_persistent_assembly:With_Isolations {
        static ObjectSpace _objectSpace;
        static SerializationConfiguration _serializationConfiguration;
        static PersistentAssemblyInfo _persistentAssemblyInfo;

        Establish context = () => {
            _objectSpace = new ObjectSpaceProvider(new MemoryDataStoreProvider()).CreateObjectSpace();
            _persistentAssemblyInfo = (PersistentAssemblyInfo)_objectSpace.CreateObject(typeof(PersistentAssemblyInfo));
            _serializationConfiguration = new SerializationConfiguration(_objectSpace.Session)
            {
                TypeToSerialize = _persistentAssemblyInfo.GetType(),
                SerializationConfigurationGroup = _objectSpace.CreateObject<SerializationConfigurationGroup>()
            };
            
        };

        Because of = () => new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);

        It should_generate_it =
            () =>
            _serializationConfiguration.SerializationGraph.Count().ShouldEqual(
                _persistentAssemblyInfo.ClassInfo.PersistentProperties.OfType<XPMemberInfo>().Count());
    }
    [Subject(typeof(ClassInfoGraphNode))]
    public class When_creating_a_graph_with_a_byte_array_property:With_Isolations {
        static SerializationConfiguration _serializationConfiguration;

        Establish context = () => {
            var objectSpace = ObjectSpaceInMemory.CreateNew();
            _serializationConfiguration = objectSpace.CreateObject<SerializationConfiguration>();
            _serializationConfiguration.TypeToSerialize = typeof (Analysis);            
            _serializationConfiguration.SerializationConfigurationGroup=objectSpace.CreateObject<SerializationConfigurationGroup>();
        };
        Because of = () => new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);

        It should_mark_that_property_as_simple =
            () =>
            _serializationConfiguration.SerializationGraph.Where(node => node.Name == "PivotGridSettingsContent").Single
                ().NodeType.ShouldEqual(NodeType.Simple);
    }
    [Subject(typeof(ClassInfoGraphNodeBuilder),"creating graphs")]
    public class When_object_has_deferred_Deletion : With_Isolations {
        static SerializationConfiguration _serializationConfiguration;

        Establish context = () => {
            var objectSpace = ObjectSpaceInMemory.CreateNew();
            _serializationConfiguration = objectSpace.CreateObject<SerializationConfiguration>();
            _serializationConfiguration.TypeToSerialize = typeof(Analysis);
            _serializationConfiguration.SerializationConfigurationGroup = objectSpace.CreateObject<SerializationConfigurationGroup>();
        };

        Because of = () => new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);

        It should_create_a_gcrecord_node =
            () =>
            _serializationConfiguration.SerializationGraph.Where(node => node.Name == GCRecordField.StaticName).
                FirstOrDefault().ShouldNotBeNull();
    }
}