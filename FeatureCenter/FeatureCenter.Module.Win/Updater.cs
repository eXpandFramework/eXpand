using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Win;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace FeatureCenter.Module.Win {
    public class MyClass : ViewController {
        public MyClass() {
            var simpleAction = new SimpleAction(this, "tset", PredefinedCategory.ObjectsCreation);
            simpleAction.Execute += SimpleActionOnExecute;
        }

        void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            var winApplication = ((WinApplication)Application);
            winApplication.ShowViewStrategy.CloseAllWindows();
            var modelApplicationBases = RemoveLayers((ModelApplicationBase)winApplication.Model).Reverse();
            AddLayers(modelApplicationBases, (ModelApplicationBase)winApplication.Model);
            winApplication.ShowViewStrategy.ShowStartupWindow();
        }

        void AddLayers(IEnumerable<ModelApplicationBase> modelApplicationBases, ModelApplicationBase model) {
            foreach (var modelApplicationBase in modelApplicationBases) {
                model.AddLayer(modelApplicationBase);
            }
        }

        IEnumerable<ModelApplicationBase> RemoveLayers(ModelApplicationBase modelApplicationBase) {
            var modelApplicationBases = new List<ModelApplicationBase>();
            while (modelApplicationBase.LastLayer.Id != "Unchanged Master Part") {
                modelApplicationBases.Add(modelApplicationBase.LastLayer);
                modelApplicationBase.RemoveLayer(modelApplicationBase.LastLayer);
            }
            return modelApplicationBases;
        }
    }

    public class Updater : ModuleUpdater {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();

        }
    }
}
