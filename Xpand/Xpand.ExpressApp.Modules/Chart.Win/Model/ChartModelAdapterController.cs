using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.PivotGrid.Win;
using DevExpress.XtraCharts;
using System.Linq;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Chart.Win.Model {
    public class ChartModelAdapterController : ModelAdapterController, IModelExtender {

        protected override void OnActivated() {
            base.OnActivated();
            var listView = View as ListView;
            var types = new[] { typeof(PivotGridListEditor), typeof(DevExpress.ExpressApp.Chart.Win.ChartListEditor) };
            if (listView != null && listView.Editor != null && types.Any(type => type.IsInstanceOfType(listView.Editor))) {
                listView.Editor.ControlsCreated += PivotGridListEditorOnControlsCreated;
            }
        }

        void PivotGridListEditorOnControlsCreated(object sender, EventArgs eventArgs) {
            new PivotGridListEditorChartModelSynchronizer(((ListView)View).Editor, Application).ApplyModel();
        }

        #region Implementation of IModelExtender
        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelListView, IModelListViewOptionsChart>();

            var builder = new InterfaceBuilder(extenders);
            XafTypesInfo.Instance.LoadTypes(typeof(Diagram).Assembly);

            var assembly = builder.Build(CreateBuilderData(), GetPath(typeof(ChartControl).Name));

            XafTypesInfo.Instance.LoadTypes(assembly);

            builder.ExtendInteface<IModelOptionsChart, ChartControl>(assembly);
            builder.ExtendInteface<IModelSeries, Series>(assembly);

            foreach (var typeInfo in DiagramDescendants()) {
                var type = Type.GetType(typeof(IModelChartDiargam).Namespace + ".IModelChart" + typeInfo.Name, true);
                builder.ExtendInteface(type, typeInfo.Type, assembly);
            }

        }

        IEnumerable<ITypeInfo> DiagramDescendants() {
            var repositoryItemDescendants = XafTypesInfo.Instance.FindTypeInfo(typeof(Diagram)).Descendants;
            return repositoryItemDescendants.Where(info => info.OwnMembers.Any(memberInfo => memberInfo.IsPublic));
        }

        IEnumerable<InterfaceBuilderData> CreateBuilderData() {
            yield return new InterfaceBuilderData(typeof(ChartControl)) {
                Act = info => info.DXFilter()
            };
            yield return new InterfaceBuilderData(typeof(Series)) {
                Act = info => info.DXFilter(typeof(object))
            };
            foreach (var diagramDescendant in DiagramDescendants()) {
                yield return new InterfaceBuilderData(diagramDescendant.Type) {
                    Act = info => info.DXFilter(),
                    RootBaseInterface = typeof(IModelChartDiargam),
                    IsAbstract = true
                };

            }
        }
        #endregion
    }
}
