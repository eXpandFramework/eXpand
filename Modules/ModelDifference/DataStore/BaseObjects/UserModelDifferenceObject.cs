using System.Collections;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.Builders;
using eXpand.Persistent.Base;

namespace eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects{
    [VisibleInReports(false)]
    [Custom(ClassInfoNodeWrapper.CaptionAttribute, "User Difference")]
    public class UserModelDifferenceObject : ModelDifferenceObject
    {
        private bool nonPersistent;


        public UserModelDifferenceObject(Session session) : base(session)
        {
        }


        public bool NonPersistent
        {
            get { return nonPersistent; }
            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref nonPersistent, value); }
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            differenceType = DifferenceType.User;
        }

        public override ModelDifferenceObject InitializeMembers(string applicationName){
            base.InitializeMembers(applicationName);
            UserDifferenceObjectBuilder.SetUp(this);
            return this;
        }

        public void AssignToCurrentUser()
        {
            var list = ((IList)GetMemberValue("Users"));
            object value =
                ((XPBaseObject)SecuritySystem.CurrentUser).ClassInfo.KeyProperty.GetValue(SecuritySystem.CurrentUser);
            object objectByKey = Session.GetObjectByKey(SecuritySystem.UserType, value);
            list.Add(objectByKey);

        }

    }
}