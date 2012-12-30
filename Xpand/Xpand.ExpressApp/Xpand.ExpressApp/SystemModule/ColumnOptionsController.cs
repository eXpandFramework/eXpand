using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.ListEditors;

namespace Xpand.ExpressApp.SystemModule {
    public interface IModelColumnOptionsBase : IModelNode {
    }
    [Obsolete("", true)]
    public abstract class ColumnOptionsController : OptionsController {
        protected override List<ModelExtenderPair> GetModelExtenderPairs() {
            return new List<ModelExtenderPair> { new ModelExtenderPair(typeof(IModelColumn), OptionsModelSynchronizer<object, IModelNode, IModelColumnOptionsBase>.GetModelOptionsType()) };
        }
    }
}