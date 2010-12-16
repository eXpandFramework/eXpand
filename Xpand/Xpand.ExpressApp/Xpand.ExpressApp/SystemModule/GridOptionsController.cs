using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.ListEditors;

namespace Xpand.ExpressApp.SystemModule {
    public interface IModelListViewMainViewOptionsBase : IModelNode {
    }

    public abstract class GridOptionsController : OptionsController {
        protected override List<ModelExtenderPair> GetModelExtenderPairs() {
            return new List<ModelExtenderPair> { new ModelExtenderPair(typeof(IModelListView), OptionsModelSynchronizer<object, IModelNode, IModelListViewMainViewOptionsBase>.GetModelOptionsType()) };
        }
    }
}