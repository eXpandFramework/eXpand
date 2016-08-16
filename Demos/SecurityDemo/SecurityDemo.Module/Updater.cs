using System;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Persistent.Base;

namespace SecurityDemo.Module
{
    public class Updater : ModuleUpdater
    {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }

		public override void UpdateDatabaseAfterUpdateSchema() {
			base.UpdateDatabaseAfterUpdateSchema();


            CreateClassLevelObjects();
            CreateMemberLevelObjects();
            CreateObjectLevelObjects();
            
            PermissionPolicyRole administratorRole = CreateAdministratorRole();

            PermissionPolicyUser userAdmin = ObjectSpace.FindObject<PermissionPolicyUser>(new BinaryOperator("UserName", "Sam"));
            if(userAdmin == null) {
                userAdmin = ObjectSpace.CreateObject<PermissionPolicyUser>();
                userAdmin.UserName = "Sam";
                userAdmin.IsActive = true;
                userAdmin.SetPassword("");
                userAdmin.Roles.Add(administratorRole);
            }

            PermissionPolicyRole securityDemoRole = CreateSecurityDemoRole();

            PermissionPolicyUser userJohn = ObjectSpace.FindObject<PermissionPolicyUser>(new BinaryOperator("UserName", "John"));
			if(userJohn == null) {
                userJohn = ObjectSpace.CreateObject<PermissionPolicyUser>();
				userJohn.UserName = "John";
                userJohn.IsActive = true;
                //userJohn.Roles.Add(defaultRole);
                userJohn.Roles.Add(securityDemoRole);
				userJohn.Save();
			}

            ObjectSpace.CommitChanges();
		}

