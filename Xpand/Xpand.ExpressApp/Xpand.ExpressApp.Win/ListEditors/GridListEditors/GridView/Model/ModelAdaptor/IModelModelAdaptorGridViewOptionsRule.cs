using System;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Model.Options;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelAdapter.Logic;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.Model.ModelAdaptor {
    [ModelInterfaceImplementor(typeof(IModelAdaptorGridViewOptionsRule), "Attribute")]
    [ModelEditorLogicRule(typeof(IModelModelAdaptorLogic))]
    public interface IModelModelAdaptorGridViewOptionsRule : IModelModelAdaptorRule, IModelOptionsGridView {
    }

    [DomainLogic(typeof(IModelModelAdaptorGridViewOptionsRule))]
    public class ModelModelAdaptorGridViewOptionsRuleDomainLogic {
        public static Type Get_RuleType(IModelModelAdaptorGridViewOptionsRule modelListView) {
            return typeof(IModelAdaptorGridViewOptionsRule);
        }
    }

}
