using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;

namespace Xpand.Persistent.Base.General.Controllers.Actions{
    public abstract class ModifyModelActionControllerBase:ViewController{
        private ActionModifyModelControler _actionModifyModelControler;

        protected override void OnActivated() {
            base.OnActivated();
            _actionModifyModelControler = Frame.GetController<ActionModifyModelControler>();
            _actionModifyModelControler.ModifyModelAction.Execute += ModifyModelActionOnExecute;
            
        }

        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            _actionModifyModelControler = Frame.GetController<ActionModifyModelControler>();
            foreach (var choiceActionItem in GetChoiceActionItems()) {
                _actionModifyModelControler.ModifyModelAction.Items.Add(choiceActionItem);
            }
        }

        protected abstract void ModifyModelActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs e);

        protected abstract IEnumerable<ChoiceActionItem> GetChoiceActionItems();

        protected override void OnDeactivated() {
            base.OnDeactivated();
            _actionModifyModelControler.ModifyModelAction.Execute -= ModifyModelActionOnExecute;
        }
    }
}