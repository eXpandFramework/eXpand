using System;
using System.Collections;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.SystemModule;

namespace eXpand.ExpressApp.Web.SystemModule
{
    public partial class FixWebRecordsNavigationController : ViewController
    {
        private WebRecordsNavigationController controller;
        protected override void OnActivated()
        {
            base.OnActivated();
            controller = Frame.GetController<WebRecordsNavigationController>();
            controller.Deactivating+=ControllerOnDeactivating;
        }

        private void ControllerOnDeactivating(object sender, EventArgs args){
            if (View is ListView)
            {
                if (controller.OrderProviderSource.OrderProvider is StandaloneOrderProvider)
                {
                    var standaloneOrderProvider = (StandaloneOrderProvider)controller.OrderProviderSource.OrderProvider;

                    if (standaloneOrderProvider.GetOrderedObjects().Count == 0)
                    {
                        ArrayList list = null;

                        var listView = (ListView)View;

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

                            controller.OrderProviderSource.OrderProvider = new StandaloneOrderProvider(View.ObjectSpace, list);
                    }
                }
            }

        }

    }
}