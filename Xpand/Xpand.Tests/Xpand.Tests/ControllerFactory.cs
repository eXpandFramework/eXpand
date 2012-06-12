using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using TypeMock;
using TypeMock.ArrangeActAssert;

namespace Xpand.Tests {

    public class ControllerFactory<TController, TObject> where TController : ViewController, new() {
        private const string STR_ControllerFactory = "ControllerFactory";
        TController _controller;

        public TController Create(ViewType viewType) {
            XafTypesInfo.Instance.RegisterEntity(typeof(TObject));

            _controller = new TController();
            _controller.Active[STR_ControllerFactory] = false;

            var unitOfWork = new UnitOfWork(Session.DefaultSession.DataLayer);
            var XPObjectSpace = new XPObjectSpace(unitOfWork, XafTypesInfo.Instance);

            var xafApplication = Isolate.Fake.Instance<XafApplication>();
            Isolate.WhenCalled(() => _controller.Application).WillReturn(xafApplication);
            Isolate.WhenCalled(() => xafApplication.CreateObjectSpace()).WillReturn(XPObjectSpace);

            var currentObject = (TObject)XPObjectSpace.CreateObject(typeof(TObject));
            View view = viewType == ViewType.DetailView ? (View)createDetailView(XPObjectSpace, currentObject) : createListView(XPObjectSpace);
            view.CurrentObject = currentObject;
            _controller.SetView(view);

            var frame = new Frame(_controller.Application, TemplateContext.View);
            frame.SetView(_controller.View);
            Isolate.WhenCalled(() => _controller.Frame).WillReturn(frame);
            return _controller;
        }

        public void Activate() {
            _controller.Active[STR_ControllerFactory] = true;
        }
        ListView createListView(XPObjectSpace XPObjectSpace) {
            var source = new CollectionSource(XPObjectSpace, typeof(TObject));
            var listEditor = Isolate.Fake.Instance<ListEditor>();
            Isolate.WhenCalled(() => listEditor.RequiredProperties).WillReturn(new string[0]);
            return new ListView(source, listEditor);
        }

        DetailView createDetailView(XPObjectSpace XPObjectSpace, object currentObject) {

            return new DetailView(XPObjectSpace, currentObject, _controller.Application, true);
        }

        public TController CreateAndActivate() {
            return CreateAndActivate(ViewType.DetailView);
        }
        public TController CreateAndActivate(ViewType viewType) {
            Create(viewType);
            Activate();
            return _controller;
        }

        public TController Controller {
            get { return _controller; }
        }
        public TObject CurrentObject {
            get { return (TObject)_controller.View.CurrentObject; }
        }
        public XPObjectSpace XPObjectSpace {
            get { return (XPObjectSpace)_controller.View.ObjectSpace; }
        }


        public UnitOfWork UnitOfWork {
            get { return (UnitOfWork)XPObjectSpace.Session; }
        }

    }
    [Obsolete("Use ControllerFactory")]
    public class ViewControllerFactory {
        private EventHandler controlsCreatedHandler;
        private EventHandler currentObjectChangedHandler;

        public void Activate<T>(T controller, HandleInfo handleInfo) where T : ViewController {
            View view = controller.View;
            MockedEvent controlsCreated = null;
            MockedEvent currentObjectChanged = null;
            if (handleInfo != null) {
                if (handleInfo.ControlsCreated) {
                    using (RecorderManager.StartRecording()) {
                        view.ControlsCreated += null;
                    }
                    controlsCreated = RecorderManager.LastMockedEvent;
                }
                if (handleInfo.CurrentObjectChanged) {
                    using (RecorderManager.StartRecording()) {
                        view.CurrentObjectChanged += null;
                    }
                    currentObjectChanged = RecorderManager.LastMockedEvent;
                }

            }
            controller.Active.Clear();
            controller.Active[""] = true;
            if (controlsCreated != null) {
                controlsCreatedHandler = (EventHandler)controlsCreated.GetEventHandle();
            }
            if (currentObjectChanged != null)
                currentObjectChangedHandler = (EventHandler)currentObjectChanged.GetEventHandle();
        }

        public void Activate<T>(T controller) where T : ViewController {
            Activate(controller, null);
        }

        public EventHandler ControlsCreatedHandler {
            get { return controlsCreatedHandler; }
        }
        public EventHandler CurrentObjectChangedHandler {
            get { return currentObjectChangedHandler; }
        }

