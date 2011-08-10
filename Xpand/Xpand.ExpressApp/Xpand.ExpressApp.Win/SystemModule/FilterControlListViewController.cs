using System;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
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

    public class FilterControlListViewController : ViewController<XpandListView>, IModelExtender {
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

        private Editors.XpandFilterControl _xpandFilterControl;

        public Editors.XpandFilterControl XpandFilterControl {
            get { return _xpandFilterControl; }
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

        private void gridControl_HandleCreated(object sender, EventArgs e) {
            var gridControl = sender as GridControl;
            _xpandFilterControl = new Editors.XpandFilterControl {
                Height = 150,
                Dock = ((IModelListViewFilterControlSettings)View.Model).FilterControlPosition,
                SourceControl = gridControl,
                UseMenuForOperandsAndOperators = false,
                AllowAggregateEditing = FilterControlAllowAggregateEditing.AggregateWithCondition
            };
            IFilterColumnCollectionHelper helper = new FilterColumnCollectionHelper(Application, ObjectSpace, View.ObjectTypeInfo);
            _xpandFilterControl.SetFilterColumnsCollection(new MemberInfoFilterColumnCollection(helper));
            OnCustomAssignFilterControlSourceControl(e);
            gridControl = (GridControl)_xpandFilterControl.SourceControl;
            if (!gridControl.FormsUseDefaultLookAndFeel)
                _xpandFilterControl.LookAndFeel.Assign(gridControl.LookAndFeel);
            _xpandFilterControl.FilterCriteria = GetCriteriaFromView();

            var accept = new SimpleButton {
                Text = CaptionHelper.GetLocalizedText(XpandSystemWindowsFormsModule.XpandWin,
                    "AcceptFilter")
            };
            accept.Click += ((o, args) => _xpandFilterControl.ApplyFilter());
            accept.Dock = Forms.DockStyle.Bottom;
            _xpandFilterControl.Controls.Add(accept);

            if (gridControl.Parent != null) {
                gridControl.Parent.Controls.Add(_xpandFilterControl);
            } else {
                gridControl.ParentChanged += gridControl_ParentChanged;
            }
            OnFilterControlCreated(EventArgs.Empty);
        }
        void gridControl_ParentChanged(object sender, EventArgs e) {
            var gridControl = (GridControl)sender;
            gridControl.ParentChanged -= gridControl_ParentChanged;
            if (gridControl.Parent != null) {
                gridControl.Parent.Controls.Add(_xpandFilterControl);
            }
        }
        void Frame_ViewChanged(object sender, ViewChangedEventArgs e) {
            var control = (View.Control) as GridControl;
            if (control != null) control.BringToFront();
        }

        private CriteriaOperator GetCriteriaFromView() {
            var criteriaWrapper = new CriteriaWrapper(View.ObjectTypeInfo.Type,
((IModelListViewWin)View.Model).ActiveFilterString, false);
            new FilterWithObjectsProcessor(ObjectSpace).Process(criteriaWrapper.CriteriaOperator,
            FilterWithObjectsProcessorMode.StringToObject);
            return criteriaWrapper.CriteriaOperator;
        }
    }
}