using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.XtraEditors;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Win.SystemModule{
    public interface IModelOptionsFormCaching : IModelNode {
        [Category("eXpand")]
        [Description("OnViewControlsCreated will be called only once")]
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
            var modelOptions = Application.Model.Options;
            bool isVisible = new ModelOptionFormCachingVisibilityCalculator().IsVisible(modelOptions, null);
            _closeWindowController = Frame.GetController<CloseWindowController>();
            if (isVisible && ((IModelOptionsFormCaching)modelOptions).FormCaching) {
                _closeWindowController.CanClose += OnCanClose;
                Application.DetailViewCreated += ApplicationOnDetailViewCreated;
            }
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            Application.DetailViewCreated-=ApplicationOnDetailViewCreated;
            _closeWindowController.CanClose -= OnCanClose;
            _closeWindowController.FormClosing -= CloseWindowControllerOnFormClosing;
        }

        private void ApplicationOnDetailViewCreated(object sender, DetailViewCreatedEventArgs e){
            var strategy = ((WinShowViewStrategyBase) Application.ShowViewStrategy);
            foreach (var winWindow in strategy.Windows.Where(winWindow => !winWindow.View.IsValueNull())){
                var view = winWindow.View;
                var shortcut = view.CreateShortcut();
                var newViewShortCut = e.View.CreateShortcut();
                shortcut.ObjectKey = newViewShortCut.ObjectKey;
                if (shortcut == newViewShortCut)
                    view.CurrentObject = view.ObjectSpace.GetObject(e.View.CurrentObject);
            }
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