using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.Xpo;
using TypeMock;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests{
    public class ViewControllerFactory{
        private EventHandler controlsCreatedHandler;
        private EventHandler currentObjectChangedHandler;

        public void Activate<T>(T controller, HandleInfo handleInfo) where T : ViewController
        {
            View view = controller.View;
            MockedEvent controlsCreated = null;
            MockedEvent currentObjectChanged = null;
            if (handleInfo != null){
                if (handleInfo.ControlsCreated){
                    using (RecorderManager.StartRecording()){
                        view.ControlsCreated += null;
                    }
                    controlsCreated = RecorderManager.LastMockedEvent;
                }
                if (handleInfo.CurrentObjectChanged){
                    using (RecorderManager.StartRecording()){
                        view.CurrentObjectChanged += null;
                    }
                    currentObjectChanged = RecorderManager.LastMockedEvent;
                }
                
            }
            controller.Active.Clear();
            controller.Active[""] = true;
            if (controlsCreated!= null){
                controlsCreatedHandler = (EventHandler) controlsCreated.GetEventHandle();
            }
            if (currentObjectChanged != null)
                currentObjectChangedHandler = (EventHandler)currentObjectChanged.GetEventHandle();
        }

        public void Activate<T>(T controller)where T:ViewController{
            Activate(controller, null);
        }

        public EventHandler ControlsCreatedHandler{
            get { return controlsCreatedHandler; }
        }
        public EventHandler CurrentObjectChangedHandler{
            get { return currentObjectChangedHandler; }
        }

        private T createController<T>(ViewType viewType, PersistentBase currentObject, bool activate, HandleInfo handleInfo) where T : ViewController, new()
        {
            if (currentObject.Session.IsNewObject(currentObject))
                currentObject.Session.Save(currentObject);
            var objectSpace = new ObjectSpace(new UnitOfWork(XpoDefault.DataLayer), XafTypesInfo.Instance);
            var persistentBase = objectSpace.GetObject(currentObject);
            T controller = viewType == ViewType.ListView
                               ? createListViewController<T>(persistentBase, activate, objectSpace,handleInfo)
                               : createDetailViewController<T>(objectSpace, persistentBase, activate, handleInfo);

            if (activate)
                controller.View.CurrentObject = persistentBase;
            
            return controller;
        }

        public T CreateAndActivateController<T>(ViewType viewType, PersistentBase currentObject, HandleInfo handleInfo) where T : ViewController, new()
        {
            return createController<T>(viewType, currentObject, true, handleInfo);
        }

        public T CreateAndActivateController<T>(ViewType viewType, PersistentBase currentObject) where T : ViewController, new(){
            return createController<T>(viewType, currentObject, true, null);
        }

        private T createDetailViewController<T>(ObjectSpace objectSpace, PersistentBase currentObject, bool activate, HandleInfo handleInfo) where T : ViewController, new()
        {
            XafTypesInfo.Instance.RegisterEntity(currentObject.GetType());
            var application = Isolate.Fake.Instance<XafApplication>();
            Isolate.WhenCalled(() => application.CreateObjectSpace()).WillReturn(objectSpace);
            var detailView = new DetailView(objectSpace, currentObject, application, true);
            var conntroller = new T();
            Isolate.WhenCalled(() => conntroller.Application).WillReturn(application);
            conntroller.Active[""] = false; 
            conntroller.SetView(detailView);
            if (activate)
                Activate(conntroller,handleInfo);
            return conntroller;
        }


        private T createListViewController<T>(PersistentBase currentObject, bool activate, ObjectSpace objectSpace, HandleInfo handleInfo) where T : ViewController, new()
        {
            var controller = createController<T>(currentObject.GetType(),activate,objectSpace,handleInfo);
            XafApplication application = controller.Application;
            Isolate.WhenCalled(() => application.CreateObjectSpace()).WillReturn(objectSpace);
            return controller;
        }

        private T createController<T>(Type objectType, bool activate, ObjectSpace objectSpace, HandleInfo handleInfo) where T : ViewController, new()
        {
            XafTypesInfo.Instance.RegisterEntity(objectType);
            var source = new CollectionSource(objectSpace, objectType);
            var listEditor = Isolate.Fake.Instance<ListEditor>();
            Isolate.WhenCalled(() => listEditor.RequiredProperties).WillReturn(new string[0]);
            var listView = new ListView(source, listEditor);
            Isolate.WhenCalled(() => listView.ObjectTypeInfo).WillReturn(XafTypesInfo.CastTypeToTypeInfo(objectType));
            Isolate.WhenCalled(() => listView.ObjectSpace).WillReturn(objectSpace);
            var controller = new T();
            Isolate.WhenCalled(() => controller.Application).WillReturn(Isolate.Fake.Instance<XafApplication>());
            
            controller.Active[""] = false; 
            controller.SetView(listView);
            View view = controller.View;
            Isolate.WhenCalled(() => view.ObjectSpace).WillReturn(objectSpace);
            if (activate)
                Activate(controller,handleInfo);
            
            return controller;
        }

        public T CreateController<T>(Type objectType) where T : ViewController, new(){
            return createController<T>(objectType, false, new ObjectSpace(new UnitOfWork(XpoDefault.DataLayer), XafTypesInfo.Instance),null);
        }

        public T CreateAndActivateController<T>(Type objectType) where T : ViewController, new(){

            return createController<T>(objectType, true,new ObjectSpace(new UnitOfWork(XpoDefault.DataLayer),XafTypesInfo.Instance),null);

        }

        public T CreateController<T>(ViewType viewType, PersistentBase currentObject) where T : ViewController, new(){
            return createController<T>(viewType, currentObject, false, null);
        }

    }

}