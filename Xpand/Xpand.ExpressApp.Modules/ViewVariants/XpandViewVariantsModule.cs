using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.ViewVariantsModule;
using DevExpress.Utils;
using Xpand.Persistent.Base.General;
using EditorBrowsableState = System.ComponentModel.EditorBrowsableState;

namespace Xpand.ExpressApp.ViewVariants {
    [Description(
        "Includes Property Editors and Controllers to DevExpress.ExpressApp.ViewVariants Module. Enables View Cloning"),
     EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(ViewVariantsModule), "Resources.Toolbox_Module_ViewVariants.ico")]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class XpandViewVariantsModule : XpandModuleBase {
        public const string XpandViewVariants = "eXpand.ViewVariants";
        public XpandViewVariantsModule() {
            RequiredModuleTypes.Add(typeof(ViewVariantsModule));
            RequiredModuleTypes.Add(typeof(ConditionalAppearanceModule));
        }
    }
}