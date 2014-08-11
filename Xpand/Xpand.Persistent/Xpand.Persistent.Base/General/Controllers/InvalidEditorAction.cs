using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.Web.ASPxEditors;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.Persistent.Base.General.Controllers {
    public enum InvalidEditorAction {
        Default,
        CancelCommit
    }
    public interface IModelClassInvalidEditorAction:IModelNode {
        [Category(AttributeCategoryNameProvider.Xpand)]
        [DefaultValue(InvalidEditorAction.CancelCommit)]
        InvalidEditorAction InvalidEditorAction { get; set; }
    }

    [ModelInterfaceImplementor(typeof(IModelClassInvalidEditorAction),"ModelClass")]
    public interface IModelDetailViewInvalidEditorAction : IModelClassInvalidEditorAction {
         
    }

    public class InvalidEditorActionBaseControllerWin : InvalidEditorActionBaseController {
        private readonly Dictionary<object, bool> _dictionary = new Dictionary<object, bool>();
        protected override void OnActivated(){
            base.OnActivated();
            if (Enabled){
                ObjectSpace.Committed+=ObjectSpaceOnCommitted;
                foreach (var item in View.GetItems<PropertyEditor>()){
                    item.ControlCreated += ItemOnControlCreated;
                }
            }
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            ObjectSpace.Committed-=ObjectSpaceOnCommitted;
        }

        private void ObjectSpaceOnCommitted(object sender, EventArgs eventArgs){
            _dictionary.Clear();
        }

        protected override bool Cancel(){
            return _dictionary.Any(pair => pair.Value);
        }

        private void ItemOnControlCreated(object sender, EventArgs eventArgs) {
            var propertyEditor = ((PropertyEditor)sender);
            var control = (propertyEditor.Control);
            _dictionary.Add(control, false);
            ((Control)propertyEditor.Control).Validating += (o, args) => {
                _dictionary[o] = args.Cancel;
            };
        }
    }

    public class InvalidEditorActionBaseWebController : InvalidEditorActionBaseController {
        protected override bool Cancel(){
            var asPxEdits = View.GetItems<WebPropertyEditor>().Select(editor => editor.Editor).OfType<ASPxEdit>();
            return asPxEdits.Any(edit => !edit.IsValid);
        }
    }
    public abstract class InvalidEditorActionBaseController:ViewController<DetailView>{
        
        private ModificationsController _modificationsController;

        protected override void OnActivated(){
            base.OnActivated();
            _modificationsController = Frame.GetController<ModificationsController>();
            if (Enabled){
                _modificationsController.SaveAction.Executing+=SaveOnExecuting;
                _modificationsController.SaveAndCloseAction.Executing+=SaveOnExecuting;
                _modificationsController.SaveAndNewAction.Executing+=SaveOnExecuting;
                View.ObjectSpace.Committing += ObjectSpaceOnCommitting;
            }
        }

        public bool Enabled{
            get{
                return ((IModelDetailViewInvalidEditorAction) View.Model).InvalidEditorAction==InvalidEditorAction.CancelCommit;
            }
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            View.ObjectSpace.Committing -= ObjectSpaceOnCommitting;
            _modificationsController.SaveAction.Executing -= SaveOnExecuting;
            _modificationsController.SaveAndCloseAction.Executing -= SaveOnExecuting;
            _modificationsController.SaveAndNewAction.Executing -= SaveOnExecuting;
        }

        private void SaveOnExecuting(object sender, CancelEventArgs cancelEventArgs){
            cancelEventArgs.Cancel = Cancel();
        }

        protected abstract bool Cancel();

        private void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs){
            cancelEventArgs.Cancel = Cancel();
        }

    }
}
