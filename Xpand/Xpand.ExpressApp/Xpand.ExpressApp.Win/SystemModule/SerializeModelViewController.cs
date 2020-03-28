using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win;

namespace Xpand.ExpressApp.Win.SystemModule{
    [ModelAbstractClass]
    public interface IModelViewSerializable:IModelView {
        [Category(nameof(XpandSystemWindowsFormsModule))]
        bool? SerializableView { get; set; }
    }
    public class SerializeModelViewController : WindowController,IModelExtender {  
        MdiShowViewStrategy _viewStrategy;  
        protected override void OnActivated() {  
            base.OnActivated();  
            _viewStrategy = this.Application.ShowViewStrategy as MdiShowViewStrategy;  
            if(_viewStrategy != null) {  
                _viewStrategy.CustomCanCreateViewDocumentControlDescription += ViewStrategy_CustomCanCreateViewDocumentControlDescription;  
                _viewStrategy.CustomCanProcessDocumentControlDescription += ViewStrategy_CustomCanProcessDocumentControlDescription;  
            }  
        }  
        protected override void OnDeactivated() {  
            base.OnDeactivated();  
            if(_viewStrategy != null) {  
                _viewStrategy.CustomCanCreateViewDocumentControlDescription -= ViewStrategy_CustomCanCreateViewDocumentControlDescription;  
                _viewStrategy.CustomCanProcessDocumentControlDescription -= ViewStrategy_CustomCanProcessDocumentControlDescription;  
            }  
        }  
        void ViewStrategy_CustomCanProcessDocumentControlDescription(object sender, CustomCanProcessDocumentControlDescriptionEventArgs e) {  
            var viewShortcut = ViewShortcut.FromString(e.DocumentControlDescription.SerializedControl);  
            var modelView = ((IModelViewSerializable) Application.FindModelView(viewShortcut.ViewId));
            if (modelView.SerializableView.HasValue) {
                e.Result = modelView.SerializableView.Value;  
                e.Handled = true;  
            }
        }  

        void ViewStrategy_CustomCanCreateViewDocumentControlDescription(object sender, CustomCanCreateViewDocumentControlDescriptionEventArgs e) {
            var model = ((IModelViewSerializable) e.View.Model).SerializableView;
            if (model.HasValue) {
                e.Result = model.Value;
                e.Handled = true;
            }
        }  

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelView,IModelViewSerializable>();
        }
    }
}