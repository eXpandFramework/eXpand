using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Validation;
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
    public sealed class XpandViewVariantsModule :XpandModuleBase, IModelXmlConverter {
        public const string XpandViewVariants = "eXpand.ViewVariants";
        public XpandViewVariantsModule() {
            RequiredModuleTypes.Add(typeof(ViewVariantsModule));
            RequiredModuleTypes.Add(typeof(ConditionalAppearanceModule));
            RequiredModuleTypes.Add(typeof(ValidationModule));
        }

        void IModelXmlConverter.ConvertXml(ConvertXmlParameters parameters) {
            ConvertXml(parameters);
            if (typeof(IModelListView).IsAssignableFrom(parameters.NodeType)&&parameters.ContainsKey("IsClonable")) {
                var value = parameters.Values["IsClonable"];
                parameters.Values.Remove("IsClonable");
                parameters.Values.Add(new KeyValuePair<string, string>("IsViewClonable",value));
            }
        }
    }
}