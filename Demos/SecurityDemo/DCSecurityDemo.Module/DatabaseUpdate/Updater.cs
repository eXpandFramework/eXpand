using System;
using DCSecurityDemo.Module.BusinessObjects;
using DCSecurityDemo.Module.Security;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;

namespace DCSecurityDemo.Module {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();

            CreateSecurityDemoObjects();

            IDCRole defaultRole = GetDefaultRole();
            IDCRole administratorRole = GetAdministratorRole();

            IDCUser userAdmin = ObjectSpace.FindObject<IDCUser>(new BinaryOperator("UserName", "Sam"));
            if(userAdmin == null) {
                userAdmin = ObjectSpace.CreateObject<IDCUser>();
                userAdmin.UserName = "Sam";
                userAdmin.IsActive = true;
                userAdmin.SetPassword("");
                userAdmin.Roles.Add(administratorRole);
            }

            IDCRole securityDemoRole = GetSecurityDemoRole();

            IDCUser userJohn = ObjectSpace.FindObject<IDCUser>(new BinaryOperator("UserName", "John"));
            if(userJohn == null) {
                userJohn = ObjectSpace.CreateObject<IDCUser>();
                userJohn.UserName = "John";
                userJohn.IsActive = true;
                userJohn.Roles.Add(defaultRole);
                userJohn.Roles.Add(securityDemoRole);
            }

