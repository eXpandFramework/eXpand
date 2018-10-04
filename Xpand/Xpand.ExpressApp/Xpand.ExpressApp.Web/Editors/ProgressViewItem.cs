using System;
using System.Reflection;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.Web;
using Fasterflect;
using Xpand.ExpressApp.Editors;
using StringExtensions = Xpand.Utils.Helpers.StringExtensions;

namespace Xpand.ExpressApp.Web.Editors{
    [ViewItem(typeof(IModelProgressViewItem))]
    public class ProgressViewItem : ExpressApp.Editors.ProgressViewItem,IXafCallbackHandler {
        private string _handlerId;
        private static readonly MethodInvoker DelegateForGetShowMessageScript;

        static ProgressViewItem() {
            var methodInfoGetShowMessageScript = typeof(PopupWindowManager).GetMethod("GetShowMessageScript",BindingFlags.Static|BindingFlags.NonPublic);
            DelegateForGetShowMessageScript = methodInfoGetShowMessageScript.DelegateForCallMethod();
            
        }

        public ProgressViewItem(IModelProgressViewItem info, Type classType) : base(info, classType){
            
        }

        public ASPxProgressBar ProgressBar{ get; private set; }

        public void ProcessAction(string parameter) {
            var script = $"{parameter}.SetPosition('{Position}');";
            if (FinishOptions!=null) {
                script = $"{parameter}.SetPosition(100);{DelegateForGetShowMessageScript(null, FinishOptions)}";
                SetFinishOptions(null);
            }
            WebWindow.CurrentRequestWindow.RegisterStartupScript(_handlerId,script,true);
        }

        private XafCallbackManager CallbackManager => ((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager;


        public override void Start() {
            base.Start();
            var script = CallbackManager.GetScript(_handlerId, $"'{ProgressBar.ClientInstanceName}'","",false);
            ProgressBar.ClientSideEvents.Init =
                $@"function(s,e) {{
                    s.timer = window.setInterval(function(){{
                                if (s.GetPosition()==100){{
                                    window.clearInterval(s.timer);
                                    s.SetPosition(0);
                                    return;
                                }}
                                var previous = startProgress;
                                startProgress = function () {{ }}; 
                                {script};
                                startProgress = previous;
                            }},{PollingInterval});
                }}";
        }

        protected override object CreateControlCore() {
            ProgressBar = new ASPxProgressBar{
                ClientInstanceName = $"{StringExtensions.CleanCodeName(Id)}", Width = Unit.Percentage(100)
            };
            
            View.ControlsCreated+=ViewOnControlsCreated;
            return ProgressBar;
        }

        private void ViewOnControlsCreated(object sender, EventArgs e){
            _handlerId=$"{GetType().FullName}{ProgressBar.ClientInstanceName}";
            CallbackManager.RegisterHandler(_handlerId, this);
        }
    }
}