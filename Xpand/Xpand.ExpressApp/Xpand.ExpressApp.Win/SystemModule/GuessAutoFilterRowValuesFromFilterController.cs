using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Win.ListEditors;

namespace Xpand.ExpressApp.Win.SystemModule {
    public interface IModelClassGuessAutoFilterRowValuesFromFilter : IModelNode {
        [Category("eXpand")]
        bool GuessAutoFilterRowValuesFromFilter { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassGuessAutoFilterRowValuesFromFilter), "ModelClass")]
    public interface IModelListViewGuessAutoFilterRowValuesFromFilter : IModelClassGuessAutoFilterRowValuesFromFilter {
    }

    public class GuessAutoFilterRowValuesFromFilterController : ListViewController<GridListEditor>, IModelExtender {


        #region IModelExtender Members
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelClass, IModelClassGuessAutoFilterRowValuesFromFilter>();
            extenders.Add<IModelListView, IModelListViewGuessAutoFilterRowValuesFromFilter>();
        }
        #endregion
        protected override void OnViewControlsCreated() {
            var modelListViewGridViewOptions = ((IModelListViewGuessAutoFilterRowValuesFromFilter)View.Model);
            if (modelListViewGridViewOptions.GuessAutoFilterRowValuesFromFilter) {
                XafGridView mainView = ((GridListEditor)View.Editor).GridView;
                mainView.GuessAutoFilterRowValuesFromFilter();
            }
        }

    }
}