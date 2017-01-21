using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.Web;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.Base.TreeNode;

namespace Xpand.ExpressApp.Web.SystemModule {
    public interface IModelOptionsColumnChooserContextMenu : IModelNode {
        [Category(AttributeCategoryNameProvider.Xpand)]
        [ModelReadOnly(typeof(ModelOptionsColumnChooserContextMenuReadOnlyCalculator))]
        [Description("Hierarchical support when XpandTreeListEditorsAspNetModule is installed")]
        bool ColumnChooserContextMenu { get; set; }
    }

    
    public class ModelOptionsColumnChooserContextMenuReadOnlyCalculator:IModelIsReadOnly {
        public bool IsReadOnly(IModelNode node, string propertyName){
            return !((IModelSources) node.Application).Modules.Any(m => m is ITreeUser);
        }

        public bool IsReadOnly(IModelNode node, IModelNode childNode){
            return IsReadOnly(node, "");
        }
    }

    [ModelInterfaceImplementor(typeof(IModelOptionsColumnChooserContextMenu), "Application.Options")]
    public interface IModelListViewColumnChooserContextMenu : IModelOptionsColumnChooserContextMenu {

    }
    public class ColumnChooserGridViewController:ViewController<ListView>,IXafCallbackHandler ,IModelExtender{
        public ColumnChooserGridViewController() {
            ColumnChooserAction = new SimpleAction(this, "ColumnChooser", "HiddenCategory");
            ColumnChooserAction.Execute += ColumnChooserActionOnExecute;
        }

        protected override void OnActivated(){
            base.OnActivated();
            Active["XpandTreeModule"]= !new ModelOptionsColumnChooserContextMenuReadOnlyCalculator().IsReadOnly(Application.Model.Options, (IModelNode)null);
        }

        public SimpleAction ColumnChooserAction { get; }
        private void ColumnChooserActionOnExecute(object sender, SimpleActionExecuteEventArgs e) {
            var showViewParameters = e.ShowViewParameters;
            showViewParameters.TargetWindow = TargetWindow.NewWindow;
            var objectSpace = Application.CreateObjectSpace(typeof(ColumnChooserList));
            var detailView = Application.CreateDetailView(objectSpace, new ColumnChooserList());
            detailView.ViewEditMode = ViewEditMode.Edit;
            showViewParameters.CreatedView = detailView;
            var dialogController = Application.CreateController<DialogController>();
            dialogController.CancelAction.Active[GetType().Name] = false;
            showViewParameters.Controllers.Add(dialogController);
            showViewParameters.Controllers.Add(new PopupParentFrameController(Frame));
            dialogController.AcceptAction.Execute += AcceptActionOnExecute;
        }

        private void AcceptActionOnExecute(object sender, SimpleActionExecuteEventArgs e){
            Frame.SetView(Application.ProcessShortcut(View.CreateShortcut()));
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var gridListEditor = View.Editor as ASPxGridListEditor;
            if (gridListEditor != null&& ((IModelListViewColumnChooserContextMenu) View.Model).ColumnChooserContextMenu) {
                XafCallbackManager.RegisterHandler(GetType().FullName, this);
                gridListEditor.Grid.FillContextMenuItems += GridOnFillContextMenuItems;
                var script = XafCallbackManager.GetScript(GetType().FullName, "");
                string check="if (e.item.name == '"+ColumnChooserAction.Id+"') ";
                gridListEditor.Grid.ClientSideEvents.ContextMenuItemClick = $@"function(s, e){{{check}{script}}}";
            }
        }
        XafCallbackManager XafCallbackManager => ((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager;

        public void ProcessAction(string parameter){
            ColumnChooserAction.DoExecute();
        }

        private void GridOnFillContextMenuItems(object sender, ASPxGridViewContextMenuEventArgs e) {
            if (e.MenuType == GridViewContextMenuType.Columns) {
                var item = e.CreateItem(ColumnChooserAction.Caption, ColumnChooserAction.Id);
                ASPxImageHelper.SetImageProperties(item.Image, ColumnChooserAction.ImageName);
                e.Items[10].Visible = false;
                e.Items.Insert(10, item);
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelOptions,IModelOptionsColumnChooserContextMenu>();
            extenders.Add<IModelListView,IModelListViewColumnChooserContextMenu>();
        }
    }
}

