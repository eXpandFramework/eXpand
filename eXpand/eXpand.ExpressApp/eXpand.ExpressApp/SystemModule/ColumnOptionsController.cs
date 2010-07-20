using System;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.ListEditors;

namespace eXpand.ExpressApp.SystemModule {
    public interface IModelColumnOptionsBase : IModelNode {
    }

    public abstract class ColumnOptionsController<ControlType, OptionsInterfaceType> :
        OptionsController<ControlType, OptionsInterfaceType, IModelColumn> where OptionsInterfaceType : IModelNode {
        protected override Type GetExtenderType() {
            return OptionsModelSynchronizer<object, IModelNode, IModelColumnOptionsBase>.GetModelOptionsType();
        }
    }
}