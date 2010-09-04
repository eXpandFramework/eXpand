using System;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.ListEditors;

namespace Xpand.ExpressApp.SystemModule {
    public interface IModelColumnOptionsBase : IModelNode {
    }

    public abstract class ColumnOptionsController :OptionsController<IModelColumn> {
        protected override Type GetExtenderType() {
            return OptionsModelSynchronizer<object, IModelNode, IModelColumnOptionsBase>.GetModelOptionsType();
        }
    }
}