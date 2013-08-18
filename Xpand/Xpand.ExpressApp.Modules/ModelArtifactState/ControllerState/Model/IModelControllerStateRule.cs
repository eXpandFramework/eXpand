using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.ModelArtifactState.ArtifactState.Model;
using Xpand.ExpressApp.ModelArtifactState.ControllerState.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.ModelArtifactState.ControllerState.Model {
    [ModelInterfaceImplementor(typeof (IContextControllerStateRule), "Attribute")]
    public interface IModelControllerStateRule : IContextControllerStateRule, IModelConditionalLogicRule<IControllerStateRule>, IModelArtifactStateRule {
        [Browsable(false)]
        IEnumerable<Type> Controllers { get; }
    }
}