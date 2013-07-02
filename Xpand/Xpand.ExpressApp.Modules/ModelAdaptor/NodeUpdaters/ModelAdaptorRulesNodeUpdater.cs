using System;
using System.Linq.Expressions;
using Xpand.ExpressApp.ModelAdaptor.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.Persistent.Base.ModelAdapter.Logic;

namespace Xpand.ExpressApp.ModelAdaptor.NodeUpdaters {
    public class ModelAdaptorRulesNodeUpdater :
        LogicRulesNodeUpdater<IModelAdaptorRule, IModelModelAdaptorRule, IModelApplicationModelAdaptor> {
        protected override void SetAttribute(IModelModelAdaptorRule rule, IModelAdaptorRule attribute) {
            rule.Attribute = attribute;
        }

        protected override Expression<Func<IModelApplicationModelAdaptor, object>> ExecuteExpression() {
            return controls => controls.ModelAdaptor;
        }
    }
}