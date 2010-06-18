using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Xpo;
using eXpand.ExpressApp.MemberLevelSecurity.Win.PropertyEditors;

namespace eXpand.ExpressApp.MemberLevelSecurity.Win.NodeUpdaters {
    public class MemberLevelSecurityStringPropertyEditorUpdater : ModelNodesGeneratorUpdater<ModelBOModelClassNodesGenerator>
    {
        public override void UpdateNode(ModelNode node) {
            var modelBoModel = ((IModelBOModel)node);
            var modelClasses = modelBoModel.Where(cls => typeof(XPBaseObject).IsAssignableFrom(cls.TypeInfo.Type));
            foreach (IModelMember modelMember in GetModelMembers(modelClasses)) {
                modelMember.PropertyEditorType = typeof(MemberLevelSecurityStringPropertyEditor);
            }
        }

        IEnumerable<IModelMember> GetModelMembers(IEnumerable<IModelClass> modelClasses) {
            return modelClasses.SelectMany(modelClass => (from property in modelClass.AllMembers
                                                          let isSimpleStringEdit = (property.Type != typeof (string) || (property.RowCount == 0))
                                                          let isComboStringEdit = (isSimpleStringEdit && !string.IsNullOrEmpty(property.PredefinedValues))
                                                          where property.PropertyEditorType == typeof (StringPropertyEditor) && isSimpleStringEdit && !isComboStringEdit
                                                          select property));
        }
    }
}