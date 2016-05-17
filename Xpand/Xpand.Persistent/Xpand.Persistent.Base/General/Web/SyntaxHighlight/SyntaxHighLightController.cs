using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors;

namespace Xpand.Persistent.Base.General.Web.SyntaxHighlight {
    public class SyntaxHighlightController:ViewController<DetailView>,IModelExtender{
        class LiteralControl: System.Web.UI.LiteralControl {
            
        }
        private WebPropertyEditor[] _propertyEditors;
        protected override void OnActivated(){
            base.OnActivated();
            _propertyEditors = View.GetItems<WebPropertyEditor>().Where(SyntaxHighlightEnabled).ToArray();
            foreach (var propertyEditor in _propertyEditors){
                propertyEditor.ControlCreated+=ControlCreated;
            }
            View.ViewEditModeChanged+=ViewOnViewEditModeChanged;
        }

        private void ViewOnViewEditModeChanged(object sender, EventArgs eventArgs){
            
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            View.ViewEditModeChanged -= ViewOnViewEditModeChanged;
            foreach (var propertyEditor in _propertyEditors) {
                propertyEditor.ControlCreated -= ControlCreated;
            }
        }

        private void ControlCreated(object sender, EventArgs e) {
            var webPropertyEditor = ((WebPropertyEditor)sender);
            var neighborControl = GetAceEditorNeighborControl(webPropertyEditor);
            neighborControl.Load += (o, args) => ConfigureAceEditor(webPropertyEditor);
            var literalControl = new LiteralControl();
            GetParentControlCollection(neighborControl,webPropertyEditor.ViewEditMode).Add(literalControl);
        }

        private ControlCollection GetParentControlCollection(WebControl webControl, ViewEditMode viewEditMode){
            return viewEditMode==ViewEditMode.View?webControl.Controls:webControl.Parent.Controls;
        }

        private WebControl GetAceEditorNeighborControl(WebPropertyEditor webPropertyEditor){
            return (WebControl) (View.ViewEditMode==ViewEditMode.Edit? webPropertyEditor.Editor:webPropertyEditor.Control);
        }

        private bool SyntaxHighlightEnabled(WebPropertyEditor propertyEditor){
            return ((IModelPropertyEditorSyntaxHighlight) propertyEditor.Model).SyntaxHighlight.Mode != null;
        }

        private void ConfigureAceEditor(WebPropertyEditor propertyEditor){
            var aceEditorNeighborControl = GetAceEditorNeighborControl(propertyEditor);
            var aceEditor = new AceEditor();
            var literalControl = GetParentControlCollection(aceEditorNeighborControl,propertyEditor.ViewEditMode).OfType<LiteralControl>().First();
            literalControl.Text = aceEditor.CreateContainer(aceEditorNeighborControl.ClientID, (string) propertyEditor.PropertyValue);
            var syntaxHighLight = ((IModelPropertyEditorSyntaxHighlight) propertyEditor.Model).SyntaxHighlight;
            var allowEdit = View.ViewEditMode == ViewEditMode.Edit && propertyEditor.AllowEdit;
            aceEditor.Configure(aceEditorNeighborControl.ClientID, syntaxHighLight, allowEdit,(string) propertyEditor.PropertyValue);
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelPropertyEditor,IModelPropertyEditorSyntaxHighlight>();
        }
    }
}
