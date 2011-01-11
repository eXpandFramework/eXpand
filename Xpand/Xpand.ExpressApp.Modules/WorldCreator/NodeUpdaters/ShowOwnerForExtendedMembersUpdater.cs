using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.WorldCreator.NodeUpdaters {
    public class ShowOwnerForExtendedMembersUpdater : ModelNodesGeneratorUpdater<ModelBOModelClassNodesGenerator> {
        public override void UpdateNode(ModelNode node) {
            IEnumerable<IModelColumn> columnInfoNodeWrappers =
                GetListViewInfoNodeWrappers(node.Application).Select(listViewInfoNodeWrapper => listViewInfoNodeWrapper.Columns["Owner"])
                    .Where(columnInfoNodeWrapper => columnInfoNodeWrapper != null);
            foreach (IModelColumn columnInfoNodeWrapper in columnInfoNodeWrappers) {
                columnInfoNodeWrapper.Index = 2;
            }
        }
        IEnumerable<IModelListView> GetListViewInfoNodeWrappers(IModelApplication application) {
            return
                application.Views.OfType<IModelListView>().Where(
                view => view.ModelClass.TypeInfo.Type == WCTypesInfo.Instance.FindBussinessObjectType<IExtendedReferenceMemberInfo>() ||
                view.ModelClass.TypeInfo.Type == WCTypesInfo.Instance.FindBussinessObjectType<IExtendedCollectionMemberInfo>() ||
                view.ModelClass.TypeInfo.Type == WCTypesInfo.Instance.FindBussinessObjectType<IExtendedCoreTypeMemberInfo>());
        }

    }
}