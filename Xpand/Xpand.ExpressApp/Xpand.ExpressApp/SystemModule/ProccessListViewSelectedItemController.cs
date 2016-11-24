using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.SystemModule {
    [ModelAbstractClass]
    public interface IModelListViewProcessSelectedItem : IModelListView {
        IModelProccessListViewSelectItem ProccessListViewSelectItem { get; }
    }

    public interface IModelProccessListViewSelectItem : IModelNode {
        TemplateContext? TemplateContext { get; set; }
        bool? AllowAllControllers { get; set; }
        NewWindowTarget? NewWindowTarget { get; set; }
        TargetWindow? TargetWindow { get; set; }
        bool? Handled { get; set; }
        [DefaultValue(true)]
        bool HandledOnActionNonExecution { get; set; }
        [DataSourceProperty("Application.ActionDesign.Actions")]
        IModelAction Action { get; set; }
        [DataSourceProperty("DetailViews")]
        IModelDetailView DetailView { get; set; }
        [Browsable(false)]
        IModelList<IModelDetailView> DetailViews { get; }
    }

    [DomainLogic(typeof(IModelProccessListViewSelectItem))]
    public class MasterDetailRuleDomainLogic {
        public static IModelList<IModelDetailView> Get_DetailViews(IModelProccessListViewSelectItem masterDetailRule) {
            var modelDetailViews = masterDetailRule.Application.Views.OfType<IModelDetailView>();
            var views = modelDetailViews.Where(view => ModelClassMatch(masterDetailRule, view));
            return new CalculatedModelNodeList<IModelDetailView>(views);
        }

        static bool ModelClassMatch(IModelProccessListViewSelectItem masterDetailRule, IModelDetailView view) {
            return view.ModelClass == ((IModelListView)masterDetailRule.Parent).ModelClass;
        }
    }

    public class ProccessListViewSelectedItemController : ViewController<ListView>, IModelExtender {
        protected override void OnActivated() {
            base.OnActivated();
            var listViewProcessCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
            var processCurrentObjectAction = listViewProcessCurrentObjectController.ProcessCurrentObjectAction;
            var proccessListViewSelectItem = ((IModelListViewProcessSelectedItem)View.Model).ProccessListViewSelectItem;
            var modelProccessListViewSelectItem = proccessListViewSelectItem.Handled;
            if (modelProccessListViewSelectItem.HasValue && (proccessListViewSelectItem.DetailView == null && proccessListViewSelectItem.Action == null))
                processCurrentObjectAction.Active[typeof(IModelProccessListViewSelectItem).Name] = !modelProccessListViewSelectItem.Value;
            listViewProcessCurrentObjectController.CustomProcessSelectedItem += OnCustomProcessSelectedItem;
        }


        protected override void OnDeactivated() {
            base.OnDeactivated();
            Frame.GetController<ListViewProcessCurrentObjectController>().CustomProcessSelectedItem -= OnCustomProcessSelectedItem;
        }
        void OnCustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e){
            if (e.Handled) return;
            var model = ((IModelListViewProcessSelectedItem)View.Model).ProccessListViewSelectItem;
            if (model.AllowAllControllers.HasValue)
                e.InnerArgs.ShowViewParameters.CreateAllControllers = model.AllowAllControllers.Value;
            if (model.NewWindowTarget.HasValue)
                e.InnerArgs.ShowViewParameters.NewWindowTarget = model.NewWindowTarget.Value;
            if (model.TargetWindow.HasValue)
                e.InnerArgs.ShowViewParameters.TargetWindow = model.TargetWindow.Value;
            if (model.Handled.HasValue)
                e.Handled = model.Handled.Value;
            if (model.Action != null) {
                var allActions = Frame.Controllers.Cast<Controller>().SelectMany(controller => controller.Actions);
                var actionBase = allActions.SingleOrDefault(action => action.Id == model.Action.Id);
                if (actionBase != null){
                    var doExecute = actionBase.DoExecute();
                    if (!doExecute){
                        e.Handled = model.HandledOnActionNonExecution;
                    }
                }
            }
            if (model.DetailView != null && View != null) {
                var objectSpace = Application.CreateObjectSpace(model.DetailView.ModelClass.TypeInfo.Type);
                object currentObject = objectSpace.GetObject(View.CurrentObject);
                e.InnerArgs.ShowViewParameters.CreatedView = Application.CreateDetailView(objectSpace, model.DetailView.Id, true, currentObject);
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelListView, IModelListViewProcessSelectedItem>();
        }
    }
}
