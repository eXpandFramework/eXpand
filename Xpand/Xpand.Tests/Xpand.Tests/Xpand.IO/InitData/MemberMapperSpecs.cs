using System;
using System.Linq;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using Machine.Specifications;
using Xpand.ExpressApp.IO.Core;

namespace Xpand.Tests.Xpand.IO.InitData {
    [Subject(typeof(MemberMapper))]
    public class When_Class_Has_a_flag_that_includes_all_members {
        private const string InputPutClass = "InputPutClass";
        static XPClassInfo _outputClassInfo;
        static XPClassInfo _inputClassInfo;
        static MemberMapper _memberMapper;

        [InitialData(AllOwnMembers = true)]
        public class AllOwnMembersClass : BaseObject {
            public string Test { get; set; }
        }

        Establish context = () => {
            var table = InitDataImporterHelper.CreateDbTable(InputPutClass, new ColumnInfo("Test", DBColumnType.String));
            _memberMapper = new MemberMapper(table);
            _outputClassInfo = new ReflectionClassInfo(typeof(AllOwnMembersClass), new ReflectionDictionary());
            _inputClassInfo = new ReflectionDictionary().CreateClass(InputPutClass);
        };

        Because of = () => _memberMapper.Map(_outputClassInfo, _inputClassInfo);

        It should_collect_all_members_that_belong_to_the_class = () => _inputClassInfo.Members.Single(info => info.Name == "Test").ShouldNotBeNull();
        It should_not_collect_any_base_class_member = () => _inputClassInfo.Members.SingleOrDefault(info => info.Name == "Oid").ShouldBeNull();
        It should_create_new_members_for_each_one_of_them = () => _inputClassInfo.Members.Single(info => info.Name == "Test").ShouldNotBeNull();
    }

    [Subject(typeof(MemberMapper))]
    public class When_class_does_not_have_a_flag_that_includes_all_ownmembers {
        const int TestProperty = 1;
        const int TotalPropertiesCount = TestProperty;
        private const string InputPutClass = "InputPutClass";
        static XPClassInfo _inputClassInfo;
        static XPClassInfo _outputClassInfo;
        static MemberMapper _memberMapper;

        [InitialData]
        public class NotAllOwnMembersClass : BaseObject {
            [InitialData(Name = "Test1")]
            public string Test { get; set; }

            public string NotIncluded { get; set; }
        }

        Establish context = () => {
            var table = InitDataImporterHelper.CreateDbTable(InputPutClass, new ColumnInfo("Test1", DBColumnType.String));
            _memberMapper = new MemberMapper(table);
            _outputClassInfo = new ReflectionClassInfo(typeof(NotAllOwnMembersClass), new ReflectionDictionary());
            _inputClassInfo = new ReflectionDictionary().CreateClass(InputPutClass);
        };

        Because of = () => _memberMapper.Map(_outputClassInfo, _inputClassInfo);

        It should_not_create_members_for_those_not_marked_with_the_attribute = () => _inputClassInfo.FindMember("NotIncluded").ShouldBeNull();

        It should_create_members_and_name_them_according_to_the_attribute_data = () => _inputClassInfo.FindMember("Test1").ShouldNotBeNull();

        It should_create_a_key_property_if_not_included_in_the_conficuration = () => _inputClassInfo.KeyProperty.ShouldNotBeNull();
    }
    [Subject(typeof(MemberMapper))]
    public class When_class_attribute_has_data_for_base_members {
        private const string InputPutClass = "InputPutClass";
        static ReflectionClassInfo _outputClassInfo;
        static XPMemberInfo _memberInfo;
        static XPClassInfo _inputClassInfo;
        static MemberMapper _memberMapper;

        [InitialData(BaseMembers = "oid|Oid")]
        public class BaseMembersClass : BaseObject {

        }

        Establish context = () => {
            var table = InitDataImporterHelper.CreateDbTable(InputPutClass, new ColumnInfo("Oid", DBColumnType.Guid));
            _memberMapper = new MemberMapper(table);
            _outputClassInfo = new ReflectionClassInfo(typeof(BaseMembersClass), new ReflectionDictionary());
            _inputClassInfo = new ReflectionDictionary().CreateClass(InputPutClass);
        };

        Because of = () => _memberMapper.Map(_outputClassInfo, _inputClassInfo);

        It should_create_and_name_them_according_to_the_attribute_data = () => {
            _memberInfo = _inputClassInfo.FindMember("Oid");
            _memberInfo.ShouldNotBeNull();
        };

