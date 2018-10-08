using System;
using System.ComponentModel;
using System.Web;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.Web.SystemModule {
    public interface IModelListViewOpenDetailViewInNewTab{
        [Category(AttributeCategoryNameProvider.Xpand)]
        [Description("WebApplication.EnableMultipleBrowserTabsSupport} must enabled for this to work")]
        bool OpenDetailViewInNewTab{ get; set; }
    }

    public class OpenDetailViewInNewTabController:ViewController<ListView>,IModelExtender{
        protected override void OnActivated() {
            base.OnActivated();
            if (((IModelListViewOpenDetailViewInNewTab) View.Model).OpenDetailViewInNewTab){
                if (!WebApplication.EnableMultipleBrowserTabsSupport)
                    throw new NotSupportedException($"{WebApplication.EnableMultipleBrowserTabsSupport} is not enabled");
                Frame.GetController<ListViewProcessCurrentObjectController>().CustomProcessSelectedItem += OnCustomProcessSelectedItem;
            }
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            var objectController = Frame.GetController<ListViewProcessCurrentObjectController>();
            objectController.CustomProcessSelectedItem -= OnCustomProcessSelectedItem;
        }

        private void OnCustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e) {
            e.Handled = true;
            var shortcut = new ViewShortcut(GetViewId(e.InnerArgs.CurrentObject), ObjectSpace.GetKeyValueAsString(e.InnerArgs.CurrentObject));
            var requestManager = ((WebApplication)Application).RequestManager;
            string hashUrl = requestManager.GetQueryString(shortcut);
            var script = $"window.open('{HttpContext.Current.Request.Url}#{hashUrl}','_blank');";
            WebWindow.CurrentRequestWindow.RegisterStartupScript($"WindowOpen{View.ObjectTypeInfo.FullName}-{ObjectSpace.GetKeyValue(e.InnerArgs.CurrentObject)}", script, true);
        }

        protected virtual string GetViewId(object currentObject){
            return Application.FindDetailViewId(currentObject, View);
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelListView,IModelListViewOpenDetailViewInNewTab>();
        }
    }
}
