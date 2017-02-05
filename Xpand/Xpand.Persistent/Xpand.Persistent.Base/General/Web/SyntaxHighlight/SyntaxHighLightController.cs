using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors;

namespace Xpand.Persistent.Base.General.Web.SyntaxHighlight {
    public class SyntaxHighlightController:ViewController<DetailView>,IModelExtender{
        class Literal: System.Web.UI.WebControls.Literal {
            
        }
        private WebPropertyEditor[] _propertyEditors;

        protected override void OnActivated(){
            base.OnActivated();
            var webWindow =  Frame as WebWindow;
            if (webWindow != null) webWindow.PagePreRender+=OnPagePreRender;
            _propertyEditors = View.GetItems<WebPropertyEditor>().Where(SyntaxHighlightEnabled).ToArray();
            foreach (var propertyEditor in _propertyEditors){
                propertyEditor.ControlCreated+=ControlCreated;
            }   
        }

        private void OnPagePreRender(object sender, EventArgs eventArgs){
            foreach (var propertyEditor in _propertyEditors) {
                ConfigureAceEditor(propertyEditor);
            }
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            var webWindow = Frame as WebWindow;
            if (webWindow != null) webWindow.PagePreRender -= OnPagePreRender;
            foreach (var propertyEditor in _propertyEditors) {
                propertyEditor.ControlCreated -= ControlCreated;
            }
        }

        private void ControlCreated(object sender, EventArgs e) {
            var webPropertyEditor = ((WebPropertyEditor)sender);
            var neighborControl = GetAceEditorNeighborControl(webPropertyEditor);
            var literalControl = new Literal();
            GetParentControlCollection(neighborControl, webPropertyEditor.ViewEditMode).Add(literalControl);
        }

        private ControlCollection GetParentControlCollection(WebControl webControl, ViewEditMode viewEditMode){
            return viewEditMode==ViewEditMode.View?webControl.Controls:webControl.Parent.Controls;
        }

        private WebControl GetAceEditorNeighborControl(WebPropertyEditor webPropertyEditor){
            return (WebControl) (View.ViewEditMode==ViewEditMode.Edit? webPropertyEditor.Editor:webPropertyEditor.Control);
        }

        private bool SyntaxHighlightEnabled(WebPropertyEditor propertyEditor){
            return !string.IsNullOrWhiteSpace(((IModelPropertyEditorSyntaxHighlight)propertyEditor.Model).SyntaxHighlight.Mode);
        }

        private void ConfigureAceEditor(WebPropertyEditor propertyEditor){
            var aceEditorNeighborControl = GetAceEditorNeighborControl(propertyEditor);
            var aceEditor = new AceEditor();
            var literalControl = GetParentControlCollection(aceEditorNeighborControl,propertyEditor.ViewEditMode).OfType<Literal>().First();
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
