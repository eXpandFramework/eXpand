using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.TreeListEditors.Win;
using DevExpress.Utils;
using Xpand.ExpressApp.TreeListEditors.Win.Model;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;
using DevExpress.ExpressApp.Updating;
using Xpand.XAF.Modules.ModelMapper;
using Xpand.XAF.Modules.ModelMapper.Configuration;
using Xpand.XAF.Modules.ModelMapper.Services;

namespace Xpand.ExpressApp.TreeListEditors.Win {
    [Description(
        "Includes Property Editors and Controllers to DevExpress.ExpressApp.TreeListEditors.Win Module.Enables recursive filtering"
        ), EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(TreeListEditorsWindowsFormsModule), "Resources.Toolbox_Module_TreeListEditors_Win.ico")]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed class XpandTreeListEditorsWinModule : XpandModuleBase, IModelXmlConverter, IColumnCellFilterUser {
        public static string TreeListMapName = "TreeListOptions";
        public static string TreeListColumnMapName = "TreeListColumnOptions";
        public XpandTreeListEditorsWinModule() {
            RequiredModuleTypes.Add(typeof(ConditionalAppearanceModule));
            RequiredModuleTypes.Add(typeof(TreeListEditorsWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(XpandTreeListEditorsModule));
            RequiredModuleTypes.Add(typeof(ModelMapperModule));
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            moduleManager.Extend(PredefinedMap.TreeList,configuration => configuration.MapName=TreeListMapName);
            moduleManager.Extend(PredefinedMap.TreeListColumn,configuration => configuration.MapName=TreeListColumnMapName);
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelListView, IModelListViewTreeUseServerMode>();
        }

        void IModelXmlConverter.ConvertXml(ConvertXmlParameters parameters) {
            ConvertXml(parameters);
            if (typeof(IModelListView).IsAssignableFrom(parameters.NodeType) &&
                parameters.Values.ContainsKey("EditorTypeName")) {
                if (parameters.Values["EditorTypeName"] ==
                    "Xpand.ExpressApp.TreeListEditors.Win.XpandCategorizedListEditor")
                    parameters.Values["EditorTypeName"] =
                        "Xpand.ExpressApp.TreeListEditors.Win.ListEditor.XpandCategorizedListEditor";
            }
        }
    }
}