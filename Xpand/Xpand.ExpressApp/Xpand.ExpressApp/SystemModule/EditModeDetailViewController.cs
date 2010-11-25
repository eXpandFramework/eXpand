using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.SystemModule {
    public interface IModelClassViewModeDetailView : IModelNode {
        [Category("eXpand")]
        [Description("This view is going to be used when ViewMode==Edit")]
        [DataSourceCriteria("ModelClass Is Not Null And ModelClass.Name = '@This.Name'")]
        [DataSourceProperty("Application.Views")]
        [ModelValueCalculator("ModelClass", typeof(IModelClass), "DefaultDetailView")]
        IModelDetailView ViewModeDetailView { get; set; }
    }

    public interface IModelDetailViewViewModeDetailView : IModelNode {
        [Category("eXpand")]
        [Description("This view is going to be used when ViewMode==Edit")]
        [DataSourceCriteria("ModelClass Is Not Null And ModelClass.Name = '@This.ModelClass.Name'")]
        [DataSourceProperty("Application.Views")]
        [ModelValueCalculator("ModelClass", typeof(IModelClassViewModeDetailView), "ViewModeDetailView")]
        IModelDetailView ViewModeDetailView { get; set; }
    }
    [DomainLogic(typeof(IModelClassViewModeDetailView))]
    public class ModelClassViewModeDetailViewDomainLogic {
        public static IModelDetailView Get_ViewModeDetailView(IModelClassViewModeDetailView modelClassViewModeDetailView) {
            if (modelClassViewModeDetailView is IModelClass)
                return ((IModelClass)modelClassViewModeDetailView).DefaultDetailView;
            return null;
        }
    }
    public class EditModeDetailViewController : ViewController<XpandDetailView> {
        protected override void OnViewControllersActivated() {
            base.OnViewControllersActivated();
            if (View.ViewEditMode == ViewEditMode.View && IsDefaultModel()) {
                View.SetInfo(((IModelDetailViewViewModeDetailView)View.Model).ViewModeDetailView);
                Frame.SetView(View);
            }
        }

        bool IsDefaultModel() {
            ListView listView = Frame.GetController<LinkToListViewController>().Link.ListView;
            if (listView != null)
                return listView.DetailViewId == View.Model.Id;
            return false;
        }
    }
}
