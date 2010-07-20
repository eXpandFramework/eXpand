using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using eXpand.ExpressApp.SystemModule;
using XafGridView = eXpand.ExpressApp.Win.ListEditors.XafGridView;

namespace eXpand.ExpressApp.Win.SystemModule {
    public interface IModelClassGroupLevelExpandIndex : IModelNode
    {
        [DefaultValue(-1)]
        [Category("eXpand")]
        [Description("Expand all groups of a gridview up to this level")]
        int GroupLevelExpandIndex { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassGroupLevelExpandIndex), "ModelClass")]
    public interface IModelListViewGroupLevelExpandIndex : IModelClassGroupLevelExpandIndex
    {
    }
    public class GroupLevelExpandIndexController:ListViewController<GridListEditor>,IModelExtender
    {
        XafGridView _xafGridView;

        protected override void OnViewControllersActivated()
        {
            base.OnViewControllersActivated();
            if (((IModelListViewGroupLevelExpandIndex)View.Model).GroupLevelExpandIndex>-1) {
                _xafGridView = ((ListEditors.GridListEditor)View.Editor).GridView;
                GroupLevelExpandIndex();
            }
        }
        void GroupLevelExpandIndex()
        {
            int groupLevel = ((IModelListViewGroupLevelExpandIndex)View.Model).GroupLevelExpandIndex;
            if (groupLevel > 0){
                if (_xafGridView.GroupCount == groupLevel)
                    for (int i = -1; ; i--){
                        if (!_xafGridView.IsValidRowHandle(i)) return;
                        if (_xafGridView.GetRowLevel(i) < groupLevel - 1)
                            _xafGridView.SetRowExpanded(i, true);
                    }
            }

        }


        #region IModelExtender Members

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelClass,IModelClassGroupLevelExpandIndex>();
            extenders.Add<IModelListView, IModelListViewGroupLevelExpandIndex>();
        }

        #endregion
    }
}