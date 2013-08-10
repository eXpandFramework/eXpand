using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Updating;
using DevExpress.Utils;
using Xpand.ExpressApp.ConditionalObjectView.Logic;
using Xpand.ExpressApp.ConditionalObjectView.Model;
using Xpand.ExpressApp.ConditionalObjectView.NodeUpdaters;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.NodeUpdaters;

namespace Xpand.ExpressApp.ConditionalObjectView {
    [ToolboxBitmap(typeof(ConditionalObjectViewModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class ConditionalObjectViewModule : LogicModuleBase<IConditionalObjectViewRule, ConditionalObjectViewRule, IModelConditionalObjectViewRule, IModelApplicationConditionalObjectView,IModelLogicConditionalObjectView>, IModelXmlConverter{
        public ConditionalObjectViewModule() {
            RequiredModuleTypes.Add(typeof(LogicModule));
        }
        #region IModelExtender Members

        public override List<ExecutionContext> ExecutionContexts {
            get { return new List<ExecutionContext>{ ExecutionContext.CustomProcessSelectedItem }; }
        }

        public override LogicRulesNodeUpdater<IConditionalObjectViewRule, IModelConditionalObjectViewRule, IModelApplicationConditionalObjectView> LogicRulesNodeUpdater {
            get { return new ConditionalObjectViewRulesNodeUpdater(); }
        }
        #endregion
        public override IModelLogicConditionalObjectView GetModelLogic(IModelApplicationConditionalObjectView modelApplicationConditionalObjectView) {
            return modelApplicationConditionalObjectView.ConditionalObjectView;
        }

        void IModelXmlConverter.ConvertXml(ConvertXmlParameters parameters) {
            ConvertXml(parameters);
            if (parameters.XmlNodeName == "ConditionalDetailView") {
                parameters.NodeType = typeof(IModelLogicConditionalObjectView);
            } else if (parameters.XmlNodeName == "ConditionalDetailViewRule") {
                parameters.NodeType = typeof(IModelConditionalObjectViewRule);
            }
        }
    }

}

