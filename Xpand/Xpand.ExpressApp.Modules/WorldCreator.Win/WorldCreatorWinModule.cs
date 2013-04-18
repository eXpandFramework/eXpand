using System.ComponentModel;
using System.Drawing;
using System.IO;
using DevExpress.ExpressApp.FileAttachments.Win;
using DevExpress.Utils;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.WorldCreator.Win {
    [ToolboxBitmap(typeof(WorldCreatorWinModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabModules)]
    public sealed class WorldCreatorWinModule : WorldCreatorModuleBase {
        public WorldCreatorWinModule() {
            RequiredModuleTypes.Add(typeof(WorldCreatorModule));
            RequiredModuleTypes.Add(typeof(FileAttachmentsWindowsFormsModule));
        }

        public override string GetPath() {
            return Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
        }
    }
}