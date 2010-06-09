using System.Linq;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.Logic.Model;
using eXpand.ExpressApp.Logic.NodeUpdaters;

namespace eXpand.ExpressApp.Logic.DomainLogic {
    [DomainLogic(typeof (IModelGroupContexts))]
    public class ModelGroupContextsDefaultContextDomainLogic {
        bool gettingValue;

        public void BeforeGet(object logicRule, string propertyName) {
            if (propertyName == "DefaultContext") {
                if (!gettingValue) {
                    gettingValue = true;
                    var rule = ((IModelGroupContexts) logicRule);
                    if (rule.DefaultContext == null)
                        rule.DefaultContext =
                            rule.Where(context => context.Id == LogicDefaultGroupContextNodeUpdater.Default).Single();
                    gettingValue = false;
                }
            }
        }
    }
}