            ObjectSpace.CommitChanges();
        }

        private void CreateSecurityDemoObjects() {
            IFullAccessObject fullAccessObject = ObjectSpace.FindObject<IFullAccessObject>(new BinaryOperator("Name", "Fully Accessible Object"));
            if(fullAccessObject == null) {
                fullAccessObject = ObjectSpace.CreateObject<IFullAccessObject>();
                fullAccessObject.Name = "Fully Accessible Object";
            }
            IProtectedContentObject protectedContentObject = ObjectSpace.FindObject<IProtectedContentObject>(new BinaryOperator("Name", "Protected Object"));
            if(protectedContentObject == null) {
                protectedContentObject = ObjectSpace.CreateObject<IProtectedContentObject>();
                protectedContentObject.Name = "Protected Object";
            }
            IReadOnlyObject readOnlyObject = ObjectSpace.FindObject<IReadOnlyObject>(new BinaryOperator("Name", "Read-Only Object"));
            if(readOnlyObject == null) {
                readOnlyObject = ObjectSpace.CreateObject<IReadOnlyObject>();
                readOnlyObject.Name = "Read-Only Object";
            }

            IIrremovableObject irremovableObject = ObjectSpace.FindObject<IIrremovableObject>(new BinaryOperator("Name", "Protected Deletion Object"));
            if(irremovableObject == null) {
                irremovableObject = ObjectSpace.CreateObject<IIrremovableObject>();
                irremovableObject.Name = "Protected Deletion Object";
            }
            IUncreatableObject uncreatableObject = ObjectSpace.FindObject<IUncreatableObject>(new BinaryOperator("Name", "Protected Creation Object"));
            if(uncreatableObject == null) {
                uncreatableObject = ObjectSpace.CreateObject<IUncreatableObject>();
                uncreatableObject.Name = "Protected Creation Object";
            }
            IMemberLevelSecurityObject memberLevelSecurityObject = ObjectSpace.FindObject<IMemberLevelSecurityObject>(new BinaryOperator("Name", "Member-Level Protected Object"));
            if(memberLevelSecurityObject == null) {
                memberLevelSecurityObject = ObjectSpace.CreateObject<IMemberLevelSecurityObject>();
                memberLevelSecurityObject.Name = "Member-Level Protected Object";
                memberLevelSecurityObject.ProtectedContentProperty = "Secure Property Value";
                memberLevelSecurityObject.ReadWriteProperty = "Read Write Property Value";
                memberLevelSecurityObject.ReadOnlyProperty = "Read Only Property Value";
                IMemberLevelReferencedObject1 obj1 = ObjectSpace.CreateObject<IMemberLevelReferencedObject1>();
                obj1.Name = "Referenced Object Type 1";
                memberLevelSecurityObject.ProtectedContentCollection.Add(obj1);
                IMemberLevelReferencedObject2 obj2 = ObjectSpace.CreateObject<IMemberLevelReferencedObject2>();
                obj2.Name = "Referenced Object Type 2";
                memberLevelSecurityObject.ReadOnlyCollection.Add(obj2);
            }
            IObjectLevelSecurityObject fullAccessObjectObject = ObjectSpace.FindObject<IObjectLevelSecurityObject>(new BinaryOperator("Name", "Fully Accessible Object"));
            if(fullAccessObjectObject == null) {
                fullAccessObjectObject = ObjectSpace.CreateObject<IObjectLevelSecurityObject>();
                fullAccessObjectObject.Name = "Fully Accessible Object";
            }
            IObjectLevelSecurityObject protectedContentObjectObject = ObjectSpace.FindObject<IObjectLevelSecurityObject>(new BinaryOperator("Name", "Protected Object"));
            if(protectedContentObjectObject == null) {
                protectedContentObjectObject = ObjectSpace.CreateObject<IObjectLevelSecurityObject>();
                protectedContentObjectObject.Name = "Protected Object";
            }
            IObjectLevelSecurityObject readOnlyObjectObject = ObjectSpace.FindObject<IObjectLevelSecurityObject>(new BinaryOperator("Name", "Read-Only Object"));
            if(readOnlyObjectObject == null) {
                readOnlyObjectObject = ObjectSpace.CreateObject<IObjectLevelSecurityObject>();
                readOnlyObjectObject.Name = "Read-Only Object";
            }
            IObjectLevelSecurityObject irremovableObjectObject = ObjectSpace.FindObject<IObjectLevelSecurityObject>(new BinaryOperator("Name", "Protected Deletion Object"));
            if(irremovableObjectObject == null) {
                irremovableObjectObject = ObjectSpace.CreateObject<IObjectLevelSecurityObject>();
                irremovableObjectObject.Name = "Protected Deletion Object";
            }
        }

        private IDCRole GetSecurityDemoRole() {
            IDCRole securityDemoRole = ObjectSpace.FindObject<IDCRole>(new BinaryOperator("Name", "Demo"));
            if(securityDemoRole == null) {
                securityDemoRole = ObjectSpace.CreateObject<IDCRole>();
                securityDemoRole.Name = "Demo";

                // Type Operation Permissions
                IDCTypePermissions fullAccessPermission = ObjectSpace.CreateObject<IDCTypePermissions>();
                fullAccessPermission.TargetType = typeof(IFullAccessObject);
                fullAccessPermission.AllowCreate = true;
                fullAccessPermission.AllowDelete = true;
                fullAccessPermission.AllowNavigate = true;
                fullAccessPermission.AllowRead = true;
                fullAccessPermission.AllowWrite = true;
                securityDemoRole.TypePermissions.Add(fullAccessPermission);
                IDCTypePermissions protectedContentPermission = ObjectSpace.CreateObject<IDCTypePermissions>();
                protectedContentPermission.TargetType = typeof(IProtectedContentObject);
                protectedContentPermission.AllowNavigate = true;
                securityDemoRole.TypePermissions.Add(protectedContentPermission);
                IDCTypePermissions readOnlyPermission = ObjectSpace.CreateObject<IDCTypePermissions>();
                readOnlyPermission.TargetType = typeof(IReadOnlyObject);
                readOnlyPermission.AllowNavigate = true;
                readOnlyPermission.AllowRead = true;
                securityDemoRole.TypePermissions.Add(readOnlyPermission);

                IDCTypePermissions irremovablePermission = ObjectSpace.CreateObject<IDCTypePermissions>();
                irremovablePermission.TargetType = typeof(IIrremovableObject);
                irremovablePermission.AllowCreate = true;
                irremovablePermission.AllowNavigate = true;
                irremovablePermission.AllowRead = true;
                irremovablePermission.AllowWrite = true;
                securityDemoRole.TypePermissions.Add(irremovablePermission);
                IDCTypePermissions uncreatablePermission = ObjectSpace.CreateObject<IDCTypePermissions>();
                uncreatablePermission.TargetType = typeof(IUncreatableObject);
                uncreatablePermission.AllowDelete = true;
                uncreatablePermission.AllowNavigate = true;
                uncreatablePermission.AllowRead = true;
                uncreatablePermission.AllowWrite = true;
                securityDemoRole.TypePermissions.Add(uncreatablePermission);

                // Member Operation Permissions
                IDCTypePermissions navigateMemberLevelOperationObjectPermission = ObjectSpace.CreateObject<IDCTypePermissions>();
                navigateMemberLevelOperationObjectPermission.TargetType = typeof(IMemberLevelSecurityObject);
                navigateMemberLevelOperationObjectPermission.AllowCreate = true;
                navigateMemberLevelOperationObjectPermission.AllowDelete = true;
                navigateMemberLevelOperationObjectPermission.AllowNavigate = true;
                securityDemoRole.TypePermissions.Add(navigateMemberLevelOperationObjectPermission);

                IDCMemberPermissions readWriteMemberPermission = ObjectSpace.CreateObject<IDCMemberPermissions>();
                readWriteMemberPermission.Members = "ReadWriteProperty; Name; oid; Oid; OptimisticLockField";
                readWriteMemberPermission.AllowRead = true;
                readWriteMemberPermission.AllowWrite = true;
                navigateMemberLevelOperationObjectPermission.MemberPermissions.Add(readWriteMemberPermission);

                IDCMemberPermissions protectedContentMemberPermission = ObjectSpace.CreateObject<IDCMemberPermissions>();
                protectedContentMemberPermission.Members = "ProtectedContentProperty; ProtectedContentCollection";
                navigateMemberLevelOperationObjectPermission.MemberPermissions.Add(protectedContentMemberPermission);

                IDCMemberPermissions readOnlyMemberPermission = ObjectSpace.CreateObject<IDCMemberPermissions>();
                readOnlyMemberPermission.Members = "ReadOnlyProperty; ReadOnlyCollection";
                readOnlyMemberPermission.AllowRead = true;
                navigateMemberLevelOperationObjectPermission.MemberPermissions.Add(readOnlyMemberPermission);

                IDCTypePermissions memberLevelReferencedObject1Permission = ObjectSpace.CreateObject<IDCTypePermissions>();
                memberLevelReferencedObject1Permission.TargetType = typeof(IMemberLevelReferencedObject1);
                memberLevelReferencedObject1Permission.AllowRead = true;
                memberLevelReferencedObject1Permission.AllowWrite = true;
                memberLevelReferencedObject1Permission.AllowCreate = true;
                memberLevelReferencedObject1Permission.AllowDelete = true;
                securityDemoRole.TypePermissions.Add(memberLevelReferencedObject1Permission);

                IDCTypePermissions memberLevelReferencedObject2Permission = ObjectSpace.CreateObject<IDCTypePermissions>();
                memberLevelReferencedObject2Permission.TargetType = typeof(IMemberLevelReferencedObject2);
                memberLevelReferencedObject2Permission.AllowRead = true;
                memberLevelReferencedObject2Permission.AllowWrite = true;
                memberLevelReferencedObject2Permission.AllowCreate = true;
                memberLevelReferencedObject2Permission.AllowDelete = true;
                securityDemoRole.TypePermissions.Add(memberLevelReferencedObject2Permission);

                // Object Operation Permissions
                IDCTypePermissions navigateObjectLevelSecurityObjectPermission = ObjectSpace.CreateObject<IDCTypePermissions>();
                navigateObjectLevelSecurityObjectPermission.TargetType = typeof(IObjectLevelSecurityObject);
                navigateObjectLevelSecurityObjectPermission.AllowNavigate = true;
                securityDemoRole.TypePermissions.Add(navigateObjectLevelSecurityObjectPermission);

                IDCObjectPermissions fullAccessObjectPermission = ObjectSpace.CreateObject<IDCObjectPermissions>();
                fullAccessObjectPermission.Criteria = "[Name] Like '%Fully Accessible%'";
                fullAccessObjectPermission.AllowDelete = true;
                fullAccessObjectPermission.AllowNavigate = true;
                fullAccessObjectPermission.AllowRead = true;
                fullAccessObjectPermission.AllowWrite = true;
                navigateObjectLevelSecurityObjectPermission.ObjectPermissions.Add(fullAccessObjectPermission);

                IDCObjectPermissions protectedContentObjectPermission = ObjectSpace.CreateObject<IDCObjectPermissions>();
                protectedContentObjectPermission.Criteria = "[Name] Like '%Protected%'";
                protectedContentObjectPermission.AllowNavigate = true;
                navigateObjectLevelSecurityObjectPermission.ObjectPermissions.Add(protectedContentObjectPermission);

                IDCObjectPermissions readOnlyObjectPermission = ObjectSpace.CreateObject<IDCObjectPermissions>();
                readOnlyObjectPermission.Criteria = "[Name] Like '%Read-Only%'";
                readOnlyObjectPermission.AllowNavigate = true;
                readOnlyObjectPermission.AllowRead = true;
                navigateObjectLevelSecurityObjectPermission.ObjectPermissions.Add(readOnlyObjectPermission);

                IDCObjectPermissions irremovableObjectPermission = ObjectSpace.CreateObject<IDCObjectPermissions>();
                irremovableObjectPermission.Criteria = "[Name] Like '%Protected Deletion%'";
                irremovableObjectPermission.AllowNavigate = true;
                irremovableObjectPermission.AllowRead = true;
                irremovableObjectPermission.AllowWrite = true;
                navigateObjectLevelSecurityObjectPermission.ObjectPermissions.Add(irremovableObjectPermission);
            }
            return securityDemoRole;
        }

        private IDCRole GetAdministratorRole() {
            IDCRole administratorRole = ObjectSpace.FindObject<IDCRole>(new BinaryOperator("Name", "Administrator"));
            if(administratorRole == null) {
                administratorRole = ObjectSpace.CreateObject<IDCRole>();
                administratorRole.Name = "Administrator";
                administratorRole.IsAdministrative = true;
            }
            return administratorRole;
        }

        private IDCRole GetDefaultRole() {
            IDCRole defaultRole = ObjectSpace.FindObject<IDCRole>(new BinaryOperator("Name", "Default"));
            if(defaultRole == null) {
                defaultRole = ObjectSpace.CreateObject<IDCRole>();
                defaultRole.Name = "Default";

                IDCTypePermissions securityDemoUserPermissions = ObjectSpace.CreateObject<IDCTypePermissions>();
                securityDemoUserPermissions.TargetType = typeof(IDCUser);
                defaultRole.TypePermissions.Add(securityDemoUserPermissions);

                IDCObjectPermissions myDetailsPermission = ObjectSpace.CreateObject<IDCObjectPermissions>();
                myDetailsPermission.Criteria = "[Oid] = CurrentUserId()";
                myDetailsPermission.AllowNavigate = true;
                myDetailsPermission.AllowRead = true;
                securityDemoUserPermissions.ObjectPermissions.Add(myDetailsPermission);

                IDCTypePermissions userPermissions = ObjectSpace.CreateObject<IDCTypePermissions>();
                userPermissions.TargetType = typeof(IDCUser);
                defaultRole.TypePermissions.Add(userPermissions);

                IDCMemberPermissions ownPasswordPermission = ObjectSpace.CreateObject<IDCMemberPermissions>();
                ownPasswordPermission.Members = "ChangePasswordOnFirstLogon; StoredPassword";
                ownPasswordPermission.AllowWrite = true;
                userPermissions.MemberPermissions.Add(ownPasswordPermission);

                IDCTypePermissions securityRolePermissions = ObjectSpace.CreateObject<IDCTypePermissions>();
                securityRolePermissions.TargetType = typeof(IDCRole);
                defaultRole.TypePermissions.Add(userPermissions);

                IDCObjectPermissions defaultRolePermission = ObjectSpace.CreateObject<IDCObjectPermissions>();
                defaultRolePermission.Criteria = "[Name] = 'Default'";
                defaultRolePermission.AllowNavigate = true;
                defaultRolePermission.AllowRead = true;
                securityRolePermissions.ObjectPermissions.Add(defaultRolePermission);
            }
            return defaultRole;
        }
    }
}
