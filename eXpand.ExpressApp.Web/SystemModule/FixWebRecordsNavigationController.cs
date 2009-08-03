using System;
using System.Collections;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.SystemModule;

namespace eXpand.ExpressApp.Web.SystemModule
{
    public partial class FixWebRecordsNavigationController : WebRecordsNavigationController
    {
        protected override void OnDeactivating()
        {
            base.OnDeactivating();

            if (View is ListView)
            {
                if (OrderProviderSource.OrderProvider is StandaloneOrderProvider)
                {
                    var standaloneOrderProvider = (StandaloneOrderProvider) OrderProviderSource.OrderProvider;

                    if (standaloneOrderProvider.GetOrderedObjects().Count == 0)
                    {
                        ArrayList list = null;

                        var listView = (ListView) View;

                        if (listView.Editor is ASPxGridListEditor)
                        {
                            var gridListEditor = listView.Editor as ASPxGridListEditor;

                            if (gridListEditor.Grid is ASPxGridControl)
                            {
                                ASPxGridControl grid = gridListEditor.Grid;

                                list = new ArrayList();

                                int maxRow = Math.Min(grid.VisibleStartIndex + grid.SettingsPager.PageSize,
                                                      grid.VisibleRowCount);

                                for (int i = grid.VisibleStartIndex; i < maxRow; i++)
                                {
                                    object row = grid.GetRow(i);

                                    if (!list.Contains(row))
                                    {
                                        list.Add(row);
                                    }
                                }
                            }
                        }

                        if (list != null)

                            OrderProviderSource.OrderProvider = new StandaloneOrderProvider(View.ObjectSpace, list);
                    }
                }
            }
        }
    }
}