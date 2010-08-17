using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using eXpand.ExpressApp.MasterDetail.Logic;
using XafGridView = eXpand.ExpressApp.Win.ListEditors.XafGridView;

namespace eXpand.ExpressApp.MasterDetail.Win {
    public class GridViewBuilder {
        readonly XafApplication _xafApplication;
        readonly ObjectSpace _objectSpace;
        readonly Window _masterWindow;

        public GridViewBuilder(XafApplication xafApplication,ObjectSpace objectSpace,Window masterWindow) {
            _xafApplication = xafApplication;
            _objectSpace = objectSpace;
            _masterWindow = masterWindow;
        }

        public XafGridView GetLevelDefaultView(XafGridView masterGridView, int rowHandle, int relationIndex, IModelListView masterModelListView,  List<IMasterDetailRule> masterDetailRules) {
            return GetLevelDefaultViewCore(masterModelListView, masterGridView, rowHandle, relationIndex, masterDetailRules);
        }
        
        XafGridView GetLevelDefaultViewCore(IModelListView masterModelListView, XafGridView masterGridView, int rowHandle, int relationIndex,  List<IMasterDetailRule> masterDetailRules) {
            var modelDetailRelationCalculator = new ModelDetailRelationCalculator(masterModelListView, masterGridView, masterDetailRules);
            bool isRelationSet = modelDetailRelationCalculator.IsRelationSet(rowHandle, relationIndex);
            if (isRelationSet)
            {
                IModelListView childModelListView = modelDetailRelationCalculator.GetChildModelListView(rowHandle, relationIndex);
                DevExpress.ExpressApp.ListView listView = GetListView(modelDetailRelationCalculator, rowHandle, relationIndex, childModelListView);
                XafGridView defaultXafGridView = null;
                EventHandler[] listViewOnControlsCreated = { null };
                listViewOnControlsCreated[0] = (sender, args) =>
                {
                    defaultXafGridView = (XafGridView)((GridListEditor)((ListView)sender).Editor).GridView;
                    listView.ControlsCreated -= listViewOnControlsCreated[0];
                };
                listView.ControlsCreated += listViewOnControlsCreated[0];
                listView.CreateControls();
                return defaultXafGridView;
            }
            return null;
        }

        DevExpress.ExpressApp.ListView GetListView(ModelDetailRelationCalculator modelDetailRelationCalculator, int rowHandle, int relationIndex, IModelListView childModelListView) {
            var listViewBuilder = new ListViewBuilder(modelDetailRelationCalculator, _objectSpace);
            return listViewBuilder.CreateListView(childModelListView, rowHandle, relationIndex);
        }


        public void ModifyInstanceGridView(XafGridView masterGridView, int rowHandle, int relationIndex, IModelListView masterModelListView, List<IMasterDetailRule> masterDetailRules) {
            var modelDetailRelationCalculator = new ModelDetailRelationCalculator(masterModelListView, masterGridView, masterDetailRules);
            bool isRelationSet = modelDetailRelationCalculator.IsRelationSet(rowHandle, relationIndex);
            if (isRelationSet)
            {
                IModelListView childModelListView = modelDetailRelationCalculator.GetChildModelListView(rowHandle, relationIndex);
                Window window = _xafApplication.CreateWindow(TemplateContext.View, null, true, true);
                DevExpress.ExpressApp.ListView listView = GetListView(modelDetailRelationCalculator, rowHandle, relationIndex, childModelListView);
                var detailXafGridView = (XafGridView)masterGridView.GetDetailView(rowHandle, relationIndex);
                ((ExpressApp.Win.ListEditors.GridListEditor) listView.Editor).CustomGridViewCreate +=
                    (o, eventArgs) => {
                        eventArgs.Handled = true;    
                        eventArgs.GridView=detailXafGridView;
                    };
                
                EventHandler[] listViewOnControlsCreated = { null };
                listViewOnControlsCreated[0] = (sender, args) =>
                {
                    detailXafGridView.MasterWindow = masterGridView.MasterWindow ?? _masterWindow;
                    detailXafGridView.Window = window;
                    detailXafGridView.GridControl = masterGridView.GridControl;
                    listView.ControlsCreated -= listViewOnControlsCreated[0];
                };
                listView.ControlsCreated += listViewOnControlsCreated[0];
                ((PropertyCollectionSource) listView.CollectionSource).MasterObject = masterGridView.GetRow(rowHandle);
                window.SetView(listView);
            }            
        }


    }
}