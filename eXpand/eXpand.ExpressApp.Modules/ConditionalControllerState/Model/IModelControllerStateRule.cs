using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.ArtifactState.Model;
using eXpand.ExpressApp.ConditionalControllerState.Logic;
using eXpand.ExpressApp.Logic.Conditional.Model;

namespace eXpand.ExpressApp.ConditionalControllerState.Model {
    [ModelInterfaceImplementor(typeof (IControllerStateRule), "Attribute")]
    public interface IModelControllerStateRule : IControllerStateRule, IModelConditionalLogicRule<IControllerStateRule>,IModelArtifactStateRule {
        [Browsable(false)]
        IEnumerable<Type> Controllers { get; }
    
    }
}