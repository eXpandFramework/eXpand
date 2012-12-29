using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Machine.Specifications;
using Xpand.ExpressApp.IO.Core;

namespace Xpand.Tests.Xpand.IO.InitData {
    [Subject(typeof(ClassMapper))]
    public class When_creating_dynamic_classes {
        static ReflectionDictionary _inputDictionary;
        static ReflectionDictionary _outputDictionary;
        static ClassMapper _classMapper;
        [InitialData(Name = "MarkedObject1")]
        public class MarkedObject : XPBaseObject {
            [Key(true)]
            public int ID { get; set; }
        }

        public class NotMarkedObject : XPBaseObject {

        }

        Establish context = () => {
            _classMapper = new ClassMapper();
            _outputDictionary = new ReflectionDictionary();
            new ReflectionClassInfo(typeof(MarkedObject), _outputDictionary);
            new ReflectionClassInfo(typeof(NotMarkedObject), _outputDictionary);
            _inputDictionary = new ReflectionDictionary();
        };

        Because of = () => _classMapper.Map(_outputDictionary, _inputDictionary);

        It should_Create_Classes_only_for_the_ones_that_have_a_marking_attribute = () => _inputDictionary.QueryClassInfo(null, typeof(NotMarkedObject).Name).ShouldBeNull();

        It should_name_the_classes_taking_data_from_marking_attribute = () => _inputDictionary.QueryClassInfo(null, "MarkedObject1").ShouldNotBeNull();
    }


    [Subject(typeof(ClassMapper))]
    public class When_class_has_many_to_many_collection_marked_for_importing {
        static XPClassInfo _classInfo;
        static ReflectionDictionary _inputDictionary;
        static ClassMapper _classMapper;
        static ReflectionDictionary _outputDictionary;

        [InitialData(Name = "MarkedObject1")]
        public class MarkedObject2 : XPBaseObject {
            [Key(true)]
            public int ID { get; set; }

            [Association("MarkedObject-MarkedObject2s")]
            public XPCollection<MarkedObject> MarkedObjects {
                get {
                    return GetCollection<MarkedObject>("MarkedObjects");
                }
            }
        }

        [InitialData(Name = "MarkedObject1")]
        public class MarkedObject : XPBaseObject {
            public const string IntermediateTable = "IntermediateTable";
            public const string QueryColumn = "QueryColumn";
            public const string ResultColumn = "ResultColumn";

            [Key(true)]
            public int ID { get; set; }

            [Association("MarkedObject-MarkedObject2s")]
            [InitialData(DataProviderTableName = IntermediateTable, DataProviderQueryColumnName = QueryColumn,
                DataProviderResultColumnName = ResultColumn)]
            public XPCollection<MarkedObject2> MarkedObject2s {
                get { return GetCollection<MarkedObject2>("MarkedObject2s"); }
            }
        }

        Establish context = () => {
            _classMapper = new ClassMapper();
            _outputDictionary = new ReflectionDictionary();
            new ReflectionClassInfo(typeof(MarkedObject), _outputDictionary);
            new ReflectionClassInfo(typeof(MarkedObject2), _outputDictionary);
            _inputDictionary = new ReflectionDictionary();
        };

        Because of = () => _classMapper.Map(_outputDictionary, _inputDictionary);

        It should_create_intermediate_classes = () => {
            _classInfo = _inputDictionary.QueryClassInfo(null, MarkedObject.IntermediateTable);
            _classInfo.ShouldNotBeNull();
        };

        It should_create_an_auto_genareted_key_for_this_class = () => {
            _classInfo.KeyProperty.ShouldNotBeNull();
            ((KeyAttribute)_classInfo.KeyProperty.FindAttributeInfo(typeof(KeyAttribute))).AutoGenerate.ShouldBeTrue();
        };

        It should_create_2_columns_with_names_taken_from_the_marked_attribute = () => {
            _classInfo.FindMember(MarkedObject.QueryColumn).ShouldNotBeNull();
            _classInfo.FindMember(MarkedObject.ResultColumn).ShouldNotBeNull();
        };
    }
}
