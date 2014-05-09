using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.ModelAdaptor.Model;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ModelAdaptor {
    [ToolboxBitmap(typeof (ModelAdaptorModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    [Obsolete("Use Application.ModelAdapters node",true)]
    public sealed class ModelAdaptorModule :XpandModuleBase {
        public ModelAdaptorModule() {
            LogicInstallerManager.RegisterInstaller(new ModelAdaptorLogicInstaller(this));
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelApplication,IModelApplicationModelAdaptor>();
        }
    }
}