        private T createController<T>(ViewType viewType, PersistentBase currentObject, bool activate, HandleInfo handleInfo) where T : ViewController, new() {
            if (currentObject.Session.IsNewObject(currentObject))
                currentObject.Session.Save(currentObject);
            var XPObjectSpace = new XPObjectSpace(new UnitOfWork(currentObject.Session.DataLayer), XafTypesInfo.Instance);
            var persistentBase = XPObjectSpace.GetObject(currentObject);
            T controller = viewType == ViewType.ListView
                               ? createListViewController<T>(persistentBase, activate, XPObjectSpace, handleInfo)
                               : createDetailViewController<T>(XPObjectSpace, persistentBase, activate, handleInfo);
            var frame = new Frame(controller.Application, TemplateContext.View);
            frame.SetView(controller.View);
            Isolate.WhenCalled(() => controller.Frame).WillReturn(frame);
            if (activate)
                controller.View.CurrentObject = persistentBase;

            return controller;
        }

        public T CreateAndActivateController<T>(ViewType viewType, PersistentBase currentObject, HandleInfo handleInfo) where T : ViewController, new() {
            return createController<T>(viewType, currentObject, true, handleInfo);
        }

        public T CreateAndActivateController<T>(ViewType viewType, PersistentBase currentObject) where T : ViewController, new() {
            return createController<T>(viewType, currentObject, true, null);
        }

        private T createDetailViewController<T>(XPObjectSpace XPObjectSpace, PersistentBase currentObject, bool activate, HandleInfo handleInfo) where T : ViewController, new() {
            XafTypesInfo.Instance.RegisterEntity(currentObject.GetType().BaseType);
            XafTypesInfo.Instance.RegisterEntity(currentObject.GetType());
            XPObjectSpace.Session.UpdateSchema(currentObject.GetType().BaseType);
            XPObjectSpace.Session.UpdateSchema(currentObject.GetType());
            XPObjectSpace.Session.DataLayer.UpdateSchema(true,
                                                       XPObjectSpace.Session.GetClassInfo(currentObject.GetType().BaseType));
            XPObjectSpace.Session.DataLayer.UpdateSchema(true, XPObjectSpace.Session.GetClassInfo(currentObject.GetType()));
            var application = Isolate.Fake.Instance<XafApplication>();
            Isolate.WhenCalled(() => application.CreateObjectSpace()).WillReturn(XPObjectSpace);
            var detailView = new DetailView(XPObjectSpace, currentObject, application, true);
            var conntroller = new T();
            Isolate.WhenCalled(() => conntroller.Application).WillReturn(application);
            conntroller.Active[""] = false;
            conntroller.SetView(detailView);
            if (activate)
                Activate(conntroller, handleInfo);
            return conntroller;
        }


        private T createListViewController<T>(PersistentBase currentObject, bool activate, XPObjectSpace XPObjectSpace, HandleInfo handleInfo) where T : ViewController, new() {
            var controller = createController<T>(currentObject.GetType(), activate, XPObjectSpace, handleInfo);
            XafApplication application = controller.Application;
            Isolate.WhenCalled(() => application.CreateObjectSpace()).WillReturn(XPObjectSpace);
            return controller;
        }

        private T createController<T>(Type objectType, bool activate, XPObjectSpace XPObjectSpace, HandleInfo handleInfo) where T : ViewController {
            XafTypesInfo.Instance.RegisterEntity(objectType);
            var source = new CollectionSource(XPObjectSpace, objectType);
            var listEditor = Isolate.Fake.Instance<ListEditor>();
            Isolate.WhenCalled(() => listEditor.RequiredProperties).WillReturn(new string[0]);
            var listView = new ListView(source, listEditor);
            Isolate.WhenCalled(() => listView.ObjectTypeInfo).WillReturn(XafTypesInfo.CastTypeToTypeInfo(objectType));
            Isolate.WhenCalled(() => listView.ObjectSpace).WillReturn(XPObjectSpace);
            var controller = Isolate.Fake.Instance<T>(Members.CallOriginal, ConstructorWillBe.Called);
            Isolate.WhenCalled(() => controller.Application).WillReturn(Isolate.Fake.Instance<XafApplication>());

            controller.Active[""] = false;
            controller.SetView(listView);
            View view = controller.View;
            Isolate.WhenCalled(() => view.ObjectSpace).WillReturn(XPObjectSpace);
            if (activate)
                Activate(controller, handleInfo);

            return controller;
        }

        public T CreateController<T>(Type objectType) where T : ViewController {
            return createController<T>(objectType, false, new XPObjectSpace(new UnitOfWork(XpoDefault.DataLayer), XafTypesInfo.Instance), null);
        }

        public T CreateAndActivateController<T>(Type objectType) where T : ViewController, new() {
            return createController<T>(ViewType.Any, (PersistentBase)Activator.CreateInstance(objectType, Session.DefaultSession), true, null);
        }


        public T CreateController<T>(ViewType viewType, PersistentBase currentObject) where T : ViewController, new() {
            return createController<T>(viewType, currentObject, false, null);
        }

        public T CreateAndActivateController<T, T1>() where T : ViewController, new() {
            return CreateAndActivateController<T>(typeof(T1));
        }
    }

}