using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.ExpressApp.ModelAdaptor.Logic;
using Xpand.ExpressApp.ModelAdaptor.Model;
using Xpand.ExpressApp.ModelAdaptor.NodeUpdaters;

namespace Xpand.ExpressApp.ModelAdaptor {
    [ToolboxBitmap(typeof (ModelAdaptorModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class ModelAdaptorModule :
        LogicModuleBase
            <IModelAdaptorRule, ModelAdaptorRule, IModelModelAdaptorRule, IModelApplicationModelAdaptor,
            IModelModelAdaptorLogic> {
        readonly List<ExecutionContext> _executionContexts = new List<ExecutionContext>{ExecutionContext.ControllerActivated};

        public override List<ExecutionContext> ExecutionContexts {
            get { return _executionContexts; }
        }

        public override LogicRulesNodeUpdater<IModelAdaptorRule, IModelModelAdaptorRule, IModelApplicationModelAdaptor>
            LogicRulesNodeUpdater {
            get { return new ModelAdaptorRulesNodeUpdater(); }
        }

        public override IModelModelAdaptorLogic GetModelLogic(IModelApplicationModelAdaptor modelApplicationModelAdaptor) {
            return modelApplicationModelAdaptor.ModelAdaptor;
        }
    }
}