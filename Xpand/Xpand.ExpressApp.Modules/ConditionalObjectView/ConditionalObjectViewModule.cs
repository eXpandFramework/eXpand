using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Updating;
using DevExpress.Utils;
using Xpand.ExpressApp.ConditionalObjectView.Logic;
using Xpand.ExpressApp.ConditionalObjectView.Model;
using Xpand.ExpressApp.ConditionalObjectView.NodeUpdaters;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.Model;

namespace Xpand.ExpressApp.ConditionalObjectView {
    [ToolboxBitmap(typeof(ConditionalObjectViewModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class ConditionalObjectViewModule : LogicModuleBase<IConditionalObjectViewRule, ConditionalObjectViewRule>, IModelXmlConverter {
        public ConditionalObjectViewModule() {
            RequiredModuleTypes.Add(typeof(LogicModule));
        }
        #region IModelExtender Members
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelApplication, IModelApplicationConditionalObjectView>();
        }
        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new ConditionalObjectViewDefaultGroupContextNodeUpdater());
            updaters.Add(new ConditionalObjectViewRulesNodeUpdater());
            updaters.Add(new ConditionalObjectViewDefaultContextNodeUpdater());
        }
        #endregion

        protected override IModelLogic GetModelLogic(IModelApplication applicationModel) {
            return ((IModelApplicationConditionalObjectView)applicationModel).ConditionalObjectView;
        }

        public void ConvertXml(ConvertXmlParameters parameters) {
            if (parameters.XmlNodeName == "ConditionalDetailView") {
                parameters.NodeType = typeof(IModelApplicationConditionalObjectView);
            } else if (parameters.XmlNodeName == "ConditionalDetailViewRule") {
                parameters.NodeType = typeof(IConditionalObjectViewRule);
            }
        }
    }

}

