using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp.PivotChart {
    public abstract class PivotChartXpandModuleBase : ModuleBase {
        public override void UpdateModel(Dictionary model)
        {
            base.UpdateModel(model);
            var analysisInfoNodeWrappers = new ApplicationNodeWrapper(model).BOModel.Classes.Where(wrapper => typeof(IAnalysisInfo).IsAssignableFrom(wrapper.ClassTypeInfo.Type)).SelectMany(nodeWrapper => nodeWrapper.Properties).Where(infoNodeWrapper => infoNodeWrapper.PropertyEditorType==typeof(IAnalysisInfo).FullName);
            foreach (var propertyInfoNodeWrapper in analysisInfoNodeWrappers) {
                propertyInfoNodeWrapper.PropertyEditorType = GetPropertyEditorType().FullName;
            }
            
        }

        protected abstract Type GetPropertyEditorType();
    }
}