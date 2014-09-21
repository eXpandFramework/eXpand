using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.WorldCreator.Controllers {
    public interface IModelOptionsWorldCreator{
        [Category("eXpand.WorldCreator")]
        [DefaultValue(true)]
        bool ValidateAssemblyOnSave { get; set; }
    }

    public class AssemblyToolsController : ViewController<DetailView>,IModelExtender {
        readonly SingleChoiceAction _singleChoiceAction;
        public AssemblyToolsController() {
            TargetObjectType = typeof(IPersistentAssemblyInfo);
            _singleChoiceAction = new SingleChoiceAction(this, "Tools", PredefinedCategory.ObjectsCreation);
            _singleChoiceAction.Items.Add(new ChoiceActionItem("Validate Assembly", "Validate"));
            _singleChoiceAction.Execute += SingleChoiceActionOnExecute;
            _singleChoiceAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
        }

        protected override void OnActivated(){
            base.OnActivated();
            ObjectSpace.Committing+=ObjectSpaceOnCommitting;
        }

        private void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs){
            if (((IModelOptionsWorldCreator) Application.Model.Options).ValidateAssemblyOnSave)
                Validate((IPersistentAssemblyInfo) View.CurrentObject);
        }

        void SingleChoiceActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {
            if (ReferenceEquals(singleChoiceActionExecuteEventArgs.SelectedChoiceActionItem.Data, "Validate")) {
                var persistentAssemblyInfo = ((IPersistentAssemblyInfo)singleChoiceActionExecuteEventArgs.CurrentObject);
                Validate(persistentAssemblyInfo);
            }
        }

        public Type Validate(IPersistentAssemblyInfo persistentAssemblyInfo) {
            var worldCreatorModuleBase = Application.Modules.OfType<WorldCreatorModuleBase>().Single();
            return persistentAssemblyInfo.Validate(worldCreatorModuleBase.GetPath());
        }

        public SingleChoiceAction SingleChoiceAction {
            get { return _singleChoiceAction; }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelOptions,IModelOptionsWorldCreator>();
        }
    }
}