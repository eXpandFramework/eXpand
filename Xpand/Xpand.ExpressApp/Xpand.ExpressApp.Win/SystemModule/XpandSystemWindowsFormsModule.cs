using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Utils;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Win.Model;
using Xpand.ExpressApp.Win.Model.NodeUpdaters;
using Xpand.ExpressApp.Win.PropertyEditors;
using Xpand.ExpressApp.Win.PropertyEditors.RichEdit;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.Base.General.Model.Options;

namespace Xpand.ExpressApp.Win.SystemModule {
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    [Description("Overrides Controllers from the SystemModule and supplies additional basic Controllers that are specific for Windows Forms applications.")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(WinApplication), "Resources.Toolbox_Module_System_Win.ico")]
    public sealed class XpandSystemWindowsFormsModule : XpandModuleBase, IColumnCellFilterUser,IModelXmlConverter,IGridOptionsUser {
        public const string XpandWin = "Xpand.Win";
        public XpandSystemWindowsFormsModule() {
            RequiredModuleTypes.Add(typeof(XpandSystemModule));
            RequiredModuleTypes.Add(typeof(SystemWindowsFormsModule));
        }
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelRootNavigationItems, IModelRootNavigationItemsAutoSelectedGroupItem>();
            extenders.Add<IModelMemberViewItem, IModelMemberViewItemFastSearch>();
            extenders.Add<IModelMemberViewItem, IModelMemberViewItemRichEdit>();
            extenders.Add<IModelMemberViewItem, IModelMemberViewItemDuration>();
        }

        protected override IEnumerable<Type> GetDeclaredExportedTypes() {
            return new List<Type>();
        }

        public override void AddModelNodeUpdaters(IModelNodeUpdaterRegistrator updaterRegistrator){
            base.AddModelNodeUpdaters(updaterRegistrator);
            updaterRegistrator.AddUpdater(new ModelOptionsAdvBandedViewUpdater());
        }

        void IModelXmlConverter.ConvertXml(ConvertXmlParameters parameters) {
            ConvertXml(parameters);
            if (parameters.XmlNodeName == "GridColumnOptions")
                parameters.NodeType = typeof (IModelOptionsColumnGridView);
            
        }

    }


}