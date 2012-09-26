using System;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.Security.Core;
using Xpand.ExpressApp.ModelDifference.Security;

namespace SecurityDemo.Module {

    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();

            CreateSecurityDemoObjects();

            var defaultRole = ObjectSpace.GetDefaultRole();
            var administratorRole = ObjectSpace.GetAdminRole("Administrator");
            var securityDemoRole = CreateSecurityDemoRole();
            var modelRole = ObjectSpace.GetDefaultModelRole("ModelDifference");

            ObjectSpace.GetUser("Sam", "", administratorRole);
            ObjectSpace.GetUser("John", "", defaultRole, securityDemoRole, modelRole);

            ObjectSpace.CommitChanges();
        }

        XpandRole CreateSecurityDemoRole() {
            var securityDemoRole = ObjectSpace.GetRole("Demo");
            if (ObjectSpace.IsNewObject(securityDemoRole)) {
                securityDemoRole.CreateTypePermission<FullAccessObject>();

                //ProtectedContentObject has only navigate so it will only be visible in the navigation menu. The ListView,DetailView will use the ProtectedContentPropertyEditor
                securityDemoRole.CreateTypePermission<ProtectedContentObject>(o => {
                    o.AllowNavigate = true;
                }, false);

                //For ReadOnlyObjects we want only to navigate and read all other permissions are disallowed
                securityDemoRole.CreateTypePermission<ReadOnlyObject>(o => {
                    o.AllowNavigate = true;
                    o.AllowRead = true;
                }, false);

                //For IrremovableObjects we allow everything except delete operations
                securityDemoRole.CreateTypePermission<IrremovableObject>(o => {
                    o.AllowDelete = false;
                });

                //For UncreatableObjects we allow everything except create operations
                securityDemoRole.CreateTypePermission<UncreatableObject>(o => {
                    o.AllowCreate = false;
                });

                CreateMemberLevelSecurityObjectPermissions(securityDemoRole);

                //For MemberLevelReferencedObject1s we allow everything except navigate operations
                securityDemoRole.CreateTypePermission<MemberLevelReferencedObject1>(o => {
                    o.AllowNavigate = false;
                });

                //For MemberLevelReferencedObject2s we allow everything except navigate operations
                securityDemoRole.CreateTypePermission<MemberLevelReferencedObject2>(o => {
                    o.AllowNavigate = false;
                });

                CreateNavigateObjectLevelSecurityObjectPermissions(securityDemoRole);
            }
            return securityDemoRole;
        }

        void CreateMemberLevelSecurityObjectPermissions(XpandRole securityDemoRole) {
            //We want to selectively assign Read/Write permission to members of MemberLevelSecurityObject class, so first we create a type permission that allows everything except Read/Write
            var memberLevelTypePermission = securityDemoRole.CreateTypePermission<MemberLevelSecurityObject>(o => {
                o.AllowCreate = true;
                o.AllowDelete = true;
                o.AllowNavigate = true;
            }, false);

            //Only for ReadWriteProperty,Name members we overwrite the Read/Write permissions we got from the TypePermission. All other members members will inherit permissions from the TypePermission Read/Write ==false
            memberLevelTypePermission.CreateMemberPermission(o => {
                o.AllowRead = true;
                o.AllowWrite = true;
                o.Members = "ReadWriteProperty; Name";
            }, false);

            //Maybe this is not needed
            memberLevelTypePermission.CreateMemberPermission(o => o.Members = "ProtectedContentProperty; ProtectedContentCollection", false);

            //Only for ReadWriteProperty,Name members we overwrite the Read/Write permissions we got from the TypePermission. All other members members will inherit permissions from the TypePermission Read ==false
            memberLevelTypePermission.CreateMemberPermission(o => {
                o.AllowRead = true;
                o.Members = "ReadOnlyProperty; ReadOnlyCollection";
            }, false);
        }

        void CreateNavigateObjectLevelSecurityObjectPermissions(XpandRole securityDemoRole) {
            //We want to allow operations for objects that fit in a criterion, so first we create a TypePermission that allows no operation except Navigate
            var navigateObjectLevelSecurityObjectTypePermission = securityDemoRole.CreateTypePermission<ObjectLevelSecurityObject>(o => {
                o.AllowNavigate = true;
            }, false);

            //We create an ObjectPermission that allows all operations for ObjectLevelSecurityObjects that fit to [Name] Like '%Fully Accessible%'
            navigateObjectLevelSecurityObjectTypePermission.CreateObjectPermission(
                o => { o.Criteria = "[Name] Like '%Fully Accessible%'"; });

            //We create an ObjectPermission that allows only Navigate for ObjectLevelSecurityObjects that fit to [Name] Like '%Protected%'
            navigateObjectLevelSecurityObjectTypePermission.CreateObjectPermission(o => {
                o.AllowNavigate = true;
                o.Criteria = "[Name] Like '%Protected%'";
            }, false);

            //We create an ObjectPermission that allows only Navigate/Read for ObjectLevelSecurityObjects that fit to [Name] Like '%Read-Only%''
            navigateObjectLevelSecurityObjectTypePermission.CreateObjectPermission(o => {
                o.Criteria = "[Name] Like '%Read-Only%'";
                o.AllowNavigate = true;
                o.AllowRead = true;
            }, false);

            //We create an ObjectPermission that allows only Navigate/Read/Write for ObjectLevelSecurityObjects that fit to [Name] Like '%Read-Only%''
            navigateObjectLevelSecurityObjectTypePermission.CreateObjectPermission(o => {
                o.Criteria = "[Name] Like '%Protected Deletion%'";
                o.AllowNavigate = true;
                o.AllowRead = true;
                o.AllowWrite = true;
            }, false);
        }

        void CreateSecurityDemoObjects() {
            var fullAccessObject = ObjectSpace.FindObject<FullAccessObject>(new BinaryOperator("Name", "Fully Accessible Object"));
            if (fullAccessObject == null) {
                fullAccessObject = ObjectSpace.CreateObject<FullAccessObject>();
                fullAccessObject.Name = "Fully Accessible Object";
                fullAccessObject.Save();
            }
            var protectedContentObject = ObjectSpace.FindObject<ProtectedContentObject>(new BinaryOperator("Name", "Protected Object"));
            if (protectedContentObject == null) {
                protectedContentObject = ObjectSpace.CreateObject<ProtectedContentObject>();
                protectedContentObject.Name = "Protected Object";
                protectedContentObject.Save();
            }
            var readOnlyObject = ObjectSpace.FindObject<ReadOnlyObject>(new BinaryOperator("Name", "Read-Only Object"));
            if (readOnlyObject == null) {
                readOnlyObject = ObjectSpace.CreateObject<ReadOnlyObject>();
                readOnlyObject.Name = "Read-Only Object";
                readOnlyObject.Save();
            }


            var irremovableObject = ObjectSpace.FindObject<IrremovableObject>(new BinaryOperator("Name", "Protected Deletion Object"));
            if (irremovableObject == null) {
                irremovableObject = ObjectSpace.CreateObject<IrremovableObject>();
                irremovableObject.Name = "Protected Deletion Object";
                irremovableObject.Save();
            }
            var uncreatableObject = ObjectSpace.FindObject<UncreatableObject>(new BinaryOperator("Name", "Protected Creation Object"));
            if (uncreatableObject == null) {
                uncreatableObject = ObjectSpace.CreateObject<UncreatableObject>();
                uncreatableObject.Name = "Protected Creation Object";
                uncreatableObject.Save();
            }
            var memberLevelSecurityObject = ObjectSpace.FindObject<MemberLevelSecurityObject>(new BinaryOperator("Name", "Member-Level Protected Object"));
            if (memberLevelSecurityObject == null) {
                memberLevelSecurityObject = ObjectSpace.CreateObject<MemberLevelSecurityObject>();
                memberLevelSecurityObject.Name = "Member-Level Protected Object";
                memberLevelSecurityObject.ProtectedContentProperty = "Secure Property Value";
                memberLevelSecurityObject.ReadWriteProperty = "Read Write Property Value";
                memberLevelSecurityObject.ReadOnlyProperty = "Read Only Property Value";
                var obj1 = ObjectSpace.CreateObject<MemberLevelReferencedObject1>();
                obj1.Name = "Referenced Object Type 1";
                obj1.Save();
                memberLevelSecurityObject.ProtectedContentCollection.Add(obj1);
                var obj2 = ObjectSpace.CreateObject<MemberLevelReferencedObject2>();
                obj2.Name = "Referenced Object Type 2";
                obj2.Save();
                memberLevelSecurityObject.ReadOnlyCollection.Add(obj2);
                memberLevelSecurityObject.Save();
            }
            var fullAccessObjectObject = ObjectSpace.FindObject<ObjectLevelSecurityObject>(new BinaryOperator("Name", "Fully Accessible Object"));
            if (fullAccessObjectObject == null) {
                fullAccessObjectObject = ObjectSpace.CreateObject<ObjectLevelSecurityObject>();
                fullAccessObjectObject.Name = "Fully Accessible Object";
                fullAccessObjectObject.Save();
            }
            var protectedContentObjectObject =
                ObjectSpace.FindObject<ObjectLevelSecurityObject>(new BinaryOperator("Name", "Protected Object"));
            if (protectedContentObjectObject == null) {
                protectedContentObjectObject = ObjectSpace.CreateObject<ObjectLevelSecurityObject>();
                protectedContentObjectObject.Name = "Protected Object";
                protectedContentObjectObject.Save();
            }
            var readOnlyObjectObject = ObjectSpace.FindObject<ObjectLevelSecurityObject>(new BinaryOperator("Name", "Read-Only Object"));
            if (readOnlyObjectObject == null) {
                readOnlyObjectObject = ObjectSpace.CreateObject<ObjectLevelSecurityObject>();
                readOnlyObjectObject.Name = "Read-Only Object";
                readOnlyObjectObject.Save();
            }
            var irremovableObjectObject = ObjectSpace.FindObject<ObjectLevelSecurityObject>(new BinaryOperator("Name", "Protected Deletion Object"));
            if (irremovableObjectObject == null) {
                irremovableObjectObject = ObjectSpace.CreateObject<ObjectLevelSecurityObject>();
                irremovableObjectObject.Name = "Protected Deletion Object";
                irremovableObjectObject.Save();
            }
        }
    }
}
