using System;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.ListEditors;

namespace eXpand.ExpressApp.SystemModule {
    public interface IModelListViewMainViewOptionsBase : IModelNode {
    }

    public abstract class GridOptionsController:OptionsController<IModelListView> {
        protected override Type GetExtenderType() {
            return OptionsModelSynchronizer<object, IModelNode, IModelListViewMainViewOptionsBase>.GetModelOptionsType();
        }
    }
}