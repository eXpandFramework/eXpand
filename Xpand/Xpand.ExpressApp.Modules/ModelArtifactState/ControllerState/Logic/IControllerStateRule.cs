using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.ModelArtifactState.ArtifactState.Logic;
using Xpand.ExpressApp.ModelArtifactState.ControllerState.Model;

namespace Xpand.ExpressApp.ModelArtifactState.ControllerState.Logic {
    public interface IControllerStateRule : IArtifactStateRule {
        [Category("Data")]
        [Required(typeof(ControllerStateRuleControllerTypeRequiredCalculator))]
        [DataSourceProperty("Controllers")]
        [TypeConverter(typeof(StringToTypeConverter))]
        Type ControllerType { get; set; }

        [Category("Behavior")]
        [ModelPersistentName("State")]
        ControllerState ControllerState { get; set; }
    }

    public class ControllerStateRuleControllerTypeRequiredCalculator:IModelIsRequired{
        public bool IsRequired(IModelNode node, string propertyName){
            return string.IsNullOrEmpty(((IControllerStateRule) node).Module);
        }
    }

    [DomainLogic(typeof(IControllerStateRule))]
    public static class ControllerStateRuleDomainLogic {
        public static List<Type> Get_Controllers(IModelControllerStateRule controllerStateRule) {
            return controllerStateRule.Application.ActionDesign.Controllers.Select(controller => XafTypesInfo.Instance.FindTypeInfo(controller.Name).Type).ToList();
        }
    }

}