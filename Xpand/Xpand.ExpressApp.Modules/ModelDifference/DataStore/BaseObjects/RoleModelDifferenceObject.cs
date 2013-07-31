using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.Attributes;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;
using Xpand.Persistent.Base;

namespace Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects {
    [HideFromNewMenu, VisibleInReports(false), ModelDefault("Caption", "Role Difference")]
    [CloneView(CloneViewType.DetailView, "RDO_DetailView",true)]
    public class RoleModelDifferenceObject : ModelDifferenceObject {

    
        public RoleModelDifferenceObject(Session session) : base(session) {
        }


        public override void AfterConstruction() {
            base.AfterConstruction();
            DifferenceType = DifferenceType.Role;
        }

        public override IEnumerable<ModelApplicationBase> GetAllLayers(ModelApplicationBase master) {
            return GetAllLayers(new QueryModelDifferenceObject(Session).GetActiveModelDifferences(PersistentApplication.UniqueName,null), master);
        }

    }
}