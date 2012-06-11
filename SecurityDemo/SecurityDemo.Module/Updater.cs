using System;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Updating;

namespace SecurityDemo.Module
{
    public class Updater : ModuleUpdater
    {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }

		public override void UpdateDatabaseAfterUpdateSchema() {
			base.UpdateDatabaseAfterUpdateSchema();

            CreateSecurityDemoObjects();

            DevExpress.ExpressApp.Security.Strategy.SecuritySystemRole defaultRole = CreateDefaultRole();
            DevExpress.ExpressApp.Security.Strategy.SecuritySystemRole administratorRole = CreateAdministratorRole();

            SecurityDemoUser userAdmin = ObjectSpace.FindObject<SecurityDemoUser>(new BinaryOperator("UserName", "Sam"));
            if(userAdmin == null) {
                userAdmin = ObjectSpace.CreateObject<SecurityDemoUser>();
                userAdmin.UserName = "Sam";
                userAdmin.IsActive = true;
                userAdmin.SetPassword("");
                userAdmin.Roles.Add(administratorRole);
            }

            DevExpress.ExpressApp.Security.Strategy.SecuritySystemRole securityDemoRole = CreateSecurityDemoRole();

            SecurityDemoUser userJohn = ObjectSpace.FindObject<SecurityDemoUser>(new BinaryOperator("UserName", "John"));
			if(userJohn == null) {
                userJohn = ObjectSpace.CreateObject<SecurityDemoUser>();
				userJohn.UserName = "John";
                userJohn.IsActive = true;
                userJohn.Roles.Add(defaultRole);
                userJohn.Roles.Add(securityDemoRole);
				userJohn.Save();
			}

            ObjectSpace.CommitChanges();
		}

