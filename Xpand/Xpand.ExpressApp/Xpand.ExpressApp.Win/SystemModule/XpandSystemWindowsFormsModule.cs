using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win;
using DevExpress.Utils;
using Xpand.ExpressApp.Security;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.Model;
using Xpand.ExpressApp.Win.Model;
using Xpand.Persistent.Base.ModelAdapter;
using System.Linq;
using Guard = DevExpress.ExpressApp.Utils.Guard;

namespace Xpand.ExpressApp.Win.SystemModule {
    [ToolboxItem(true)]
    [ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
    [Description("Overrides Controllers from the SystemModule and supplies additional basic Controllers that are specific for Windows Forms applications.")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(WinApplication), "Resources.WinSystemModule.ico")]
    public sealed class XpandSystemWindowsFormsModule : XpandModuleBase {
        public const string XpandWin = "Xpand.Win";
        static HashSet<string> _modelValues = new HashSet<string> { ModelValueNames.Id, ModelValueNames.Index, ModelValueNames.IsNewNode, ModelValueNames.IsRemovedNode };
        public XpandSystemWindowsFormsModule() {
            RequiredModuleTypes.Add(typeof(XpandSystemModule));
        }
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelRootNavigationItems, IModelRootNavigationItemsAutoSelectedGroupItem>();
        }

        protected override IEnumerable<Type> GetDeclaredExportedTypes() {
            return new List<Type>();
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            if (Application != null)
                Application.LogonFailed += (o, eventArgs) => {
                    var logonParameters = SecuritySystem.LogonParameters as IXpandLogonParameters;
                    if (logonParameters != null && logonParameters.RememberMe) {
                        eventArgs.Handled = true;
                        logonParameters.RememberMe = false;
                        ((IXafApplication)Application).WriteLastLogonParameters(null, SecuritySystem.LogonParameters);
                    }

                };
        }

        public override void AddModelNodeUpdaters(IModelNodeUpdaterRegistrator updaterRegistrator) {
            base.AddModelNodeUpdaters(updaterRegistrator);
            //            updaterRegistrator.AddUpdater(this);
        }
        #region Implementation of IModelNodeUpdater<IModelGridViewOptions>
        //        void IModelNodeUpdater<IModelOptionsGridView>.UpdateNode(IModelOptionsGridView node, IModelApplication application) {
        //            //            UpdateNodes((ModelNode)node);
        //        }

        //        void UpdateNodes(ModelNode gridViewOptions) {
        //            var modelOptionsGridView = gridViewOptions.GetNodeByPath(gridViewOptions.Path.Replace("OptionsGridView", "GridViewOptions"));
        //            if (modelOptionsGridView != null) {
        //                var valueInfos = ModelValueInfos(modelOptionsGridView);
        //                var nodeByPath = modelOptionsGridView;
        //                foreach (var valueInfo in valueInfos) {
        //                    var propertyValue = modelOptionsGridView.GetValue(valueInfo.Name);
        //                    modelOptionsGridView.ClearValue(valueInfo.Name);
        //                    nodeByPath.SetValue(valueInfo.Name, propertyValue);
        //                }
        //                for (int i = 0; i < gridViewOptions.NodeCount; i++) {
        //                    var modelNode = gridViewOptions[i];
        //                    UpdateNodes(modelNode);
        //                }
        //            }
        //        }


        IEnumerable<ModelValueInfo> ModelValueInfos(IModelNode node) {
            return ((ModelNode)node).NodeInfo.ValuesInfo.Where(info => !_modelValues.Contains(info.Name) && node.HasValue(info.Name));
        }


        #endregion
    }


}