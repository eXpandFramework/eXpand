using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;
using Xpand.ExpressApp.SystemModule;
using PessimisticLockingViewController = Xpand.ExpressApp.Win.SystemModule.PessimisticLockingViewController;
using ViewEditModeController = Xpand.ExpressApp.Win.SystemModule.ViewEditModeController;
using Xpand.Persistent.Base.General;

namespace Xpand.Tests.Xpand.ExpressApp {
    [Subject(typeof(PessimisticLockingViewController))]
    public class When_Object_Change : With_Application {

        Because of = () => {
            Info.PessimisticLockObject.PropertyName = "dfgfd";
            Info.XPObjectSpace.ReloadObject(Info.PessimisticLockObject);
        };

        It should_lock_the_object = () => Info.PessimisticLockObject.LockedUser.ShouldNotBeNull();
    }
    [Subject(typeof(PessimisticLockingViewController))]
    public class When_XPObjectSpace_rollback : With_Application {
        Establish context = () => { Info.PessimisticLockObject.PropertyName = "sdfdf"; };
        Because of = () => {
            Info.XPObjectSpace.RollBackSilent();
            Info.XPObjectSpace.ReloadObject(Info.PessimisticLockObject);
        };
        It should_unlock_object = () => Info.PessimisticLockObject.LockedUser.ShouldBeNull();
    }

    [Subject(typeof(PessimisticLockingViewController))]
    public class When_ospace_commited : With_Application {
        Establish context = () => { Info.PessimisticLockObject.PropertyName = "dffdf"; };
        Because of = () => {
            Info.XPObjectSpace.CommitChanges();
            Info.XPObjectSpace.ReloadObject(Info.PessimisticLockObject);
        };
        It should_unlock_object = () => Info.PessimisticLockObject.LockedUser.ShouldBeNull();
    }
    [Subject(typeof(PessimisticLockingViewController))]
    public class When_View_CurrentObject_Changing : With_Application {
        Establish context = () => { Info.PessimisticLockObject.PropertyName = "dffdf"; };
        Because of = () => { Info.DetailView.CurrentObject = Info.XPObjectSpace.CreateObject<PessimisticLockObject>(); };
        It should_unlock_object = () => Info.PessimisticLockObject.LockedUser.ShouldBeNull();
    }
    [Subject(typeof(PessimisticLockingViewController))]
    public class When_View_Is_closing : With_Application {
        Establish context = () => { Info.PessimisticLockObject.PropertyName = "dffdf"; };
        Because of = () => Info.DetailView.Close();
        It should_unlock_object = () => Info.PessimisticLockObject.LockedUser.ShouldBeNull();
    }
    [Subject(typeof(PessimisticLocker))]
    public class When_object_is_about_to_be_unlocked {
        static IObjectSpace _XPObjectSpace;
        static PessimisticLockObject _pessimisticLockObject;
        static PessimisticLocker _pessimisticLocker;

        Establish context = () => {
            _XPObjectSpace = ObjectSpaceInMemory.CreateNew();
            _pessimisticLockObject = _XPObjectSpace.CreateObject<PessimisticLockObject>();
            Isolate.WhenCalled(() => SecuritySystem.CurrentUser).WillReturn(_XPObjectSpace.CreateObject<User>());
            Isolate.WhenCalled(() => SecuritySystem.UserType).WillReturn(typeof(User));
            _XPObjectSpace.CommitChanges();
            _pessimisticLocker = new PessimisticLocker(((XPObjectSpace)_XPObjectSpace).Session.DataLayer, _pessimisticLockObject);
            _pessimisticLocker.Lock();

            var user = _XPObjectSpace.CreateObject<User>();
            _XPObjectSpace.CommitChanges();
            Isolate.WhenCalled(() => SecuritySystem.CurrentUser).WillReturn(user);
            _XPObjectSpace.ReloadObject(_pessimisticLockObject);
        };

        Because of = () => _pessimisticLocker.UnLock();

        It should_not_unlock_if_current_user_does_not_match_locked_user = () => _pessimisticLockObject.LockedUser.ShouldNotBeNull();
    }
    [Subject(typeof(PessimisticLockingViewController))]
    public class When_a_locked_object_is_open_by_a_second_user : With_Application {
        static Window _window;
        static DetailView _detailView;
        static User _user;

