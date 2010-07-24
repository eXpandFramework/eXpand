using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.FilterDataStore.Model;

namespace eXpand.ExpressApp.FilterDataStore.NodeGenerators {
    public class ModelSystemTablesNodesGenerator:ModelNodesGeneratorBase {
        protected override void GenerateNodesCore(ModelNode node) {
            node.AddNode<IModelFilterDataStoreSystemTable>("AuditDataItemPersistent");
            node.AddNode<IModelFilterDataStoreSystemTable>("AuditedObjectWeakReference");
            node.AddNode<IModelFilterDataStoreSystemTable>("XPWeakReference");
            node.AddNode<IModelFilterDataStoreSystemTable>("ModuleInfo");
            node.AddNode<IModelFilterDataStoreSystemTable>("User");
            node.AddNode<IModelFilterDataStoreSystemTable>("Party");
            node.AddNode<IModelFilterDataStoreSystemTable>("Person");
            node.AddNode<IModelFilterDataStoreSystemTable>("Role");
            node.AddNode<IModelFilterDataStoreSystemTable>("RoleBase");
            node.AddNode<IModelFilterDataStoreSystemTable>("PersistentPermission");
            node.AddNode<IModelFilterDataStoreSystemTable>("UserUsers_RoleRoles");
            node.AddNode<IModelFilterDataStoreSystemTable>("XPObjectType");
            node.AddNode<IModelFilterDataStoreSystemTable>("ModelDifferenceObject");
            node.AddNode<IModelFilterDataStoreSystemTable>("PersistentApplication");
            node.AddNode<IModelFilterDataStoreSystemTable>("RoleModelDifferenceObject");
            node.AddNode<IModelFilterDataStoreSystemTable>("UserModelDifferenceObject");
            node.AddNode<IModelFilterDataStoreSystemTable>("RoleRoles_RoleModelDifferenceObjectRoleModelDifferenceObjects");
            node.AddNode<IModelFilterDataStoreSystemTable>("UserUsers_UserModelDifferenceObjectUserModelDifferenceObjects");
        }
    }
}