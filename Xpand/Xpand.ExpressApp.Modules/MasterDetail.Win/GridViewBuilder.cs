using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Views.Grid;
using Xpand.ExpressApp.MasterDetail.Logic;
using Xpand.ExpressApp.Win.ListEditors;

namespace Xpand.ExpressApp.MasterDetail.Win {
    public class GridViewBuilder {
        readonly XafApplication _xafApplication;
        readonly IObjectSpace _objectSpace;
        readonly Frame _masterFrame;

        public GridViewBuilder(XafApplication xafApplication, IObjectSpace objectSpace, Frame masterFrame) {
            _xafApplication = xafApplication;
            _objectSpace = objectSpace;
            _masterFrame = masterFrame;
        }

        public IMasterDetailXafGridView GetLevelDefaultView(IMasterDetailXafGridView masterGridView, int rowHandle, int relationIndex, IModelListView masterModelListView, List<MasterDetailRuleInfo> masterDetailRules) {
            return GetLevelDefaultViewCore(masterModelListView, masterGridView, rowHandle, relationIndex, masterDetailRules);
        }

        IMasterDetailXafGridView GetLevelDefaultViewCore(IModelListView masterModelListView, IMasterDetailXafGridView masterGridView, int rowHandle, int relationIndex, List<MasterDetailRuleInfo> masterDetailRules) {
            var modelDetailRelationCalculator = new ModelDetailRelationCalculator(masterModelListView, masterGridView, masterDetailRules);
            bool isRelationSet = modelDetailRelationCalculator.IsRelationSet(rowHandle, relationIndex);
            if (isRelationSet) {
                IModelListView childModelListView = modelDetailRelationCalculator.GetChildModelListView(rowHandle, relationIndex);
                ListView listView = GetListView(modelDetailRelationCalculator, rowHandle, relationIndex, childModelListView);
                IMasterDetailXafGridView defaultXpandXafGridView = null;
                EventHandler[] listViewOnControlsCreated = { null };
                listViewOnControlsCreated[0] = (sender, args) => {
                    defaultXpandXafGridView = ((IMasterDetailGridListEditor)((ListView)sender).Editor).GridView;
                    listView.ControlsCreated -= listViewOnControlsCreated[0];
                };
                listView.ControlsCreated += listViewOnControlsCreated[0];
                listView.CreateControls();
                return defaultXpandXafGridView;
            }
            return null;
        }

        ListView GetListView(ModelDetailRelationCalculator modelDetailRelationCalculator, int rowHandle, int relationIndex, IModelListView childModelListView) {
            var listViewBuilder = new ListViewBuilder(modelDetailRelationCalculator, _objectSpace);
            return listViewBuilder.CreateListView(childModelListView, rowHandle, relationIndex, _xafApplication);
        }


        public void ModifyInstanceGridView(IMasterDetailXafGridView masterGridView, int rowHandle, int relationIndex, IModelListView masterModelListView, List<MasterDetailRuleInfo> masterDetailRules) {
            var modelDetailRelationCalculator = new ModelDetailRelationCalculator(masterModelListView, masterGridView, masterDetailRules);
            bool isRelationSet = modelDetailRelationCalculator.IsRelationSet(rowHandle, relationIndex);
            if (isRelationSet) {
                IModelListView childModelListView = modelDetailRelationCalculator.GetChildModelListView(rowHandle, relationIndex);
                Window window = _xafApplication.CreateWindow(TemplateContext.View, null, true, false);
                ListView listView = GetListView(modelDetailRelationCalculator, rowHandle, relationIndex, childModelListView);
                var detailXafGridView = (IMasterDetailXafGridView)masterGridView.GetDetailView(rowHandle, relationIndex);
                ((IMasterDetailGridListEditor)listView.Editor).CustomGridViewCreate +=
                    (o, eventArgs) => {
                        ((IMasterDetailGridListEditor)o).DataSource = detailXafGridView.DataSource;
                        eventArgs.Handled = true;
                        eventArgs.GridView = (GridView)detailXafGridView;
                        eventArgs.GridControl.DataSource = detailXafGridView.DataSource;
                    };

                EventHandler[] listViewOnControlsCreated = { null };
                listViewOnControlsCreated[0] = (sender, args) => {
                    detailXafGridView.MasterFrame = masterGridView.MasterFrame ?? _masterFrame;
                    detailXafGridView.Window = window;
                    detailXafGridView.GridControl = masterGridView.GridControl;
                    listView.ControlsCreated -= listViewOnControlsCreated[0];
                };
                listView.ControlsCreated += listViewOnControlsCreated[0];
                ((PropertyCollectionSource)listView.CollectionSource).MasterObject = masterGridView.GetRow(rowHandle);
                window.SetView(listView);
            }
        }


    }
}