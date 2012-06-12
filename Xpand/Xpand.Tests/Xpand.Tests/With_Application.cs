using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;

namespace Xpand.Tests {
    public abstract class With_Application<T, TObject> where T : With_Application<T, TObject> {
        protected static DetailView DetailView;
        protected static Window Window;
        protected static IObjectSpace XPObjectSpace;
        protected static TObject Object;

        protected static XafApplication Application;
        static T _instance;


        Establish Context = () => {
            _instance = Activator.CreateInstance<T>();
            Instance.Initialize();
            Isolate.Fake.XafApplicationInstance(Instance.ApplicationCreated, () => Instance.GetDomaincomponentTypes(), Instance.ViewCreated, Instance.WindowCreated,
                                                                             Instance.GetControllers().ToArray());
        };

        protected virtual IList<Type> GetDomaincomponentTypes() {
            return new List<Type> { typeof(TObject) };
        }

        protected virtual void ApplicationCreated(XafApplication application) {
            Application = application;
        }

        protected virtual void WindowCreated(Window window) {
            Window = window;
        }
        protected virtual void Initialize() {
        }

        protected virtual void ViewCreated(DetailView detailView) {
            XPObjectSpace = detailView.ObjectSpace;
            Object = (TObject)detailView.CurrentObject;
            DetailView = detailView;
        }
        protected virtual List<Controller> GetControllers() {
            return new List<Controller>();
        }

        public static T Instance {
            get { return _instance; }
        }


    }
}
