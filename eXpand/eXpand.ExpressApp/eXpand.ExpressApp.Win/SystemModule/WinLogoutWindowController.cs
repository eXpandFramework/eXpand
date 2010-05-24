using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.Win.Interfaces;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public interface IModelOptionsLogOutEnable : IModelNode
    {
        [Category("eXpand")]
        bool LogOutEnable { get; set; }
    }

    public class WinLogoutWindowController : WindowController, IModelExtender
    {
        public const string LogOutEnable = "LogOutEnable";

        public WinLogoutWindowController()
        {
            var logOutAction = new SimpleAction(this, "logOutSimpleAction", "Export") { Caption = "Log Out" };
            logOutAction.Execute+=logOutSimpleAction_Execute;            
            
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            Active[LogOutEnable] = ((IModelOptionsLogOutEnable)Application.Model.Options).LogOutEnable;
        }

        private void logOutSimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ((ILogOut)Application).Logout();
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelOptions, IModelOptionsLogOutEnable>();
        }
    }
}