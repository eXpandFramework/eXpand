using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.Templates;
using Xpand.Persistent.Base.General.Model;
using PopupWindow = DevExpress.ExpressApp.Web.PopupWindow;

namespace Xpand.ExpressApp.Web.SystemModule {
    public interface IModelListViewHideLoopupSearch{
        [Category(AttributeCategoryNameProvider.Xpand)]
        bool HideLookupSearch{ get; set; }
    }
    public class FindPopupController:ViewController<ListView>,IModelExtender{
        private bool _enablePaging;
        protected virtual bool IsFindPopup => View.Editor is ASPxGridListEditor && Frame is PopupWindow && WebApplication.Instance.RequestManager.IsFindPopupWindow();

        protected override void OnActivated(){
            base.OnActivated();
            if (IsFindPopup){
                if (((IModelListViewShowAutoFilterRow) View.Model).ShowAutoFilterRow){
                    _enablePaging = DevExpress.ExpressApp.Web.SystemModule.FindPopupController.EnablePaging;
                    DevExpress.ExpressApp.Web.SystemModule.FindPopupController.EnablePaging = true;
                }
                
            }
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            if (IsFindPopup && ((IModelListViewShowAutoFilterRow)View.Model).ShowAutoFilterRow)
                DevExpress.ExpressApp.Web.SystemModule.FindPopupController.EnablePaging = _enablePaging;
        }

        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();
            if (IsFindPopup&& ((IModelListViewHideLoopupSearch)View.Model).HideLookupSearch) {
                ((BaseXafPage)Frame.Template).InnerContentPlaceHolder.Controls.OfType<FindDialogTemplateContentNew>()
                    .First().IsSearchEnabled = false;
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelListView,IModelListViewHideLoopupSearch>();
        }
    }

}
