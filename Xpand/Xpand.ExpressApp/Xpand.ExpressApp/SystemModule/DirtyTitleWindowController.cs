using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.SystemModule {
    public interface IModelViewDirtyTitle : IModelNode {
        [Category("eXpand")]
        bool DirtyTitle { get; set; }
    }

    public class DirtyTitleWindowController : WindowController, IModelExtender {
        public const char DirtyMark = '*';
        private WindowTemplateController _windowTemplateController;
        protected override void OnActivated() {
            base.OnActivated();
            Window.ViewChanged += Window_ViewChanged;
            _windowTemplateController = Window.GetController<WindowTemplateController>();
            _windowTemplateController.CustomizeWindowCaption += windowTemplateController_CustomizeWindowCaption;
        }
        void Window_ViewChanged(object sender, ViewChangedEventArgs e) {
            if (Window.View is ObjectView) {
                Window.View.ObjectSpace.ModifiedChanged += ObjectSpace_ModifiedChanged;
                Window.View.ObjectSpace.Reloaded += ObjectSpace_Reloaded;
            }
        }
        private void ObjectSpace_ModifiedChanged(object sender, EventArgs e) {
            if (((IObjectSpace)sender).IsModified) {
                UpdateCaption();
            }
        }

        void ObjectSpace_Reloaded(object sender, EventArgs e) {
            UpdateCaption();
        }

        protected virtual void UpdateCaption() {
            if (_windowTemplateController != null) {
                _windowTemplateController.UpdateWindowCaption();
            }
        }

        private void windowTemplateController_CustomizeWindowCaption(object sender, CustomizeWindowCaptionEventArgs e) {
            if (Enabled()) {
                e.WindowCaption.FirstPart = e.WindowCaption.FirstPart.TrimStart(DirtyMark);
                if (Window.View.ObjectSpace.IsModified) {
                    e.WindowCaption.FirstPart = String.Format("{0} {1}", DirtyMark, e.WindowCaption.FirstPart);
                }
            }
        }

        private bool Enabled() {
            var view = Window.View;
            return view != null && ((IModelViewDirtyTitle)view.Model).DirtyTitle;
        }

        protected override void OnDeactivated() {
            Window.ViewChanged -= Window_ViewChanged;
            if (Window.View is ObjectView) {
                _windowTemplateController.CustomizeWindowCaption -= windowTemplateController_CustomizeWindowCaption;
                Window.View.ObjectSpace.ModifiedChanged -= ObjectSpace_ModifiedChanged;
                Window.View.ObjectSpace.Reloaded -= ObjectSpace_Reloaded;
                _windowTemplateController = null;
            }
            base.OnDeactivated();
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelView, IModelViewDirtyTitle>();
        }
    }
}