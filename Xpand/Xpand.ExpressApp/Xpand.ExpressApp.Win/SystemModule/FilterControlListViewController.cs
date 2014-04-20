using System;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.LookAndFeel;
using DevExpress.Persistent.Base;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraGrid;
using Xpand.ExpressApp.Win.Editors;
using FilterEditorControl = DevExpress.XtraFilterEditor.FilterEditorControl;
using Forms = System.Windows.Forms;

namespace Xpand.ExpressApp.Win.SystemModule {
    public interface IModelClassFilterControlSettings : IModelNode {
        [Category("eXpand")]
        [Description("For listviews displays a filter expression editor control at the specified position")]
        Forms.DockStyle FilterControlPosition { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassFilterControlSettings), "ModelClass")]
    public interface IModelListViewFilterControlSettings : IModelClassFilterControlSettings {

    }

    public class FilterControlListViewController : ViewController<ListView>, IModelExtender {
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelClass, IModelClassFilterControlSettings>();
            extenders.Add<IModelListView, IModelListViewFilterControlSettings>();
        }

        protected override void OnActivated() {
            if (((IModelListViewFilterControlSettings)View.Model).FilterControlPosition !=
Forms.DockStyle.None) {
                View.ControlsCreated +=
                    (sender, args) => ((Forms.Control)View.Control).HandleCreated += gridControl_HandleCreated;
                Frame.ViewChanged += Frame_ViewChanged;
            }
        }

        private XpandFilterControl _filterControl;

        public XpandFilterControl FilterControl {
            get { return _filterControl; }
        }

        public event EventHandler FilterControlCreated;

        protected void OnFilterControlCreated(EventArgs e) {
            EventHandler handler = FilterControlCreated;
            if (handler != null) handler(this, e);
        }

        public event EventHandler CustomAssignFilterControlSourceControl;

        protected void OnCustomAssignFilterControlSourceControl(EventArgs e) {
            EventHandler activated = CustomAssignFilterControlSourceControl;
            if (activated != null) activated(this, e);
        }

        private void AssignControlDatasource(FilterEditorControl filterEditorControl) {
            filterEditorControl.SourceControl = CriteriaPropertyEditorHelper.CreateFilterControlDataSource(View.ObjectTypeInfo.Type, Application.ObjectSpaceProvider);
            if (View.ObjectTypeInfo.DefaultMember != null) {
                foreach (FilterColumn filterColumn in filterEditorControl.FilterColumns) {
                    if (View.ObjectTypeInfo.DefaultMember.Name == filterColumn.FieldName) {
                        filterEditorControl.SetDefaultColumn(filterColumn);
                    }
                }
            }
        }


        private void gridControl_HandleCreated(object sender, EventArgs e) {
            ((Forms.Control) sender).HandleCreated-=gridControl_HandleCreated;
            var filterEditorControl = new Editors.FilterEditorControl(() => View.Model.ModelClass);
            var helper = new FilterEditorControlHelper(Application,ObjectSpace);
            helper.Attach(filterEditorControl);
            _filterControl = (XpandFilterControl) filterEditorControl.FilterControl;
            _filterControl.Height = 150;
            _filterControl.Dock = ((IModelListViewFilterControlSettings)View.Model).FilterControlPosition;
            _filterControl.UseMenuForOperandsAndOperators = false;
            AssignControlDatasource(filterEditorControl);

            OnCustomAssignFilterControlSourceControl(e);
            var gridControl = AssignLookAndFeel(sender);
            _filterControl.FilterCriteria = GetCriteriaFromView();

            var accept = new SimpleButton {
                Text = CaptionHelper.GetLocalizedText(XpandSystemWindowsFormsModule.XpandWin,
                    "AcceptFilter")
            };
            accept.Click += ((o, args) => _filterControl.ApplyFilter());
            accept.Dock = Forms.DockStyle.Bottom;
            _filterControl.Controls.Add(accept);

            if (gridControl.Parent != null) {
                gridControl.Parent.Controls.Add(_filterControl);
            } else {
                gridControl.ParentChanged += gridControl_ParentChanged;
            }
            OnFilterControlCreated(EventArgs.Empty);
        }

        private Forms.Control AssignLookAndFeel(object sender){
            var gridControl = sender as GridControl;
            var userLookAndFeel = _filterControl.LookAndFeel;
            if (gridControl != null && !gridControl.FormsUseDefaultLookAndFeel){
                userLookAndFeel.Assign(gridControl.LookAndFeel);
                return gridControl;
            }
            var supportLookAndFeel = sender as ISupportLookAndFeel;
            if (supportLookAndFeel != null) 
                userLookAndFeel.Assign(supportLookAndFeel.LookAndFeel);
            return (Forms.Control) supportLookAndFeel;
        }

        void gridControl_ParentChanged(object sender, EventArgs e) {
            var gridControl = (GridControl)sender;
            gridControl.ParentChanged -= gridControl_ParentChanged;
            if (gridControl.Parent != null) {
                gridControl.Parent.Controls.Add(_filterControl);
            }
        }
        void Frame_ViewChanged(object sender, ViewChangedEventArgs e) {
            if (View != null && View.IsControlCreated) {
                var control = (View.Control) as GridControl;
                if (control != null) control.BringToFront();
            }
        }

        private CriteriaOperator GetCriteriaFromView() {
            var criteriaWrapper = new CriteriaWrapper(View.ObjectTypeInfo.Type,View.Model.Filter, false);
            new FilterWithObjectsProcessor(ObjectSpace).Process(criteriaWrapper.CriteriaOperator,
            FilterWithObjectsProcessorMode.StringToObject);
            return criteriaWrapper.CriteriaOperator;
        }
    }
}