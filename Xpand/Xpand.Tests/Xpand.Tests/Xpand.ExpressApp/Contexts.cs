using System;
using System.Collections.Generic;
using System.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;
using Xpand.ExpressApp;
using Xpand.ExpressApp.Attributes;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Win;
using Xpand.Persistent.Base.General;
using PessimisticLockingViewController = Xpand.ExpressApp.Win.SystemModule.PessimisticLockingViewController;
using ViewEditModeController = Xpand.ExpressApp.Win.SystemModule.ViewEditModeController;

namespace Xpand.Tests.Xpand.ExpressApp {
    public abstract class With_Application : With_Types_info {
        protected static TestPessimisticLockingInfo Info;

        protected static TestApplication Application;

        Establish context = () => {
            Application = new TestApplication();
            Info = Application.TestSetup();
        };

    }

    public class TestApplication : XpandWinApplication {
        readonly XPObjectSpaceProvider _XPObjectSpaceProvider = new XPObjectSpaceProvider(new MemoryDataStoreProvider(DataSet));
        static DataSet _dataSet;

        static DataSet DataSet {
            get { return _dataSet ?? (_dataSet = new DataSet()); }
        }

        public TestApplication() {
            DatabaseVersionMismatch += OnVersionMismatch;
            Modules.Add(new TestModule());
            SetupComplete += OnSetupComplete;
            ConfirmationRequired += (sender, args) => args.Cancel = true;
        }

        protected override void OnCreateCustomObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProvider = _XPObjectSpaceProvider;
        }

        public TestPessimisticLockingInfo TestSetup(PessimisticLockObject pessimisticLockObject) {
            var info = new TestPessimisticLockingInfo { PessimisticLockObject = pessimisticLockObject };
            return TestSetupCore(info);
        }

        private static PessimisticLockObject GetPessimisticLockObject(TestPessimisticLockingInfo info) {
            return info.PessimisticLockObject == null ? info.XPObjectSpace.CreateObject<PessimisticLockObject>() : info.XPObjectSpace.GetObjectByKey<PessimisticLockObject>(info.PessimisticLockObject.Oid);
        }
        private TestPessimisticLockingInfo TestSetupCore(TestPessimisticLockingInfo info) {
            var systemModule = new XpandSystemModule();
            // ReSharper disable ConvertClosureToMethodGroup
            Isolate.WhenCalled(() => systemModule.InitializeSequenceGenerator()).IgnoreCall();
            // ReSharper restore ConvertClosureToMethodGroup
            Modules.Add(systemModule);
            Setup("TestApplication", _XPObjectSpaceProvider);
            info.XPObjectSpace = CreateObjectSpace();
            var pessimisticViewController = new PessimisticLockingViewController();
            info.ViewEditModeController = new ViewEditModeController();
            info.PessimisticLockObject = GetPessimisticLockObject(info);
            info.SecondXPObjectSpace = CreateObjectSpace();
            var user = info.SecondXPObjectSpace.CreateObject<User>();
            info.SecondXPObjectSpace.CommitChanges();
            Isolate.WhenCalled(() => SecuritySystem.CurrentUser).WillReturn(user);
            Isolate.WhenCalled(() => SecuritySystem.UserType).WillReturn(typeof(User));
            info.XPObjectSpace.CommitChanges();
            info.DetailView = CreateDetailView(info.XPObjectSpace, info.PessimisticLockObject);
            info.Window = CreateWindow(TemplateContext.View, new List<Controller> { pessimisticViewController, info.ViewEditModeController }, true);
            info.Window.SetView(info.DetailView);
            return info;
        }
        public TestPessimisticLockingInfo TestSetup() {
            return TestSetupCore(new TestPessimisticLockingInfo());
        }

        void OnSetupComplete(object sender, EventArgs eventArgs) {
            EnsureShowViewStrategy();
        }


        void OnVersionMismatch(object sender, DatabaseVersionMismatchEventArgs args) {
            args.Updater.Update();
            args.Handled = true;
        }
    }

    public class TestPessimisticLockingInfo {
        public ViewEditModeController ViewEditModeController { get; set; }

        public Window Window { get; set; }

        public DetailView DetailView { get; set; }

        public IObjectSpace XPObjectSpace { get; set; }

        public IObjectSpace SecondXPObjectSpace { get; set; }

        public PessimisticLockObject PessimisticLockObject { get; set; }
    }

    class TestModule : XpandModuleBase {
    }
    public abstract class With_Types_info {
        Establish context = () => {
            ReflectionHelper.Reset();
            XafTypesInfo.Reset();
            XafTypesInfo.HardReset();
            if (XafTypesInfo.PersistentEntityStore != null)
                ((XpoTypeInfoSource)XafTypesInfo.PersistentEntityStore).Reset();
            else {
                XafTypesInfo.SetPersistentEntityStore(new XpandXpoTypeInfoSource((TypesInfo)XafTypesInfo.Instance));
            }
            foreach (var type in typeof(User).Assembly.GetTypes()) {
                XafTypesInfo.Instance.RegisterEntity(type);
            }

        };
    }

    //    public abstract class With_Application : With_Types_info {
    //        //        protected static DataSet DataSet;
    //        protected static IObjectSpace XPObjectSpace;
    //        protected static Window Window;
    //        protected static ViewEditModeController ViewEditModeController;
    //        public static XafApplication Application;
    //        protected static DetailView DetailView;
    //        protected static PessimisticLockObject PessimisticLockObject;
    //
    //        Establish context = () => {
    //            var XPObjectSpaceProvider = new XPObjectSpaceProvider(new MemoryDataStoreProvider());
    //            Application = new TestApplication();
    //            var testModule = new XpandSystemModule();
    //            Application.Modules.Add(testModule);
    //
    //            Application.Setup("TestApplication", XPObjectSpaceProvider);
    //            //            XPObjectSpace = XPObjectSpaceProvider.CreateObjectSpace();
    //            //            controller = new PostponeController();
    //
    //            //            DataSet = new DataSet();
    //            var pessimisticViewController = new PessimisticLockingViewController();
    //            //            // ReSharper disable ConvertClosureToMethodGroup
    //            //            Isolate.WhenCalled(delegate { XpandModuleBase.DisposeManagers(); }).IgnoreCall();
    //            //            // ReSharper restore ConvertClosureToMethodGroup
    //            ViewEditModeController = new ViewEditModeController();
    //            //            Application = Isolate.Fake.XafApplicationInstance(() => new List<Type> { typeof(PessimisticLockObject) }, DataSet, new Controller[] { pessimisticViewController, ViewEditModeController });
    //            XPObjectSpace = Application.CreateObjectSpace();
    //            PessimisticLockObject = XPObjectSpace.CreateObject<PessimisticLockObject>();
    //            var secondXPObjectSpace = Application.CreateObjectSpace();
    //            var user = secondXPObjectSpace.CreateObject<User>();
    //            secondXPObjectSpace.CommitChanges();
    //            Isolate.WhenCalled(() => SecuritySystem.CurrentUser).WillReturn(user);
    //            Isolate.WhenCalled(() => SecuritySystem.UserType).WillReturn(typeof(User));
    //            XPObjectSpace.CommitChanges();
    //            DetailView = Application.CreateDetailView(XPObjectSpace, PessimisticLockObject);
    //            //
    //            Window = Application.CreateWindow(TemplateContext.View, new List<Controller> { pessimisticViewController, ViewEditModeController }, true);
    //            Window.SetView(DetailView);
    //        };
    //    }
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

}