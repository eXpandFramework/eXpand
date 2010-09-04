using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Xpo;
using Xpand.ExpressApp.SystemModule;

namespace FeatureCenter.Module.ListViewControl.PropertyPathFilters {
    public class PropertyPathFiltersNodeUpdater : ModelNodesGeneratorUpdater<ModelPropertyPathFiltersNodesGenerator>
    {
        readonly XafApplication _xafApplication;

        public PropertyPathFiltersNodeUpdater(XafApplication xafApplication) {
            _xafApplication = xafApplication;
        }

        public override void UpdateNode(ModelNode node) {
            
            var modelPropertyPathFilters = ((IModelPropertyPathFilters)node);
            if (IsDefaultPPCustomerListView(modelPropertyPathFilters)) {
                var modelPropertyPathFilter = modelPropertyPathFilters.AddNode<IModelPropertyPathFilter>("Filter by OrderLineDate");
                if (_xafApplication != null) {
                    Session session = _xafApplication.ObjectSpaceProvider.CreateObjectSpace().Session;
                    modelPropertyPathFilter.PropertyPathFilter =new XPQuery<PPOrderLine>(session).TransformExpression(
                        line => line.OrderLineDate > GetDateTime(session)).ToString();
                }
                modelPropertyPathFilter.PropertyPath = string.Format("{0}.{1}", "Orders", "OrderLines");
                modelPropertyPathFilter.PropertyPathListViewId =node.Application.BOModel.GetClass(typeof (PPOrderLine)).DefaultListView;
            }
        }

        DateTime GetDateTime(Session session) {
            DateTime dateTime = new XPQuery<PPOrderLine>(session).Max(line => line.OrderLineDate);
            return dateTime.AddDays(-10);
        }

        bool IsDefaultPPCustomerListView(IModelPropertyPathFilters modelPropertyPathFilters) {
            return modelPropertyPathFilters.Parent==modelPropertyPathFilters.Application.BOModel.GetClass(typeof(PPCustomer)).DefaultListView;
        }


    }
}