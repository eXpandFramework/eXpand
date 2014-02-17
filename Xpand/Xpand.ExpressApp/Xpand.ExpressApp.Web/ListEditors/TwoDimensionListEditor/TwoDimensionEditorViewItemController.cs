using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;

namespace Xpand.ExpressApp.Web.ListEditors.TwoDimensionListEditor{
    
    public partial class TwoDimensionEditorViewItemController : ViewController{
        public TwoDimensionEditorViewItemController(){
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnActivated(){
            base.OnActivated();
            TwoDimensionViewItem.Active.SetItemValue("non_persistent", View.ObjectTypeInfo.IsPersistent);
        }

        private void TwoDimensionViewItem_Execute(object sender, SimpleActionExecuteEventArgs e){
            ShowViewParameters svp = e.ShowViewParameters;
            var cbal = e.SelectedObjects[0] as ITwoDimensionItem;
            IObjectSpace ospace = Application.CreateObjectSpace();
            DetailView view = Application.CreateDetailView(ospace, ospace.GetObject(cbal), true);
            view.ViewEditMode = ViewEditMode.View;
            svp.CreatedView = view;
            svp.TargetWindow = TargetWindow.NewModalWindow;
            svp.Context = TemplateContext.PopupWindow;
        }
    }
}