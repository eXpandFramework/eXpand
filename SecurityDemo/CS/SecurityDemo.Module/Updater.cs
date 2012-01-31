using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.ModelDifference.Security;
using Xpand.ExpressApp.Security;
using Xpand.Persistent.BaseImpl;

namespace SecurityDemo.Module {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }

        public override void UpdateDatabaseAfterUpdateSchema() {

            base.UpdateDatabaseAfterUpdateSchema();

            CreateSecurityDemoObjects();
            CreateAnonymousAccess();

            SecurityRole defaultRole = CreateDefaultRole();
            SecurityRole administratorRole = CreateAdministratorRole();

            SecurityDemoUser userAdmin = ObjectSpace.FindObject<SecurityDemoUser>(new BinaryOperator("UserName", "Sam"));
            if (userAdmin == null) {
                userAdmin = ObjectSpace.CreateObject<SecurityDemoUser>();
                userAdmin.UserName = "Sam";
                userAdmin.IsActive = true;
                userAdmin.SetPassword("");
                userAdmin.Roles.Add(defaultRole);
                userAdmin.Roles.Add(administratorRole);
                userAdmin.Save();
            }

            SecurityRole securityDemoRole = CreateSecurityDemoRole();

            SecurityDemoUser userJohn = ObjectSpace.FindObject<SecurityDemoUser>(new BinaryOperator("UserName", "John"));
            if (userJohn == null) {
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
            if (fullAccessObject == null) {
                fullAccessObject = ObjectSpace.CreateObject<FullAccessObject>();
                fullAccessObject.Name = "Fully Accessible Object";
                fullAccessObject.Save();
            }
            ProtectedContentObject protectedContentObject = ObjectSpace.FindObject<ProtectedContentObject>(new BinaryOperator("Name", "Protected Object"));
            if (protectedContentObject == null) {
                protectedContentObject = ObjectSpace.CreateObject<ProtectedContentObject>();
                protectedContentObject.Name = "Protected Object";
                protectedContentObject.Save();
            }
            ReadOnlyObject readOnlyObject = ObjectSpace.FindObject<ReadOnlyObject>(new BinaryOperator("Name", "Read-Only Object"));
            if (readOnlyObject == null) {
                readOnlyObject = ObjectSpace.CreateObject<ReadOnlyObject>();
                readOnlyObject.Name = "Read-Only Object";
                readOnlyObject.Save();
            }
            IrremovableObject irremovableObject = ObjectSpace.FindObject<IrremovableObject>(new BinaryOperator("Name", "Protected Deletion Object"));
            if (irremovableObject == null) {
                irremovableObject = ObjectSpace.CreateObject<IrremovableObject>();
                irremovableObject.Name = "Protected Deletion Object";
                irremovableObject.Save();
            }
            UncreatableObject uncreatableObject = ObjectSpace.FindObject<UncreatableObject>(new BinaryOperator("Name", "Protected Creation Object"));
            if (uncreatableObject == null) {
                uncreatableObject = ObjectSpace.CreateObject<UncreatableObject>();
                uncreatableObject.Name = "Protected Creation Object";
                uncreatableObject.Save();
            }
            MemberLevelSecurityObject memberLevelSecurityObject = ObjectSpace.FindObject<MemberLevelSecurityObject>(new BinaryOperator("Name", "Member-Level Protected Object"));
            if (memberLevelSecurityObject == null) {
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
            if (fullAccessObjectObject == null) {
                fullAccessObjectObject = ObjectSpace.CreateObject<ObjectLevelSecurityObject>();
                fullAccessObjectObject.Name = "Fully Accessible Object";
                fullAccessObjectObject.Save();
            }
            ObjectLevelSecurityObject protectedContentObjectObject = ObjectSpace.FindObject<ObjectLevelSecurityObject>(new BinaryOperator("Name", "Protected Object"));
            if (protectedContentObjectObject == null) {
                protectedContentObjectObject = ObjectSpace.CreateObject<ObjectLevelSecurityObject>();
                protectedContentObjectObject.Name = "Protected Object";
                protectedContentObjectObject.Save();
            }
            ObjectLevelSecurityObject readOnlyObjectObject = ObjectSpace.FindObject<ObjectLevelSecurityObject>(new BinaryOperator("Name", "Read-Only Object"));
            if (readOnlyObjectObject == null) {
                readOnlyObjectObject = ObjectSpace.CreateObject<ObjectLevelSecurityObject>();
                readOnlyObjectObject.Name = "Read-Only Object";
                readOnlyObjectObject.Save();
            }
            ObjectLevelSecurityObject irremovableObjectObject = ObjectSpace.FindObject<ObjectLevelSecurityObject>(new BinaryOperator("Name", "Protected Deletion Object"));
            if (irremovableObjectObject == null) {
                irremovableObjectObject = ObjectSpace.CreateObject<ObjectLevelSecurityObject>();
                irremovableObjectObject.Name = "Protected Deletion Object";
                irremovableObjectObject.Save();
            }
        }

        private SecurityRole CreateSecurityDemoRole() {
            SecurityRole securityDemoRole = ObjectSpace.FindObject<SecurityRole>(new BinaryOperator("Name", "Demo"));
            if (securityDemoRole == null) {
                securityDemoRole = ObjectSpace.CreateObject<SecurityRole>();
                securityDemoRole.Name = "Demo";

                // Type Operation Permissions
                TypeOperationPermissionData fullAccessPermission = ObjectSpace.CreateObject<TypeOperationPermissionData>();
                fullAccessPermission.TargetType = typeof(FullAccessObject);
                fullAccessPermission.AllowCreate = true;
                fullAccessPermission.AllowDelete = true;
                fullAccessPermission.AllowNavigate = true;
                fullAccessPermission.AllowRead = true;
                fullAccessPermission.AllowWrite = true;
                fullAccessPermission.Save();
                securityDemoRole.PersistentPermissions.Add(fullAccessPermission);
                TypeOperationPermissionData protectedContentPermission = ObjectSpace.CreateObject<TypeOperationPermissionData>();
                protectedContentPermission.TargetType = typeof(ProtectedContentObject);
                protectedContentPermission.AllowNavigate = true;
                protectedContentPermission.Save();
                securityDemoRole.PersistentPermissions.Add(protectedContentPermission);
                TypeOperationPermissionData readOnlyPermission = ObjectSpace.CreateObject<TypeOperationPermissionData>();
                readOnlyPermission.TargetType = typeof(ReadOnlyObject);
                readOnlyPermission.AllowNavigate = true;
                readOnlyPermission.AllowRead = true;
                readOnlyPermission.Save();
                securityDemoRole.PersistentPermissions.Add(readOnlyPermission);


                TypeOperationPermissionData irremovablePermission = ObjectSpace.CreateObject<TypeOperationPermissionData>();
                irremovablePermission.TargetType = typeof(IrremovableObject);
                irremovablePermission.AllowCreate = true;
                irremovablePermission.AllowNavigate = true;
                irremovablePermission.AllowRead = true;
                irremovablePermission.AllowWrite = true;
                irremovablePermission.Save();
                securityDemoRole.PersistentPermissions.Add(irremovablePermission);
                TypeOperationPermissionData uncreatablePermission = ObjectSpace.CreateObject<TypeOperationPermissionData>();
                uncreatablePermission.TargetType = typeof(UncreatableObject);
                uncreatablePermission.AllowDelete = true;
                uncreatablePermission.AllowNavigate = true;
                uncreatablePermission.AllowRead = true;
                uncreatablePermission.AllowWrite = true;
                uncreatablePermission.Save();
                securityDemoRole.PersistentPermissions.Add(uncreatablePermission);

                // Member Operation Permissions
                TypeOperationPermissionData navigateMemberLevelOperationObjectPermission = ObjectSpace.CreateObject<TypeOperationPermissionData>();
                navigateMemberLevelOperationObjectPermission.TargetType = typeof(MemberLevelSecurityObject);
                navigateMemberLevelOperationObjectPermission.AllowCreate = true;
                navigateMemberLevelOperationObjectPermission.AllowDelete = true;
                navigateMemberLevelOperationObjectPermission.AllowNavigate = true;
                navigateMemberLevelOperationObjectPermission.Save();
                securityDemoRole.PersistentPermissions.Add(navigateMemberLevelOperationObjectPermission);
                MemberOperationPermissionData readWriteMemberPermission = ObjectSpace.CreateObject<MemberOperationPermissionData>();
                readWriteMemberPermission.TargetType = typeof(MemberLevelSecurityObject);
                readWriteMemberPermission.Members = "ReadWriteProperty, Name, oid, Oid, OptimisticLockField"; // TODO - Slava D - service fields - XPO responsibility
                readWriteMemberPermission.AllowRead = true;
                readWriteMemberPermission.AllowWrite = true;
                readWriteMemberPermission.Save();
                securityDemoRole.PersistentPermissions.Add(readWriteMemberPermission);
                MemberOperationPermissionData protectedContentMemberPermission = ObjectSpace.CreateObject<MemberOperationPermissionData>();
                protectedContentMemberPermission.TargetType = typeof(MemberLevelSecurityObject);
                protectedContentMemberPermission.Members = "ProtectedContentProperty, ProtectedContentCollection";
                protectedContentMemberPermission.Save();
                securityDemoRole.PersistentPermissions.Add(protectedContentMemberPermission);
                MemberOperationPermissionData readOnlyMemberPermission = ObjectSpace.CreateObject<MemberOperationPermissionData>();
                readOnlyMemberPermission.TargetType = typeof(MemberLevelSecurityObject);
                readOnlyMemberPermission.Members = "ReadOnlyProperty, ReadOnlyCollection";
                readOnlyMemberPermission.AllowRead = true;
                readOnlyMemberPermission.Save();
                securityDemoRole.PersistentPermissions.Add(readOnlyMemberPermission);

                TypeOperationPermissionData memberLevelReferencedObject1Permission = ObjectSpace.CreateObject<TypeOperationPermissionData>();
                memberLevelReferencedObject1Permission.TargetType = typeof(MemberLevelReferencedObject1);
                memberLevelReferencedObject1Permission.AllowRead = true;
                memberLevelReferencedObject1Permission.AllowWrite = true;
                memberLevelReferencedObject1Permission.AllowCreate = true;
                memberLevelReferencedObject1Permission.AllowDelete = true;
                memberLevelReferencedObject1Permission.Save();
                securityDemoRole.PersistentPermissions.Add(memberLevelReferencedObject1Permission);

                TypeOperationPermissionData memberLevelReferencedObject2Permission = ObjectSpace.CreateObject<TypeOperationPermissionData>();
                memberLevelReferencedObject2Permission.TargetType = typeof(MemberLevelReferencedObject2);
                memberLevelReferencedObject2Permission.AllowRead = true;
                memberLevelReferencedObject2Permission.AllowWrite = true;
                memberLevelReferencedObject2Permission.AllowCreate = true;
                memberLevelReferencedObject2Permission.AllowDelete = true;
                memberLevelReferencedObject2Permission.Save();
                securityDemoRole.PersistentPermissions.Add(memberLevelReferencedObject2Permission);



                // Object Operation Permissions
                TypeOperationPermissionData navigateObjectLevelSecurityObjectPermission = ObjectSpace.CreateObject<TypeOperationPermissionData>();
                navigateObjectLevelSecurityObjectPermission.TargetType = typeof(ObjectLevelSecurityObject);
                navigateObjectLevelSecurityObjectPermission.AllowNavigate = true;
                navigateObjectLevelSecurityObjectPermission.Save();
                securityDemoRole.PersistentPermissions.Add(navigateObjectLevelSecurityObjectPermission);
                MemberOperationPermissionData oidObjectLevelMemberPermission = ObjectSpace.CreateObject<MemberOperationPermissionData>();
                oidObjectLevelMemberPermission.TargetType = typeof(ObjectLevelSecurityObject);
                oidObjectLevelMemberPermission.Members = "oid, Oid"; // TODO - Slava D - service fields - XPO responsibility
                oidObjectLevelMemberPermission.AllowRead = true;
                oidObjectLevelMemberPermission.AllowWrite = true;
                oidObjectLevelMemberPermission.Save();
                securityDemoRole.PersistentPermissions.Add(oidObjectLevelMemberPermission);
                ObjectOperationPermissionData fullAccessObjectPermission = ObjectSpace.CreateObject<ObjectOperationPermissionData>();
                fullAccessObjectPermission.TargetType = typeof(ObjectLevelSecurityObject);
                fullAccessObjectPermission.Criteria = "[Name] Like '%Fully Accessible%'";
                fullAccessObjectPermission.AllowCreate = true;
                fullAccessObjectPermission.AllowDelete = true;
                fullAccessObjectPermission.AllowNavigate = true;
                fullAccessObjectPermission.AllowRead = true;
                fullAccessObjectPermission.AllowWrite = true;
                fullAccessObjectPermission.Save();
                securityDemoRole.PersistentPermissions.Add(fullAccessObjectPermission);
                ObjectOperationPermissionData protectedContentObjectPermission = ObjectSpace.CreateObject<ObjectOperationPermissionData>();
                protectedContentObjectPermission.TargetType = typeof(ObjectLevelSecurityObject);
                protectedContentObjectPermission.Criteria = "[Name] Like '%Protected%'";
                protectedContentObjectPermission.AllowNavigate = true;
                protectedContentObjectPermission.Save();
                securityDemoRole.PersistentPermissions.Add(protectedContentObjectPermission);
                ObjectOperationPermissionData readOnlyObjectPermission = ObjectSpace.CreateObject<ObjectOperationPermissionData>();
                readOnlyObjectPermission.TargetType = typeof(ObjectLevelSecurityObject);
                readOnlyObjectPermission.Criteria = "[Name] Like '%Read-Only%'";
                readOnlyObjectPermission.AllowNavigate = true;
                readOnlyObjectPermission.AllowRead = true;
                readOnlyObjectPermission.Save();
                securityDemoRole.PersistentPermissions.Add(readOnlyObjectPermission);
                ObjectOperationPermissionData irremovableObjectPermission = ObjectSpace.CreateObject<ObjectOperationPermissionData>();
                irremovableObjectPermission.TargetType = typeof(ObjectLevelSecurityObject);
                irremovableObjectPermission.Criteria = "[Name] Like '%Protected Deletion%'";
                irremovableObjectPermission.AllowCreate = true;
                irremovableObjectPermission.AllowNavigate = true;
                irremovableObjectPermission.AllowRead = true;
                irremovableObjectPermission.AllowWrite = true;
                irremovableObjectPermission.Save();
                securityDemoRole.PersistentPermissions.Add(irremovableObjectPermission);

                securityDemoRole.Save();
            }
            return securityDemoRole;
        }

        private SecurityRole CreateAdministratorRole() {
            SecurityRole administratorRole = ObjectSpace.FindObject<SecurityRole>(new BinaryOperator("Name", "Administrator"));
            if (administratorRole == null) {
                administratorRole = ObjectSpace.CreateObject<SecurityRole>();
                administratorRole.Name = "Administrator";
                ModelOperationPermissionData modelPermission = ObjectSpace.CreateObject<ModelOperationPermissionData>();
                modelPermission.Save();

                administratorRole.BeginUpdate();
                administratorRole.Permissions.GrantRecursive(typeof(object), SecurityOperations.Read);
                administratorRole.Permissions.GrantRecursive(typeof(object), SecurityOperations.Write);
                administratorRole.Permissions.GrantRecursive(typeof(object), SecurityOperations.Create);
                administratorRole.Permissions.GrantRecursive(typeof(object), SecurityOperations.Delete);
                administratorRole.Permissions.GrantRecursive(typeof(object), SecurityOperations.Navigate);
                administratorRole.EndUpdate();

                administratorRole.PersistentPermissions.Add(modelPermission);
                administratorRole.Save();
            }
            return administratorRole;
        }

        private SecurityRole CreateDefaultRole() {
            SecurityRole defaultRole = ObjectSpace.FindObject<SecurityRole>(new BinaryOperator("Name", "Default"));
            if (defaultRole == null) {
                defaultRole = ObjectSpace.CreateObject<SecurityRole>();
                defaultRole.Name = "Default";
                TypeOperationPermissionData changePasswordOnLogonPermission = ObjectSpace.CreateObject<TypeOperationPermissionData>();
                changePasswordOnLogonPermission.TargetType = typeof(ChangePasswordOnLogonParameters);
                changePasswordOnLogonPermission.AllowRead = true;
                changePasswordOnLogonPermission.AllowWrite = true;
                changePasswordOnLogonPermission.Save();
                defaultRole.PersistentPermissions.Add(changePasswordOnLogonPermission);
                TypeOperationPermissionData changePasswordPermission = ObjectSpace.CreateObject<TypeOperationPermissionData>();
                changePasswordPermission.TargetType = typeof(ChangePasswordParameters);
                changePasswordPermission.AllowRead = true;
                changePasswordPermission.AllowWrite = true;
                changePasswordPermission.Save();
                defaultRole.PersistentPermissions.Add(changePasswordPermission);
                ObjectOperationPermissionData myDetailsPermission = ObjectSpace.CreateObject<ObjectOperationPermissionData>();
                myDetailsPermission.TargetType = typeof(SecurityUser);
                myDetailsPermission.Criteria = "[Oid] = CurrentUserId()";
                myDetailsPermission.AllowNavigate = true;
                myDetailsPermission.AllowRead = true;
                myDetailsPermission.Save();
                defaultRole.PersistentPermissions.Add(myDetailsPermission);
                MemberOperationPermissionData userNamePermission = ObjectSpace.CreateObject<MemberOperationPermissionData>();
                userNamePermission.TargetType = typeof(SecurityUser);
                userNamePermission.Members = "ChangePasswordOnFirstLogon";
                userNamePermission.AllowWrite = true;
                userNamePermission.Save();
                defaultRole.PersistentPermissions.Add(userNamePermission);
                MemberOperationPermissionData ownPasswordPermission = ObjectSpace.CreateObject<MemberOperationPermissionData>();
                ownPasswordPermission.TargetType = typeof(SecurityUser);
                ownPasswordPermission.Members = "StoredPassword";
                ownPasswordPermission.AllowWrite = true;
                ownPasswordPermission.Save();
                defaultRole.PersistentPermissions.Add(ownPasswordPermission);
                ObjectOperationPermissionData defaultRolePermission = ObjectSpace.CreateObject<ObjectOperationPermissionData>();
                defaultRolePermission.TargetType = typeof(SecurityRole);
                defaultRolePermission.Criteria = "[Name] = 'Default'";
                defaultRolePermission.AllowNavigate = true;
                defaultRolePermission.AllowRead = true;
                defaultRolePermission.Save();
                defaultRole.PersistentPermissions.Add(defaultRolePermission);
                TypeOperationPermissionData aboutInfoPermission = ObjectSpace.CreateObject<TypeOperationPermissionData>();
                aboutInfoPermission.TargetType = typeof(DevExpress.ExpressApp.SystemModule.AboutInfo);
                aboutInfoPermission.AllowRead = true;
                aboutInfoPermission.Save();
                defaultRole.PersistentPermissions.Add(aboutInfoPermission);
                defaultRole.Save();
                TypeOperationPermissionData validationDetailsPermission = ObjectSpace.CreateObject<TypeOperationPermissionData>();
                validationDetailsPermission.TargetType = typeof(DevExpress.Persistent.Validation.RuleSetValidationResultItem);
                validationDetailsPermission.AllowRead = true;
                validationDetailsPermission.Save();
                defaultRole.PersistentPermissions.Add(validationDetailsPermission);
                TypeOperationPermissionData validationResultsPermission = ObjectSpace.CreateObject<TypeOperationPermissionData>();
                validationResultsPermission.TargetType = typeof(DevExpress.ExpressApp.Validation.AllContextsView.ValidationResults);
                validationResultsPermission.AllowRead = true;
                validationResultsPermission.Save();
                defaultRole.PersistentPermissions.Add(validationResultsPermission);
                TypeOperationPermissionData diagnosticInfoPermission = ObjectSpace.CreateObject<TypeOperationPermissionData>();
                diagnosticInfoPermission.TargetType = typeof(DevExpress.ExpressApp.SystemModule.DiagnosticInfoObject);
                diagnosticInfoPermission.AllowRead = true;
                diagnosticInfoPermission.Save();
                defaultRole.PersistentPermissions.Add(diagnosticInfoPermission);
            }
            return defaultRole;
        }

        private void CreateAnonymousAccess() {
            SecurityRole anonymousRole = ObjectSpace.FindObject<SecurityRole>(new BinaryOperator("Name", SecurityStrategy.AnonymousUserName));
            if (anonymousRole == null) {
                anonymousRole = ObjectSpace.CreateObject<SecurityRole>();
                anonymousRole.Name = SecurityStrategy.AnonymousUserName;
                anonymousRole.BeginUpdate();
                anonymousRole.Permissions[typeof(SecurityDemoUser)].Grant(SecurityOperations.Write);
                anonymousRole.Permissions[typeof(SecurityDemoUser)].Grant(SecurityOperations.Read);
                anonymousRole.GrantPermissionsObjects(new List<Type> { typeof(SequenceObject) });
                anonymousRole.GrantPermissionsForModelDifferenceObjects();
                anonymousRole.EndUpdate();
                anonymousRole.Save();
            }

            SecurityDemoUser anonymousUser = ObjectSpace.FindObject<SecurityDemoUser>(new BinaryOperator("UserName", SecurityStrategy.AnonymousUserName));
            if (anonymousUser == null) {
                anonymousUser = ObjectSpace.CreateObject<SecurityDemoUser>();
                anonymousUser.UserName = SecurityStrategy.AnonymousUserName;
                anonymousUser.IsActive = true;
                anonymousUser.SetPassword("");
                anonymousUser.Roles.Add(anonymousRole);
                anonymousUser.Save();
            }
        }
    }
}
