using System.Collections.Generic;
using System.Data;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;
using Xpand.ExpressApp.Attributes;
using Xpand.ExpressApp.SystemModule;
using PessimisticLockingViewController = Xpand.ExpressApp.Win.SystemModule.PessimisticLockingViewController;
using ViewEditModeController = Xpand.ExpressApp.Win.SystemModule.ViewEditModeController;

namespace Xpand.Tests.Xpand.ExpressApp {
    [PessimisticLocking]
    public class PessimisticLockObject : BaseObject {
        User _lockedUser;
        string _propertyName;

        public PessimisticLockObject(Session session)
            : base(session) {
        }

        public string PropertyName {
            get { return _propertyName; }
            set { SetPropertyValue("PropertyName", ref _propertyName, value); }
        }

        public User LockedUser {
            get {
                return _lockedUser;
            }
            set {
                SetPropertyValue("LockedUser", ref _lockedUser, value);
            }
        }
    }


    public abstract class With_Application {
        protected static DataSet DataSet;
        protected static IObjectSpace ObjectSpace;
        protected static Window Window;
        protected static ViewEditModeController ViewEditModeController;
        public static XafApplication Application;
        protected static DetailView DetailView;
        protected static PessimisticLockObject PessimisticLockObject;

        Establish context = () => {
            DataSet = new DataSet();
            var pessimisticViewController = new PessimisticLockingViewController();

            ViewEditModeController = new ViewEditModeController();
            Application = Isolate.Fake.XafApplicationInstance(typeof(PessimisticLockObject), DataSet, new Controller[] { pessimisticViewController ,ViewEditModeController});
            ObjectSpace = Application.CreateObjectSpace();
            PessimisticLockObject = ObjectSpace.CreateObject<PessimisticLockObject>();
            var secondObjectSpace = ObjectSpaceInMemory.CreateNew(DataSet);
            var user = secondObjectSpace.CreateObject<User>();
            secondObjectSpace.CommitChanges();
            Isolate.WhenCalled(() => SecuritySystem.CurrentUser).WillReturn(user);
            Isolate.WhenCalled(() => SecuritySystem.UserType).WillReturn(typeof(User));
            ObjectSpace.CommitChanges();
            DetailView = Application.CreateDetailView(ObjectSpace, PessimisticLockObject);

            Window = Application.CreateWindow(TemplateContext.View, new List<Controller> { pessimisticViewController }, true);
            Window.SetView(DetailView);
        };
    }

    public class When_Object_Change : With_Application {
        Because of = () => {
            PessimisticLockObject.PropertyName = "dfgfd";
            ObjectSpace.ReloadObject(PessimisticLockObject);
        };
        It should_lock_the_object = () =>PessimisticLockObject.LockedUser.ShouldNotBeNull();
    }

    public class When_objectspace_rollback : With_Application {
        Establish context = () => { PessimisticLockObject.PropertyName = "sdfdf"; };
        Because of = () => {
            ObjectSpace.Rollback();
            ObjectSpace.ReloadObject(PessimisticLockObject);
        };
        It should_unlock_object = () => PessimisticLockObject.LockedUser.ShouldBeNull();
    }

    public class When_ospace_commited : With_Application {
        Establish context = () => { PessimisticLockObject.PropertyName = "dffdf"; };
        Because of = () => {
            ObjectSpace.CommitChanges();
            ObjectSpace.ReloadObject(PessimisticLockObject);
        };
        It should_unlock_object = () => PessimisticLockObject.LockedUser.ShouldBeNull();
    }

    public class When_View_CurrentObject_Changing : With_Application {
        Establish context = () => { PessimisticLockObject.PropertyName = "dffdf"; };
        Because of = () => { DetailView.CurrentObject = ObjectSpace.CreateObject<PessimisticLockObject>(); };
        It should_unlock_object = () => PessimisticLockObject.LockedUser.ShouldBeNull();
    }
    public class When_View_Is_closing : With_Application {
        Establish context = () => { PessimisticLockObject.PropertyName = "dffdf"; };
        Because of = () => DetailView.Close();
        It should_unlock_object = () => PessimisticLockObject.LockedUser.ShouldBeNull();
    }

