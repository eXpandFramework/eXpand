using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.PivotChart {
    public abstract class PivotChartXpandModuleBase : ModuleBase {
        public override void UpdateModel(Dictionary model)
        {
            base.UpdateModel(model);
            IAnalysisInfo analysisInfo = null;
            string propertyName = analysisInfo.GetPropertyName(x=>x.Self);
            var analysisInfoNodeWrappers = new ApplicationNodeWrapper(model).BOModel.Classes.Where(wrapper => typeof(IAnalysisInfo).IsAssignableFrom(wrapper.ClassTypeInfo.Type)).SelectMany(nodeWrapper => nodeWrapper.Properties).Where(infoNodeWrapper => infoNodeWrapper.Name == propertyName);
            foreach (var propertyInfoNodeWrapper in analysisInfoNodeWrappers) {
                propertyInfoNodeWrapper.PropertyEditorType = GetPropertyEditorType().FullName;
            }
            
        }

        protected abstract Type GetPropertyEditorType();
    }
}