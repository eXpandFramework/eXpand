using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using FeatureCenter.Base;
using Xpand.ExpressApp.FilterDataStore.Providers;
using Xpand.ExpressApp.PropertyEditors;
using Xpand.Persistent.Base.General.CustomAttributes;

namespace FeatureCenter.Module {
    [DefaultClassOptions]
    public class CLASSNAME : BaseObject {
        public CLASSNAME(Session session)
            : base(session) {
        }
        private string _propertyName;
        [PropertyEditor(typeof(IStringLookupPropertyEditor))]
        public string PropertyName {
            get { return _propertyName; }
            set {
                _propertyName = value;
            }
        }
        
    }

    public class Updater : Xpand.Persistent.BaseImpl.Updater {
        protected bool initializeSecurity;

        public Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override object EnsureUserExists(string userName, string firstName, object customizableRole) {
            var ensureUserExists = base.EnsureUserExists(userName, firstName, customizableRole);
            if (!IsNewSecuritySystem) {
                if (((IUser)ensureUserExists).UserName == Admin) {
                    ((User)ensureUserExists).SetPassword("Admin");
                    ObjectSpace.CommitChanges();
                    UserFilterProvider.UpdaterUserKey = ((User)ensureUserExists).Oid;
                }
            } else {
                if (((SecurityUser)ensureUserExists).UserName == Admin) {
                    ((SecurityUser)ensureUserExists).SetPassword("Admin");
                    ObjectSpace.CommitChanges();
                    UserFilterProvider.UpdaterUserKey = ((SecurityUser)ensureUserExists).Oid;
                }
            }
            return ensureUserExists;
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();

            InitializeSecurity();

            new DummyDataBuilder((XPObjectSpace)ObjectSpace).CreateObjects();
            var workflowServiceUser = ObjectSpace.FindObject(SecuritySystem.UserType, new BinaryOperator("UserName", "WorkflowService"));
            if (workflowServiceUser == null) {
                CriteriaOperator criteriaOperator = CriteriaOperator.Parse("Name=?", SecurityStrategy.AdministratorRoleName);
                CreateworkflowServiceUser(ObjectSpace.FindObject<Role>(criteriaOperator));
                ObjectSpace.CommitChanges();


                var updaters = ReflectionHelper.FindTypeDescendants(XafTypesInfo.CastTypeToTypeInfo(typeof(FCUpdater)));
                foreach (var findTypeDescendant in updaters) {
                    var updater = (FCUpdater)Activator.CreateInstance(findTypeDescendant.Type, ObjectSpace, CurrentDBVersion, this);
                    updater.UpdateDatabaseAfterUpdateSchema();
                }
            }

        }


        private void CreateworkflowServiceUser(Role role) {
            var workflowServiceUser = ObjectSpace.CreateObject<User>();
            workflowServiceUser.UserName = "WorkflowService";
            workflowServiceUser.FirstName = "WorkflowService";
            workflowServiceUser.Roles.Add(role);
        }
    }
}