        private void CreateSecurityDemoObjects() {
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

        private DevExpress.ExpressApp.Security.Strategy.SecuritySystemRole CreateSecurityDemoRole() {
            DevExpress.ExpressApp.Security.Strategy.SecuritySystemRole securityDemoRole = ObjectSpace.FindObject<DevExpress.ExpressApp.Security.Strategy.SecuritySystemRole>(new BinaryOperator("Name", "Demo"));
            if(securityDemoRole == null) {
                securityDemoRole = ObjectSpace.CreateObject<DevExpress.ExpressApp.Security.Strategy.SecuritySystemRole>();
                securityDemoRole.Name = "Demo";

                // Type Operation Permissions
                SecuritySystemTypePermissionObject fullAccessPermission = ObjectSpace.CreateObject<SecuritySystemTypePermissionObject>();
                fullAccessPermission.TargetType = typeof(FullAccessObject);
                fullAccessPermission.AllowCreate = true;
                fullAccessPermission.AllowDelete = true;
                fullAccessPermission.AllowNavigate = true;
                fullAccessPermission.AllowRead = true;
                fullAccessPermission.AllowWrite = true;
                fullAccessPermission.Save();
                securityDemoRole.TypePermissions.Add(fullAccessPermission);
                SecuritySystemTypePermissionObject protectedContentPermission = ObjectSpace.CreateObject<SecuritySystemTypePermissionObject>();
                protectedContentPermission.TargetType = typeof(ProtectedContentObject);
                protectedContentPermission.AllowNavigate = true;
                protectedContentPermission.Save();
                securityDemoRole.TypePermissions.Add(protectedContentPermission);
                SecuritySystemTypePermissionObject readOnlyPermission = ObjectSpace.CreateObject<SecuritySystemTypePermissionObject>();
                readOnlyPermission.TargetType = typeof(ReadOnlyObject);
                readOnlyPermission.AllowNavigate = true;
                readOnlyPermission.AllowRead = true;
                readOnlyPermission.Save();
                securityDemoRole.TypePermissions.Add(readOnlyPermission);



                SecuritySystemTypePermissionObject irremovablePermission = ObjectSpace.CreateObject<SecuritySystemTypePermissionObject>();
                irremovablePermission.TargetType = typeof(IrremovableObject);
                irremovablePermission.AllowCreate = true;
                irremovablePermission.AllowNavigate = true;
                irremovablePermission.AllowRead = true;
                irremovablePermission.AllowWrite = true;
                irremovablePermission.Save();
                securityDemoRole.TypePermissions.Add(irremovablePermission);
                SecuritySystemTypePermissionObject uncreatablePermission = ObjectSpace.CreateObject<SecuritySystemTypePermissionObject>();
                uncreatablePermission.TargetType = typeof(UncreatableObject);
                uncreatablePermission.AllowDelete = true;
                uncreatablePermission.AllowNavigate = true;
                uncreatablePermission.AllowRead = true;
                uncreatablePermission.AllowWrite = true;
                uncreatablePermission.Save();
                securityDemoRole.TypePermissions.Add(uncreatablePermission);

                // Member Operation Permissions
                SecuritySystemTypePermissionObject navigateMemberLevelOperationObjectPermission = ObjectSpace.CreateObject<SecuritySystemTypePermissionObject>();
                navigateMemberLevelOperationObjectPermission.TargetType = typeof(MemberLevelSecurityObject);
                navigateMemberLevelOperationObjectPermission.AllowCreate = true;
                navigateMemberLevelOperationObjectPermission.AllowDelete = true;
                navigateMemberLevelOperationObjectPermission.AllowNavigate = true;
                navigateMemberLevelOperationObjectPermission.Save();
                securityDemoRole.TypePermissions.Add(navigateMemberLevelOperationObjectPermission);
                
                SecuritySystemMemberPermissionsObject readWriteMemberPermission = ObjectSpace.CreateObject<SecuritySystemMemberPermissionsObject>();
                //readWriteMemberPermission.TargetType = typeof(MemberLevelSecurityObject);
                readWriteMemberPermission.Members = "ReadWriteProperty; Name; oid; Oid; OptimisticLockField"; // TODO - Slava D - service fields - XPO responsibility
                readWriteMemberPermission.AllowRead = true;
                readWriteMemberPermission.AllowWrite = true;
                readWriteMemberPermission.Save();
                navigateMemberLevelOperationObjectPermission.MemberPermissions.Add(readWriteMemberPermission);
                //securityDemoRole.TypePermissions.Add(readWriteMemberPermission);
                
                SecuritySystemMemberPermissionsObject protectedContentMemberPermission = ObjectSpace.CreateObject<SecuritySystemMemberPermissionsObject>();
                //protectedContentMemberPermission.TargetType = typeof(MemberLevelSecurityObject);
                protectedContentMemberPermission.Members = "ProtectedContentProperty; ProtectedContentCollection";
                protectedContentMemberPermission.Save();
                navigateMemberLevelOperationObjectPermission.MemberPermissions.Add(protectedContentMemberPermission);
                //securityDemoRole.TypePermissions.Add(protectedContentMemberPermission);

                SecuritySystemMemberPermissionsObject readOnlyMemberPermission = ObjectSpace.CreateObject<SecuritySystemMemberPermissionsObject>();
                //readOnlyMemberPermission.TargetType = typeof(MemberLevelSecurityObject);
                readOnlyMemberPermission.Members = "ReadOnlyProperty; ReadOnlyCollection";
                readOnlyMemberPermission.AllowRead = true;
                readOnlyMemberPermission.Save();
                navigateMemberLevelOperationObjectPermission.MemberPermissions.Add(readOnlyMemberPermission);
                //securityDemoRole.TypePermissions.Add(readOnlyMemberPermission);

                SecuritySystemTypePermissionObject memberLevelReferencedObject1Permission = ObjectSpace.CreateObject<SecuritySystemTypePermissionObject>();
                memberLevelReferencedObject1Permission.TargetType = typeof(MemberLevelReferencedObject1);
                memberLevelReferencedObject1Permission.AllowRead = true;
                memberLevelReferencedObject1Permission.AllowWrite = true;
                memberLevelReferencedObject1Permission.AllowCreate = true;
                memberLevelReferencedObject1Permission.AllowDelete = true;
                memberLevelReferencedObject1Permission.Save();
                securityDemoRole.TypePermissions.Add(memberLevelReferencedObject1Permission);

                SecuritySystemTypePermissionObject memberLevelReferencedObject2Permission = ObjectSpace.CreateObject<SecuritySystemTypePermissionObject>();
                memberLevelReferencedObject2Permission.TargetType = typeof(MemberLevelReferencedObject2);
                memberLevelReferencedObject2Permission.AllowRead = true;
                memberLevelReferencedObject2Permission.AllowWrite = true;
                memberLevelReferencedObject2Permission.AllowCreate = true;
                memberLevelReferencedObject2Permission.AllowDelete = true;
                memberLevelReferencedObject2Permission.Save();
                securityDemoRole.TypePermissions.Add(memberLevelReferencedObject2Permission);

                

                // Object Operation Permissions
                SecuritySystemTypePermissionObject navigateObjectLevelSecurityObjectPermission = ObjectSpace.CreateObject<SecuritySystemTypePermissionObject>();
                navigateObjectLevelSecurityObjectPermission.TargetType = typeof(ObjectLevelSecurityObject);
                navigateObjectLevelSecurityObjectPermission.AllowNavigate = true;
                navigateObjectLevelSecurityObjectPermission.Save();
                securityDemoRole.TypePermissions.Add(navigateObjectLevelSecurityObjectPermission);
                
                SecuritySystemObjectPermissionsObject fullAccessObjectPermission = ObjectSpace.CreateObject<SecuritySystemObjectPermissionsObject>();
                //fullAccessObjectPermission.TargetType = typeof(ObjectLevelSecurityObject);
                fullAccessObjectPermission.Criteria = "[Name] Like '%Fully Accessible%'";
                //fullAccessObjectPermission.AllowCreate = true;
                fullAccessObjectPermission.AllowDelete = true;
                fullAccessObjectPermission.AllowNavigate = true;
                fullAccessObjectPermission.AllowRead = true;
                fullAccessObjectPermission.AllowWrite = true;
                fullAccessObjectPermission.Save();
                navigateObjectLevelSecurityObjectPermission.ObjectPermissions.Add(fullAccessObjectPermission);
                //securityDemoRole.TypePermissions.Add(fullAccessObjectPermission);

                SecuritySystemObjectPermissionsObject protectedContentObjectPermission = ObjectSpace.CreateObject<SecuritySystemObjectPermissionsObject>();
                //protectedContentObjectPermission.TargetType = typeof(ObjectLevelSecurityObject);
                protectedContentObjectPermission.Criteria = "[Name] Like '%Protected%'";
                protectedContentObjectPermission.AllowNavigate = true;
                protectedContentObjectPermission.Save();
                navigateObjectLevelSecurityObjectPermission.ObjectPermissions.Add(protectedContentObjectPermission);
                //securityDemoRole.TypePermissions.Add(protectedContentObjectPermission);

                SecuritySystemObjectPermissionsObject readOnlyObjectPermission = ObjectSpace.CreateObject<SecuritySystemObjectPermissionsObject>();
                //readOnlyObjectPermission.TargetType = typeof(ObjectLevelSecurityObject);
                readOnlyObjectPermission.Criteria = "[Name] Like '%Read-Only%'";
                readOnlyObjectPermission.AllowNavigate = true;
                readOnlyObjectPermission.AllowRead = true;
                readOnlyObjectPermission.Save();
                navigateObjectLevelSecurityObjectPermission.ObjectPermissions.Add(readOnlyObjectPermission);
                //securityDemoRole.TypePermissions.Add(readOnlyObjectPermission);

                SecuritySystemObjectPermissionsObject irremovableObjectPermission = ObjectSpace.CreateObject<SecuritySystemObjectPermissionsObject>();
                //irremovableObjectPermission.TargetType = typeof(ObjectLevelSecurityObject);
                irremovableObjectPermission.Criteria = "[Name] Like '%Protected Deletion%'";
                //irremovableObjectPermission.AllowCreate = true;
                irremovableObjectPermission.AllowNavigate = true;
                irremovableObjectPermission.AllowRead = true;
                irremovableObjectPermission.AllowWrite = true;
                irremovableObjectPermission.Save();
                navigateObjectLevelSecurityObjectPermission.ObjectPermissions.Add(irremovableObjectPermission);
                //securityDemoRole.TypePermissions.Add(irremovableObjectPermission);

                securityDemoRole.Save();
            }
            return securityDemoRole;
        }

        private DevExpress.ExpressApp.Security.Strategy.SecuritySystemRole CreateAdministratorRole() {
            DevExpress.ExpressApp.Security.Strategy.SecuritySystemRole administratorRole = ObjectSpace.FindObject<DevExpress.ExpressApp.Security.Strategy.SecuritySystemRole>(new BinaryOperator("Name", "Administrator"));
            if(administratorRole == null) {
                administratorRole = ObjectSpace.CreateObject<DevExpress.ExpressApp.Security.Strategy.SecuritySystemRole>();
                administratorRole.Name = "Administrator";
                administratorRole.IsAdministrative = true;
            }
            return administratorRole;
        }

        private DevExpress.ExpressApp.Security.Strategy.SecuritySystemRole CreateDefaultRole() {
            DevExpress.ExpressApp.Security.Strategy.SecuritySystemRole defaultRole = ObjectSpace.FindObject<DevExpress.ExpressApp.Security.Strategy.SecuritySystemRole>(new BinaryOperator("Name", "Default"));
            if(defaultRole == null) {
                defaultRole = ObjectSpace.CreateObject<DevExpress.ExpressApp.Security.Strategy.SecuritySystemRole>();
                defaultRole.Name = "Default";

                SecuritySystemTypePermissionObject securityDemoUserPermissions = ObjectSpace.CreateObject<SecuritySystemTypePermissionObject>();
                securityDemoUserPermissions.TargetType = typeof(SecurityDemoUser);
                defaultRole.TypePermissions.Add(securityDemoUserPermissions);

                SecuritySystemObjectPermissionsObject myDetailsPermission = ObjectSpace.CreateObject<SecuritySystemObjectPermissionsObject>();
                //myDetailsPermission.TargetType = typeof(SecurityDemoUser);
                myDetailsPermission.Criteria = "[Oid] = CurrentUserId()";
                myDetailsPermission.AllowNavigate = true;
                myDetailsPermission.AllowRead = true;
                securityDemoUserPermissions.ObjectPermissions.Add(myDetailsPermission);
                //defaultRole.PersistentPermissions.Add(myDetailsPermission);

                SecuritySystemTypePermissionObject userPermissions = ObjectSpace.CreateObject<SecuritySystemTypePermissionObject>();
                userPermissions.TargetType = typeof(DevExpress.ExpressApp.Security.Strategy.SecuritySystemUser);
                defaultRole.TypePermissions.Add(userPermissions);
                
                SecuritySystemMemberPermissionsObject ownPasswordPermission = ObjectSpace.CreateObject<SecuritySystemMemberPermissionsObject>();
                //ownPasswordPermission.TargetType = typeof(SecurityUser);
                ownPasswordPermission.Members = "ChangePasswordOnFirstLogon; StoredPassword";
                ownPasswordPermission.AllowWrite = true;
                userPermissions.MemberPermissions.Add(ownPasswordPermission);
                //defaultRole.PersistentPermissions.Add(ownPasswordPermission);

                SecuritySystemTypePermissionObject securityRolePermissions = ObjectSpace.CreateObject<SecuritySystemTypePermissionObject>();
                securityRolePermissions.TargetType = typeof(DevExpress.ExpressApp.Security.Strategy.SecuritySystemRole);
                defaultRole.TypePermissions.Add(userPermissions);

                SecuritySystemObjectPermissionsObject defaultRolePermission = ObjectSpace.CreateObject<SecuritySystemObjectPermissionsObject>();
                //defaultRolePermission.TargetType = typeof(DevExpress.ExpressApp.Security.Strategy.SecuritySystemRole);
                defaultRolePermission.Criteria = "[Name] = 'Default'";
                defaultRolePermission.AllowNavigate = true;
                defaultRolePermission.AllowRead = true;
                securityRolePermissions.ObjectPermissions.Add(defaultRolePermission);
                //defaultRole.PersistentPermissions.Add(defaultRolePermission);
            }
            return defaultRole;
        }
    }
}
