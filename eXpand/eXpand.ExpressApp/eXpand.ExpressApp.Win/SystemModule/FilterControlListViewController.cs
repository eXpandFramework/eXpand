using System;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Persistent.Base;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public partial class FilterControlListViewController : BaseViewController
    {
//        private static bool recursive;
//        private static bool lookUpQueryPopUp;
        private const string FilterControlPosition = @"FilterControlPosition";
        public FilterControlListViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetViewType = ViewType.ListView;
            
        }

        public override Schema GetSchema()
        {
            string CommonTypeInfos = @"<Element Name=""Application"">
                        <Element Name=""Views"">
                            <Element Name=""ListView"" >
                                <Attribute Name=""" + FilterControlPosition + @""" Choice=""{"+typeof(DockStyle).FullName + @"}""/>
                            </Element>
                        </Element>
                  </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(CommonTypeInfos));
        }

        protected override void OnActivated()
        {
            if (View.Info.GetAttributeEnumValue(FilterControlPosition,DockStyle.None)!=DockStyle.None)
                View.ControlsCreated +=
                    (sender, args) => ((Control) View.Control).HandleCreated += gridControl_HandleCreated;
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
                                    Dock = View.Info.GetAttributeEnumValue(FilterControlPosition, DockStyle.None),
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

            var accept = new SimpleButton {Text = "Accept filter"};
            accept.Click += ((o, args) => filterControl.ApplyFilter());
            accept.Dock = DockStyle.Bottom;
            filterControl.Controls.Add(accept);

            ((Control) sender).Parent.Controls.Add(filterControl);
        }



        private void setCriteriaFromView(FilterControl filter)
        {
            var criteriaWrapper = new CriteriaWrapper(View.ObjectTypeInfo.Type, View.Info.GetAttributeValue(GridListEditor.ActiveFilterString), false);
            new FilterWithObjectsProcessor(ObjectSpace).Process(criteriaWrapper.CriteriaOperator,
                                                                FilterWithObjectsProcessorMode.StringToObject);
            filter.FilterCriteria =criteriaWrapper.CriteriaOperator;
        }





    }
}