using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.MasterDetail {
    public class ListViewBuilder {
        readonly ModelDetailRelationCalculator _modelDetailRelationCalculator;
        readonly IObjectSpace _objectSpace;
        public ListViewBuilder(ModelDetailRelationCalculator modelDetailRelationCalculator, IObjectSpace objectSpace) {
            _modelDetailRelationCalculator = modelDetailRelationCalculator;
            _objectSpace = objectSpace;
        }

        public ListView CreateListView(IModelListView childModelListView, int rowHandle, int relationIndex, XafApplication application) {
            IModelMember relationModelMember = _modelDetailRelationCalculator.GetRelationModelMember(rowHandle, relationIndex);
            return CreateListView(childModelListView, relationModelMember, application);
        }

        ListView CreateListView(IModelListView childModelListView, IModelMember relationModelMember, XafApplication application) {
            var propertyCollectionSource = application.CreatePropertyCollectionSource(_objectSpace, relationModelMember.ModelClass.TypeInfo.Type, null, relationModelMember.MemberInfo, childModelListView.Id);
            return application.CreateListView(childModelListView, propertyCollectionSource, false);
        }

    }

    public class GridViewBuilder {
        readonly XafApplication _xafApplication;
        readonly IObjectSpace _objectSpace;
        readonly Frame _masterFrame;

        public GridViewBuilder(XafApplication xafApplication, IObjectSpace objectSpace, Frame masterFrame) {
            _xafApplication = xafApplication;
            _objectSpace = objectSpace;
            _masterFrame = masterFrame;
        }

        public DevExpress.XtraGrid.Views.Base.ColumnView GetLevelDefaultView(DevExpress.XtraGrid.Views.Grid.GridView masterGridView, int rowHandle, int relationIndex, IModelListView masterModelListView, List<MasterDetailRuleInfo> masterDetailRules) {
            return GetLevelDefaultViewCore(masterModelListView, masterGridView, rowHandle, relationIndex, masterDetailRules);
        }

        DevExpress.XtraGrid.Views.Base.ColumnView GetLevelDefaultViewCore(IModelListView masterModelListView, DevExpress.XtraGrid.Views.Grid.GridView masterGridView, int rowHandle, int relationIndex, List<MasterDetailRuleInfo> masterDetailRules) {
            var modelDetailRelationCalculator = new ModelDetailRelationCalculator(masterModelListView, masterGridView, masterDetailRules);
            bool isRelationSet = modelDetailRelationCalculator.IsRelationSet(rowHandle, relationIndex);
            if (isRelationSet) {
                IModelListView childModelListView = modelDetailRelationCalculator.GetChildModelListView(rowHandle, relationIndex);
                ListView listView = GetListView(modelDetailRelationCalculator, rowHandle, relationIndex, childModelListView);
                listView.CreateControls();
                return ((WinColumnsListEditor)listView.Editor).ColumnView;
            }
            return null;
        }

        ListView GetListView(ModelDetailRelationCalculator modelDetailRelationCalculator, int rowHandle, int relationIndex, IModelListView childModelListView) {
            var listViewBuilder = new ListViewBuilder(modelDetailRelationCalculator, _objectSpace);
            return listViewBuilder.CreateListView(childModelListView, rowHandle, relationIndex, _xafApplication);
        }

        public void ModifyGridViewInstance(DevExpress.XtraGrid.Views.Grid.GridView masterGridView, int rowHandle, int relationIndex, IModelListView masterModelListView, List<MasterDetailRuleInfo> masterDetailRules) {
            var modelDetailRelationCalculator = new ModelDetailRelationCalculator(masterModelListView, masterGridView, masterDetailRules);
            bool isRelationSet = modelDetailRelationCalculator.IsRelationSet(rowHandle, relationIndex);
            if (isRelationSet) {
                IModelListView childModelListView = modelDetailRelationCalculator.GetChildModelListView(rowHandle, relationIndex);
                Window window = _xafApplication.CreateWindow(TemplateContext.View, null, true, false);
                ListView listView = GetListView(modelDetailRelationCalculator, rowHandle, relationIndex, childModelListView);
                var detailXafGridView = (DevExpress.XtraGrid.Views.Grid.GridView)masterGridView.GetDetailView(rowHandle, relationIndex);
                ((IColumnViewEditor)listView.Editor).CustomGridViewCreate +=
                    (o, eventArgs) => {
                        ((WinColumnsListEditor)o).DataSource = detailXafGridView.DataSource;
                        eventArgs.Handled = true;
                        eventArgs.GridView = detailXafGridView;
                        eventArgs.GridControl.DataSource = detailXafGridView.DataSource;
                    };

                EventHandler[] listViewOnControlsCreated = { null };
                listViewOnControlsCreated[0] = (sender, args) => {
                    ((IMasterDetailColumnView)detailXafGridView).MasterFrame = ((IMasterDetailColumnView)masterGridView).MasterFrame ?? _masterFrame;
                    ((IMasterDetailColumnView)detailXafGridView).Window = window;
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
