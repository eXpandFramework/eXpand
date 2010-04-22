using System.Diagnostics;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.SystemModule {
    public class AutoCreatableObjectController : WindowController {
        ShowNavigationItemController _showNavigationItemController;
        private void UnsubscribeFromEvents(){
            _showNavigationItemController.CustomShowNavigationItem -= OnCustomShowNavigationItem;
        }
        private void SubscribeToEvents(){
            _showNavigationItemController.CustomShowNavigationItem += OnCustomShowNavigationItem;
        }

        protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			_showNavigationItemController = Frame.GetController<ShowNavigationItemController>();
			if(_showNavigationItemController != null) {
				UnsubscribeFromEvents();
				SubscribeToEvents();
			}
		}
		protected override void Dispose(bool disposing) {
			if(_showNavigationItemController != null) {
				UnsubscribeFromEvents();
				_showNavigationItemController = null;
			}
			base.Dispose(disposing);
		}
        public AutoCreatableObjectController()
        {
			TargetWindowType = WindowType.Main;
		}

        void OnCustomShowNavigationItem(object sender, CustomShowNavigationItemEventArgs customShowNavigationItemEventArgs) {
            if (customShowNavigationItemEventArgs.ActionArguments.SelectedChoiceActionItem.Data is DetailView)
                Debug.Print("");
        }
        

    }
}