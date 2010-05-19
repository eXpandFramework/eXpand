using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.SystemModule;
using eXpand.ExpressApp.Win.Interfaces;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public interface IModelLogOutEnable : IModelNode
    {
        bool LogOutEnable { get; set; }
    }

    public partial class WinLogoutWindowController : BaseWindowController, IModelExtender
    {
        public const string LogOutEnable = "LogOutEnable";

        public WinLogoutWindowController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            Active[LogOutEnable] = ((IModelLogOutEnable)Application.Model.Options).LogOutEnable;
        }

        private void logOutSimpleAction_Execute(object sender, DevExpress.ExpressApp.Actions.SimpleActionExecuteEventArgs e)
        {
            ((ILogOut)Application).Logout();
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelOptions, IModelLogOutEnable>();
        }
    }
}