using System;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.Persistent.Base;

namespace eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects{
    [VisibleInReports(false)]
    [Custom(ClassInfoNodeWrapper.CaptionAttribute, "Role Difference")]
    public class RoleModelDifferenceObject : ModelDifferenceObject
    {
        private readonly QueryUserModelDifferenceObject _queryUserModelDifferenceObject;
        public RoleModelDifferenceObject(Session session) : base(session)
        {
            _queryUserModelDifferenceObject=new QueryUserModelDifferenceObject(Session);
        }


        public override void AfterConstruction()
        {
            base.AfterConstruction();
            differenceType = DifferenceType.Role;
        }
    }
}