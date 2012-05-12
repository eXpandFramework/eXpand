using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.Xpo;

namespace Xpand.ExpressApp.ModelDifference.Core {
    
    public class ModelApplicationLoader {
        readonly XafApplication _application;

        public ModelApplicationLoader(XafApplication application) {
            _application = application;
        }

        public void EnableModel(Expression<Func<ModelDifferenceObject, bool>> expression, IEnumerable<string> modelsToUnload) {
            var winApplication = _application;
            using (var objectSpace = (XPObjectSpace)winApplication.CreateObjectSpace()) {
                var modelDifferenceObject = objectSpace.Session.FindObject(expression);
                var modelApplicationBase = (ModelApplicationBase)winApplication.Model;
                var modelApplicationBases = RemoveLayers(modelApplicationBase, modelsToUnload).Reverse().ToList();
                GetModelUnSafe(modelApplicationBase, modelDifferenceObject);
                AddLayers(modelApplicationBases);
            }
        
        }

        void GetModelUnSafe(ModelApplicationBase modelApplicationBase, ModelDifferenceObject modelDifferenceObject) {
            var afterSetupLayer = GetAfterSetupLayer(modelApplicationBase);
            modelApplicationBase.AddLayer(afterSetupLayer);
            modelDifferenceObject.GetModel(modelApplicationBase);
            modelApplicationBase.RemoveLayer(afterSetupLayer);
        }

        ModelApplicationBase GetAfterSetupLayer(ModelApplicationBase modelApplicationBase) {
            ModelApplicationBase afterSetupLayer = modelApplicationBase.CreatorInstance.CreateModelApplication();
            afterSetupLayer.Id = "After Setup";
            return afterSetupLayer;
        }


        void AddLayers(IEnumerable<ModelApplicationBase> modelApplicationBases) {
            var applicationBase = ((ModelApplicationBase)_application.Model);
            foreach (ModelApplicationBase modelApplicationBase in modelApplicationBases) {
                applicationBase.AddLayer(modelApplicationBase);
            }
        }

        IEnumerable<ModelApplicationBase> RemoveLayers(ModelApplicationBase modelApplicationBase, IEnumerable<string> strings) {
            var modelApplicationBases = new List<ModelApplicationBase>();
            while (modelApplicationBase.LastLayer.Id != "Unchanged Master Part") {
                if (!(strings.Contains(modelApplicationBase.LastLayer.Id)))
                    modelApplicationBases.Add(modelApplicationBase.LastLayer);
                modelApplicationBase.RemoveLayer(modelApplicationBase.LastLayer);
            }
            return modelApplicationBases;
        }
    }
}
