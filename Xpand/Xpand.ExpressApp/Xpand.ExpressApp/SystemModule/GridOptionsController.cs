using System;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.ListEditors;

namespace Xpand.ExpressApp.SystemModule {
    public interface IModelListViewMainViewOptionsBase : IModelNode {
    }

    public abstract class GridOptionsController:OptionsController<IModelListView> {
        protected override Type GetExtenderType() {
            return OptionsModelSynchronizer<object, IModelNode, IModelListViewMainViewOptionsBase>.GetModelOptionsType();
        }
    }
}