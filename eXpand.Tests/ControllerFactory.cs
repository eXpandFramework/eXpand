using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests{
    public class ViewControllerFactory{
        public void Activate<T>(T controller)where T:ViewController{    
            controller.Active.Clear();
            controller.Active[""] = true;
        }


        private T createController<T>(ViewType viewType, PersistentBase currentObject,bool activate) where T : ViewController, new(){
            if (currentObject.Session.IsNewObject(currentObject))
                currentObject.Session.Save(currentObject);
            var objectSpace = new ObjectSpace(new UnitOfWork(XpoDefault.DataLayer), XafTypesInfo.Instance);
            var persistentBase = objectSpace.GetObject(currentObject);
            T controller = viewType == ViewType.ListView ? createListViewController<T>(persistentBase,activate,objectSpace) : createDetailViewController<T>(objectSpace, persistentBase, activate);
            if (activate)
                controller.View.CurrentObject = persistentBase;
            Isolate.WhenCalled(() => controller.Application).WillReturn(Isolate.Fake.Instance<XafApplication>());
            return controller;
        }

        public T CreateAndActivateController<T>(ViewType viewType, PersistentBase currentObject) where T : ViewController, new(){
            return createController<T>(viewType, currentObject, true);
        }

        private T createDetailViewController<T>(ObjectSpace objectSpace, PersistentBase currentObject,bool activate) where T : ViewController, new()
        {
            XafTypesInfo.Instance.RegisterEntity(currentObject.GetType());
            var detailView = new DetailView(objectSpace, currentObject, Isolate.Fake.Instance<XafApplication>(), true);
            var conntroller = new T();
            conntroller.Active[""] = false; 
            conntroller.SetView(detailView);
            if (activate)
                Activate(conntroller);
            return conntroller;
        }

        private T createListViewController<T>(PersistentBase currentObject, bool activate, ObjectSpace objectSpace) where T :ViewController, new(){
            var controller = createController<T>(currentObject.GetType(),activate,objectSpace);
            return controller;
        }

        private T createController<T>(Type objectType,bool activate, ObjectSpace objectSpace) where T : ViewController, new(){
            XafTypesInfo.Instance.RegisterEntity(objectType);
            var source = new CollectionSource(objectSpace, typeof(ModelDifferenceObject));
            var listEditor = Isolate.Fake.Instance<ListEditor>();
            Isolate.WhenCalled(() => listEditor.RequiredProperties).WillReturn(new string[0]);
            var listView = new ListView(source, listEditor);
            var controller = new T();
            controller.SetView(listView);
            if (activate)
                Activate(controller);
            Isolate.WhenCalled(() => controller.Application).WillReturn(Isolate.Fake.Instance<XafApplication>());
            return controller;
        }

        public T CreateAndActivateController<T>(Type objectType) where T : ViewController, new(){

            return createController<T>(objectType, true,new ObjectSpace(new UnitOfWork(XpoDefault.DataLayer),XafTypesInfo.Instance));

        }

        public T CreateController<T>(ViewType viewType, PersistentBase currentObject) where T : ViewController, new(){
            return createController<T>(viewType, currentObject, false);
        }
    }

}