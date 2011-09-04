using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace Xpand.ExpressApp.WorldCreator.Win {
    [ToolboxBitmap(typeof(WorldCreatorWinModule))]
    [ToolboxItem(true)]
    public sealed partial class WorldCreatorWinModule : WorldCreatorModuleBase {
        public WorldCreatorWinModule() {
            InitializeComponent();
        }

        public override string GetPath() {
            return Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
        }
    }
}