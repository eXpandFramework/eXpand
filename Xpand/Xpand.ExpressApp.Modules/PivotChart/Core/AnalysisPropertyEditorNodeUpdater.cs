using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.PivotChart.Core {
    public abstract class AnalysisPropertyEditorNodeUpdater : ModelNodesGeneratorUpdater<ModelDetailViewItemsNodesGenerator>{
        public override void UpdateNode(ModelNode node) {
            IEnumerable<IModelPropertyEditor> modelPropertyEditors = ((IModelViewItems) node).OfType<IModelPropertyEditor>().Where(
                editor => typeof (IAnalysisInfo).IsAssignableFrom(editor.ModelMember.MemberInfo.MemberType));
            foreach (var modelPropertyEditor in modelPropertyEditors) {
                modelPropertyEditor.PropertyEditorType = GetPropertyEditorType();
            }
        }

        protected abstract Type GetPropertyEditorType();
    }
}