using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule {
    public interface IModelViewRollBackObjectChangesOnDetailViewQueryCanChangeCurrentObject : IModelNode
    {
        [DefaultValue(true)]
        bool RollBackObjectChangesOnDetailViewQueryCanChangeCurrentObject { get; set; }
    }

    public class RollBackObjectChangesOnDetailViewQueryCanChangeCurrentObjectController : WinDetailViewController
    {
        protected override void OnViewQueryCanChangeCurrentObject(System.ComponentModel.CancelEventArgs e)
        {
            if (((IModelViewRollBackObjectChangesOnDetailViewQueryCanChangeCurrentObject)View.Model)
                .RollBackObjectChangesOnDetailViewQueryCanChangeCurrentObject)
                base.OnViewQueryCanChangeCurrentObject(e);
        }
   
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelView, IModelViewRollBackObjectChangesOnDetailViewQueryCanChangeCurrentObject>();
        }
    }
}