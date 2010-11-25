using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.IO.Observers;
using Xpand.ExpressApp.IO.PersistentTypesHelpers;
using Xpand.Persistent.Base.ImportExport;

namespace Xpand.ExpressApp.IO.Controllers {
    public class SerializationConfigurationDetailViewController : ViewController<DetailView> {
        public SerializationConfigurationDetailViewController() {
            TargetObjectType = typeof(ISerializationConfiguration);
            var applyStrategyAction = new SingleChoiceAction(this, "Apply Strategy", PredefinedCategory.ObjectsCreation);
            applyStrategyAction.Execute += ApplyStrategyActionOnExecute;
            applyStrategyAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
            foreach (var serializationStrategy in Enum.GetValues(typeof(SerializationStrategy))) {
                applyStrategyAction.Items.Add(new ChoiceActionItem(serializationStrategy.ToString(),serializationStrategy));
            }
        }

        void ApplyStrategyActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs e) {
            var serializationConfiguration = ((ISerializationConfiguration)View.CurrentObject);
            var serializationStrategy = (SerializationStrategy)e.SelectedChoiceActionItem.Data;
            new ClassInfoGraphNodeBuilder().ApplyStrategy(serializationStrategy, serializationConfiguration);
        }


        protected override void OnActivated() {
            base.OnActivated();
            new SerializationConfigurationObserver(ObjectSpace);
        }
    }
}