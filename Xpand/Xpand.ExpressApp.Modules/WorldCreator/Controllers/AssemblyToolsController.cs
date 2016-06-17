using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using Xpand.ExpressApp.WorldCreator.CodeProvider.Validation;
using Xpand.ExpressApp.WorldCreator.System;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.Validation;

namespace Xpand.ExpressApp.WorldCreator.Controllers {
    public interface IModelOptionsWorldCreator{
        [Category("eXpand.WorldCreator")]
        [DefaultValue(true)]
        bool ValidateAssemblyOnSave { get; set; }
    }

    public class AssemblyToolsController : ViewController<DetailView>,IModelExtender {
        private const string ValidateAssemblyKey = "Validate code";
        readonly SingleChoiceAction _singleChoiceAction;
        private bool _validating;

        public AssemblyToolsController() {
            TargetObjectType = typeof(IPersistentAssemblyInfo);
            _singleChoiceAction = new SingleChoiceAction(this, "Tools", PredefinedCategory.ObjectsCreation);
            _singleChoiceAction.Items.Add(new ChoiceActionItem(ValidateAssemblyKey, ValidateAssemblyKey));
            _singleChoiceAction.Execute += SingleChoiceActionOnExecute;
            _singleChoiceAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
        }

        protected override void OnActivated(){
            base.OnActivated();
            ObjectSpace.Committed+=ObjectSpaceOnCommitted;
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            ObjectSpace.Committed-=ObjectSpaceOnCommitted;
        }

        private void ObjectSpaceOnCommitted(object sender, EventArgs eventArgs){
            if (((IModelOptionsWorldCreator) Application.Model.Options).ValidateAssemblyOnSave){
                ValidateAssembly((IPersistentAssemblyInfo) View.CurrentObject);
            }
        }

        void SingleChoiceActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {
            if (ReferenceEquals(singleChoiceActionExecuteEventArgs.SelectedChoiceActionItem.Data, ValidateAssemblyKey)) {
                var persistentAssemblyInfo = ((IPersistentAssemblyInfo)singleChoiceActionExecuteEventArgs.CurrentObject);
                ValidateAssembly(persistentAssemblyInfo);
            }
        }

        private void ValidateAssembly(IPersistentAssemblyInfo persistentAssemblyInfo) {
            if(!_validating){
                _validating = true;
                var worldCreatorModuleBase = Application.Modules.OfType<WorldCreatorModuleBase>().First();
                var validatorResult = persistentAssemblyInfo.Validate(worldCreatorModuleBase.GetPath());
                persistentAssemblyInfo.Errors=validatorResult.Message;
                ObjectSpace.CommitChanges();
                _validating = false;
                if (!validatorResult.Valid){
                    var messageResult = Validator.RuleSet.NewRuleSetValidationMessageResult(ObjectSpace,
                        "Validation error! check Compile Errors Tab.", View.CurrentObject);
                    throw new ValidationException(messageResult);
                }
            }
        }

        public SingleChoiceAction SingleChoiceAction => _singleChoiceAction;

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelOptions,IModelOptionsWorldCreator>();
        }
    }
}