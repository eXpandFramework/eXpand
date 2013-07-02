using System;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.ModelAdaptor.Model;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.HtmlPropertyEditor.Web.Model.ModelAdaptor {
    [ModelInterfaceImplementor(typeof(IModelAdaptorHtmlEditorRule), "Attribute")]
    [ModelEditorLogicRule(typeof(IModelModelAdaptorLogic))]
    public interface IModelModelAdaptorHtmlEditorRule : IModelModelAdaptorRule, IModelHtmlEditor {
    }

    [DomainLogic(typeof(IModelModelAdaptorHtmlEditorRule))]
    public class ModelModelAdaptorHtmlEditorRuleDomainLogic {
        public static Type Get_RuleType(IModelModelAdaptorHtmlEditorRule modelListView) {
            return typeof(IModelAdaptorHtmlEditorRule);
        }
    }
}
