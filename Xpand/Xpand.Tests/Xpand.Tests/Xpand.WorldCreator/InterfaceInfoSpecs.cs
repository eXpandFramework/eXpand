using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo.DB;
using Xpand.ExpressApp.WorldCreator.Core;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;
using System.Linq;
using System;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.Tests.Xpand.WorldCreator {

    [Isolated]
    public class When_creating_members_from_interfaceinfo_with_collection_members : with_classInfo_with_interfaceInfos<INotSupported> {
        static Exception _exception;

        Because of = () => {
            _exception = Catch.Exception(() => _persistentClassInfo.CreateMembersFromInterfaces());
        };

        It should_throw_exception = () => _exception.ShouldNotBeNull();
    }

    [Isolated]
    public class When_creating_members_from_interfaceinfo : with_classInfo_with_interfaceInfos<IDummyString> {
        Establish context = () => {

        };
        Because of = () => _persistentClassInfo.CreateMembersFromInterfaces();

        It should_create_all_missing_core_type_persistent_member_infos = () => {
            var persistentMemberInfo = _persistentClassInfo.OwnMembers.Where(info => info.Name == "StringPropertyName").FirstOrDefault();
            persistentMemberInfo.ShouldNotBeNull();
            if (persistentMemberInfo != null)
                ((IPersistentCoreTypeMemberInfo)persistentMemberInfo).DataType.ShouldEqual(DBColumnType.String);
        };

        It should_create_all_missing_reference_member_infos = () => {
            var persistentMemberInfo = _persistentClassInfo.OwnMembers.Where(info => info.Name == "ReferencePropertyName").FirstOrDefault();
            persistentMemberInfo.ShouldNotBeNull();
            if (persistentMemberInfo != null)
                ((IPersistentReferenceMemberInfo)persistentMemberInfo).ReferenceTypeFullName.ShouldEqual(typeof(User).FullName);
        };

    }
}