    public class When_object_is_about_to_be_unlocked {
        static IObjectSpace _objectSpace;
        static PessimisticLockObject _pessimisticLockObject;
        static PessimisticLocker _pessimisticLocker;

        Establish context = () => {
            _objectSpace = ObjectSpaceInMemory.CreateNew();
            _pessimisticLockObject = _objectSpace.CreateObject<PessimisticLockObject>();
            Isolate.WhenCalled(() => SecuritySystem.CurrentUser).WillReturn(_objectSpace.CreateObject<User>());
            Isolate.WhenCalled(() => SecuritySystem.UserType).WillReturn(typeof(User));
            _objectSpace.CommitChanges();
            _pessimisticLocker = new PessimisticLocker(((ObjectSpace) _objectSpace).Session.DataLayer,_pessimisticLockObject);
            _pessimisticLocker.Lock();

            var user = _objectSpace.CreateObject<User>();
            _objectSpace.CommitChanges();
            Isolate.WhenCalled(() => SecuritySystem.CurrentUser).WillReturn(user);
            _objectSpace.ReloadObject(_pessimisticLockObject);
        };

        Because of = () => _pessimisticLocker.Unlock();

        It should_not_unlock_if_current_user_does_not_match_locked_user = () => _pessimisticLockObject.LockedUser.ShouldNotBeNull();
    }
    public class When_a_locked_object_is_open_by_a_second_user : With_Application {
        static Window _window;
        static DetailView _detailView;
        static User _user;

        Establish context = () => {
            PessimisticLockObject.PropertyName = "fdgfdg";
            var objectSpace = ObjectSpaceInMemory.CreateNew(DataSet);
            _user = objectSpace.CreateObject<User>();
            _user.UserName = "2";
            objectSpace.CommitChanges();
            Isolate.WhenCalled(() => SecuritySystem.CurrentUser).WillReturn(_user);
            _user = (User) SecuritySystem.CurrentUser;
            var pessimisticLockObject = objectSpace.GetObjectByKey<PessimisticLockObject>(PessimisticLockObject.Oid);
            _detailView = Application.CreateDetailView(objectSpace, pessimisticLockObject);
            _window = Application.CreateWindow(TemplateContext.View, new List<Controller> { new PessimisticLockingViewController() }, true);
        };

        Because of = () => _window.SetView(_detailView);

        It should_not_allowedit_on_view = () => _detailView.AllowEdit.ResultValue.ShouldBeFalse();
    }

    public class When_2_users_open_the_same_object_and_both_try_to_change_it : With_Application {
        static Window _window;
        static DetailView _detailView;
        static PessimisticLockObject _pessimisticLockObject;
        static User _user;

        Establish context = () => {
            var objectSpace = ObjectSpaceInMemory.CreateNew(DataSet);
            _user = objectSpace.CreateObject<User>();
            _user.UserName = "2";
            objectSpace.CommitChanges();
            Isolate.WhenCalled(() => SecuritySystem.CurrentUser).WillReturn(_user);
            _pessimisticLockObject = objectSpace.GetObjectByKey<PessimisticLockObject>(PessimisticLockObject.Oid);
            _detailView = Application.CreateDetailView(objectSpace, _pessimisticLockObject);
            _window = Application.CreateWindow(TemplateContext.View, new List<Controller> { new PessimisticLockingViewController() }, true);
            _window.SetView(_detailView);
        };

        Because of = () => {
            PessimisticLockObject.PropertyName = "fdgfdg";
            _pessimisticLockObject.PropertyName = "ddd";
        };

        It should_mark_as_readonly_last_user_view = () => _detailView.AllowEdit.ResultValue.ShouldBeFalse();
    }

    public class When_editing_a_locked_detailview:With_Application {
        Establish context = () => {
            DetailView.AllowEdit[global::Xpand.ExpressApp.SystemModule.PessimisticLockingViewController.PessimisticLocking] = false;
        };
        Because of = () => Window.GetController<ViewEditModeController>().EditAction.DoExecute();
        It should_allow_edit_for_the_pessimistic_locking_context = () => DetailView.AllowEdit[global::Xpand.ExpressApp.SystemModule.PessimisticLockingViewController.PessimisticLocking].ShouldBeTrue();
    }
}
