using System;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Model.Options;
using Xpand.ExpressApp.ModelAdaptor.Model;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelAdapter.Logic;

namespace Xpand.ExpressApp.Web.ListEditors.Model.ModelAfaptor {
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
