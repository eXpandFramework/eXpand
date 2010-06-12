using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp.PivotChart.Core {
    public abstract class AnalysisPropertyEditorNodeUpdater : ModelNodesGeneratorUpdater<ModelDetailViewItemsNodesGenerator>{
        public override void UpdateNode(ModelNode node) {
            IEnumerable<IModelPropertyEditor> modelPropertyEditors = ((IModelDetailView) node).Items.OfType<IModelPropertyEditor>().Where(
                editor => typeof (IAnalysisInfo).IsAssignableFrom(editor.ModelMember.ModelClass.TypeInfo.Type));
            foreach (var modelPropertyEditor in modelPropertyEditors) {
                modelPropertyEditor.PropertyEditorType = GetPropertyEditorType();
            }
            return ;
        }

        protected abstract Type GetPropertyEditorType();
    }
}