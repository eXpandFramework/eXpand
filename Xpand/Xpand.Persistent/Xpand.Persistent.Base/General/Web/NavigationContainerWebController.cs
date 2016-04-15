using System;
using DevExpress.ExpressApp.Web;
using Xpand.Persistent.Base.General.Controllers;

namespace Xpand.Persistent.Base.General.Web{
    public class NavigationContainerWebController : NavigationContainerController {
        public NavigationContainerWebController(){
        }

        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            if (!WebApplicationStyleManager.IsNewStyle) {
                Frame.Disposing += FrameOnDisposing;
                if (WebWindow.CurrentRequestPage != null)
                    WebWindow.CurrentRequestPage.PreRender += CurrentRequestPageOnPreRender;
            }
        }

        private void FrameOnDisposing(object sender, EventArgs eventArgs){
            Frame.Disposing-=FrameOnDisposing;
            if (WebWindow.CurrentRequestPage != null)
                WebWindow.CurrentRequestPage.PreRender -= CurrentRequestPageOnPreRender;
        }

        private void CurrentRequestPageOnPreRender(object sender, EventArgs eventArgs){
            var options = ((IModelOptionsNavigationContainer)Application.Model.Options);
            string script = null;
            if (options.NavigationAlwaysVisibleOnStartup){
                var localScript = @" function EnsureNavIsVisibleOnStartUp(){
                                var cellElement = document.getElementById('LPcell');
                                var visible=!cellElement.style.display || cellElement.style.display == tableCellDefaultDisplay
                                if (!visible){
                                    OnClick('LPcell', 'separatorImage', true);
                                }
                            }";
                WebWindow.CurrentRequestWindow.RegisterStartupScript("EnsureNavIsVisibleOnStartUp", localScript);
                script = "EnsureNavIsVisibleOnStartUp();"+Environment.NewLine;
            }

            var hideNavigationOnStartup = options.HideNavigationOnStartup;
            if (hideNavigationOnStartup.HasValue && hideNavigationOnStartup.Value){
                var localScript = @"function HideNavOnStartup(){
                                var displayChanged=false;
                                var hide=" + options.HideNavigationOnStartup.ToString().ToLower() + @";
                                var cellElement = document.getElementById('LPcell');
                                var visible=!cellElement.style.display || cellElement.style.display == tableCellDefaultDisplay
                                if ((hide&&visible)||(!visible&&!hide)){
                                    OnClick('LPcell', 'separatorImage', true);
                                }
                            }";
                WebWindow.CurrentRequestWindow.RegisterStartupScript("HideNavigationOnStartup", localScript);
                script += "HideNavOnStartup();";
            }
            
            if (!string.IsNullOrEmpty(script)){
                script = @"window.onload = function () {" + script +"};";
                WebWindow.CurrentRequestWindow.RegisterStartupScript(GetType().Name, script);
            }
        }

    }
}