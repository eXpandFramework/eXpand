using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.XtraEditors;
using Fasterflect;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Win.SystemModule{
    public interface IModelOptionsFormCaching : IModelNode {
        [Category("eXpand")]
        [Description("OnViewControlsCreated will be called only once")]
        [ModelBrowsable(typeof(ModelOptionFormCachingVisibilityCalculator))]
        bool FormCaching { get; set; }
    }

    [ModelInterfaceImplementor(typeof(IModelOptionsFormCaching),"Application.Options")]
    public interface IModelDetailViewFormCaching:IModelOptionsFormCaching{
         
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
            if (isVisible ) {
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
            var winWindow = strategy.Windows.FirstOrDefault(window => window.View!=null&&window.View.Model == e.View.Model);
            if (winWindow != null){
                if (((IModelDetailViewFormCaching) e.View.Model).FormCaching){
                    if (e.View.ObjectSpace.IsNewObject(e.View.CurrentObject)){
                        var newViewShortcut = e.View.CreateShortcut();
                        var temporaryObjectKey = newViewShortcut[ViewShortcut.TemporaryObjectKeyParamName];
                        var newObj = winWindow.View.ObjectSpace.CreateObject(winWindow.View.ObjectTypeInfo.Type);
                        e.View.CurrentObject.Map(newObj,Flags.Public);
                        CustomizeViewShortcut(e, winWindow, temporaryObjectKey);
                        winWindow.View.CurrentObject = newObj;
                    }
                    else{
                        winWindow.View.CurrentObject = winWindow.View.ObjectSpace.GetObject(e.View.CurrentObject);
                    }
                }
            }
        }

        private static void CustomizeViewShortcut(DetailViewCreatedEventArgs e, WinWindow winWindow, string temporaryObjectKey){
            e.View.CustomizeViewShortcut += (o, args) =>{
                var shortcut = args.ViewShortcut;
                if (shortcut.ContainsKey(ViewShortcut.IsNewObject)){
                    shortcut.Remove(ViewShortcut.IsNewObject);
                }
            };
            winWindow.View.CustomizeViewShortcut += (o, args) =>{
                var shortcut = args.ViewShortcut;
                if (shortcut.ContainsKey(ViewShortcut.TemporaryObjectKeyParamName))
                    shortcut[ViewShortcut.TemporaryObjectKeyParamName] = temporaryObjectKey;
                if (shortcut.ContainsKey(ViewShortcut.IsNewObject)){
                    shortcut.Remove(ViewShortcut.IsNewObject);
                }
            };
        }

        private void CloseWindowControllerOnFormClosing(object sender, FormClosingEventArgs formClosingEventArgs) {
            var view = Frame.View;
            if (((IModelDetailViewFormCaching) view.Model).FormCaching){
                if (view.ObjectSpace.IsNewObject(view.CurrentObject))
                    view.ObjectSpace.SetIsModified(false);
                ((XtraForm) sender).Hide();
            }
        }

        private void OnCanClose(object sender, HandledEventArgs handledEventArgs) {
            if (!handledEventArgs.Handled ){
                handledEventArgs.Handled = Frame.Context == TemplateContext.View;
                if (handledEventArgs.Handled)
                    _closeWindowController.FormClosing += CloseWindowControllerOnFormClosing;
            }
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions, IModelOptionsFormCaching>();
            extenders.Add<IModelDetailView, IModelDetailViewFormCaching>();
        }
    }
}