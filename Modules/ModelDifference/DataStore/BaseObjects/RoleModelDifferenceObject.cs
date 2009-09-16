using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.ExpressApp.Attributes;
using eXpand.Persistent.Base;

namespace eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects{
    [HideFromNewMenu]
    [VisibleInReports(false)]
    [Custom(ClassInfoNodeWrapper.CaptionAttribute, "Role Difference")]
    public class RoleModelDifferenceObject : ModelDifferenceObject
    {
        
        public RoleModelDifferenceObject(Session session) : base(session)
        {
            
        }


        public override void AfterConstruction()
        {
            base.AfterConstruction();
            differenceType = DifferenceType.Role;
        }
    }
}