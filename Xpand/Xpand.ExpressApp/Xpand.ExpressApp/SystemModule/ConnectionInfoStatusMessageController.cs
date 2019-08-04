using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;

namespace Xpand.ExpressApp.SystemModule {
    public interface IModelOptionsConnectionInfoStatusMessage {
        [Category(AttributeCategoryNameProvider.Xpand+".ConnectionInfo")]
        [Description("Display connection info (server/database) at status bar")]
        [ModelBrowsable(typeof(WinOnlyVisibilityCalculator))]
        bool ConnectionInfoMessage { get; set; }
        [Category(AttributeCategoryNameProvider.Xpand+".ConnectionInfo")]
        [Description("Display connection info (server/database) at child window status bar")]
        [ModelBrowsable(typeof(WinOnlyVisibilityCalculator))]
        bool ConnectionInfoChildMessage { get; set; }

    }
    public class ConnectionInfoStatusMessageController : WindowController, IModelExtender {
        protected override void OnActivated() {
            base.OnActivated();
            Frame.GetController<WindowTemplateController>(controller => {
                var modelOptions = ((IModelOptionsConnectionInfoStatusMessage)Application.Model.Options);
                if (modelOptions.ConnectionInfoMessage||modelOptions.ConnectionInfoChildMessage)
                    controller.CustomizeWindowStatusMessages += controller_CustomizeWindowStatusMessages;
            });
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions, IModelOptionsConnectionInfoStatusMessage>();
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            Frame.GetController<WindowTemplateController>(controller => {
                var modelOptions = ((IModelOptionsConnectionInfoStatusMessage)Application.Model.Options);
                if (modelOptions.ConnectionInfoMessage||modelOptions.ConnectionInfoChildMessage)
                    controller.CustomizeWindowStatusMessages -= controller_CustomizeWindowStatusMessages;
            });
        }

        void controller_CustomizeWindowStatusMessages(object sender, CustomizeWindowStatusMessagesEventArgs e) {
            var modelOptions = ((IModelOptionsConnectionInfoStatusMessage)Application.Model.Options);
            if (modelOptions.ConnectionInfoMessage && Frame.Context == TemplateContext.ApplicationWindow ||
                modelOptions.ConnectionInfoChildMessage && new[]{TemplateContext.View,TemplateContext.PopupWindow}.Contains(Frame.Context))
                if (((XPObjectSpace)Application.ObjectSpaceProvider.CreateUpdatingObjectSpace(false)).Session.Connection is DbConnection dbConnection)
                    e.StatusMessages.Add($"({dbConnection.DataSource} - {dbConnection.Database})");
        }
    }
}