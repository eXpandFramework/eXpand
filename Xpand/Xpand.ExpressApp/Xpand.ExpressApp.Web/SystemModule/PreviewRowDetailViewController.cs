using System.ComponentModel;
using System.Web.UI;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Persistent.Base;
using DevExpress.Web.ASPxGridView;

namespace Xpand.ExpressApp.Web.SystemModule {
    [ModelAbstractClass]
    public interface IModelListViewPreviewRowDetailView : IModelListView {
        [DataSourceProperty(ModelViewsDomainLogic.DataSourcePropertyPath)]
        [DataSourceCriteria(ModelObjectViewDomainLogic.ModelViewsByClassCriteria)]
        [Category("eXpand")]
        IModelDetailView PreviewRowDetailView { get; set; }
    }

    public class PreviewRowDetailViewController:ViewController<ListView>,IModelExtender {

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var previewRowDetailView = ((IModelListViewPreviewRowDetailView) View.Model).PreviewRowDetailView;
            if (previewRowDetailView!=null) {
                var gridView = ((ASPxGridListEditor) View.Editor).Grid;
                gridView.Templates.PreviewRow = new PreviewRowTemplate(Application,ObjectSpace,previewRowDetailView.Id);
                gridView.Settings.ShowPreview = true;
            }
        }

        class PreviewRowTemplate :  ITemplate {
            readonly XafApplication _xafApplication;
            readonly IObjectSpace _objectSpace;
            readonly string _viewId;

            public PreviewRowTemplate(XafApplication xafApplication, IObjectSpace objectSpace, string viewId) {
                _xafApplication = xafApplication;
                _objectSpace = objectSpace;
                _viewId = viewId;
            }

            void ITemplate.InstantiateIn(Control container) {
                var templateContainer = (GridViewPreviewRowTemplateContainer) container;
                object obj = templateContainer.Grid.GetRow(templateContainer.VisibleIndex);
                var detailView = ViewFactory.CreateDetailView(_xafApplication, _viewId, _objectSpace, obj, false);
                detailView.CreateControls();
                templateContainer.Controls.Add((Control) detailView.Control);
            }

        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelListView, IModelListViewPreviewRowDetailView>();
        }
    }
}