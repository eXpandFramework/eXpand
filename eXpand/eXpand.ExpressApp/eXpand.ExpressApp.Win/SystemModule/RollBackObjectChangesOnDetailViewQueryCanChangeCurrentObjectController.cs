using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public interface IModelClassRollBackObjectChangesOnDetailViewQueryCanChangeCurrentObject : IModelNode
    {
        [Category("eXpand")]
        [DefaultValue(true)]
        [Description("If set to false when the view queries if it can change current object all changes will remain in transaction, thus allowing to save them at a later time")]
        bool RollBackObjectChangesOnDetailViewQueryCanChangeCurrentObject { get; set; }
    }
    public interface IModelViewRollBackObjectChangesOnDetailViewQueryCanChangeCurrentObject : IModelNode
    {
        [Category("eXpand")]
        [DefaultValue(true)]
        [Description("If set to false when the view queries if it can change current object all changes will remain in transaction, thus allowing to save them at a later time")]
        [ModelValueCalculator("((IModelClassRollBackObjectChangesOnDetailViewQueryCanChangeCurrentObject)ModelClass)", "RollBackObjectChangesOnDetailViewQueryCanChangeCurrentObject")]
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
            extenders.Add<IModelClass, IModelClassRollBackObjectChangesOnDetailViewQueryCanChangeCurrentObject>();
            extenders.Add<IModelView, IModelViewRollBackObjectChangesOnDetailViewQueryCanChangeCurrentObject>();
        }
    }
}