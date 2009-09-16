using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.Xpo;
using TypeMock;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests{
    public class ViewControllerFactory{
        private EventHandler controlsCreatedHandler;

        public void Activate<T>(T controller, bool getControlsCreatedHandle) where T : ViewController
        {
            View view = controller.View;
            if (getControlsCreatedHandle){
                using (RecorderManager.StartRecording()){
                    view.ControlsCreated += null;
                }
            }
            controller.Active.Clear();
            controller.Active[""] = true;
            if (getControlsCreatedHandle){
                controlsCreatedHandler = (EventHandler)RecorderManager.LastMockedEvent.GetEventHandle();
            }
        }

        public void Activate<T>(T controller)where T:ViewController{
            Activate(controller, false);
        }

        public EventHandler ControlsCreatedHandler{
            get { return controlsCreatedHandler; }
        }

        private T createController<T>(ViewType viewType, PersistentBase currentObject, bool activate, bool getControlsCreatedHandle) where T : ViewController, new()
        {
            if (currentObject.Session.IsNewObject(currentObject))
                currentObject.Session.Save(currentObject);
            var objectSpace = new ObjectSpace(new UnitOfWork(XpoDefault.DataLayer), XafTypesInfo.Instance);
            var persistentBase = objectSpace.GetObject(currentObject);
            T controller = viewType == ViewType.ListView
                               ? createListViewController<T>(persistentBase, activate, objectSpace,
                                                             getControlsCreatedHandle)
                               : createDetailViewController<T>(objectSpace, persistentBase, activate, getControlsCreatedHandle);
            if (activate)
                controller.View.CurrentObject = persistentBase;
            Isolate.WhenCalled(() => controller.Application).WillReturn(Isolate.Fake.Instance<XafApplication>());
            return controller;
        }

        public T CreateAndActivateController<T>(ViewType viewType, PersistentBase currentObject, bool getControlsCreatedHandle) where T : ViewController, new()
        {
            return createController<T>(viewType, currentObject, true, getControlsCreatedHandle);
        }

        public T CreateAndActivateController<T>(ViewType viewType, PersistentBase currentObject) where T : ViewController, new(){
            return createController<T>(viewType, currentObject, true, false);
        }

        private T createDetailViewController<T>(ObjectSpace objectSpace, PersistentBase currentObject, bool activate, bool handle) where T : ViewController, new()
        {
            XafTypesInfo.Instance.RegisterEntity(currentObject.GetType());
            var detailView = new DetailView(objectSpace, currentObject, Isolate.Fake.Instance<XafApplication>(), true);
            var conntroller = new T();
            conntroller.Active[""] = false; 
            conntroller.SetView(detailView);
            if (activate)
                Activate(conntroller,handle);
            return conntroller;
        }

        private T createListViewController<T>(PersistentBase currentObject, bool activate, ObjectSpace objectSpace, bool getControlsCreatedHandle) where T : ViewController, new()
        {
            var controller = createController<T>(currentObject.GetType(),activate,objectSpace,getControlsCreatedHandle);
            return controller;
        }

        private T createController<T>(Type objectType, bool activate, ObjectSpace objectSpace, bool getControlsCreatedHandle) where T : ViewController, new()
        {
            XafTypesInfo.Instance.RegisterEntity(objectType);
            var source = new CollectionSource(objectSpace, objectType);
            var listEditor = Isolate.Fake.Instance<ListEditor>();
            Isolate.WhenCalled(() => listEditor.RequiredProperties).WillReturn(new string[0]);
            var listView = new ListView(source, listEditor);
            var controller = new T();
            controller.Active[""] = false; 
            controller.SetView(listView);
            if (activate)
                Activate(controller,getControlsCreatedHandle);
            Isolate.WhenCalled(() => controller.Application).WillReturn(Isolate.Fake.Instance<XafApplication>());
            return controller;
        }

        public T CreateController<T>(Type objectType) where T : ViewController, new(){
            return createController<T>(objectType, false, new ObjectSpace(new UnitOfWork(XpoDefault.DataLayer), XafTypesInfo.Instance), false);
        }

        public T CreateAndActivateController<T>(Type objectType) where T : ViewController, new(){

            return createController<T>(objectType, true,new ObjectSpace(new UnitOfWork(XpoDefault.DataLayer),XafTypesInfo.Instance),false);

        }

        public T CreateController<T>(ViewType viewType, PersistentBase currentObject) where T : ViewController, new(){
            return createController<T>(viewType, currentObject, false, false);
        }

    }

}