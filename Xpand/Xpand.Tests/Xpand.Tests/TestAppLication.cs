using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.SystemModule;
using TypeMock.ArrangeActAssert;
using System.Linq;

namespace eXpand.Tests.eXpand.WorldCreator
{
    public class TestAppLication<TObject> : IApplicationHandler<TObject>, IArtifactHandler<TObject>, IViewCreationHandler, IFrameCreationHandler, IControlsCreatedHandler where TObject:BaseObject
    {
        ObjectSpace _objectSpace;
        ApplicationNodeWrapper _applicationNodeWrapper;
        XafApplication _xafApplication;
        View _view;
        Frame _frame;
        TObject _currentObject;
        List<Controller> _collectedControllers;
        

        public IArtifactHandler<TObject> Setup(Action<XafApplication> created,Action<TObject> action)
        {
            _applicationNodeWrapper = new ApplicationNodeWrapper(new Dictionary(new DictionaryNode(ApplicationNodeWrapper.NodeName), Schema.GetCommonSchema()));
            _applicationNodeWrapper.Load(typeof(TObject));

            var objectSpaceProvider = new ObjectSpaceProvider(new MemoryDataStoreProvider());
            _objectSpace = objectSpaceProvider.CreateObjectSpace();

            _currentObject = (TObject) _objectSpace.CreateObject(typeof (TObject));
            if (action != null) action.Invoke(_currentObject);

            _xafApplication = Isolate.Fake.Instance<XafApplication>();
//            CollectionSourceBase _collectionSource = null;
//            var collectionSourceBase = Isolate.Fake.Instance<CollectionSourceBase>();
//            Isolate.WhenCalled((String listViewId, CollectionSourceBase collectionSource, bool isRoot) => _xafApplication.CreateListView("", collectionSourceBase,
//                                                                    false)).AndArgumentsMatch((s, @base, arg3) => {
//                                                                        _collectionSource = @base;
//                                                                        return true;
//                                                                    }).WillReturn(new ListView(_collectionSource, _xafApplication, false));
            if (created != null) created.Invoke(_xafApplication);
            return this;
        }


        public IArtifactHandler<TObject> Setup()
        {
            return Setup(null, null);
        }



        TObject IArtifactHandler<TObject>.CurrentObject {
            get { return _currentObject; }
        }

        UnitOfWork IArtifactHandler<TObject>.UnitOfWork {
            get { return (UnitOfWork) _objectSpace.Session; }
        }

        ObjectSpace IArtifactHandler<TObject>.ObjectSpace {
            get { return _objectSpace; }
        }

        IViewCreationHandler IArtifactHandler<TObject>.WithArtiFacts(Func<Type[]> func){
            XafTypesInfo.Instance.RegisterEntity(typeof(TObject));
            XafTypesInfo.Instance.LoadTypes(typeof(SystemModule).Assembly);
            XafTypesInfo.Instance.LoadTypes(typeof(ValidationModule).Assembly);
//            XafTypesInfo.Instance.LoadTypes(typeof(SystemWindowsFormsModule).Assembly);
            XafTypesInfo.Instance.LoadTypes(typeof(eXpandSystemModule).Assembly);
            if (func != null)
                foreach (var type in func.Invoke()){
                    XafTypesInfo.Instance.LoadTypes(type.Assembly);
                }
            return this;

        }



        IViewCreationHandler IArtifactHandler<TObject>.WithArtiFacts()
        {
            return ((IArtifactHandler<TObject>)this).WithArtiFacts(null);
        }
        IFrameCreationHandler IViewCreationHandler.CreateListView() {
            return ((IViewCreationHandler)this).CreateListView(true, null);
        }

