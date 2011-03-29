using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
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

        public XpandXafGridView GetLevelDefaultView(XpandXafGridView masterGridView, int rowHandle, int relationIndex, IModelListView masterModelListView, List<IMasterDetailRule> masterDetailRules) {
            return GetLevelDefaultViewCore(masterModelListView, masterGridView, rowHandle, relationIndex, masterDetailRules);
        }

        XpandXafGridView GetLevelDefaultViewCore(IModelListView masterModelListView, XpandXafGridView masterGridView, int rowHandle, int relationIndex, List<IMasterDetailRule> masterDetailRules) {
            var modelDetailRelationCalculator = new ModelDetailRelationCalculator(masterModelListView, masterGridView, masterDetailRules);
            bool isRelationSet = modelDetailRelationCalculator.IsRelationSet(rowHandle, relationIndex);
            if (isRelationSet) {
                IModelListView childModelListView = modelDetailRelationCalculator.GetChildModelListView(rowHandle, relationIndex);
                ListView listView = GetListView(modelDetailRelationCalculator, rowHandle, relationIndex, childModelListView);
                XpandXafGridView defaultXpandXafGridView = null;
                EventHandler[] listViewOnControlsCreated = { null };
                listViewOnControlsCreated[0] = (sender, args) => {
                    defaultXpandXafGridView = (XpandXafGridView)((GridListEditor)((XpandListView)sender).Editor).GridView;
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
            return listViewBuilder.CreateListView(childModelListView, rowHandle, relationIndex,_xafApplication);
        }


        public void ModifyInstanceGridView(XpandXafGridView masterGridView, int rowHandle, int relationIndex, IModelListView masterModelListView, List<IMasterDetailRule> masterDetailRules) {
            var modelDetailRelationCalculator = new ModelDetailRelationCalculator(masterModelListView, masterGridView, masterDetailRules);
            bool isRelationSet = modelDetailRelationCalculator.IsRelationSet(rowHandle, relationIndex);
            if (isRelationSet) {
                IModelListView childModelListView = modelDetailRelationCalculator.GetChildModelListView(rowHandle, relationIndex);
                Window window = _xafApplication.CreateWindow(TemplateContext.View, null, true, false);
                ListView listView = GetListView(modelDetailRelationCalculator, rowHandle, relationIndex, childModelListView);
                var detailXafGridView = (XpandXafGridView)masterGridView.GetDetailView(rowHandle, relationIndex);
                ((XpandGridListEditor)listView.Editor).CustomGridViewCreate +=
                    (o, eventArgs) => {
                        eventArgs.Handled = true;
                        eventArgs.GridView = detailXafGridView;
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