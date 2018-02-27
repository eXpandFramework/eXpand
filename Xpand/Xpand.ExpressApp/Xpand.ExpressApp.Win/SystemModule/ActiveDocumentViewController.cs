using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.XtraBars.Docking2010.Views;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.Win.SystemModule{
    public class ActiveDocumentViewController:ExpressApp.SystemModule.ActiveDocumentViewController{

        protected override void OnActivated(){
            base.OnActivated();
            Frame.TemplateChanged += Frame_TemplateChanged;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            Frame.TemplateChanged -= Frame_TemplateChanged;
            if (Frame.Template is IXafDocumentsHostWindow) {
                ((IXafDocumentsHostWindow)Frame.Template).DocumentManager.ViewChanged -= DocumentManager_ViewChanged;
                ((IXafDocumentsHostWindow)Frame.Template).DocumentManager.ViewChanging -= DocumentManager_ViewChanging;
            }
        }

        void Frame_TemplateChanged(object sender, EventArgs e) {
            if (((IModelOptionsWin)Application.Model.Options).UIType == UIType.TabbedMDI && Frame.Template is IXafDocumentsHostWindow) {
                ((IXafDocumentsHostWindow)Frame.Template).DocumentManager.ViewChanged += DocumentManager_ViewChanged;
                ((IXafDocumentsHostWindow)Frame.Template).DocumentManager.ViewChanging += DocumentManager_ViewChanging;
            }
        }
        void DocumentManager_ViewChanging(object sender, DevExpress.XtraBars.Docking2010.ViewEventArgs args) {
            args.View.DocumentActivated -= View_DocumentActivated;
        }
        void DocumentManager_ViewChanged(object sender, DevExpress.XtraBars.Docking2010.ViewEventArgs args) {
            args.View.DocumentActivated += View_DocumentActivated;
        }
        void View_DocumentActivated(object sender, DocumentEventArgs e){
            var documentForm = e.Document.Form as IViewHolder;
            if (documentForm != null) {
                OnActiveViewChanged(new ActiveViewChangedEventArgs(documentForm.View));
            }
        }
    }
}
