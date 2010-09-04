using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.CloneObject;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using eXpand.ExpressApp.WorldCreator.Core;

namespace eXpand.ExpressApp.WorldCreator.NodeUpdaters {
    public class EnableCloningForAllWCPersistentTypesUpdater : ModelNodesGeneratorUpdater<ModelBOModelClassNodesGenerator> {
        public override void UpdateNode(ModelNode node) {
            var modelBoModel = ((IModelBOModel) node);
            IEnumerable<Type> types = typeof (ITypesInfo).GetProperties().Select(propertyInfo => (Type) propertyInfo.GetValue(TypesInfo.Instance, null));
            foreach (var type in types) {
                ((IModelClassClonable) modelBoModel[type.FullName]).IsClonable = true;
            }
        }
    }
}