using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.CombiningDictionaries{
    internal class ControllerFactory{
        public T CreateController<T>(Type objectType) where T:ViewController, new(){
            var controller = new T();
            ListView listView = GetListView(objectType);

            controller.SetView(listView);
            return controller;
        }

        private ListView GetListView(Type objectType){
            XafTypesInfo.Instance.RegisterEntity(objectType);
            var collectionSource = new CollectionSource(new ObjectSpace(new UnitOfWork(XpoDefault.DataLayer),XafTypesInfo.Instance), objectType);
            var listEditor = Isolate.Fake.Instance<ListEditor>();
            Isolate.WhenCalled(() => listEditor.RequiredProperties).WillReturn(new string[0]);
            var listView = new ListView(collectionSource,listEditor);
            
            return listView;
        }

        public T CreateController<T>(PersistentBase persistentBase) where T :ViewController, new(){
            var controller = new T();
            var listView = GetListView(persistentBase.GetType());
            if (persistentBase.Session.IsNewObject(persistentBase))
                persistentBase.Session.Save(persistentBase);
            listView.CurrentObject = listView.ObjectSpace.GetObject(persistentBase);
            controller.SetView(listView);
            return controller;
        }
    }
}