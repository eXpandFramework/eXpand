using System.ComponentModel;
using System.Data.SqlClient;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.SystemModule {
    public interface IConnectionInfoStatusMessage {
        [Category("eXpand")]
        [Description("Display connection info (server/database) at status bar")]
        bool ConnectionInfoMessage { get; set; }

    }
    public class ConnectionInfoStatusMessageController : WindowController, IModelExtender {

        public ConnectionInfoStatusMessageController() {
            TargetWindowType = WindowType.Main;
        }
        protected override void OnActivated() {
            base.OnActivated();
            var controller = Frame.GetController<WindowTemplateController>();
            if (((IConnectionInfoStatusMessage)Application.Model.Options).ConnectionInfoMessage)
                controller.CustomizeWindowStatusMessages += controller_CustomizeWindowStatusMessages;
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions, IConnectionInfoStatusMessage>();
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            var controller = Frame.GetController<WindowTemplateController>();
            if (((IConnectionInfoStatusMessage)Application.Model.Options).ConnectionInfoMessage)
                controller.CustomizeWindowStatusMessages -= controller_CustomizeWindowStatusMessages;
        }

        void controller_CustomizeWindowStatusMessages(object sender, CustomizeWindowStatusMessagesEventArgs e) {
            var dbConnection = ((ObjectSpace)Application.ObjectSpaceProvider.CreateUpdatingObjectSpace(false)).Session.Connection;
            if (dbConnection != null)
                e.StatusMessages.Add(string.Format("({0} - {1})", ((SqlConnection)dbConnection).DataSource, dbConnection.Database));
        }
    }
}