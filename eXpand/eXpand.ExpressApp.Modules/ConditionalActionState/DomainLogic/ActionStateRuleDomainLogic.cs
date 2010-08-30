using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.ConditionalActionState.Model;

namespace eXpand.ExpressApp.ConditionalActionState.DomainLogic
{
    [DomainLogic(typeof(IModelActionStateRule))]
    public static class ActionStateRuleDomainLogic{
        public static IEnumerable<string> Get_Actions(IModelActionStateRule modelActionStateRule) {
            return modelActionStateRule.Application.ActionDesign.Actions.Select(action => action.Id);
        }
    }
}
