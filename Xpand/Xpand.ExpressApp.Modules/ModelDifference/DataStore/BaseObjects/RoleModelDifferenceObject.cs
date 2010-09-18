using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System.Collections.Generic;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base;

namespace Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects{

    [HideFromNewMenu, VisibleInReports(false), Custom("Caption", "Role Difference")]
    public class RoleModelDifferenceObject : ModelDifferenceObject
    {
        public RoleModelDifferenceObject(Session session) : base(session){       
        }

        public override void AfterConstruction(){
            base.AfterConstruction();
            DifferenceType = DifferenceType.Role;
        }

        public override IEnumerable<ModelApplicationBase> GetAllLayers(ModelApplicationBase master)
        {
            return GetAllLayers(new QueryModelDifferenceObject(Session).GetActiveModelDifferences(PersistentApplication.UniqueName, null), master);
        }
    }
}