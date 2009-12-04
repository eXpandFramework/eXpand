using DevExpress.Xpo;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using Machine.Specifications;

namespace eXpand.Tests.WorldCreator {
    [Subject(typeof(InterfaceInfo))]
    public class When_linking_with_a_PersistentInterfaceInfo {
        Because of = () => {
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession);
            var interfaceInfos = persistentClassInfo.Interfaces;
            interfaceInfos.Add(new InterfaceInfo(Session.DefaultSession));            
//            CreateMembersFrom(interfaceInfos);
        };
        It should_create_all_missing_persistent_member_infos;
        It should_have_generated_code_to_all_missing_member_infos;
    }
    public class When_creating_members_from_interfaceinfo_with_unsupported_members
    {
        It should_throw_if_interface_properties_count_not_the_same_as_members_count;
    }
    public class When_creating_members_from_interfaceinfo
    {
        It should_create_a_core_member_info_with_correct_datatype_for_non_bussinsess_interface_properties;
        It should_create_a_reference_member_info_with_correct_ReferenceType_for_bussinsess_interface_properties;

    }
}