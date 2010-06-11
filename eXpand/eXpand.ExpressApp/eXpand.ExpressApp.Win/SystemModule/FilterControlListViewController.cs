using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using Forms = System.Windows.Forms;
using DevExpress.ExpressApp.Utils;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public interface IModelClassFilterControlSettings : IModelNode,IModelExtender
    {
        [Category("eXpand")]
        [Description("For listviews displays a filter expression editor control at the specified position")]
        Forms.DockStyle FilterControlPosition { get; set; }
    }
    public interface IModelListViewFilterControlSettings : IModelNode,IModelExtender
    {
        [Category("eXpand")]
        [ModelValueCalculator("((IModelClassFilterControlSettings)ModelClass)", "FilterControlPosition")]
        [Description("For listviews displays a filter expression editor control at the specified position")]
        Forms.DockStyle FilterControlPosition { get; set; }
    }

    public class FilterControlListViewController : ViewController<ListView>,IModelExtender
    {
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelClass, IModelClassFilterControlSettings>();
            extenders.Add<IModelListView, IModelListViewFilterControlSettings>();
        }

        protected override void OnActivated()
        {
            if (((IModelListViewFilterControlSettings)View.Model).FilterControlPosition != Forms.DockStyle.None)
                View.ControlsCreated +=
                    (sender, args) => ((Forms.Control)View.Control).HandleCreated += gridControl_HandleCreated;
        }
        
        private Editors.FilterControl filterControl;

        public Editors.FilterControl FilterControl
        {
            get { return filterControl; }
        }

        public event EventHandler FilterActivated;
        
        private void InvokeFilterActivated(EventArgs e)
        {
            EventHandler activated = FilterActivated;
            if (activated != null) activated(this, e);
        }

        private void gridControl_HandleCreated(object sender, EventArgs e)
        {
            var gridControl = sender as GridControl;
            filterControl = new Editors.FilterControl
                            {
                                Height = 150,
                                Dock = ((IModelListViewFilterControlSettings)View.Model).FilterControlPosition,
                                SourceControl = gridControl
                            };
            InvokeFilterActivated(e);
            gridControl = filterControl.SourceControl as GridControl;
            if (gridControl != null )
            {
                if (!gridControl.FormsUseDefaultLookAndFeel)
                    filterControl.LookAndFeel.Assign(gridControl.LookAndFeel);
                setCriteriaFromView(filterControl);
            }

            var accept = new SimpleButton { Text = CaptionHelper.GetLocalizedText("eXpand", "AcceptFilter") };
            accept.Click += ((o, args) => filterControl.ApplyFilter());
            accept.Dock = Forms.DockStyle.Bottom;
            filterControl.Controls.Add(accept);

            ((Forms.Control) sender).Parent.Controls.Add(filterControl);
        }

        private void setCriteriaFromView(FilterControl filter)
        {
            var criteriaWrapper = new CriteriaWrapper(View.ObjectTypeInfo.Type, ((IModelListViewWin)View.Model).ActiveFilterString, false);
            new FilterWithObjectsProcessor(ObjectSpace).Process(criteriaWrapper.CriteriaOperator,
                                                                FilterWithObjectsProcessorMode.StringToObject);
            filter.FilterCriteria =criteriaWrapper.CriteriaOperator;
        }
    }
}