        It should_map_their_attributes = () => _memberInfo.IsKey.ShouldBeTrue();
    }
    [Subject(typeof(MemberMapper))]
    public class When_class_is_marked_to_include_all_member_but_member_is_marked_seperately {
        private const string InputPutClass = "InputPutClass";
        static ReflectionClassInfo _outputClassInfo;
        static XPMemberInfo _memberInfo;
        static XPClassInfo _inputClassInfo;
        static MemberMapper _memberMapper;

        [InitialData(AllOwnMembers = true)]
        public class AttributeWeightClass : BaseObject {
            [InitialData(Name = "Test1")]
            public string Test { get; set; }
        }

        Establish context = () => {
            var table = InitDataImporterHelper.CreateDbTable(InputPutClass, new ColumnInfo("Test1", DBColumnType.String));
            _memberMapper = new MemberMapper(table);
            _outputClassInfo = new ReflectionClassInfo(typeof(AttributeWeightClass), new ReflectionDictionary());
            _inputClassInfo = new ReflectionDictionary().CreateClass(InputPutClass);
        };

        Because of = () => _memberMapper.Map(_outputClassInfo, _inputClassInfo);
        It should_use_the_member_marking = () => _inputClassInfo.FindMember("Test1");
    }
    [Subject(typeof(MemberMapper))]
    public class When_mapping_a_referenced_member {
        private const string InputPutClass = "InputPutClass";
        private const string Test1 = "Test1";
        static ReflectionClassInfo _outputClassInfo;
        static XPMemberInfo _memberInfo;
        static XPClassInfo _inputMemberClassInfo;
        static MemberMapper _memberMapper;

        static XPClassInfo _inputRefMemberClassInfo;

        [InitialData(AllOwnMembers = true)]
        public class MemberClass : BaseObject {
            [InitialData(Name = Test1)]
            public RefMemberClass Test { get; set; }
        }
        [InitialData]
        public class RefMemberClass : BaseObject {

        }

        Establish context = () => {
            var table = InitDataImporterHelper.CreateDbTable(InputPutClass, new ColumnInfo(Test1, DBColumnType.String));
            _memberMapper = new MemberMapper(table);
            _outputClassInfo = new ReflectionClassInfo(typeof(MemberClass), new ReflectionDictionary());
            var inputReflectionDictionary = new ReflectionDictionary();
            _inputMemberClassInfo = inputReflectionDictionary.CreateClass(InputPutClass);
            _inputRefMemberClassInfo = inputReflectionDictionary.CreateClass("RefMemberClass");
        };

        Because of = () => _memberMapper.Map(_outputClassInfo, _inputMemberClassInfo);

        It should_create_a_member_with_a_dynamic_type_ownwer = () => {
            var xpMemberInfo = _inputMemberClassInfo.FindMember(Test1);
            xpMemberInfo.ReferenceType.ShouldEqual(_inputRefMemberClassInfo);
        };
    }
    [Subject(typeof(MemberMapper))]
    public class When_reference_type_is_not_includedIN_the_configuration {
        private const string InputPutClass = "InputPutClass";
        private const string Test1 = "Test1";
        static ReflectionClassInfo _outputClassInfo;
        static XPMemberInfo _memberInfo;
        static XPClassInfo _inputMemberClassInfo;
        static MemberMapper _memberMapper;



        [InitialData(AllOwnMembers = true)]
        public class MemberClass : BaseObject {
            [InitialData(Name = Test1)]
            public RefMemberClass Test { get; set; }
        }

        public class RefMemberClass : BaseObject {

        }

        Establish context = () => {
            var dbTable = InitDataImporterHelper.CreateDbTable(InputPutClass, new ColumnInfo(Test1, DBColumnType.Guid));
            _memberMapper = new MemberMapper(dbTable);
            _outputClassInfo = new ReflectionClassInfo(typeof(MemberClass), new ReflectionDictionary());
            var inputReflectionDictionary = new ReflectionDictionary();
            _inputMemberClassInfo = inputReflectionDictionary.CreateClass(InputPutClass);

        };

        Because of = () => _memberMapper.Map(_outputClassInfo, _inputMemberClassInfo);

        It should_create_non_reference_member = () => {
            var xpMemberInfo = _inputMemberClassInfo.FindMember(Test1);
            xpMemberInfo.ReferenceType.ShouldBeNull();
            xpMemberInfo.MemberType.ShouldEqual(typeof(Guid));
        };
    }



}
