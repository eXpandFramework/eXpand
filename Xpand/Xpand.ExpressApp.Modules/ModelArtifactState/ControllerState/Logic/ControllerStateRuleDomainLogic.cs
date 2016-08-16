using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.ModelArtifactState.ControllerState.Model;
using Xpand.Persistent.Base.ModelArtifact;

namespace Xpand.ExpressApp.ModelArtifactState.ControllerState.Logic{
    [DomainLogic(typeof(IControllerStateRule))]
    public static class ControllerStateRuleDomainLogic {
        public static List<Type> Get_Controllers(IModelControllerStateRule controllerStateRule) {
            return controllerStateRule.Application.ActionDesign.Controllers.Select(controller =>{
                var typeInfo = XafTypesInfo.Instance.FindTypeInfo(controller.Name);
                if (typeInfo==null){
                    return AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).First(type => type.FullName==controller.Name);
                }
                return typeInfo.Type;
            }).ToList();
        }
    }
}