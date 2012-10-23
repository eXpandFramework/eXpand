using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.LayoutView.Model {
    public class LayoutViewModelAdapterController : ColumnViewModelAdapterController {

        protected override void ExtendInterfaces(ModelInterfaceExtenders extenders) {

            extenders.Add<IModelListView, IModelListViewOptionsLayoutView>();
            extenders.Add<IModelColumn, IModelColumnOptionsLayoutView>();

            var builder = new InterfaceBuilder(extenders);
            var assembly = BuildAssembly(builder, typeof(XafLayoutView), typeof(XafLayoutViewColumn));


            builder.ExtendInteface<IModelOptionsLayoutView, XafLayoutView>(assembly);
            builder.ExtendInteface<IModelOptionsColumnLayoutView, XafLayoutViewColumn>(assembly);
        }

    }

}