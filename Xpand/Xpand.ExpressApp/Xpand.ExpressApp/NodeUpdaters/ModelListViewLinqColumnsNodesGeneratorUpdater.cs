using System;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using Xpand.ExpressApp.Model;
using Xpand.ExpressApp.Core;

namespace Xpand.ExpressApp.NodeUpdaters {
    public class ModelListViewLinqColumnsNodesGeneratorUpdater : ModelNodesGeneratorUpdater<ModelListViewColumnsNodesGenerator>
    {
        public override void UpdateNode(ModelNode node)
        {
            var linqViewInfo = node.Parent as IModelListViewLinq;
            if (linqViewInfo != null && !string.IsNullOrEmpty(linqViewInfo.XPQueryMethod))
            {
                var listViewInfo = (IModelListView)linqViewInfo;
                string[] columns = LinqCollectionSourceHelper.GetDisplayableProperties(listViewInfo.ModelClass.TypeInfo.Type, linqViewInfo.XPQueryMethod);
                if (columns != null)
                {
                    if (listViewInfo.Columns == null){
                        listViewInfo.AddNode<IModelColumns>("Columns");
                    }
                    for (int i = listViewInfo.Columns.Count; i > 0; ){
                        i--;
                        IModelColumn col = listViewInfo.Columns[i];
                        if (Array.IndexOf(columns, col.Id) < 0){
                            listViewInfo.Columns.Remove(col);
                        }
                    }
                    foreach (string column in columns){
                        var col = listViewInfo.Columns.GetNode<IModelColumn>(column);
                        if (col == null){
                            col = listViewInfo.Columns.AddNode<IModelColumn>(column);
                            col.PropertyName = column;
                        }
                    }
                }
            }
        }
    }
}