using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.WorldCreator.NodeUpdaters {
    public class ShowOwnerForExtendedMembersUpdater : ModelNodesGeneratorUpdater<ModelBOModelClassNodesGenerator> {
        private readonly Type _extendedReferenceType;
        private readonly Type _extendedCollectionMemberType;
        private readonly Type _extendedCoreTypeMemberType;

        public ShowOwnerForExtendedMembersUpdater() {
            _extendedReferenceType = WCTypesInfo.Instance.FindBussinessObjectType<IExtendedReferenceMemberInfo>();
            _extendedCollectionMemberType = WCTypesInfo.Instance.FindBussinessObjectType<IExtendedCollectionMemberInfo>();
            _extendedCoreTypeMemberType = WCTypesInfo.Instance.FindBussinessObjectType<IExtendedCoreTypeMemberInfo>();
        }

        public override void UpdateNode(ModelNode node) {
            if (!InterfaceBuilder.RuntimeMode)
                return;
            var columnInfoNodeWrappers = GetListViewInfoNodeWrappers(node.Application).Select(listViewInfoNodeWrapper => listViewInfoNodeWrapper.Columns["Owner"])
                    .Where(columnInfoNodeWrapper => columnInfoNodeWrapper != null);
            foreach (IModelColumn columnInfoNodeWrapper in columnInfoNodeWrappers) {
                columnInfoNodeWrapper.Index = 2;
            }
        }
        IEnumerable<IModelListView> GetListViewInfoNodeWrappers(IModelApplication application) {
            return application.Views.OfType<IModelListView>().Where(
                view => view.ModelClass.TypeInfo.Type == _extendedReferenceType ||
                        view.ModelClass.TypeInfo.Type == _extendedCollectionMemberType ||
                        view.ModelClass.TypeInfo.Type == _extendedCoreTypeMemberType);
        }

    }
}