using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public interface IModelViewRollBackObjectChangesOnDetailViewQueryCanChangeCurrentObject : IModelNode
    {
        [Category("eXpand")]
        [DefaultValue(true)]
        bool RollBackObjectChangesOnDetailViewQueryCanChangeCurrentObject { get; set; }
    }

    public class RollBackObjectChangesOnDetailViewQueryCanChangeCurrentObjectController : WinDetailViewController, IModelExtender
    {
        protected override void OnViewQueryCanChangeCurrentObject(CancelEventArgs e)
        {
            if (((IModelViewRollBackObjectChangesOnDetailViewQueryCanChangeCurrentObject)View.Model)
                .RollBackObjectChangesOnDetailViewQueryCanChangeCurrentObject)
                base.OnViewQueryCanChangeCurrentObject(e);
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelView, IModelViewRollBackObjectChangesOnDetailViewQueryCanChangeCurrentObject>();
        }
    }
}