using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.Attributes;
using Xpand.ExpressApp.ModelDifference.DataStore.Builders;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;
using Xpand.Persistent.Base;

namespace Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects {
    [HideFromNewMenu, Custom("Caption", "User Difference"), VisibleInReports(false)]
    public class UserModelDifferenceObject : ModelDifferenceObject {
        private bool nonPersistent;

        public UserModelDifferenceObject(Session session)
            : base(session) {
        }

        public bool NonPersistent {
            get { return nonPersistent; }
            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref nonPersistent, value); }
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            DifferenceType = DifferenceType.User;
        }
        public override ModelDifferenceObject InitializeMembers(string name, string applicationTitle, string uniqueName) {
            ModelDifferenceObject modelDifferenceObject = base.InitializeMembers(name, applicationTitle, uniqueName);
            UserDifferenceObjectBuilder.SetUp(this);
            return modelDifferenceObject;
        }

        public void AssignToCurrentUser() {
            var list = ((IList)GetMemberValue("Users"));
            object value =
                ((XPBaseObject)SecuritySystem.CurrentUser).ClassInfo.KeyProperty.GetValue(SecuritySystem.CurrentUser);
            object objectByKey = Session.GetObjectByKey(SecuritySystem.UserType, value);
            list.Add(objectByKey);
        }
        public override IEnumerable<ModelApplicationBase> GetAllLayers(ModelApplicationBase master) {
            IQueryable<ModelDifferenceObject> differenceObjects = new QueryRoleModelDifferenceObject(Session).GetActiveModelDifferences(PersistentApplication.UniqueName, null).Cast<ModelDifferenceObject>();
            differenceObjects = differenceObjects.Concat(new QueryModelDifferenceObject(Session).GetActiveModelDifferences(PersistentApplication.UniqueName, null));
            return GetAllLayers(differenceObjects, master);
        }
    }
}