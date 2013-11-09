using System;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Model.Options;
using Xpand.Persistent.Base.ModelAdapter.Logic;

namespace Xpand.ExpressApp.Web.ListEditors.Model.ModelAdaptor {
    [ModelInterfaceImplementor(typeof(IModelAdaptorRule), "Attribute")]
    public interface IModelModelAdaptorGridViewOptionsRule : IModelModelAdaptorRule, IModelOptionsGridView {
    }

    [DomainLogic(typeof(IModelModelAdaptorGridViewOptionsRule))]
    public class ModelModelAdaptorGridViewOptionsRuleDomainLogic {
        public static Type Get_RuleType(IModelModelAdaptorGridViewOptionsRule modelListView) {
            return typeof(IModelAdaptorGridViewOptionsRule);
        }
    }
}
