using System.ComponentModel;
using System.Data.Common;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Xpo;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;

namespace Xpand.ExpressApp.SystemModule {
    public interface IModelOptionsConnectionInfoStatusMessage {
        [Category(AttributeCategoryNameProvider.Xpand)]
        [Description("Display connection info (server/database) at status bar")]
        [ModelBrowsable(typeof(WinOnlyVisibilityCalculator))]
        bool ConnectionInfoMessage { get; set; }

    }
    public class ConnectionInfoStatusMessageController : WindowController, IModelExtender {

        public ConnectionInfoStatusMessageController() {
            TargetWindowType = WindowType.Main;
        }
        protected override void OnActivated() {
            base.OnActivated();
            var controller = Frame.GetController<WindowTemplateController>();
            if (((IModelOptionsConnectionInfoStatusMessage)Application.Model.Options).ConnectionInfoMessage)
                controller.CustomizeWindowStatusMessages += controller_CustomizeWindowStatusMessages;
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions, IModelOptionsConnectionInfoStatusMessage>();
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            var controller = Frame.GetController<WindowTemplateController>();
            if (((IModelOptionsConnectionInfoStatusMessage)Application.Model.Options).ConnectionInfoMessage)
                controller.CustomizeWindowStatusMessages -= controller_CustomizeWindowStatusMessages;
        }

        void controller_CustomizeWindowStatusMessages(object sender, CustomizeWindowStatusMessagesEventArgs e) {
            var dbConnection = ((XPObjectSpace)Application.ObjectSpaceProvider.CreateUpdatingObjectSpace(false)).Session.Connection as DbConnection;
            if (dbConnection != null)
                e.StatusMessages.Add(string.Format("({0} - {1})", dbConnection.DataSource, dbConnection.Database));
        }
    }
}