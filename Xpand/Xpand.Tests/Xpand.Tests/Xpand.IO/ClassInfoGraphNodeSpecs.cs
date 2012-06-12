using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;
using Machine.Specifications;
using Xpand.ExpressApp.IO.PersistentTypesHelpers;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using Xpand.Persistent.Base.ImportExport;
using Xpand.Persistent.BaseImpl.ImportExport;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using Xpand.Xpo;
using Xpand.Persistent.Base.General;

namespace Xpand.Tests.Xpand.IO {
    [Subject(typeof(ClassInfoGraphNodeBuilder))]
    public class When_creating_a_graph_for_type_with_associated_collection_and_associated_type_derived_types_exist_in_the_domain : With_Isolations {
        static Type _derivedOrderType;
        static SerializationConfiguration _serializationConfiguration;
        static XPObjectSpace _XPObjectSpace;

        Establish context = () => {

            ITypeHandler<ICustomer, IOrder> oneToMany = ModelBuilder<ICustomer, IOrder>.Build().OneToMany();
            _XPObjectSpace = oneToMany.XPObjectSpace;

            _serializationConfiguration = new SerializationConfiguration(_XPObjectSpace.Session) {
                TypeToSerialize = oneToMany.T1Type,
                SerializationConfigurationGroup = _XPObjectSpace.CreateObject<SerializationConfigurationGroup>()
            };

            var persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(_XPObjectSpace, GetUniqueAssemblyName());
            var classHandler = persistentAssemblyBuilder.CreateClasses(new[] { "DerivedOrder" });
            classHandler.SetInheritance(info => oneToMany.T2Type);

            _XPObjectSpace.CommitChanges();

            var compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));
            _derivedOrderType = compileModule.Assembly.GetTypes().Where(type => type.Name == "DerivedOrder").Single();
            var existentConfiguration =
                (SerializationConfiguration)_XPObjectSpace.CreateObject(typeof(SerializationConfiguration));
            existentConfiguration.TypeToSerialize = _derivedOrderType;
            existentConfiguration.SerializationConfigurationGroup = _serializationConfiguration.SerializationConfigurationGroup;
        };

        Because of = () => {
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
            _XPObjectSpace.CommitChanges();
        };

        It should_create_graph_for_derived_types =
            () =>
            new XPQuery<SerializationConfiguration>(_XPObjectSpace.Session).Where(
                configuration => configuration.TypeToSerialize == _derivedOrderType).Count().ShouldEqual(1);
    }
    [Subject(typeof(ClassInfoGraphNodeBuilder))]
    public class When_creating_a_graph_for_type_with_associated_object_and_associated_type_derived_types_exist_in_the_domain : With_Isolations {
        static Type _derivedCustomerType;
        static SerializationConfiguration _serializationConfiguration;
        static XPObjectSpace _XPObjectSpace;

        Establish context = () => {

            ITypeHandler<ICustomer, IOrder> oneToMany = ModelBuilder<ICustomer, IOrder>.Build().OneToMany();
            _XPObjectSpace = oneToMany.XPObjectSpace;
            var existentConfiguration =
                (SerializationConfiguration)_XPObjectSpace.CreateObject(typeof(SerializationConfiguration));
            existentConfiguration.TypeToSerialize = oneToMany.T2Type;
            _serializationConfiguration = new SerializationConfiguration(_XPObjectSpace.Session) {
                TypeToSerialize = oneToMany.T2Type,
                SerializationConfigurationGroup = _XPObjectSpace.CreateObject<SerializationConfigurationGroup>()
            };

            var persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(_XPObjectSpace, GetUniqueAssemblyName());
            var classHandler = persistentAssemblyBuilder.CreateClasses(new[] { "DerivedCustomer" });
            classHandler.SetInheritance(info => oneToMany.T1Type);

            _XPObjectSpace.CommitChanges();

            var compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));
            _derivedCustomerType = compileModule.Assembly.GetTypes().Where(type => type.Name == "DerivedCustomer").Single();
        };
        Because of = () => {
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
            _XPObjectSpace.CommitChanges();
        };

        It should_create_graph_for_derived_types = () => new XPQuery<SerializationConfiguration>(_XPObjectSpace.Session).Where(configuration => configuration.TypeToSerialize == _derivedCustomerType).Count().ShouldEqual(1);
    }
    [Subject(typeof(ClassInfoGraphNodeBuilder))]
    public class When_creating_a_graph_for_type_and_type_derived_types_exist_in_the_domain : With_Isolations {
        static SerializationConfiguration _serializationConfiguration;
        static Type _t1Type;
        static Type _t2Type;
        static XPObjectSpace _XPObjectSpace;

        Establish context = () => {
            _XPObjectSpace = (XPObjectSpace)new XPObjectSpaceProvider(new MemoryDataStoreProvider()).CreateObjectSpace();
            var persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(_XPObjectSpace, GetUniqueAssemblyName());
            var classHandler = persistentAssemblyBuilder.CreateClasses(new[] { "Customer", "DerivedCustomer" });
            classHandler.SetInheritance(info => info.Name == "DerivedCustomer" ? persistentAssemblyBuilder.PersistentAssemblyInfo.PersistentClassInfos[0] : null);

            _XPObjectSpace.CommitChanges();

            var compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));
            _t1Type = compileModule.Assembly.GetTypes().Where(type => type.Name == "Customer").Single();
            _t2Type = compileModule.Assembly.GetTypes().Where(type => type.Name == "DerivedCustomer").Single();
            _serializationConfiguration = new SerializationConfiguration(_XPObjectSpace.Session) {
                TypeToSerialize = _t1Type,
                SerializationConfigurationGroup = _XPObjectSpace.CreateObject<SerializationConfigurationGroup>()
            };
        };


        Because of = () => {
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
            _XPObjectSpace.CommitChanges();
        };
        It should_create_graph_for_derived_types = () => new XPQuery<SerializationConfiguration>(_XPObjectSpace.Session).Where(
                                                           configuration => configuration.TypeToSerialize == _t2Type).Count().ShouldEqual(1);
    }
    [Subject(typeof(ClassInfoGraphNodeBuilder))]
    public class When_creating_a_graph__for_type_with_associated_collection_and_associated_type_graph_exists : With_Isolations {
        static SerializationConfiguration _serializationConfiguration;
        static XPObjectSpace _XPObjectSpace;

        Establish context = () => {
            ITypeHandler<ICustomer, IOrder> oneToMany = ModelBuilder<ICustomer, IOrder>.Build().OneToMany();
            _XPObjectSpace = oneToMany.XPObjectSpace;
            var existentConfiguration =
                (SerializationConfiguration)_XPObjectSpace.CreateObject(typeof(SerializationConfiguration));
            var serializationConfigurationGroup = _XPObjectSpace.CreateObject<SerializationConfigurationGroup>();
            existentConfiguration.TypeToSerialize = oneToMany.T2Type;
            existentConfiguration.SerializationConfigurationGroup = serializationConfigurationGroup;
            _serializationConfiguration = new SerializationConfiguration(_XPObjectSpace.Session) {
                TypeToSerialize = oneToMany.T1Type,
                SerializationConfigurationGroup = serializationConfigurationGroup
            };
        };

        Because of = () => {
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
            _XPObjectSpace.CommitChanges();
        };

        It should_not_create_any_more_graphs_for_associated_type = () => {
            const int customer_order = 2;
            _XPObjectSpace.Session.GetCount(typeof(SerializationConfiguration)).ShouldEqual(customer_order);
        };
    }
    [Subject(typeof(ClassInfoGraphNodeBuilder))]
    public class When_creating_a_graph__for_type_with_associated_object_and_associated_type_graph_exists : With_Isolations {
        static XPObjectSpace _XPObjectSpace;
        static SerializationConfiguration _serializationConfiguration;

        Establish context = () => {
            ITypeHandler<ICustomer, IOrder> oneToMany = ModelBuilder<ICustomer, IOrder>.Build().OneToMany();
            _XPObjectSpace = oneToMany.XPObjectSpace;
            var existentConfiguration =
                (SerializationConfiguration)_XPObjectSpace.CreateObject(typeof(SerializationConfiguration));
            var serializationConfigurationGroup = _XPObjectSpace.CreateObject<SerializationConfigurationGroup>();
            existentConfiguration.TypeToSerialize = oneToMany.T1Type;
            existentConfiguration.SerializationConfigurationGroup = serializationConfigurationGroup;
            _serializationConfiguration = new SerializationConfiguration(_XPObjectSpace.Session) {
                TypeToSerialize = oneToMany.T2Type,
                SerializationConfigurationGroup = serializationConfigurationGroup
            };
        };

        Because of = () => {
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
            _XPObjectSpace.CommitChanges();
        };

        It should_not_create_any_more_graphs_for_associated_type = () => {
            const int customer_order = 2;
            _XPObjectSpace.Session.GetCount(typeof(SerializationConfiguration)).ShouldEqual(customer_order);
        };
    }

    [Subject(typeof(ClassInfoGraphNodeBuilder))]
    public class When_creating_a_graph_for_type_with_associated_collection : With_Isolations {
        static XPObjectSpace _XPObjectSpace;
        static SerializationConfiguration _serializationConfiguration2;
        static Type _t2Type;
        static ISerializationConfiguration _serializationConfiguration;

        Establish context = () => {
            ITypeHandler<ICustomer, IOrder> oneToMany = ModelBuilder<ICustomer, IOrder>.Build().OneToMany();
            _XPObjectSpace = oneToMany.XPObjectSpace;
            var serializationConfigurationGroup = _XPObjectSpace.CreateObject<SerializationConfigurationGroup>();
            serializationConfigurationGroup.Name = "dummy";
            _serializationConfiguration = new SerializationConfiguration(_XPObjectSpace.Session) { TypeToSerialize = oneToMany.T1Type, SerializationConfigurationGroup = serializationConfigurationGroup };
            _t2Type = oneToMany.T2Type;
        };

        Because of = () => new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);

        It should_create_nodes_for_all_properties = () => {
            const int oid_and_orders_and_gcrecord = 3;
            _serializationConfiguration.SerializationGraph.Count().ShouldEqual(oid_and_orders_and_gcrecord);
        };

        It should_create_graph_for_the_associated_collection_type = () => {
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
        It should_create_nodes_for_all_properties_of_associated_collection_type = () => {
            const int customer_and_oid_and_gcrecord = 3;
            _serializationConfiguration2.SerializationGraph.Count.ShouldEqual(customer_and_oid_and_gcrecord);
        };
    }
    [Subject(typeof(ClassInfoGraphNodeBuilder))]
    public class When_generating_graph_for_self_reference_object : With_Isolations {
        static XPObjectSpace _XPObjectSpace;
        static SerializationConfiguration _serializationConfiguration;

        Establish context = () => {

            _XPObjectSpace = (XPObjectSpace)ObjectSpaceInMemory.CreateNew();
            var persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(_XPObjectSpace, GetUniqueAssemblyName());
            var classHandler = persistentAssemblyBuilder.CreateClasses(new[] { "CustomerSelfRef" });
            var persistentClassInfo = persistentAssemblyBuilder.PersistentAssemblyInfo.PersistentClassInfos[0];
            classHandler.CreateRefenenceMember(persistentClassInfo, "Parent1", persistentClassInfo, false);

            _XPObjectSpace.CommitChanges();
            var compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));
            var customerType = compileModule.Assembly.GetTypes().Where(type => type.Name == "CustomerSelfRef").Single();
            _serializationConfiguration = new SerializationConfiguration(_XPObjectSpace.Session) {
                TypeToSerialize = customerType,
                SerializationConfigurationGroup = _XPObjectSpace.CreateObject<SerializationConfigurationGroup>()
            };
        };


        Because of = () => new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);

        It should_not_create_graph_for_property_of_the_same_type =
            () => {
                _XPObjectSpace.CommitChanges();
                _serializationConfiguration.Session.GetCount(typeof(SerializationConfiguration)).ShouldEqual(1);
            };
    }
    [Subject(typeof(ClassInfoGraphNodeBuilder))]
    public class When_creating_a_graph_for_a_persistent_assembly : With_Isolations {
        private const int GcRecord = 1;
        static XPObjectSpace _XPObjectSpace;
        static SerializationConfiguration _serializationConfiguration;
        static PersistentAssemblyInfo _persistentAssemblyInfo;

        Establish context = () => {
            _XPObjectSpace = (XPObjectSpace)new XPObjectSpaceProvider(new MemoryDataStoreProvider()).CreateObjectSpace();
            _persistentAssemblyInfo = (PersistentAssemblyInfo)_XPObjectSpace.CreateObject(typeof(PersistentAssemblyInfo));
            _serializationConfiguration = new SerializationConfiguration(_XPObjectSpace.Session) {
                TypeToSerialize = _persistentAssemblyInfo.GetType(),
                SerializationConfigurationGroup = _XPObjectSpace.CreateObject<SerializationConfigurationGroup>()
            };

        };

        Because of = () => new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);

        It should_generate_it =
            () => _serializationConfiguration.SerializationGraph.Count().ShouldEqual(
                _persistentAssemblyInfo.ClassInfo.PersistentProperties.OfType<XPMemberInfo>().Count() + GcRecord);
    }
    [Subject(typeof(ClassInfoGraphNodeBuilder))]
    public class When_creating_a_graph_with_a_byte_array_property : With_Isolations {
        static SerializationConfiguration _serializationConfiguration;

        Establish context = () => {
            var XPObjectSpace = ObjectSpaceInMemory.CreateNew();
            _serializationConfiguration = XPObjectSpace.CreateObject<SerializationConfiguration>();
            _serializationConfiguration.TypeToSerialize = typeof(Analysis);
            _serializationConfiguration.SerializationConfigurationGroup = XPObjectSpace.CreateObject<SerializationConfigurationGroup>();
        };
        Because of = () => new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);

        It should_mark_that_property_as_simple =
            () =>
            _serializationConfiguration.SerializationGraph.Where(node => node.Name == "PivotGridSettingsContent").Single
                ().NodeType.ShouldEqual(NodeType.Simple);
    }
    [Subject(typeof(ClassInfoGraphNodeBuilder), "creating graphs")]
    public class When_object_has_deferred_Deletion : With_Isolations {
        static SerializationConfiguration _serializationConfiguration;

        Establish context = () => {
            var XPObjectSpace = ObjectSpaceInMemory.CreateNew();
            _serializationConfiguration = XPObjectSpace.CreateObject<SerializationConfiguration>();
            _serializationConfiguration.TypeToSerialize = typeof(Analysis);
            _serializationConfiguration.SerializationConfigurationGroup = XPObjectSpace.CreateObject<SerializationConfigurationGroup>();
        };

        Because of = () => new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);

        It should_create_a_gcrecord_node =
            () =>
            _serializationConfiguration.SerializationGraph.Where(node => node.Name == GCRecordField.StaticName).
                FirstOrDefault().ShouldNotBeNull();
    }

    [Subject(typeof(ClassInfoGraphNodeBuilder), "creating graphs")]
    public class When_property_is_mark_with_serialializationStrategyAttribute : With_Isolations {
        static SerializationConfiguration _serializationConfiguration;

        Establish context = () => {
            var XPObjectSpace = ObjectSpaceInMemory.CreateNew();
            _serializationConfiguration = XPObjectSpace.CreateObject<SerializationConfiguration>();
            _serializationConfiguration.TypeToSerialize = typeof(Analysis);
            _serializationConfiguration.SerializationConfigurationGroup = XPObjectSpace.CreateObject<SerializationConfigurationGroup>();
            XafTypesInfo.Instance.FindTypeInfo(typeof(Analysis)).FindMember("Name").AddAttribute(new SerializationStrategyAttribute(SerializationStrategy.SerializeAsObject));
        };

        Because of = () => new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);

        It should_create_a_node_with_serialize_strategy_the_same_as_the_attribute_strategy =
            () =>
            _serializationConfiguration.SerializationGraph.Where(node => node.Name == "Name").Single().
                SerializationStrategy.ShouldEqual(SerializationStrategy.SerializeAsObject);
    }
    [Subject(typeof(ClassInfoGraphNodeBuilder), "creating graphs")]
    public class When_property_type_is_mark_with_serialializationStrategyAttribute : With_Isolations {
        static SerializationConfiguration _serializationConfiguration;

        Establish context = () => {
            var XPObjectSpace = ObjectSpaceInMemory.CreateNew();
            _serializationConfiguration = XPObjectSpace.CreateObject<SerializationConfiguration>();
            _serializationConfiguration.TypeToSerialize = typeof(Analysis);
            _serializationConfiguration.SerializationConfigurationGroup = XPObjectSpace.CreateObject<SerializationConfigurationGroup>();
            XafTypesInfo.Instance.RegisterEntity(typeof(User));
            XafTypesInfo.Instance.FindTypeInfo(typeof(Analysis)).CreateMember("User", typeof(User));
            XafTypesInfo.Instance.FindTypeInfo(typeof(User)).AddAttribute(new SerializationStrategyAttribute(SerializationStrategy.DoNotSerialize));
        };

        Because of = () => new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);

        It should_create_a_node_with_serialize_strategy_the_same_as_the_type_attribute_strategy =
            () =>
            _serializationConfiguration.SerializationGraph.Where(node => node.Name == "User").Single().
                SerializationStrategy.ShouldEqual(SerializationStrategy.DoNotSerialize);
    }
    [Subject(typeof(ClassInfoGraphNodeBuilder), "creating graphs")]
    public class When_property_is_marked_with_serializationKeyAttribute : With_Isolations {
        static SerializationConfiguration _serializationConfiguration;

        Establish context = () => {
            var XPObjectSpace = ObjectSpaceInMemory.CreateNew();
            _serializationConfiguration = XPObjectSpace.CreateObject<SerializationConfiguration>();
            _serializationConfiguration.TypeToSerialize = typeof(Analysis);
            _serializationConfiguration.SerializationConfigurationGroup = XPObjectSpace.CreateObject<SerializationConfigurationGroup>();
            XafTypesInfo.Instance.FindTypeInfo(typeof(Analysis)).FindMember("Name").AddAttribute(new SerializationKeyAttribute());
        };

        Because of = () => new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
        It should_create_a_node_marked_as_key =
            () =>
            _serializationConfiguration.SerializationGraph.Where(node => node.Name == "Name").Single().Key.ShouldEqual(true);

        It should_not_have_any_other_key_nodes =
            () => _serializationConfiguration.SerializationGraph.Where(node => node.Key).Count().ShouldEqual(1);
    }
    [Subject(typeof(ClassInfoGraphNodeBuilder), "applying strategy")]
    public class When_applying_a_serialization_strategy_to_a_reference_property : With_Isolations {
        static SerializationConfiguration _serializationConfiguration;

        Establish context = () => {
            var XPObjectSpace = ObjectSpaceInMemory.CreateNew();
            XafTypesInfo.Instance.RegisterEntity(typeof(User));
            XafTypesInfo.Instance.RegisterEntity(typeof(Analysis));
            XafTypesInfo.Instance.CreateBothPartMembers(typeof(Analysis), typeof(User), XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary);
            _serializationConfiguration = XPObjectSpace.CreateObject<SerializationConfiguration>();
            _serializationConfiguration.TypeToSerialize = typeof(Analysis);
            _serializationConfiguration.SerializationConfigurationGroup = XPObjectSpace.CreateObject<SerializationConfigurationGroup>();
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
        };

        Because of = () => new ClassInfoGraphNodeBuilder().ApplyStrategy(SerializationStrategy.DoNotSerialize, _serializationConfiguration.SerializationConfigurationGroup.SerializationConfigurations.Where(configuration => configuration.TypeToSerialize == typeof(User)).Single());
        It should_apply_the_strategy_to_all_reference_properties =
            () =>
            _serializationConfiguration.SerializationGraph.Where(node => node.Name == "Users").Single().SerializationStrategy.ShouldEqual(SerializationStrategy.DoNotSerialize);
    }
    [Subject(typeof(ClassInfoGraphNodeBuilder), "applying strategy")]
    public class When_applying_a_serialization_and_reference_exists : With_Isolations {
        static SerializationConfiguration _serializationConfiguration;

        Establish context = () => {
            var XPObjectSpace = ObjectSpaceInMemory.CreateNew();
            XafTypesInfo.Instance.RegisterEntity(typeof(User));
            XafTypesInfo.Instance.RegisterEntity(typeof(Analysis));
            XafTypesInfo.Instance.CreateBothPartMembers(typeof(Analysis), typeof(User), XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary);
            _serializationConfiguration = XPObjectSpace.CreateObject<SerializationConfiguration>();
            _serializationConfiguration.TypeToSerialize = typeof(User);
            _serializationConfiguration.SerializationConfigurationGroup = XPObjectSpace.CreateObject<SerializationConfigurationGroup>();
            new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
        };

        Because of = () => new ClassInfoGraphNodeBuilder().ApplyStrategy(SerializationStrategy.DoNotSerialize, _serializationConfiguration.SerializationConfigurationGroup.SerializationConfigurations.Where(configuration => configuration.TypeToSerialize == typeof(Analysis)).Single());
        It should_apply_the_strategy_to_all_reference_properties =
            () =>
            _serializationConfiguration.SerializationGraph.Where(node => node.Name == "Analysis").Single().SerializationStrategy.ShouldEqual(SerializationStrategy.DoNotSerialize);


    }

    public class When_property_is_a_non_XPCollection : With_Isolations {
        static SerializationConfiguration _serializationConfiguration;

        public class ObjectWithIBindingListCollection : BaseObject {
            public ObjectWithIBindingListCollection(Session session)
                : base(session) {
            }

            private readonly IBindingList _collection = new BindingList<ObjectWithIBindingListCollection>();
            public IBindingList Collection {
                get { return _collection; }
            }
        }

        Establish context = () => {
            var XPObjectSpace = ObjectSpaceInMemory.CreateNew();
            _serializationConfiguration = XPObjectSpace.CreateObject<SerializationConfiguration>();
            _serializationConfiguration.TypeToSerialize = typeof(ObjectWithIBindingListCollection);
            _serializationConfiguration.SerializationConfigurationGroup = XPObjectSpace.CreateObject<SerializationConfigurationGroup>();
        };

        Because of = () => new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);

        It should_not_create_node_for_that_property =
            () => _serializationConfiguration.SerializationGraph.Where(node => node.Name == "Collection").FirstOrDefault().ShouldBeNull();
    }
}