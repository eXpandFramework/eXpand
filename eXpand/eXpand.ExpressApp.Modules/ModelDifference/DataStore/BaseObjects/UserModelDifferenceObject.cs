using System.Linq;
using System.Collections;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.ExpressApp.Attributes;
using eXpand.ExpressApp.ModelDifference.DataStore.Builders;
using eXpand.Persistent.Base;
using System.Collections.Generic;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using DevExpress.ExpressApp.Model.Core;

namespace eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects
{
    [HideFromNewMenu, Custom("Caption", "User Difference"), VisibleInReports(false)]
    public class UserModelDifferenceObject : ModelDifferenceObject
    {
        private bool nonPersistent;

        public UserModelDifferenceObject(Session session)
            : base(session){
        }

        public bool NonPersistent
        {
            get { return nonPersistent; }
            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref nonPersistent, value); }
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            DifferenceType = DifferenceType.User;
        }
        public override ModelDifferenceObject InitializeMembers(string modelId) {
            ModelDifferenceObject modelDifferenceObject = base.InitializeMembers(modelId);
            UserDifferenceObjectBuilder.SetUp(this);
            return modelDifferenceObject;
        }

        public void AssignToCurrentUser()
        {
            var list = ((IList)GetMemberValue("Users"));
            object value =
                ((XPBaseObject)SecuritySystem.CurrentUser).ClassInfo.KeyProperty.GetValue(SecuritySystem.CurrentUser);
            object objectByKey = Session.GetObjectByKey(SecuritySystem.UserType, value);
            list.Add(objectByKey);
        }

        public override ModelApplicationBase[] GetAllLayers()
        {
            var modelDifferenceObjects =
                new List<ModelDifferenceObject>(
                    new QueryRoleModelDifferenceObject(Session).GetActiveModelDifferences(PersistentApplication.UniqueName,null).Cast<ModelDifferenceObject>()) 
            {
                new QueryModelDifferenceObject(Session).GetActiveModelDifference(PersistentApplication.UniqueName,null)
            };

            return GetAllLayers(modelDifferenceObjects.AsEnumerable());
        }
    }
}