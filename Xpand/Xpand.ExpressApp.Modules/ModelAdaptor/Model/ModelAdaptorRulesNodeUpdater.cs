using System;
using System.Linq.Expressions;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.Persistent.Base.Logic.Model;
using Xpand.Persistent.Base.ModelAdapter.Logic;

namespace Xpand.ExpressApp.ModelAdaptor.Model {
    public class ModelAdaptorRulesNodeUpdater :LogicRulesNodeUpdater<IModelAdaptorRule, IModelModelAdaptorRule> {
        protected override void SetAttribute(IModelModelAdaptorRule rule, IModelAdaptorRule attribute) {
            rule.Attribute = attribute;
        }

        protected override Expression<Func<IModelApplication, IModelLogic>> ExecuteExpression() {
            return application => ((IModelApplicationModelAdaptor) application).ModelAdaptor;
        }
    }
}