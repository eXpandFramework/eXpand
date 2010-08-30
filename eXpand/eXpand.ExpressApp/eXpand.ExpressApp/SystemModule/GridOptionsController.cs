using System;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.ListEditors;

namespace eXpand.ExpressApp.SystemModule {
    public interface IModelListViewMainViewOptionsBase : IModelNode {
    }

    public abstract class GridOptionsController<ControlType, OptionsInterfaceType> :
        OptionsController<ControlType, OptionsInterfaceType, IModelListView> where OptionsInterfaceType : IModelNode {
        protected override Type GetExtenderType() {
            return OptionsModelSynchronizer<object, IModelNode, IModelListViewMainViewOptionsBase>.GetModelOptionsType();
        }
    }
}