using eXpand.Persistent.BaseImpl.PersistentMetaData;
using Machine.Specifications;

namespace eXpand.Tests.WorldCreator {
    [Subject(typeof(PersistentTypeInfo),"Creation")]
    public class When_A_persistentTypeInfo_object_is_creating {
        It should_initialize_its_template;
    }
    public class When_modifying_default_generated_code_of_a_persistentTypeInfo_object
    {
        It should_copy_template_code_to_generated_code;
    }

}