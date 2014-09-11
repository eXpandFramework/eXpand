using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.Win.SystemModule {
    public interface IModelClassGuessAutoFilterRowValuesFromFilter : IModelNode {
        [Category(AttributeCategoryNameProvider.Xpand)]
        bool GuessAutoFilterRowValuesFromFilter { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassGuessAutoFilterRowValuesFromFilter), "ModelClass")]
    public interface IModelListViewGuessAutoFilterRowValuesFromFilter : IModelClassGuessAutoFilterRowValuesFromFilter {
    }

    public class GuessAutoFilterRowValuesFromFilterController : ListViewController<ColumnsListEditor>, IModelExtender {


        #region IModelExtender Members
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelClass, IModelClassGuessAutoFilterRowValuesFromFilter>();
            extenders.Add<IModelListView, IModelListViewGuessAutoFilterRowValuesFromFilter>();
        }
        #endregion
        protected override void OnViewControlsCreated() {
            var modelListViewGridViewOptions = ((IModelListViewGuessAutoFilterRowValuesFromFilter)View.Model);
            if (modelListViewGridViewOptions.GuessAutoFilterRowValuesFromFilter) {
                var mainView = ((ColumnsListEditor)View.Editor).GridView();
                mainView.GuessAutoFilterRowValuesFromFilter();
            }
        }

    }
}