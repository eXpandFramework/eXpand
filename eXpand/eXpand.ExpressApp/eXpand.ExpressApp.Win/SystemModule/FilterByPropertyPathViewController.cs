using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public partial class FilterByPropertyPathViewController : ExpressApp.SystemModule.FilterByPropertyPathViewController
    {
        public FilterByPropertyPathViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void SynchronizeInfo(View view) {
            view.SynchronizeInfo();
        }

        protected override string FilterStringAttributeName {
            get { return GridListEditor.ActiveFilterString; }
        }
    }
}
