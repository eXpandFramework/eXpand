using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.BaseImpl;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using eXpand.Xpo;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;
using eXpand.ExpressApp.WorldCreator;
using System.Linq;
using System;

namespace eXpand.Tests.eXpand.WorldCreator {
    [Subject(typeof(InterfaceInfo))]
    [Isolated]
    public class When_linking_with_a_PersistentInterfaceInfo {
        static IPersistentClassInfo _persistentClassInfo;

        static LinkUnlinkController _linkUnlinkController;
        static Frame _frame;
        static PopupWindowShowActionExecuteEventArgs _popupWindowShowActionExecuteEventArgs;

        Establish context = () => {
            new TestAppLication<InterfaceInfo>().Setup().
                                            WithArtiFacts(() => new[] {
                                              typeof (WorldCreatorModule),
                                              typeof (SystemModule)
                                          }).CreateListView(false, null).CreateFrame(frame => _frame = frame);
            _persistentClassInfo = Isolate.Fake.Instance<PersistentClassInfo>();
            Isolate.WhenCalled(() => _persistentClassInfo.CreateMembersFromInterfaces()).IgnoreCall();

            _popupWindowShowActionExecuteEventArgs = Isolate.Fake.Instance<PopupWindowShowActionExecuteEventArgs>();
            Isolate.WhenCalled(() => _popupWindowShowActionExecuteEventArgs.PopupWindow.View.SelectedObjects.Count).WillReturn(1);

        };

        Because of = () => {
            PopupWindowShowAction linkAction = _frame.GetController<LinkUnlinkController>().LinkAction;
            Isolate.Invoke.Event(() => linkAction.Execute += null, null, _popupWindowShowActionExecuteEventArgs);
        };


        It should_create_all_missing_persistent_member_infos=() => Isolate.Verify.WasCalledWithAnyArguments(() => _persistentClassInfo.CreateMembersFromInterfaces());
    }

    [Isolated]
    public class When_creating_members_from_interfaceinfo_with_collection_members:with_classInfo_with_interfaceInfos<INotSupported>
    {
        static Exception _exception;

        Because of = () => {
            _exception = Catch.Exception(() => _persistentClassInfo.CreateMembersFromInterfaces());
        };

        It should_throw_exception=() => _exception.ShouldNotBeNull();
    }

    [Isolated]
    public class When_creating_members_from_interfaceinfo : with_classInfo_with_interfaceInfos<IDummyString> {
        Because of = () => _persistentClassInfo.CreateMembersFromInterfaces();

        It should_create_all_missing_core_type_persistent_member_infos = () =>
        {
            var persistentMemberInfo = _persistentClassInfo.OwnMembers.Where(info => info.Name == "StringPropertyName").FirstOrDefault();
            persistentMemberInfo.ShouldNotBeNull();
            ((IPersistentCoreTypeMemberInfo)persistentMemberInfo).DataType.ShouldEqual(XPODataType.String);
        };

        It should_create_all_missing_reference_member_infos = () =>
        {
            var persistentMemberInfo = _persistentClassInfo.OwnMembers.Where(info => info.Name == "ReferencePropertyName").FirstOrDefault();
            persistentMemberInfo.ShouldNotBeNull();
            ((IPersistentReferenceMemberInfo)persistentMemberInfo).ReferenceTypeFullName.ShouldEqual(typeof(User).FullName);
        };

    }
}