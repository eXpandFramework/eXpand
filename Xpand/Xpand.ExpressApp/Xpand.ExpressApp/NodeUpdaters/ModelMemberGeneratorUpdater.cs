using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.Xpo;
using Xpand.ExpressApp.Attributes;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.Model;

namespace Xpand.ExpressApp.NodeUpdaters {
    public class ModelMemberGeneratorUpdater : ModelNodesGeneratorUpdater<ModelBOModelClassNodesGenerator> {
        public override void UpdateNode(ModelNode node) {
            var modelBoModel = (IModelBOModel)node;
            foreach (var modelClass in modelBoModel) {
                IEnumerable<CustomQueryPropertyAttribute> customQueryPropertyAttributes =
                    LinqCollectionSourceHelper.GetQueryProperties(modelClass.TypeInfo.Type);
                foreach (var customQueryPropertyAttribute in customQueryPropertyAttributes) {
                    if (modelClass.TypeInfo.FindMember(customQueryPropertyAttribute.Name)==null) {
                        var memberInfo = modelClass.TypeInfo.CreateMember(customQueryPropertyAttribute.Name,customQueryPropertyAttribute.Type);
                        memberInfo.AddAttribute(new BrowsableAttribute(false));
                        memberInfo.AddAttribute(new NonPersistentAttribute());
                        var modelRuntimeMember = modelClass.OwnMembers.AddNode<IModelRuntimeNonPersistentMebmer>(customQueryPropertyAttribute.Name);
                        modelRuntimeMember.Type = customQueryPropertyAttribute.Type;
                    }
                }
            }

        }
    }
}