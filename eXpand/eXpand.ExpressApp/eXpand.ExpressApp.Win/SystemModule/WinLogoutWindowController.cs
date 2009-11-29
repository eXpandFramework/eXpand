using DevExpress.ExpressApp;
using eXpand.ExpressApp.SystemModule;
using eXpand.ExpressApp.Win.Interfaces;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public partial class WinLogoutWindowController : BaseWindowController
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

            Active[LogOutEnable] = Application.Info.GetChildNodeByPath("Options").GetAttributeBoolValue((LogOutEnable));
        }

        private void logOutSimpleAction_Execute(object sender, DevExpress.ExpressApp.Actions.SimpleActionExecuteEventArgs e)
        {
            ((ILogOut)Application).Logout();
        }

        public override Schema GetSchema()
        {
            const string CommonTypeInfos = @"<Element Name=""Application"">
                    <Element Name=""Options"" >
                            <Attribute Name=""" + LogOutEnable + @""" Choice=""False,True""/>
                    </Element>
                </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(CommonTypeInfos));

        }
    }
}