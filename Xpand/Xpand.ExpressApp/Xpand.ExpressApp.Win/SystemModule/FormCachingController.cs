using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.XtraEditors;

namespace Xpand.ExpressApp.Win.SystemModule{
    public interface IModelOptionsFormCaching : IModelNode {
        [Category("eXpand")]
        [Description("OnViewControlsCreated will be called only once when possible")]
        [ModelBrowsable(typeof(ModelOptionFormCachingVisibilityCalculator))]
        bool FormCaching { get; set; }
    }

    public class ModelOptionFormCachingVisibilityCalculator:IModelIsVisible{
        public bool IsVisible(IModelNode node, string propertyName){
            return ((IModelOptionsWin) node.Application.Options).UIType != UIType.SingleWindowSDI;
        }
    }

    public class FormCachingController : WindowController, IModelExtender {
        private CloseWindowController _closeWindowController;

        protected override void OnActivated() {
            base.OnActivated();
            _closeWindowController = Frame.GetController<CloseWindowController>();
            var modelOptions = Application.Model.Options;
            bool isVisible = new ModelOptionFormCachingVisibilityCalculator().IsVisible(modelOptions, null);
            if (isVisible&&((IModelOptionsFormCaching) modelOptions).FormCaching)
                _closeWindowController.CanClose += OnCanClose;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            _closeWindowController.CanClose -= OnCanClose;
            _closeWindowController.FormClosing -= CloseWindowControllerOnFormClosing;
        }

        private void CloseWindowControllerOnFormClosing(object sender, FormClosingEventArgs formClosingEventArgs) {
            ((XtraForm)sender).Hide();
        }

        private void OnCanClose(object sender, HandledEventArgs handledEventArgs) {
            if (!handledEventArgs.Handled){
                handledEventArgs.Handled = Frame.Context == TemplateContext.View ;
                if (handledEventArgs.Handled)
                    _closeWindowController.FormClosing += CloseWindowControllerOnFormClosing;
            }
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions, IModelOptionsFormCaching>();
        }
    }
}