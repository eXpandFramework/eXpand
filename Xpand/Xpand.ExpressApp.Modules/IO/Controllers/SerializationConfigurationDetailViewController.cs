using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.IO.PersistentTypesHelpers;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ImportExport;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.IO.Controllers {
    public class SerializationConfigurationDetailViewController : ObjectViewController<DetailView,ISerializationConfiguration> {
        private readonly SimpleAction _generateGraphAction;
        private bool _serializing;

        public SerializationConfigurationDetailViewController() {
            TargetObjectType = typeof(ISerializationConfiguration);
            var applyStrategyAction = new SingleChoiceAction(this, "Apply Strategy", PredefinedCategory.ObjectsCreation);
            applyStrategyAction.Execute += ApplyStrategyActionOnExecute;
            applyStrategyAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
            foreach (var serializationStrategy in Enum.GetValues(typeof(SerializationStrategy))) {
                applyStrategyAction.Items.Add(new ChoiceActionItem(serializationStrategy.ToString(),serializationStrategy));
            }
            _generateGraphAction = new SimpleAction(this,"GenerateGraph",PredefinedCategory.ObjectsCreation);
            _generateGraphAction.Execute+=GenerateGraphActionOnExecute;
        }

        public SimpleAction GenerateGraphAction{
            get { return _generateGraphAction; }
        }

        private void GenerateGraphActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs){
            var serializationConfiguration = ((ISerializationConfiguration)View.CurrentObject);
            ObjectSpace.Delete(serializationConfiguration.SerializationGraph);
            new ClassInfoGraphNodeBuilder().Generate(serializationConfiguration);
            ObjectSpace.CommitChanges();
        }

        void ApplyStrategyActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs e) {
            var serializationConfiguration = ((ISerializationConfiguration)View.CurrentObject);
            GenerateGraph(serializationConfiguration);
        }

        protected override void OnActivated() {
            base.OnActivated();
            var isHosted = Application.IsHosted();
            _generateGraphAction.Active["Hosted"] = isHosted;
            ObjectSpace.ObjectChanged+=ObjectSpaceOnObjectChanged;
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            ObjectSpace.ObjectChanged-=ObjectSpaceOnObjectChanged;
        }

        private void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs e){
            var serializationConfiguration = e.Object as ISerializationConfiguration;
            if (serializationConfiguration != null && (e.PropertyName == serializationConfiguration.GetPropertyName(x => x.TypeToSerialize) && e.NewValue != null)) {
                if (!_serializing) {
                    _serializing = true;
                    GenerateGraph(serializationConfiguration);
                    _serializing = false;
                }
            }
        }

        private void GenerateGraph(ISerializationConfiguration serializationConfiguration){
            ObjectSpace.Delete(serializationConfiguration.SerializationGraph);
            new ClassInfoGraphNodeBuilder().Generate(serializationConfiguration);
        }
    }
}