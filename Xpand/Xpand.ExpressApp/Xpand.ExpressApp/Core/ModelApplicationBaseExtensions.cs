using System;
using DevExpress.ExpressApp.Model.Core;

namespace Xpand.ExpressApp.Core {
    public static class ModelApplicationBaseExtensions {
        public static void AddLayerBeforeLast(this ModelApplicationBase application, ModelApplicationBase layer)
        {
            ModelApplicationBase lastLayer = application.LastLayer;
            if (lastLayer.Id != "After Setup" && lastLayer.Id != "UserDiff")
                throw new ArgumentException("LastLayer.Id", lastLayer.Id);
            application.RemoveLayer(lastLayer);
            application.AddLayer(layer);
            application.AddLayer(lastLayer);
        }
    }
}