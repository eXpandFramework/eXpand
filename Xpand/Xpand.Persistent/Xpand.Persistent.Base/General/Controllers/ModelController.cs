using DevExpress.ExpressApp;

namespace Xpand.Persistent.Base.General.Controllers{
    public class ModelController:ViewController{
        private bool? _handleModelSaving;

        protected override void OnActivated() {
            base.OnActivated();
            if (_handleModelSaving.HasValue) {
                ((IModelViewModelSaving)View.Model).HandleModelSaving = _handleModelSaving.Value;
                _handleModelSaving = null;
            }
        }

        public void SetView(){
            var showViewParameters = new ShowViewParameters();
            Application.ShowViewStrategy.ShowView(showViewParameters, new ShowViewSource(null, null));
        }

        public void SetView(ShowViewParameters showViewParameters) {
            _handleModelSaving = ((IModelViewModelSaving)View.Model).HandleModelSaving;
            ((IModelViewModelSaving)View.Model).HandleModelSaving = true;
            showViewParameters.CreatedView = Application.CreateView(View.Model);
            showViewParameters.CreatedView.CurrentObject = !ObjectSpace.IsNewObject(View.CurrentObject)
                ? showViewParameters.CreatedView.ObjectSpace.GetObject(View.CurrentObject)
                : showViewParameters.CreatedView.ObjectSpace.CreateObject(View.ObjectTypeInfo.Type);
            showViewParameters.TargetWindow = TargetWindow.Current;
            showViewParameters.Context = TemplateContext.View;
        }
    }
}