using System.Collections;
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
    public interface IDummyString {
        string StringPropertyName { get; set; }
        User ReferencePropertyName {get;set;}
    }
    [Subject(typeof(InterfaceInfo))]
    [Isolated]
    public class When_linking_with_a_PersistentInterfaceInfo:With_In_Memory_DataStore {
        static IPersistentClassInfo _persistentClassInfo;

        static LinkUnlinkController _linkUnlinkController;
        static PopupWindowShowActionExecuteEventArgs _popupWindowShowActionExecuteEventArgs;

        Establish context = () => {
            var viewControllerFactory = new ControllerFactory<CreateMissingInterfaceMembersController, InterfaceInfo>();
            var createMissingInterfaceMembersController = viewControllerFactory.Create(ViewType.Any);
            
            _persistentClassInfo = Isolate.Fake.Instance<IPersistentClassInfo>();
            Isolate.WhenCalled(() => _persistentClassInfo.CreateMembersFromInterfaces(null)).IgnoreCall();
            _linkUnlinkController = Isolate.Fake.Instance<LinkUnlinkController>(Members.CallOriginal,ConstructorWillBe.Ignored);
            var windowShowAction = Isolate.Fake.Instance<PopupWindowShowAction>();
            Isolate.WhenCalled(() => _linkUnlinkController.LinkAction).WillReturn(windowShowAction);
            var popupWindowShowAction = _linkUnlinkController.LinkAction;
            Isolate.WhenCalled(() => popupWindowShowAction.Execute+= null).CallOriginal();
            Isolate.WhenCalled(() => createMissingInterfaceMembersController.Frame.GetController<LinkUnlinkController>()).WillReturn(_linkUnlinkController);
            viewControllerFactory.Activate();
            _popupWindowShowActionExecuteEventArgs = Isolate.Fake.Instance<PopupWindowShowActionExecuteEventArgs>();
            Isolate.WhenCalled(() => _popupWindowShowActionExecuteEventArgs.PopupWindow.View.SelectedObjects.Count).WillReturn(1);
            Isolate.WhenCalled(() => createMissingInterfaceMembersController.Application.Modules.FindModule(typeof(WorldCreatorModule))).WillReturn(new WorldCreatorModule());
        };

        Because of = () => {
            var popupWindowShowAction = _linkUnlinkController.LinkAction;
            Isolate.Invoke.Event(() => popupWindowShowAction.Execute += null, null, _popupWindowShowActionExecuteEventArgs);
        };

        It should_create_all_missing_persistent_member_infos=() => Isolate.Verify.WasCalledWithAnyArguments(() => _persistentClassInfo.CreateMembersFromInterfaces(null));
    }

    public interface INotSupported {
        IList PropertyName {get;set;}
    }
    [Isolated]
    public class When_creating_members_from_interfaceinfo_with_collection_members:with_classInfo_with_interfaceInfos<INotSupported>
    {
        static Exception _exception;

        Because of = () => {
            _exception = Catch.Exception(() => _persistentClassInfo.CreateMembersFromInterfaces(_typesInfo));
        };

        It should_throw_exception=() => _exception.ShouldNotBeNull();
    }

    [Isolated]
    public class When_creating_members_from_interfaceinfo : with_classInfo_with_interfaceInfos<IDummyString> {
        Because of = () => _persistentClassInfo.CreateMembersFromInterfaces(_typesInfo);

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