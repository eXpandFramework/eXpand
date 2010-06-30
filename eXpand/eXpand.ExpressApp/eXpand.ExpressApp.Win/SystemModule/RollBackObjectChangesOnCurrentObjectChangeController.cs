using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public interface IModelClassRollBackObjectChangesOnCurrentObjectChange : IModelNode
    {
        [Category("eXpand")]
        [DefaultValue(true)]
        [Description("If set to false when the view queries if it can change current object all changes will remain in transaction, thus allowing to save them at a later time")]
        bool RollBackObjectChangesOnCurrentObjectChange { get; set; }
    }
    public interface IModelViewRollBackChangesBeforeViewChanges : IModelNode
    {
        [Category("eXpand")]
        [DefaultValue(true)]
        [Description("If set to false when the view queries if it can change current object all changes will remain in transaction, thus allowing to save them at a later time")]
        [ModelValueCalculator("((IModelClassRollBackObjectChangesOnCurrentObjectChange)ModelClass)", "RollBackObjectChangesOnCurrentObjectChange")]
        bool RollBackObjectChangesOnCurrentObjectChange { get; set; }
    }

    public class RollBackObjectChangesOnCurrentObjectChangeController : WinDetailViewController, IModelExtender
    {
        protected override void OnViewQueryCanChangeCurrentObject(CancelEventArgs e)
        {
            if (((IModelViewRollBackChangesBeforeViewChanges)View.Model)
                .RollBackObjectChangesOnCurrentObjectChange)
                base.OnViewQueryCanChangeCurrentObject(e);
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelClass, IModelClassRollBackObjectChangesOnCurrentObjectChange>();
            extenders.Add<IModelView, IModelViewRollBackChangesBeforeViewChanges>();
        }
    }
}