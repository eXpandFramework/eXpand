using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.MasterDetail.Win {
    public class ListViewBuilder {
        readonly ModelDetailRelationCalculator _modelDetailRelationCalculator;
        readonly ObjectSpace _objectSpace;
        public ListViewBuilder(ModelDetailRelationCalculator modelDetailRelationCalculator,ObjectSpace objectSpace) {
            _modelDetailRelationCalculator = modelDetailRelationCalculator;
            _objectSpace = objectSpace;
        }

        public DevExpress.ExpressApp.ListView CreateListView(IModelListView childModelListView, int rowHandle, int relationIndex)
        {
            IModelMember relationModelMember = _modelDetailRelationCalculator.GetRelationModelMember(rowHandle, relationIndex);
            return CreateListView(childModelListView, relationModelMember);
        }

        DevExpress.ExpressApp.ListView CreateListView(IModelListView childModelListView, IModelMember relationModelMember)
        {
            var propertyCollectionSource = ModuleBase.Application.CreatePropertyCollectionSource(_objectSpace, childModelListView.ModelClass.TypeInfo.Type, null, relationModelMember.MemberInfo, childModelListView.Id);
            return ModuleBase.Application.CreateListView(childModelListView, propertyCollectionSource, false);
        }

    }
}