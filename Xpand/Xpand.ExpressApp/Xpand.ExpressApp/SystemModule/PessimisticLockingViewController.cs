using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.Attributes;

namespace Xpand.ExpressApp.SystemModule {
    public abstract class PessimisticLockingViewController : ViewController<DetailView> {
        PessimisticLocker _pessimisticLocker;
        readonly SimpleAction _unlockObjectShipAction;
        public const string PessimisticLocking = "PessimisticLocking";
        public const string LockedUser = "LockedUser";

        protected PessimisticLockingViewController() {
            _unlockObjectShipAction = new SimpleAction(this, "UnLockObject", PredefinedCategory.RecordEdit){Caption = "UnLock"};
            _unlockObjectShipAction.Execute+=SimpleActionOnExecute;
            _unlockObjectShipAction.SelectionDependencyType=SelectionDependencyType.RequireSingleObject;
            _unlockObjectShipAction.Active[PessimisticLocking] = false;
        }

        public SimpleAction UnlockObjectShipAction {
            get { return _unlockObjectShipAction; }
        }

        void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            _pessimisticLocker.UnLock(true);
            View.AllowEdit[PessimisticLocking] = true;
            _unlockObjectShipAction.Active[PessimisticLocking] = false;
        }

        protected override void OnActivated() {
            base.OnActivated();
            if (View.ObjectTypeInfo.FindAttribute<PessimisticLockingAttribute>() != null) {
                _pessimisticLocker = new PessimisticLocker(((ObjectSpace)ObjectSpace).Session.DataLayer, View.CurrentObject);
                UpdateViewAllowEditState();   
                SubscribeToEvents();
            }
        }

        void ViewOnAllowEditChanged(object sender, EventArgs eventArgs) {
            _unlockObjectShipAction.Active[PessimisticLocking] = !View.AllowEdit[PessimisticLocking];
        }

        protected void UpdateViewAllowEditState() {
            var lockedUser = View.ObjectTypeInfo.FindMember(LockedUser).GetValue(View.CurrentObject);
            View.AllowEdit[PessimisticLocking] = (ReferenceEquals(lockedUser, ObjectSpace.GetObject(SecuritySystem.CurrentUser))||lockedUser==null);
            _unlockObjectShipAction.Active[PessimisticLocking] = !View.AllowEdit[PessimisticLocking];
        }

        protected virtual void SubscribeToEvents() {
            View.AllowEditChanged += ViewOnAllowEditChanged;
            ObjectSpace.ObjectChanged += ObjectSpaceOnObjectChanged;
            ObjectSpace.RollingBack += ObjectSpaceOnRollingBack;
            ObjectSpace.Committed += ObjectSpaceOnCommitted;
            View.QueryCanChangeCurrentObject += ViewOnQueryCanChangeCurrentObject;
            View.Closing += ViewOnClosing;
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            if (Application != null && Application.Security != null) {
                CreatePessimisticLockingField(typesInfo);
            }
        }

        void CreatePessimisticLockingField(ITypesInfo typesInfo) {
            var typeInfos = typesInfo.PersistentTypes.Where(info => info.FindAttribute<PessimisticLockingAttribute>() != null);
            foreach (var typeInfo in typeInfos) {
                typeInfo.AddAttribute(new OptimisticLockingAttribute(false));
                var memberInfo = typeInfo.FindMember(LockedUser);
                if (memberInfo == null) {
                    memberInfo = typeInfo.CreateMember(LockedUser, Application.Security.UserType);
                    memberInfo.AddAttribute(new BrowsableAttribute(false));
                }
            }
        }

        void ObjectSpaceOnCommitted(object sender, EventArgs eventArgs) {
            _pessimisticLocker.UnLock();
        }

        void ViewOnClosing(object sender, EventArgs eventArgs) {
            _pessimisticLocker.UnLock();
        }

        void ViewOnQueryCanChangeCurrentObject(object sender, CancelEventArgs cancelEventArgs) {
            _pessimisticLocker.UnLock();
        }


        void ObjectSpaceOnRollingBack(object sender, CancelEventArgs cancelEventArgs) {
            _pessimisticLocker.UnLock();
        }

        void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs objectChangedEventArgs) {
            if (objectChangedEventArgs.PropertyName != LockedUser)
                if (_pessimisticLocker.IsLocked)
                    View.AllowEdit[PessimisticLocking] = false;
                else
                    _pessimisticLocker.Lock();

        }
    }
    public class PessimisticLocker {
        readonly object _currentObject;
        readonly Session _session;
        bool? _isLocked;

        public bool IsLocked {
            get {
                if (!_isLocked.HasValue) {
                    if (_currentObject != null) {
                        var keyValue = _session.GetKeyValue(_currentObject);
                        var objectByKey = _session.GetObjectByKey(_currentObject.GetType(), keyValue, true);
                        _isLocked = new bool?(LockingMemberInfo.GetValue(objectByKey) != null);
                    }
                    else {
                        _isLocked = false;
                    }
                }
                return _isLocked.Value;
            }
        }

        public PessimisticLocker(IDataLayer dataLayer, object currentObject) {
            _session = new Session(dataLayer) { LockingOption = LockingOption.None };
            _currentObject = GetCurrentObject(currentObject);
        }

        object GetCurrentObject(object currentObject) {
            var keyValue = _session.GetKeyValue(currentObject);
            return _session.GetObjectByKey(currentObject.GetType(), keyValue, true);
        }

        public void UnLock(bool force) {
            if (force) {
                UnLockCore();
            } else if (_currentObject!=null&&LockingMemberInfo.GetValue(_currentObject) == GetCurrentUser())
                UnLockCore();
        }

        void UnLockCore() {
            _session.Reload(_currentObject);
            LockingMemberInfo.SetValue(_currentObject, null);
            _session.Save(_currentObject);
        }

        public void UnLock() {
            UnLock(false);
        }

        public void Lock() {
            if (_currentObject!=null&&LockingMemberInfo.GetValue(_currentObject) == null) {
                object user = GetCurrentUser();
                LockingMemberInfo.SetValue(_currentObject, user);
                _session.Save(_currentObject);
            }
        }

        object GetCurrentUser() {
            var keyValue = _session.GetKeyValue(SecuritySystem.CurrentUser);
            return _session.GetObjectByKey(SecuritySystem.UserType, keyValue);
        }

        XPMemberInfo LockingMemberInfo {
            get {
                return _session.GetClassInfo(_currentObject).GetMember(PessimisticLockingViewController.LockedUser);
            }
        }
    }

}
