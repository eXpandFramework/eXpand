using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Fasterflect;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.Base.Xpo;
using Xpand.Xpo;

namespace Xpand.ExpressApp.Win.SystemModule {
    public interface IModelOptionsDetailViewCaching : IModelNode {
        [Category(AttributeCategoryNameProvider.Xpand)]
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

    public class DetailViewCachingController : ViewController<DetailView>, IModelExtender {
        private bool _isVisible;
        private static bool _applicationWindowClosing;
        private CloseReason _closeReason;
        public event EventHandler<CustomNewObjectArgs> CustomNewObject;

        protected virtual void OnCustomNewObject(CustomNewObjectArgs e){
            var handler = CustomNewObject;
            if (handler != null) handler(this, e);
        }

        protected override void OnActivated() {
            base.OnActivated();
            if (_isVisible && ((IModelDetailViewCaching)View.Model).DetailViewCaching) {
                View.QueryCanClose += ViewOnQueryCanClose;
            }
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            View.QueryCanClose -= ViewOnQueryCanClose;
        }

        private void OnCancel(object sender, CancelEventArgs cancelEventArgs) {
            _applicationWindowClosing = Frame.Template == Application.MainWindow.Template;
        }

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            var modelOptions = Application.Model.Options;
            _isVisible = new ModelOptionsDetailViewCachingVisibilityCalculator().IsVisible(modelOptions, null);
            var winWindow = Frame as WinWindow;
            if (winWindow != null) winWindow.Closing += OnClosing;
            if (_isVisible && Frame.Context == TemplateContext.ApplicationWindow) {
                Application.DetailViewCreated += ApplicationOnDetailViewCreated;
                Frame.GetController<CloseFormController>().Cancel += OnCancel;
                Frame.TemplateChanged += FrameOnTemplateChanged;

            }
            Frame.Disposing += FrameOnDisposing;
        }

        private void FrameOnDisposing(object sender, EventArgs eventArgs) {
            Frame.Disposing -= FrameOnDisposing;
            Application.DetailViewCreated -= ApplicationOnDetailViewCreated;
            Frame.TemplateChanged -= FrameOnTemplateChanged;
            var winWindow = Frame as WinWindow;
            if (winWindow != null) winWindow.Closing -= OnClosing;
        }

        private void FrameOnTemplateChanged(object sender, EventArgs eventArgs) {
            var form = ((Form)Frame.Template);
            form.Closing += OnClosing;
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs) {
            _closeReason = ((FormClosingEventArgs)cancelEventArgs).CloseReason;
        }

        private void ViewOnQueryCanClose(object sender, CancelEventArgs e) {
            if (CloseFormController.IsNotLoggingOffOrModelEditing && !_applicationWindowClosing && _closeReason == CloseReason.UserClosing) {
                var cancelEventArgs = new CancelEventArgs(false);
                Frame.GetController<WinModificationsController>().CallMethod("OnViewQueryCanClose", cancelEventArgs);
                e.Cancel = true;
                if (!cancelEventArgs.Cancel) {
                    if (ObjectSpace.IsNewObject(View.CurrentObject))
                        ObjectSpace.SetIsModified(false);
                    ((Form)Frame.Template).Hide();
                }
            }
        }

        private void ApplicationOnDetailViewCreated(object sender, DetailViewCreatedEventArgs e) {
            if (((IModelDetailViewCaching)e.View.Model).DetailViewCaching) {
                var strategy = ((WinShowViewStrategyBase)Application.ShowViewStrategy);
                var winWindow = strategy.Windows.FirstOrDefault(window => window.View != null && window.View.Model == e.View.Model);
                if (winWindow != null) {
                    if (winWindow.View.ObjectSpace.IsNewObject(winWindow.View.CurrentObject))
                        winWindow.View.ObjectSpace.RollbackSilent();
                    winWindow.View.SetPropertyValue("KeyMemberValue", null);
                    var currentObject = GetCurrentObject(e, winWindow);
                    winWindow.View.CurrentObject = currentObject;
                }
            }
        }

        class Cloner : DevExpress.Persistent.Base.Cloner {
            public override IXPSimpleObject CreateObject(Session session, Type type){
                return NewObject(type);
            }

            public override void CopyMemberValue(XPMemberInfo memberInfo, IXPSimpleObject sourceObject, IXPSimpleObject targetObject){
                CheckMemberInObjects(memberInfo, sourceObject, targetObject);
                object memberValue = this.CallMethod("GetSourceMemberValue",memberInfo, sourceObject);
                if (memberValue is IXPSimpleObject) {
                    this.CallMethod("ClearTargetObjectProperty",memberInfo, targetObject, memberInfo.IsAggregated);
                    var clonedObjectReference = (IXPSimpleObject) this.CallMethod("CloneReferenceMemberValue",targetObject, memberInfo, (IXPSimpleObject)memberValue, memberValue.GetType(), memberInfo.IsAggregated);
                    if (!clonedObjectReference.IsNewObject())
                        clonedObjectReference = targetObject.XPObjectSpace().GetObject(clonedObjectReference);
                    memberInfo.SetValue(targetObject, clonedObjectReference);
                }
                else {
                    memberInfo.SetValue(targetObject, memberValue);
                }
            }


            public Func<Type,IXPSimpleObject> NewObject;
        }
        private object GetCurrentObject(DetailViewCreatedEventArgs e, WinWindow winWindow) {
            if (e.View.ObjectSpace.IsNewObject(e.View.CurrentObject)) {
                var newViewShortcut = e.View.CreateShortcut();
                var temporaryObjectKey = newViewShortcut[ViewShortcut.TemporaryObjectKeyParamName];
                var newObj =new Cloner{NewObject =type =>(IXPSimpleObject)winWindow.View.ObjectSpace.CreateObject(type)}.
                    CloneTo (e.View.CurrentObject, winWindow.View.ObjectTypeInfo.Type);
                CustomizeViewShortcut(e, winWindow, temporaryObjectKey);
                return newObj;
            }
            return winWindow.View.ObjectSpace.GetObject(e.View.CurrentObject);
        }

        private void CustomizeViewShortcut(DetailViewCreatedEventArgs e, WinWindow winWindow, string temporaryObjectKey) {
            e.View.CustomizeViewShortcut += (o, args) => {
                ViewShortcut shortcut = args.ViewShortcut;
                shortcut.Remove(ViewShortcut.IsNewObject);
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

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions, IModelOptionsDetailViewCaching>();
            extenders.Add<IModelDetailView, IModelDetailViewCaching>();
        }
    }

    public class CustomNewObjectArgs : EventArgs{
    }
}
