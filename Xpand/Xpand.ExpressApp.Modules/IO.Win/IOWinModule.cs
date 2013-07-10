using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.FileAttachments.Win;
using DevExpress.ExpressApp.TreeListEditors.Win;
using DevExpress.Utils;
using Xpand.ExpressApp.TreeListEditors.Win;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.IO.Win {
    [ToolboxBitmap(typeof(IOWinModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed class IOWinModule : XpandModuleBase {
        public IOWinModule() {
            RequiredModuleTypes.Add(typeof(IOModule));
            RequiredModuleTypes.Add(typeof(TreeListEditorsWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(XpandTreeListEditorsWinModule));
            RequiredModuleTypes.Add(typeof(FileAttachmentsWindowsFormsModule));
        }
        #region Overrides of XpandModuleBase
        protected override Type[] ApplicationTypes() {
            return new[]{typeof(IConfirmationRequired)};
        }
        #endregion
    }
}