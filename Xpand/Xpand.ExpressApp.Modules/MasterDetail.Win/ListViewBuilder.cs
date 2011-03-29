using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.MasterDetail.Win {
    public class ListViewBuilder {
        readonly ModelDetailRelationCalculator _modelDetailRelationCalculator;
        readonly IObjectSpace _objectSpace;
        public ListViewBuilder(ModelDetailRelationCalculator modelDetailRelationCalculator, IObjectSpace objectSpace) {
            _modelDetailRelationCalculator = modelDetailRelationCalculator;
            _objectSpace = objectSpace;
        }

        public ListView CreateListView(IModelListView childModelListView, int rowHandle, int relationIndex,XafApplication application) {
            IModelMember relationModelMember = _modelDetailRelationCalculator.GetRelationModelMember(rowHandle, relationIndex);
            return CreateListView(childModelListView, relationModelMember,application);
        }

        ListView CreateListView(IModelListView childModelListView, IModelMember relationModelMember,XafApplication application) {
            var propertyCollectionSource = application.CreatePropertyCollectionSource(_objectSpace, childModelListView.ModelClass.TypeInfo.Type, null, relationModelMember.MemberInfo, childModelListView.Id);
            return application.CreateListView(childModelListView, propertyCollectionSource, false);
        }

    }
}