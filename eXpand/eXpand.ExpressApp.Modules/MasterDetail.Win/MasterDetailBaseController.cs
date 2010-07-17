using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.MasterDetail.Win {
    public abstract class MasterDetailBaseController : ListViewController<global::eXpand.ExpressApp.Win.ListEditors.GridListEditor>
    {
        protected override void OnActivated()
        {
            base.OnActivated();
//            Active["ModelEnabled"] = ((IModelListViewMasterDetail) View.Model).MasterDetail.Enable;
        }
    }
}