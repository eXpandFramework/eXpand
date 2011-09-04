using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using Xpand.ExpressApp;

namespace Xpand.ExpressApp.IO.Win {
    [ToolboxBitmap(typeof(IOWinModule))]
    [ToolboxItem(true)]
    public sealed partial class IOWinModule : XpandModuleBase {

        public IOWinModule() {
            InitializeComponent();
        }


    }
}