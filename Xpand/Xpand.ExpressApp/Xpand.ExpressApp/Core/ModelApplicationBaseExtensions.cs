using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model.Core;

namespace Xpand.ExpressApp.Core {
    public static class ModelApplicationBaseExtensions {
        public static void RemoveLayer(this ModelApplicationBase application, string id) {
            RefreshLayers(application, @base => @base.Id==id?null:@base);
        }

        public static void ReplaceLayer(this ModelApplicationBase application, ModelApplicationBase layer) {
            RefreshLayers(application, @base => application.LastLayer.Id == layer.Id ? layer : @base);
        }

        static void RefreshLayers(ModelApplicationBase application,  Func<ModelApplicationBase, ModelApplicationBase> func) {
            var modelApplicationBases = new List<ModelApplicationBase>();
            var lastLayer = application.LastLayer;
            application.RemoveLayer(lastLayer);
            var afterSetup = application.LastLayer;
            application.RemoveLayer(afterSetup);
            while (application.LastLayer.Id != "Unchanged Master Part") {
                ModelApplicationBase modelApplicationBase = application.LastLayer;
                modelApplicationBase = func.Invoke(modelApplicationBase);
                if (modelApplicationBase!=null)
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