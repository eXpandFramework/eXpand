using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using eXpand.ExpressApp.MasterDetail.Logic;
using eXpand.ExpressApp.MasterDetail.Win.Logic;

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

        public XafGridView GetLevelDefaultView(XafGridView masterGridView, int rowHandle, int relationIndex, IModelListView modelListView,  List<IMasterDetailRule> masterDetailRules) {
            return GetLevelDefaultViewCore(modelListView, masterGridView, rowHandle, relationIndex, masterDetailRules);
        }

        XafGridView GetLevelDefaultViewCore(IModelListView modelListView, XafGridView masterGridView, int rowHandle, int relationIndex,  List<IMasterDetailRule> masterDetailRules) {
            var modelDetailRelationCalculator = new ModelDetailRelationCalculator(modelListView, masterGridView, masterDetailRules);
            bool isRelationSet = modelDetailRelationCalculator.IsRelationSet(rowHandle, relationIndex);
            if (isRelationSet)
            {
                IModelListView childModelListView = modelDetailRelationCalculator.GetChildModelListView(rowHandle, relationIndex);
                Window window = _xafApplication.CreateWindow(TemplateContext.View, null, true, true);
                var listViewBuilder = new ListViewBuilder(_xafApplication, _objectSpace);
                ListView listView = listViewBuilder.CreateListView(childModelListView);
                ExpressApp.Win.ListEditors.XafGridView defaultXafGridView = null;
                EventHandler[] listViewOnControlsCreated = { null };
                listViewOnControlsCreated[0] = (sender, args) =>
                {
                    defaultXafGridView = (ExpressApp.Win.ListEditors.XafGridView)((GridListEditor)((ListView)sender).Editor).GridView;
//                    defaultXafGridView.OwnerPropertyName = childModelListView.Id;
                    defaultXafGridView.MasterWindow = _masterWindow;
                    defaultXafGridView.Window = window;
                    defaultXafGridView.GridControl = masterGridView.GridControl;
                    listView.ControlsCreated -= listViewOnControlsCreated[0];
                };
                listView.ControlsCreated += listViewOnControlsCreated[0];
                window.SetView(listView);
                return defaultXafGridView;
            }
            return null;
        }
    }
}