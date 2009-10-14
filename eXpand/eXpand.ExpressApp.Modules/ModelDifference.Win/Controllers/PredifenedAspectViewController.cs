using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using eXpand.Persistent.Base;
using System.Linq;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.ModelDifference.Win.Controllers
{
    public partial class PredifenedAspectViewController : ViewController
    {
        public PredifenedAspectViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof (IXpoModelDifference);
            TargetViewType=ViewType.DetailView;
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            var name = ((IXpoModelDifference) View.CurrentObject).GetPropertyInfo(x=>x.PreferredAspect).Name;
            var firstOrDefault = ((DetailView) View).GetItems<PropertyEditor>().Where(editor => editor.PropertyName==name).FirstOrDefault() as StringPropertyEditor;
            if (firstOrDefault!= null) {
                var control = firstOrDefault.Control;
                var  repositoryItemTextEdit = (RepositoryItemComboBox) control.Properties;
                repositoryItemTextEdit.TextEditStyle=TextEditStyles.DisableTextEditor;
            }
        }
    }
}