        Establish context = () => {
            Info.PessimisticLockObject.PropertyName = "fdgfdg";
            var XPObjectSpace = Application.CreateObjectSpace();
            _user = XPObjectSpace.CreateObject<User>();
            _user.UserName = "2";
            XPObjectSpace.CommitChanges();
            Isolate.WhenCalled(() => SecuritySystem.CurrentUser).WillReturn(_user);
            _user = (User)SecuritySystem.CurrentUser;
            var pessimisticLockObject = XPObjectSpace.GetObjectByKey<PessimisticLockObject>(Info.PessimisticLockObject.Oid);
            _detailView = Application.CreateDetailView(XPObjectSpace, pessimisticLockObject);
            _window = Application.CreateWindow(TemplateContext.View, new List<Controller> { new PessimisticLockingViewController(), new ViewEditModeController() }, true);
        };

        Because of = () => _window.SetView(_detailView);

        It should_not_allowedit_on_view = () => _detailView.AllowEdit.ResultValue.ShouldBeFalse();
    }
    [Subject(typeof(PessimisticLockingViewController))]
    public class When_2_users_open_the_same_object_and_both_try_to_change_it : With_Application {
        static DetailView _detailView;
        static PessimisticLockObject _pessimisticLockObject;
        static User _user;

        Establish context = () => {
            Info.PessimisticLockObject.PropertyName = "fdgfdg";
            var XPObjectSpace = Application.CreateObjectSpace();
            _user = XPObjectSpace.CreateObject<User>();
            _user.UserName = "2";
            XPObjectSpace.CommitChanges();
            Isolate.WhenCalled(() => SecuritySystem.CurrentUser).WillReturn(_user);
            _pessimisticLockObject = XPObjectSpace.GetObjectByKey<PessimisticLockObject>(Info.PessimisticLockObject.Oid);
            var testApplication = new TestApplication();
            var info = testApplication.TestSetup(_pessimisticLockObject);
            _detailView = info.DetailView;
        };

        Because of = () => {

            _pessimisticLockObject.PropertyName = "ddd";
        };

        It should_mark_as_readonly_last_user_view = () => _detailView.AllowEdit.ResultValue.ShouldBeFalse();
    }
    [Subject(typeof(PessimisticLockingViewController))]
    public class When_editing_a_locked_detailview : With_Application {
        static SimpleAction _editAction;

        Establish context = () => {
            Info.DetailView.AllowEdit[global::Xpand.ExpressApp.SystemModule.PessimisticLockingViewController.PessimisticLocking] = false;
            _editAction = Info.Window.GetController<ViewEditModeController>().EditAction;
            _editAction.Active["ViewEditMode"] = true;
        };

        Because of = () => _editAction.DoExecute();
        It should_allow_edit_for_the_pessimistic_locking_context = () => Info.DetailView.AllowEdit[global::Xpand.ExpressApp.SystemModule.PessimisticLockingViewController.PessimisticLocking].ShouldBeTrue();
    }

    public abstract class With_new_object {
        protected static XPObjectSpace XPObjectSpace;
        protected static PessimisticLockObject PessimisticLockObject;
        protected static PessimisticLocker PessimisticLocker;

        Establish context = () => {
            XPObjectSpace = (XPObjectSpace)ObjectSpaceInMemory.CreateNew();
            PessimisticLockObject = XPObjectSpace.CreateObject<PessimisticLockObject>();
            PessimisticLocker = new PessimisticLocker(XPObjectSpace.Session.DataLayer, PessimisticLockObject);
        };
    }
    [Subject(typeof(PessimisticLocker))]
    public class When_unlocking_new_object : With_new_object {
        Establish context = () => {
            PessimisticLockObject.LockedUser = XPObjectSpace.CreateObject<User>();
        };
        Because of = () => PessimisticLocker.UnLock();

        It should_do_nothing = () => PessimisticLockObject.LockedUser.ShouldNotBeNull();
    }
    [Subject(typeof(PessimisticLocker))]
    public class When_locking_new_object : With_new_object {
        Because of = () => PessimisticLocker.Lock();

        It should_do_nothing = () => PessimisticLockObject.LockedUser.ShouldBeNull();
    }
    [Subject(typeof(PessimisticLocker))]
    public class When_new_object_locking_state_is_queried : With_new_object {
        static bool _isLocked;

        Because of = () => {
            _isLocked = PessimisticLocker.IsLocked;
        };

        It should_return_unlocked = () => _isLocked.ShouldBeFalse();
    }
}
