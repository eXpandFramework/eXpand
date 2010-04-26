using System.Data.SqlClient;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.SystemModule {
    public class ConnectionInfoStatusMessageController:WindowController {
        public const string ConnectionInfoMessage = "ConnectionInfoMessage";
        public ConnectionInfoStatusMessageController() {
            TargetWindowType = WindowType.Main;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            var controller = Frame.GetController<WindowTemplateController>();
            if (Application.Info.GetChildNode("Options").GetAttributeBoolValue(ConnectionInfoMessage))
                controller.CustomizeWindowStatusMessages += controller_CustomizeWindowStatusMessages;
        }
        public override Schema GetSchema() {
            const string s =
                @"<Element Name=""Application"">
					<Element Name=""Options"">
						<Attribute Name=""" +
                ConnectionInfoMessage + @""" Choice=""True,False""/>
					</Element>
				</Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(s));
        }

        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            var controller = Frame.GetController<WindowTemplateController>();
            controller.CustomizeWindowStatusMessages -= controller_CustomizeWindowStatusMessages;
        }

        void controller_CustomizeWindowStatusMessages(object sender, CustomizeWindowStatusMessagesEventArgs e) {
            var dbConnection = Application.ObjectSpaceProvider.CreateUpdatingReadOnlySession().Connection;
            e.StatusMessages.Add(string.Format("({0} - {1})", ((SqlConnection)dbConnection).DataSource, dbConnection.Database));
        }

    }
}