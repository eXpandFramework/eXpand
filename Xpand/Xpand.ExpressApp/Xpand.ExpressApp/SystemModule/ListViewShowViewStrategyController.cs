using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using Fasterflect;

namespace Xpand.ExpressApp.SystemModule {
    public enum OpenViewWhenNestedStrategy {
        Default,
        InMainWindow
    }

    [ModelAbstractClass]
    public interface IModelListViewShowViewStrategy {
        [Category("eXpand")]
        [Description("Works only with XpandShowViewStragey or derive from XpandWebApplication")]
        OpenViewWhenNestedStrategy OpenViewWhenNestedStrategy { get; set; }

        [Category("eXpand")]
        bool OpenDetailViewAsPopup { get; set; }
    }

    public class ListViewShowViewStrategyController:ViewController<ListView>,IModelExtender {
        List<ActionBase> _actionBases;
        IModelListViewShowViewStrategy _listViewShowViewStrategy;

        protected override void OnActivated() {
            base.OnActivated();
            _listViewShowViewStrategy = View.Model as IModelListViewShowViewStrategy;
            if (_listViewShowViewStrategy != null &&(_listViewShowViewStrategy.OpenViewWhenNestedStrategy == OpenViewWhenNestedStrategy.InMainWindow ||
                 _listViewShowViewStrategy.OpenDetailViewAsPopup)) {
                _actionBases =Frame.Controllers.Cast<Controller>().SelectMany(controller => controller.Actions).ToList();
                foreach (var action in _actionBases) {
                    action.Executed += ActionOnExecuted;
                }
            }
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (_actionBases != null)
                foreach (var action in _actionBases) {
                    action.Executed -= ActionOnExecuted;
                }
        }

        void ActionOnExecuted(object sender, ActionBaseEventArgs actionBaseEventArgs) {
            var showViewParameters = actionBaseEventArgs.ShowViewParameters;
            var createdView = showViewParameters.CreatedView;
            if (createdView!=null) {
                if (_listViewShowViewStrategy.OpenViewWhenNestedStrategy == OpenViewWhenNestedStrategy.InMainWindow &&
                    !View.IsRoot) {
                    showViewParameters.CreatedView = null;
                    Application.CallMethod("ShowViewInMainWindow", createdView, Frame);
                }
                else if (_listViewShowViewStrategy.OpenDetailViewAsPopup) {
                    showViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                }
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelListView, IModelListViewShowViewStrategy>();
        }
    }
}
