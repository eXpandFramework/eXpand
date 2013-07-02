using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Model {
    [ModelInterfaceImplementor(typeof(IAdditionalViewControlsRule), "Attribute")]
    [ModelEditorLogicRule(typeof(IModelLogicAdditionalViewControls))]
    public interface IModelAdditionalViewControlsRule : IAdditionalViewControlsRule, IModelConditionalLogicRule<IAdditionalViewControlsRule>{
        [Browsable(false)]
        IEnumerable<Type> ControlTypes { get; }
        [Browsable(false)]
        IEnumerable<Type> DecoratorTypes { get; }        
    }
}