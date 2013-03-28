using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.FilterDataStore.Model;
using System.Linq;

namespace Xpand.ExpressApp.FilterDataStore.NodeGenerators {
    public class ModelSystemTablesNodesGenerator : ModelNodesGeneratorBase {
        public static HashSet<string> SystemTables = new HashSet<string>{
                "SecuritySystemTypePermissionsObject",
                "SecuritySystemTypePermissionsObjectBase",
                "SecuritySystemRoleBase",
                "DCRuntimeCalcMember",
                "SecurityRole",
                "SecurityUser",
                "PropertyBag",
                "PropertyBagDescriptor",
                "PropertyDescriptor",
                "PropertyDescriptorPropertyDescriptors_PropertyBagDescriptorPropertyBags",
                "PropertyValue",
                "ServerPrefix",
                "XpoSequencer",
                "XpoServerId",
                "AuditDataItemPersistent",
                "AuditedObjectWeakReference",
                "XPWeakReference",
                "ModuleInfo",
                "User",
                "SimpleUser",
                "Party",
                "Person",
                "Role",
                "RoleBase",
                "PersistentPermission",
                "UserUsers_RoleRoles",
                "XPObjectType",
                "ModelDifferenceObject",
                "AspectObject",
                "PersistentApplication",
                "RoleModelDifferenceObject",
                "UserModelDifferenceObject",
                "RoleRoles_RoleModelDifferenceObjectRoleModelDifferenceObjects",
                "UserUsers_UserModelDifferenceObjectUserModelDifferenceObjects",
                "SecuritySystemRole",
                "SecuritySystemRoleParentRoles_SecuritySystemRoleChildRoles",
                "SecuritySystemTypePermissionsObject",
                "SecuritySystemUser",
                "SecuritySystemUserUsers_SecuritySystemRoleRoles",
                "XpandPermissionData",
                "XpandRole",
                "ActionStateOperationPermissionData",
                "AdditionalViewControlsOperationPermissionData",
                "ControllerStateOperationPermissionData",
                "MasterDetailOperationPermissionData",
                "ModelCombineOperationPermissionData",
                "PivotSettingsOperationPermissionData",
                "ShowInAnalysisOperationPermissionData",
                "StateMachineTransitionOperationPermissionData"
            };

        protected override void GenerateNodesCore(ModelNode node) {

            foreach (var modelClass in node.Application.BOModel.Where(@class => @class.TypeInfo.IsInterface)) {
                node.AddNode<IModelFilterDataStoreSystemTable>(modelClass.Name);
            }
            node.AddNode<IModelFilterDataStoreSystemTable>("DCRuntimeCalcMember");
            node.AddNode<IModelFilterDataStoreSystemTable>("SecurityRole");
            node.AddNode<IModelFilterDataStoreSystemTable>("SecurityUser");
            node.AddNode<IModelFilterDataStoreSystemTable>("PropertyBag");
            node.AddNode<IModelFilterDataStoreSystemTable>("PropertyBagDescriptor");
            node.AddNode<IModelFilterDataStoreSystemTable>("PropertyDescriptor");
            node.AddNode<IModelFilterDataStoreSystemTable>("PropertyDescriptorPropertyDescriptors_PropertyBagDescriptorPropertyBags");
            node.AddNode<IModelFilterDataStoreSystemTable>("PropertyValue");
            node.AddNode<IModelFilterDataStoreSystemTable>("ServerPrefix");
            node.AddNode<IModelFilterDataStoreSystemTable>("XpoSequencer");
            node.AddNode<IModelFilterDataStoreSystemTable>("XpoServerId");
            node.AddNode<IModelFilterDataStoreSystemTable>("AuditDataItemPersistent");
            node.AddNode<IModelFilterDataStoreSystemTable>("AuditedObjectWeakReference");
            node.AddNode<IModelFilterDataStoreSystemTable>("XPWeakReference");
            node.AddNode<IModelFilterDataStoreSystemTable>("ModuleInfo");
            node.AddNode<IModelFilterDataStoreSystemTable>("User");
            node.AddNode<IModelFilterDataStoreSystemTable>("SimpleUser");
            node.AddNode<IModelFilterDataStoreSystemTable>("Party");
            node.AddNode<IModelFilterDataStoreSystemTable>("Person");
            node.AddNode<IModelFilterDataStoreSystemTable>("Role");
            node.AddNode<IModelFilterDataStoreSystemTable>("RoleBase");
            node.AddNode<IModelFilterDataStoreSystemTable>("PersistentPermission");
            node.AddNode<IModelFilterDataStoreSystemTable>("UserUsers_RoleRoles");
            node.AddNode<IModelFilterDataStoreSystemTable>("XPObjectType");
            node.AddNode<IModelFilterDataStoreSystemTable>("ModelDifferenceObject");
            node.AddNode<IModelFilterDataStoreSystemTable>("AspectObject");
            node.AddNode<IModelFilterDataStoreSystemTable>("PersistentApplication");
            node.AddNode<IModelFilterDataStoreSystemTable>("RoleModelDifferenceObject");
            node.AddNode<IModelFilterDataStoreSystemTable>("UserModelDifferenceObject");
            node.AddNode<IModelFilterDataStoreSystemTable>("RoleRoles_RoleModelDifferenceObjectRoleModelDifferenceObjects");
            node.AddNode<IModelFilterDataStoreSystemTable>("UserUsers_UserModelDifferenceObjectUserModelDifferenceObjects");
            node.AddNode<IModelFilterDataStoreSystemTable>("SecuritySystemRole");
            node.AddNode<IModelFilterDataStoreSystemTable>("SecuritySystemRoleParentRoles_SecuritySystemRoleChildRoles");
            node.AddNode<IModelFilterDataStoreSystemTable>("SecuritySystemTypePermissionsObject");
            node.AddNode<IModelFilterDataStoreSystemTable>("SecuritySystemUser");
            node.AddNode<IModelFilterDataStoreSystemTable>("SecuritySystemUserUsers_SecuritySystemRoleRoles");
            node.AddNode<IModelFilterDataStoreSystemTable>("XpandPermissionData");
            node.AddNode<IModelFilterDataStoreSystemTable>("XpandRole");
            node.AddNode<IModelFilterDataStoreSystemTable>("ActionStateOperationPermissionData");
            node.AddNode<IModelFilterDataStoreSystemTable>("AdditionalViewControlsOperationPermissionData");
            node.AddNode<IModelFilterDataStoreSystemTable>("ControllerStateOperationPermissionData");
            node.AddNode<IModelFilterDataStoreSystemTable>("MasterDetailOperationPermissionData");
            node.AddNode<IModelFilterDataStoreSystemTable>("ModelCombineOperationPermissionData");
            node.AddNode<IModelFilterDataStoreSystemTable>("PivotSettingsOperationPermissionData");
            node.AddNode<IModelFilterDataStoreSystemTable>("ShowInAnalysisOperationPermissionData");
            node.AddNode<IModelFilterDataStoreSystemTable>("StateMachineTransitionOperationPermissionData");
        }
    }
}