using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.FileAttachments.Win;
using DevExpress.Utils;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.WorldCreator.Win {
    [ToolboxBitmap(typeof(WorldCreatorWinModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed class WorldCreatorWinModule : WorldCreatorModuleBase {
        
        public WorldCreatorWinModule() {
            RequiredModuleTypes.Add(typeof(WorldCreatorModule));
            RequiredModuleTypes.Add(typeof(Security.Win.XpandSecurityWinModule));
            RequiredModuleTypes.Add(typeof(FileAttachmentsWindowsFormsModule));
        }

        public override string GetPath() {
            return Application.GetStorageFolder(WCAssembliesPath);
        }
    }
}