        IFrameCreationHandler IViewCreationHandler.CreateListView(bool isRoot, Action<ListView> created) {
            ListViewInfoNodeWrapper listViewInfoNodeWrapper = _applicationNodeWrapper.Views.GetListViews(typeof(TObject))[0];
            var listEditor = Isolate.Fake.Instance<ListEditor>();
            Isolate.WhenCalled(() => listEditor.RequiredProperties).WillReturn(new string[0]);
            Isolate.WhenCalled(() => listEditor.Model).WillReturn(listViewInfoNodeWrapper);
            _view = new ListView(new CollectionSource(_objectSpace, typeof (TObject)), listEditor, isRoot, _xafApplication);
            
            Isolate.WhenCalled(() => _view.SynchronizeWithInfo()).IgnoreCall();
            _view.SetInfo(listViewInfoNodeWrapper.Node);
            if (created != null) created.Invoke((ListView) _view);
            return this;
        }

        IFrameCreationHandler IViewCreationHandler.CreateDetailView() {

            foreach (var info in _currentObject.ClassInfo.CollectionProperties.OfType<XPMemberInfo>()){
                _applicationNodeWrapper.Load(info.CollectionElementType.ClassType);
                ((IViewCreationHandler)this).CreateListView(false, null);
            }
            _view = new DetailView(_objectSpace, _currentObject, _xafApplication, true);
            Isolate.WhenCalled(() => _view.SynchronizeWithInfo()).IgnoreCall();
            _view.SetInfo(_applicationNodeWrapper.Views.GetDetailViews(typeof(TObject))[0].Node);
            return this;
        }

        IControlsCreatedHandler IFrameCreationHandler.CreateFrame(Action<Frame> created) {
            var controllersManager = new ControllersManager();
            controllersManager.CollectControllers(info => true);
            _collectedControllers = controllersManager.CreateControllers(typeof (Controller));
            _frame = new Frame(_xafApplication, TemplateContext.View,
                               _collectedControllers.Where(
                                   controller => controller.GetType() != typeof (ScriptRecorderControllerBase)).ToList());
            if (_view == null)
                ((IViewCreationHandler) this).CreateDetailView();
            _frame.SetView(_view);
            if (created != null) created.Invoke(_frame);
            return this;
        }

        IControlsCreatedHandler IFrameCreationHandler.CreateFrame() {
            return ((IFrameCreationHandler)this).CreateFrame(null);
        }

//        void IControllerActivateHandler.ActivateControllers(Action<Controller> action) {
//            IEnumerable<Controller> controllers = _frame.Controllers;
//            foreach (ViewController controller in controllers.Where(controller => typeof(ViewController).IsAssignableFrom(controller.GetType()))) {
//                ViewController controller1 = controller;
//                Isolate.WhenCalled(() => controller1.Frame).WillReturn(_frame);
//
//                controller.Application = _xafApplication;
//                if (!controller.Active) {
//                    controller.SetView(_view);
//                    if (action != null) action.Invoke(controller);
//                }
//            }
//        }

        //void IControllerActivateHandler.ActivateControllers() {
        //    ((IControllerActivateHandler) this).ActivateControllers(null);
        //}
        void IControlsCreatedHandler.RaiseControlsCreated() {
            foreach (var controller in _collectedControllers.OfType<ViewController>()) {
                var view = controller.View;
                Isolate.Invoke.Event(() => view.ControlsCreated+= null, null,EventArgs.Empty);
            }
        }
    }

    internal interface IApplicationHandler<TObject>
    {
        IArtifactHandler<TObject> Setup(Action<XafApplication> action1,Action<TObject> action);
        IArtifactHandler<TObject> Setup();
    }

    public interface IArtifactHandler<TObject>
    {
        TObject CurrentObject { get; }
        UnitOfWork UnitOfWork { get; }
        ObjectSpace ObjectSpace{ get; }
        IViewCreationHandler WithArtiFacts(Func<Type[]> func);
        IViewCreationHandler WithArtiFacts();
    }

    public interface IFrameCreationHandler {
        IControlsCreatedHandler CreateFrame(Action<Frame> created);
        IControlsCreatedHandler CreateFrame();
    }

    public interface IControlsCreatedHandler {
        void RaiseControlsCreated();
    }
    public interface IViewCreationHandler {
        IFrameCreationHandler CreateListView();
        IFrameCreationHandler CreateListView(bool isRoot, Action<ListView> created);
        IFrameCreationHandler CreateDetailView();
        
    }


}
