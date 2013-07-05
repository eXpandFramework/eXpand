using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.ArtifactState.Model;
using Xpand.ExpressApp.ConditionalControllerState.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.ConditionalControllerState.Model {
    [ModelInterfaceImplementor(typeof (IControllerStateRule), "Attribute")]
    public interface IModelControllerStateRule : IControllerStateRule, IModelConditionalLogicRule<IControllerStateRule>,IModelArtifactStateRule {
        [Browsable(false)]
        IEnumerable<Type> Controllers { get; }
    
    }
}