using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Web;
using System;
using System.Web;
using System.Web.UI;
using DevExpress.ExpressApp.Web.Templates;

namespace EFDemo.Module.Controllers {
	public partial class ChooseTemplateController : ViewController {
		public ChooseTemplateController() {
			InitializeComponent();
			CreateActionItems();
			RegisterActions(components);
		}
		private void CreateActionItems() {
            ChoiceActionItem defaultTemplateItem = new ChoiceActionItem("Horizontal navigation", TemplateType.Horizontal);
			ChooseTemplateAction.Items.Add(defaultTemplateItem);
			ChoiceActionItem defaultVerticalTemplateItem = new ChoiceActionItem("Vertical navigation", TemplateType.Vertical);
			ChooseTemplateAction.Items.Add(defaultVerticalTemplateItem);
		}
            private void ChooseTemplateAction_Execute(object sender, DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventArgs e) {
                Page page = WebWindow.CurrentRequestPage;
                if(page != null) {
                    WebApplication.PreferredApplicationWindowTemplateType = (TemplateType)e.SelectedChoiceActionItem.Data;
                    if(page.IsCallback) {
                        WebWindow.CurrentRequestWindow.RegisterStartupScript("redirect", ((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager.GetScript(true));
                    }
                    else {
                        HttpContext.Current.Response.Redirect(HttpContext.Current.Request.Path);
                    }
                }
            }
		private void ChooseTemplateController_Activated(object sender, System.EventArgs e) {
			Frame.TemplateChanged += new EventHandler(Frame_TemplateChanged);
            UpdateSelectedItem();
		}

		void Frame_TemplateChanged(object sender, EventArgs e) {
			UpdateSelectedItem();
		}
		private void ChooseTemplateController_Deactivated(object sender, System.EventArgs e) {
			Frame.TemplateChanged -= new EventHandler(Frame_TemplateChanged);
		}
		private void UpdateSelectedItem() {
			foreach(ChoiceActionItem item in ChooseTemplateAction.Items) {
                if(WebApplication.PreferredApplicationWindowTemplateType == (TemplateType)item.Data) {
					ChooseTemplateAction.SelectedItem = item;
					break;
				}
			}
		}
	}
}
