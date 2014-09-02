using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.SystemModule;
using Fasterflect;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Win.SystemModule {
    public interface IModelOptionsDetailViewCaching : IModelNode {
        [Category("eXpand")]
        [Description("OnViewControlsCreated will be called only once")]
        [ModelBrowsable(typeof(ModelOptionsDetailViewCachingVisibilityCalculator))]
        bool DetailViewCaching { get; set; }
    }

    [ModelInterfaceImplementor(typeof(IModelOptionsDetailViewCaching), "Application.Options")]
    public interface IModelDetailViewCaching : IModelOptionsDetailViewCaching {

    }

    public class ModelOptionsDetailViewCachingVisibilityCalculator : IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName) {
            return ((IModelOptionsWin)node.Application.Options).UIType != UIType.SingleWindowSDI;
        }
    }

    public class DetailViewCachingController:ViewController<DetailView>, IModelExtender{
        private bool _isVisible;

        protected override void OnActivated(){
            base.OnActivated();
            if (_isVisible && ((IModelDetailViewCaching)View.Model).DetailViewCaching) {
                View.QueryCanClose+=ViewOnQueryCanClose;
            }
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            View.QueryCanClose -= ViewOnQueryCanClose;
        }

        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            var modelOptions = Application.Model.Options;
            _isVisible = new ModelOptionsDetailViewCachingVisibilityCalculator().IsVisible(modelOptions, null);
            if (_isVisible) {
                Application.DetailViewCreated += ApplicationOnDetailViewCreated;
            }
            Frame.Disposing+=FrameOnDisposing;
        }

        private void FrameOnDisposing(object sender, EventArgs eventArgs){
            Frame.Disposing-=FrameOnDisposing;
            Application.DetailViewCreated-=ApplicationOnDetailViewCreated;
        }

        private void ViewOnQueryCanClose(object sender, CancelEventArgs e){
            if (CloseFormController.CanCancel) {
                var cancelEventArgs = new CancelEventArgs(false);
                Frame.GetController<WinModificationsController>().CallMethod("OnViewQueryCanClose", cancelEventArgs);
                e.Cancel = true;
                if (!cancelEventArgs.Cancel){
                    if (ObjectSpace.IsNewObject(View.CurrentObject))
                        ObjectSpace.SetIsModified(false);
                    ((Form) Frame.Template).Hide();
                }
            }
        }

        private void ApplicationOnDetailViewCreated(object sender, DetailViewCreatedEventArgs e) {
            var strategy = ((WinShowViewStrategyBase)Application.ShowViewStrategy);
            var winWindow = strategy.Windows.FirstOrDefault(window => window.View != null && window.View.Model == e.View.Model);
            if (winWindow != null) {
                if (((IModelDetailViewCaching)e.View.Model).DetailViewCaching) {
                    winWindow.View.CurrentObject=GetCurrentObject(e, winWindow);
                }
            }
        }

        private object GetCurrentObject(DetailViewCreatedEventArgs e, WinWindow winWindow){
            if (e.View.ObjectSpace.IsNewObject(e.View.CurrentObject)){
                var newViewShortcut = e.View.CreateShortcut();
                var temporaryObjectKey = newViewShortcut[ViewShortcut.TemporaryObjectKeyParamName];
                var newObj = winWindow.View.ObjectSpace.CreateObject(winWindow.View.ObjectTypeInfo.Type);
                e.View.CurrentObject.Map(newObj, Flags.Public);
                CustomizeViewShortcut(e, winWindow, temporaryObjectKey);
                return newObj;
            }
            return winWindow.View.ObjectSpace.GetObject(e.View.CurrentObject);
        }

        private void CustomizeViewShortcut(DetailViewCreatedEventArgs e, WinWindow winWindow, string temporaryObjectKey) {
            e.View.CustomizeViewShortcut += (o, args) => {
                var shortcut = args.ViewShortcut;
                if (shortcut.ContainsKey(ViewShortcut.IsNewObject)) {
                    shortcut.Remove(ViewShortcut.IsNewObject);
                }
            };
            winWindow.View.CustomizeViewShortcut += (o, args) => {
                var shortcut = args.ViewShortcut;
                if (shortcut.ContainsKey(ViewShortcut.TemporaryObjectKeyParamName))
                    shortcut[ViewShortcut.TemporaryObjectKeyParamName] = temporaryObjectKey;
                if (shortcut.ContainsKey(ViewShortcut.IsNewObject)) {
                    shortcut.Remove(ViewShortcut.IsNewObject);
                }
            };
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelOptions, IModelOptionsDetailViewCaching>();
            extenders.Add<IModelDetailView, IModelDetailViewCaching>();
        }
    }
}
