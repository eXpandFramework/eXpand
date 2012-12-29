using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using Machine.Specifications;
using Xpand.ExpressApp.IO.Core;
using Xpand.Tests.Xpand.ExpressApp;

namespace Xpand.Tests.Xpand.IO.InitData {
    [Subject(typeof(DictionaryMapper))]
    public class When_is_mapping_a_dictionary : With_Types_info {
        static DictionaryMapper _dictionaryMapper;
        static ReflectionDictionary _inputReflectionDictionary;
        static ReflectionDictionary _outputReflectionDictionary;

        [InitialData]
        public class ObjectA : BaseObject {

        }

        [InitialData]
        public class ObjectB : BaseObject {

        }

        Establish context = () => {
            _dictionaryMapper = new DictionaryMapper(new DBTable[0]);
            _outputReflectionDictionary = new ReflectionDictionary();
            new ReflectionClassInfo(typeof(ObjectA), _outputReflectionDictionary);
            new ReflectionClassInfo(typeof(ObjectB), _outputReflectionDictionary);
            _inputReflectionDictionary = new ReflectionDictionary();
        };

        Because of = () => _dictionaryMapper.Map(_outputReflectionDictionary, _inputReflectionDictionary);

        It should_map_all_marked_objects = () => {
            _inputReflectionDictionary.QueryClassInfo(null, "ObjectA").ShouldNotBeNull();
            _inputReflectionDictionary.QueryClassInfo(null, "ObjectB").ShouldNotBeNull();
        };
    }
}
