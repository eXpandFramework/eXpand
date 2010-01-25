using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.ExpressApp.Attributes;
using eXpand.Persistent.Base;
using System.Collections.Generic;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;

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
            DifferenceType = DifferenceType.Role;
        }

        public override DevExpress.ExpressApp.Dictionary GetCombinedModel()
        {
            List<ModelDifferenceObject> modelDifferenceObjects = new List<ModelDifferenceObject>();
            modelDifferenceObjects.Add(new QueryModelDifferenceObject(Session).GetActiveModelDifference(
                PersistentApplication.UniqueName));

            return base.GetCombinedModel(modelDifferenceObjects);
        }
    }
}