        private void CreateClassLevelObjects() {
            FullAccessObject fullAccessObject = ObjectSpace.FindObject<FullAccessObject>(new BinaryOperator("Name", "Fully Accessible Object"));
            if(fullAccessObject == null) {
                fullAccessObject = ObjectSpace.CreateObject<FullAccessObject>();
                fullAccessObject.Name = "Fully Accessible Object";
                fullAccessObject.Save();
            }
            ProtectedContentObject protectedContentObject = ObjectSpace.FindObject<ProtectedContentObject>(new BinaryOperator("Name", "Protected Object"));
            if(protectedContentObject == null) {
                protectedContentObject = ObjectSpace.CreateObject<ProtectedContentObject>();
                protectedContentObject.Name = "Protected Object";
                protectedContentObject.Save();
            }
            ReadOnlyObject readOnlyObject = ObjectSpace.FindObject<ReadOnlyObject>(new BinaryOperator("Name", "Read-Only Object"));
            if(readOnlyObject == null) {
                readOnlyObject = ObjectSpace.CreateObject<ReadOnlyObject>();
                readOnlyObject.Name = "Read-Only Object";
                readOnlyObject.Save();
            }

            IrremovableObject irremovableObject = ObjectSpace.FindObject<IrremovableObject>(new BinaryOperator("Name", "Protected Deletion Object"));
            if(irremovableObject == null) {
                irremovableObject = ObjectSpace.CreateObject<IrremovableObject>();
                irremovableObject.Name = "Protected Deletion Object";
                irremovableObject.Save();
            }
            UncreatableObject uncreatableObject = ObjectSpace.FindObject<UncreatableObject>(new BinaryOperator("Name", "Protected Creation Object"));
            if(uncreatableObject == null) {
                uncreatableObject = ObjectSpace.CreateObject<UncreatableObject>();
                uncreatableObject.Name = "Protected Creation Object";
                uncreatableObject.Save();
            }
        }
        private void CreateMemberLevelObjects() {
            MemberLevelSecurityObject memberLevelSecurityObject = ObjectSpace.FindObject<MemberLevelSecurityObject>(new BinaryOperator("Name", "Member-Level Protected Object"));
            if(memberLevelSecurityObject == null) {
                memberLevelSecurityObject = ObjectSpace.CreateObject<MemberLevelSecurityObject>();
                memberLevelSecurityObject.Name = "Member-Level Protected Object";
                memberLevelSecurityObject.ProtectedContentProperty = "Secure Property Value";
                memberLevelSecurityObject.ReadWriteProperty = "Read Write Property Value";
                memberLevelSecurityObject.ReadOnlyProperty = "Read Only Property Value";
                MemberLevelReferencedObject1 obj1 = ObjectSpace.CreateObject<MemberLevelReferencedObject1>();
                obj1.Name = "Referenced Object Type 1";
                obj1.Save();
                memberLevelSecurityObject.ProtectedContentCollection.Add(obj1);
                MemberLevelReferencedObject2 obj2 = ObjectSpace.CreateObject<MemberLevelReferencedObject2>();
                obj2.Name = "Referenced Object Type 2";
                obj2.Save();
                memberLevelSecurityObject.ReadOnlyCollection.Add(obj2);
                memberLevelSecurityObject.Save();
            }

            MemberLevelReferencedObject1 refObject = ObjectSpace.FindObject<MemberLevelReferencedObject1>(new BinaryOperator("Name", "Object 1"));
            if(refObject == null) {
                refObject = ObjectSpace.CreateObject<MemberLevelReferencedObject1>();
                refObject.Name = "Object 1";
                ObjectSpace.CreateObject<MemberLevelReferencedObject1>().Name = "Object 2";
            }
            MemberByCriteriaSecurityObject fullAccessObjectObject = ObjectSpace.FindObject<MemberByCriteriaSecurityObject>(new BinaryOperator("Name", "Fully Accessible Property Object"));
            if(fullAccessObjectObject == null) {
                fullAccessObjectObject = ObjectSpace.CreateObject<MemberByCriteriaSecurityObject>();
                fullAccessObjectObject.Name = "Fully Accessible Property Object";
                fullAccessObjectObject.Property1 = "Full Access";
                fullAccessObjectObject.ReferenceProperty = refObject;
            }
            MemberByCriteriaSecurityObject readOnlyObjectObject = ObjectSpace.FindObject<MemberByCriteriaSecurityObject>(new BinaryOperator("Name", "Read-Only Property Object"));
            if(readOnlyObjectObject == null) {
                readOnlyObjectObject = ObjectSpace.CreateObject<MemberByCriteriaSecurityObject>();
                readOnlyObjectObject.Name = "Read-Only Property Object";
                readOnlyObjectObject.Property1 = "Read-Only";
                readOnlyObjectObject.ReferenceProperty = refObject;
            }
            MemberByCriteriaSecurityObject protectedContentObjectObject = ObjectSpace.FindObject<MemberByCriteriaSecurityObject>(new BinaryOperator("Name", "Protected Property Object"));
            if(protectedContentObjectObject == null) {
                protectedContentObjectObject = ObjectSpace.CreateObject<MemberByCriteriaSecurityObject>();
                protectedContentObjectObject.Name = "Protected Property Object";
                protectedContentObjectObject.Property1 = "protected";
                protectedContentObjectObject.ReferenceProperty = refObject;
            }
            MemberByCriteriaSecurityObject noReadAccessObject = ObjectSpace.FindObject<MemberByCriteriaSecurityObject>(new BinaryOperator("Name", "No Read Access Object"));
            if(noReadAccessObject == null) {
                noReadAccessObject = ObjectSpace.CreateObject<MemberByCriteriaSecurityObject>();
                noReadAccessObject.Name = "No Read Access Object";
                noReadAccessObject.Property1 = "No Read Access";
                noReadAccessObject.ReferenceProperty = refObject;
            }
        }
        private void CreateObjectLevelObjects() {
            ObjectLevelSecurityObject fullAccessObjectObject = ObjectSpace.FindObject<ObjectLevelSecurityObject>(new BinaryOperator("Name", "Fully Accessible Object"));
            if(fullAccessObjectObject == null) {
                fullAccessObjectObject = ObjectSpace.CreateObject<ObjectLevelSecurityObject>();
                fullAccessObjectObject.Name = "Fully Accessible Object";
                fullAccessObjectObject.Save();
            }
            ObjectLevelSecurityObject protectedContentObjectObject = ObjectSpace.FindObject<ObjectLevelSecurityObject>(new BinaryOperator("Name", "Protected Object"));
            if(protectedContentObjectObject == null) {
                protectedContentObjectObject = ObjectSpace.CreateObject<ObjectLevelSecurityObject>();
                protectedContentObjectObject.Name = "Protected Object";
                protectedContentObjectObject.Save();
            }
            ObjectLevelSecurityObject readOnlyObjectObject = ObjectSpace.FindObject<ObjectLevelSecurityObject>(new BinaryOperator("Name", "Read-Only Object"));
            if(readOnlyObjectObject == null) {
                readOnlyObjectObject = ObjectSpace.CreateObject<ObjectLevelSecurityObject>();
                readOnlyObjectObject.Name = "Read-Only Object";
                readOnlyObjectObject.Save();
            }
            ObjectLevelSecurityObject irremovableObjectObject = ObjectSpace.FindObject<ObjectLevelSecurityObject>(new BinaryOperator("Name", "Protected Deletion Object"));
            if(irremovableObjectObject == null) {
                irremovableObjectObject = ObjectSpace.CreateObject<ObjectLevelSecurityObject>();
                irremovableObjectObject.Name = "Protected Deletion Object";
                irremovableObjectObject.Save();
            }
        }
        private PermissionPolicyRole CreateSecurityDemoRole() {
            PermissionPolicyRole securityDemoRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "Demo"));
            if(securityDemoRole == null) {
                securityDemoRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                securityDemoRole.Name = "Demo";

                // System Permissions
                securityDemoRole.AddObjectPermission<PermissionPolicyUser>(SecurityOperations.ReadOnlyAccess, "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                securityDemoRole.AddMemberPermission<PermissionPolicyUser>(SecurityOperations.ReadWriteAccess, "ChangePasswordOnFirstLogon; StoredPassword", null, SecurityPermissionState.Allow);
                securityDemoRole.AddObjectPermission<PermissionPolicyRole>(SecurityOperations.ReadOnlyAccess, "[Name] = 'Demo'", SecurityPermissionState.Allow);
                securityDemoRole.AddTypePermission<PermissionPolicyTypePermissionObject>(SecurityOperations.Read, SecurityPermissionState.Allow);

                // Type Operation Permissions
                securityDemoRole.SetTypePermission<FullAccessObject>(SecurityOperations.FullAccess, SecurityPermissionState.Allow);
                securityDemoRole.SetTypePermission<ProtectedContentObject>(SecurityOperations.Navigate, SecurityPermissionState.Allow);
                securityDemoRole.SetTypePermission<ReadOnlyObject>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                securityDemoRole.SetTypePermission<IrremovableObject>(SecurityOperations.Navigate + ";" + SecurityOperations.Create + ";" + SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                securityDemoRole.SetTypePermission<UncreatableObject>(SecurityOperations.FullObjectAccess, SecurityPermissionState.Allow);
                
                // Member Operation Permissions
                securityDemoRole.SetTypePermission<MemberLevelSecurityObject>(SecurityOperations.Navigate + ";" + SecurityOperations.Create + ";" + SecurityOperations.Delete, SecurityPermissionState.Allow);
                securityDemoRole.AddMemberPermission<MemberLevelSecurityObject>(SecurityOperations.ReadWriteAccess, "ReadWriteProperty;Name;oid;Oid;OptimisticLockField", null,  SecurityPermissionState.Allow);
                securityDemoRole.AddMemberPermission<MemberLevelSecurityObject>(SecurityOperations.Read, "ReadOnlyProperty;ReadOnlyCollection", null, SecurityPermissionState.Allow);
                securityDemoRole.AddMemberPermission<MemberLevelSecurityObject>(SecurityOperations.Write, "ReadOnlyCollection;ReadOnlyProperty", null, SecurityPermissionState.Deny);
                securityDemoRole.AddMemberPermission<MemberLevelSecurityObject>(SecurityOperations.ReadWriteAccess, "ProtectedContentProperty;ProtectedContentCollection", null, SecurityPermissionState.Deny);
                
                securityDemoRole.SetTypePermission<MemberLevelReferencedObject1>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                securityDemoRole.SetTypePermission<MemberLevelReferencedObject2>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);                

                // Object Operation Permissions
                securityDemoRole.SetTypePermission<ObjectLevelSecurityObject>(SecurityOperations.Navigate, SecurityPermissionState.Allow);
                securityDemoRole.AddObjectPermission<ObjectLevelSecurityObject>( SecurityOperations.FullObjectAccess, "Contains([Name], 'Fully Accessible')", SecurityPermissionState.Allow);
                securityDemoRole.AddObjectPermission<ObjectLevelSecurityObject>(SecurityOperations.Read, "Contains([Name], 'Read-Only')",  SecurityPermissionState.Allow);
                securityDemoRole.AddObjectPermission<ObjectLevelSecurityObject>(SecurityOperations.ReadWriteAccess, "Contains([Name], 'Protected Deletion')",  SecurityPermissionState.Allow);

                // Member By Criteria Operation Permissions
                securityDemoRole.SetTypePermission<MemberByCriteriaSecurityObject>(SecurityOperations.Navigate, SecurityPermissionState.Allow);
                securityDemoRole.AddMemberPermission<MemberByCriteriaSecurityObject>(SecurityOperations.ReadWriteAccess, "Name",  "[Name] <> 'No Read Access Object'", SecurityPermissionState.Allow);
                securityDemoRole.AddMemberPermission<MemberByCriteriaSecurityObject>(SecurityOperations.ReadWriteAccess, "Property1;ReferenceProperty;Oid;oid",  "[Name] = 'Fully Accessible Property Object'", SecurityPermissionState.Allow);
                securityDemoRole.AddMemberPermission<MemberByCriteriaSecurityObject>(SecurityOperations.Read, "Property1;ReferenceProperty;Oid;oid",  "[Name] = 'Read-Only Property Object'", SecurityPermissionState.Allow);
            }
            return securityDemoRole;
        }
        private PermissionPolicyRole CreateAdministratorRole() {
            PermissionPolicyRole administratorRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "Administrator"));
            if(administratorRole == null) {
                administratorRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                administratorRole.Name = "Administrator";
                administratorRole.IsAdministrative = true;
            }
            return administratorRole;
        }
    }
}
