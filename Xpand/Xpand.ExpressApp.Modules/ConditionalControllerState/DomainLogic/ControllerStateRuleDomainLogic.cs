using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.ConditionalControllerState.Logic;
using Xpand.ExpressApp.ConditionalControllerState.Model;

namespace Xpand.ExpressApp.ConditionalControllerState.DomainLogic
{
    [DomainLogic(typeof(IControllerStateRule))]
    public static class ControllerStateRuleDomainLogic{
        public static List<Type>  Get_Controllers(IModelControllerStateRule controllerStateRule) {
            return controllerStateRule.Application.ActionDesign.Controllers.Select(controller => XafTypesInfo.Instance.FindTypeInfo(controller.Name).Type).ToList();
        }
    }
}
