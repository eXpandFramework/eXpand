using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.Model;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.AdvBandedView.Model;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Win.Model.NodeUpdaters{
    public class ModelOptionsAdvBandedViewUpdater : IModelNodeUpdater<IModelOptionsAdvBandedView> {
        public void UpdateNode(IModelOptionsAdvBandedView node, IModelApplication application) {
            if (node != null && node.Parent is IModelListView) {
                var gridBandsNode = node.GetNode("GridBands");
                if (gridBandsNode != null&&gridBandsNode.NodeCount>0){
                    var modelListView = (IModelListView)node.Parent ;
                    modelListView.EditorType = typeof(GridListEditor);
                    var bandsLayout = modelListView.BandsLayout ?? ((ModelNode)modelListView).AddNode<IModelBandsLayout>("BandsLayout");
                    bandsLayout.Enable = true;
                    ((IModelBandsLayoutWin)bandsLayout).ShowBands = false;
                    for (int index = gridBandsNode.NodeCount - 1; index >= 0; index--) {
                        var gridBand = gridBandsNode.GetNode(index);
                        var band = ((ModelNode)bandsLayout).AddNode<IModelBand>(gridBand.Id());
                        band.Caption = gridBand.GetValue<string>("Caption");
                        band.Index = gridBand.Index;
                        var columns = modelListView.Columns.Cast<IModelColumnOptionsAdvBandedView>().Where(column 
                            => GridBandMatch(column, gridBand)).ToArray();
                        foreach (var column in columns) {
                            column.SetValue<IModelNode>("GridBand",null);
                            var modelBandedColumn = ((IModelBandedColumnWin)column);
                            modelBandedColumn.OwnerBand = band;
                            if (column.OptionsColumnAdvBandedView != null) {
                                modelBandedColumn.RowIndex = column.OptionsColumnAdvBandedView.GetValue<int>("RowIndex");
                                modelBandedColumn.Index = column.OptionsColumnAdvBandedView.GetValue<int>("ColVIndex");
                            }
                        }
                    }    
                }
            }
        }

        private static bool GridBandMatch(IModelColumnOptionsAdvBandedView column, IModelNode gridBand) {
            var gridBandNode = column.GetValue<IModelNode>("GridBand");
            return gridBandNode != null && gridBandNode.Id() == gridBand.Id();
        }
    }
}
