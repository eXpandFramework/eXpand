using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Web.UI;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Persistent.Base;
using DevExpress.Web;
using System.Linq;
using Fasterflect;

namespace Xpand.ExpressApp.Web.SystemModule {
    [ModelAbstractClass]
    public interface IModelListViewPreviewRowDetailView : IModelListView {
        [DataSourceProperty(ModelViewsDomainLogic.DataSourcePropertyPath)]
        [DataSourceCriteria(ModelObjectViewLogic.ModelViewsByClassCriteria)]
        [Category("eXpand")]
        IModelDetailView PreviewRowDetailView { get; set; }

        IModelListViewPreviewRowDetailViewControllers PreviewRowControllers { get; }
    }

    public enum ControllerActivationPolicy {
        Default,
        DoNotActivate,
        SelectivelyActivate
    }
    [ModelNodesGenerator(typeof(ModelListViewPreviewRowDetailViewControllersNodesGenerator))]
    [ModelReadOnly(typeof(ModelReadOnlyCalculator))]
    [ImageName("ModelEditor_Controllers")]
    public interface IModelListViewPreviewRowDetailViewControllers : IModelList<IModelPreviewRowController>,IModelNode {
        ControllerActivationPolicy ControllerActivationPolicy { get; set; }
    }

    [KeyProperty("Name")]
    public interface IModelPreviewRowController:IModelNode {
        [Required]
        string Name { get; set; }
        bool Activate { get; set; }
    }

    public interface IModelPreviewRowViewController:IModelPreviewRowController {
         
    }

    public interface IModelPreviewRowWindowController:IModelPreviewRowController {
         
    }
    public class ModelListViewPreviewRowDetailViewControllersNodesGenerator : ModelNodesGeneratorBase {
        protected override void GenerateNodesCore(ModelNode node) {
            foreach (Controller controller in ((IModelSources)node.Application).Controllers) {
                if (controller is ViewController) {
                    node.AddNode<IModelPreviewRowViewController>(controller.Name);
                } else if (controller is WindowController) {
                    node.AddNode<IModelPreviewRowWindowController>(controller.Name);
                } else {
                    node.AddNode<IModelPreviewRowController>(controller.Name);
                }
            }
        }
    }

    public class PreviewRowDetailViewController:ViewController<ListView>,IModelExtender {

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var modelListViewPreviewRowDetailView = ((IModelListViewPreviewRowDetailView) View.Model);
            if (modelListViewPreviewRowDetailView.PreviewRowDetailView!=null) {
                var gridView = ((ASPxGridListEditor) View.Editor).Grid;
                gridView.Templates.PreviewRow = new PreviewRowTemplate(Application,ObjectSpace,modelListViewPreviewRowDetailView);
                gridView.Settings.ShowPreview = true;
            }
        }

        class PreviewRowTemplate :  ITemplate {
            readonly XafApplication _xafApplication;
            readonly IObjectSpace _objectSpace;
            readonly IModelListViewPreviewRowDetailView _modelListViewPreviewRowDetailView;
            Window _window;
            readonly IEnumerable<Type> _controllerTypes;

            public PreviewRowTemplate(XafApplication xafApplication, IObjectSpace objectSpace, IModelListViewPreviewRowDetailView modelListViewPreviewRowDetailView) {
                _xafApplication = xafApplication;
                _objectSpace = objectSpace;
                _modelListViewPreviewRowDetailView = modelListViewPreviewRowDetailView;
                _controllerTypes = GetControllerTypes();
            }

            void ITemplate.InstantiateIn(Control container) {
                var templateContainer = (GridViewPreviewRowTemplateContainer) container;
                object obj = templateContainer.Grid.GetRow(templateContainer.VisibleIndex);
                var detailView = _xafApplication.CreateDetailView(_objectSpace, _modelListViewPreviewRowDetailView.PreviewRowDetailView, false, obj);
                if (_controllerTypes == null || _controllerTypes.Any()) {
                    Collection<Controller> controllers=null;
                    if (_controllerTypes!=null) {
                        controllers = new Collection<Controller>(_controllerTypes.Select(type => type.CreateInstance()).Cast<Controller>().ToList());
                    }
                    _window = _xafApplication.CreateWindow(TemplateContext.View, controllers, controllers==null, true);
                    _window.SetView(detailView);
                }
                detailView.CreateControls();
                templateContainer.Controls.Add((Control)detailView.Control);
            }

            IEnumerable<Type> GetControllerTypes() {
                var controllerActivationPolicy = _modelListViewPreviewRowDetailView.PreviewRowControllers.ControllerActivationPolicy;
                if (controllerActivationPolicy == ControllerActivationPolicy.Default)
                    return null;
                if (controllerActivationPolicy == ControllerActivationPolicy.DoNotActivate)
                    return Enumerable.Empty<Type>();
                var modelPreviewRowControllers = _modelListViewPreviewRowDetailView.PreviewRowControllers.Where(controller => controller.Activate);
                return modelPreviewRowControllers.Select(controller => Type.GetType(controller.Name));
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelListView, IModelListViewPreviewRowDetailView>();
            extenders.Add<IModelController, IModelControllerActivate>();
        }
    }

    public interface IModelControllerActivate {
        [Category("eXpand")]
        bool Activate { get; set; }
    }
}