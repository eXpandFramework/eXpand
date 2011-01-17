using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model.Core;

namespace Xpand.ExpressApp.Core {
    public static class ModelApplicationBaseExtensions {
        public static void ReplaceLayer(this ModelApplicationBase application, ModelApplicationBase layer) {
            var modelApplicationBases=new List<ModelApplicationBase>();
            var lastLayer = application.LastLayer;
            application.RemoveLayer(lastLayer);
            var afterSetup =application.LastLayer;
            application.RemoveLayer(afterSetup);
            while (application.LastLayer.Id != "Unchanged Master Part") {
                ModelApplicationBase modelApplicationBase;
                if (application.LastLayer.Id == layer.Id) 
                    modelApplicationBase = layer;
                else
                    modelApplicationBase = application.LastLayer;
                modelApplicationBases.Add(modelApplicationBase);
                application.RemoveLayer(application.LastLayer);
            }
            modelApplicationBases.Reverse();
            foreach (var modelApplicationBase in modelApplicationBases) {
                application.AddLayer(modelApplicationBase);
            }
            application.AddLayer(afterSetup);
            application.AddLayer(lastLayer);
        }

        public static void AddLayerBeforeLast(this ModelApplicationBase application, ModelApplicationBase layer) {
            ModelApplicationBase lastLayer = application.LastLayer;
            if (lastLayer.Id != "After Setup" && lastLayer.Id != "UserDiff")
                throw new ArgumentException("LastLayer.Id", lastLayer.Id);
            application.RemoveLayer(lastLayer);
            application.AddLayer(layer);
            application.AddLayer(lastLayer);
        }
        
        public static bool HasAspect(this ModelApplicationBase modelApplicationBase, string aspectName) {
            for (int i = 0; i < modelApplicationBase.AspectCount; i++) {
                if (modelApplicationBase.GetAspect(i)==aspectName)
                    return true;
            }
            return false;
        }
    }
}