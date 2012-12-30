using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Machine.Specifications;
using Xpand.ExpressApp.IO.Core;
using Xpand.Tests.Xpand.ExpressApp;

namespace Xpand.Tests.Xpand.IO.InitData {
    [Subject(typeof(ClassMapper))]
    public class When_creating_dynamic_classes : With_Types_info {
        static ReflectionDictionary _inputDictionary;
        static ReflectionDictionary _outputDictionary;
        static ClassMapper _classMapper;
        [InitialData(Name = "MarkedObject1")]
        public class InitDataMarkedObject : XPBaseObject {
            [Key(true)]
            public int ID { get; set; }
        }

        public class NotMarkedObject : XPBaseObject {

        }

        Establish context = () => {
            _classMapper = new ClassMapper();
            _outputDictionary = new ReflectionDictionary();
            new ReflectionClassInfo(typeof(InitDataMarkedObject), _outputDictionary);
            new ReflectionClassInfo(typeof(NotMarkedObject), _outputDictionary);
            _inputDictionary = new ReflectionDictionary();
        };

        Because of = () => _classMapper.Map(_outputDictionary, _inputDictionary);

        It should_Create_Classes_only_for_the_ones_that_have_a_marking_attribute = () => _inputDictionary.QueryClassInfo(null, typeof(NotMarkedObject).Name).ShouldBeNull();

        It should_name_the_classes_taking_data_from_marking_attribute = () => _inputDictionary.QueryClassInfo(null, "MarkedObject1").ShouldNotBeNull();
    }


    [Subject(typeof(ClassMapper))]
    public class When_class_has_many_to_many_collection_marked_for_importing : With_Types_info {
        private const string InitDataObject1 = "InitDataObject1";
        static XPClassInfo _classInfo;
        static ReflectionDictionary _inputDictionary;
        static ClassMapper _classMapper;
        static ReflectionDictionary _outputDictionary;

        [InitialData(Name = InitDataObject1)]
        public class MarkedObject2 : XPBaseObject {
            [Key(true)]
            public int ID { get; set; }

            [Association("InitDataObject-MarkedObject2s")]
            public XPCollection<InitDataObject> MarkedObjects {
                get {
                    return GetCollection<InitDataObject>("MarkedObjects");
                }
            }
        }

        [InitialData(Name = InitDataObject1)]
        public class InitDataObject : XPBaseObject {
            public const string IntermediateTable = "IntermediateTable";
            public const string QueryColumn = "QueryColumn";
            public const string ResultColumn = "ResultColumn";

            [Key(true)]
            public int ID { get; set; }

            [Association("InitDataObject-MarkedObject2s")]
            [InitialData(DataProviderTableName = IntermediateTable, DataProviderQueryColumnName = QueryColumn,
                DataProviderResultColumnName = ResultColumn)]
            public XPCollection<MarkedObject2> MarkedObject2s {
                get { return GetCollection<MarkedObject2>("MarkedObject2s"); }
            }
        }

        Establish context = () => {
            _classMapper = new ClassMapper();
            _outputDictionary = new ReflectionDictionary();
            new ReflectionClassInfo(typeof(InitDataObject), _outputDictionary);
            new ReflectionClassInfo(typeof(MarkedObject2), _outputDictionary);
            _inputDictionary = new ReflectionDictionary();
        };

        Because of = () => _classMapper.Map(_outputDictionary, _inputDictionary);

        It should_create_intermediate_classes = () => {
            _classInfo = _inputDictionary.QueryClassInfo(null, InitDataObject.IntermediateTable);
            _classInfo.ShouldNotBeNull();
        };

        It should_create_an_auto_genareted_key_for_this_class = () => {
            _classInfo.KeyProperty.ShouldNotBeNull();
            ((KeyAttribute)_classInfo.KeyProperty.FindAttributeInfo(typeof(KeyAttribute))).AutoGenerate.ShouldBeTrue();
        };

        It should_create_2_columns_with_names_taken_from_the_marked_attribute = () => {
            _classInfo.FindMember(InitDataObject.QueryColumn).ShouldNotBeNull();
            _classInfo.FindMember(InitDataObject.ResultColumn).ShouldNotBeNull();
        };
    }
}
