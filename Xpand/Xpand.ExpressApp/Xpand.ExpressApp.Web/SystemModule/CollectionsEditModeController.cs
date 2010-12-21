using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Web;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.Web.SystemModule {
    public class CollectionsEditModeController : ViewEditModeController{
        ViewEditMode _collectionsEditMode;
        View _view;

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.ViewChanging+=FrameOnViewChanging;
            Frame.ViewChanged+=FrameOnViewChanged;
        }

        void FrameOnViewChanged(object sender, ViewChangedEventArgs viewChangedEventArgs) {
            if (((Frame) sender).View == _view) {
                ((ShowViewStrategy)Application.ShowViewStrategy).CollectionsEditMode=_collectionsEditMode;
                _view = null;
            }
        }

        void FrameOnViewChanging(object sender, ViewChangingEventArgs viewChangingEventArgs) {
            var viewEditMode = ((IModelViewEditMode) viewChangingEventArgs.View.Model).ViewEditMode;
            if (viewEditMode.HasValue&&viewChangingEventArgs.View is ListView) {
                var showViewStrategy = ((ShowViewStrategy)Application.ShowViewStrategy);
                _collectionsEditMode = showViewStrategy.CollectionsEditMode;
                _view = viewChangingEventArgs.View;
                showViewStrategy.CollectionsEditMode = viewEditMode.Value;
            }
        }

    }
}
