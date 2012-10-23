using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

namespace Xpand.ExpressApp.Core {
    public static class CreateCustomModelSynchronizerHelper {
        public static void Assign(CreateCustomModelSynchronizerEventArgs e, IModelSynchronizable modelSynchronizer) {
            var modelSynchronizerList = e.ModelSynchronizer as ModelSynchronizerList;
            if (modelSynchronizerList == null) {
                e.ModelSynchronizer = new ModelSynchronizerList();
            }
            var synchronizerList = ((ModelSynchronizerList)e.ModelSynchronizer);
            synchronizerList.Add(modelSynchronizer);
        }
    }
    public static class ModelNodeExtensions {
        static readonly MethodInfo _methodInfo;

        static ModelNodeExtensions() {
            _methodInfo = typeof(ModelNode).GetMethod("AddNode", new[] { typeof(string) });
        }

        public static IModelNode AddNode(this IModelNode modelNode, Type type, string id) {
            return (IModelNode)_methodInfo.MakeGenericMethod(new[] { type }).Invoke(modelNode, new object[] { id });
        }

    }
    public class ModelNodeWrapper {
        readonly ModelNode _modelNode;

        public ModelNodeWrapper(ModelNode modelNode) {
            _modelNode = modelNode;
        }

        public ModelNode ModelNode {
            get { return _modelNode; }
        }

        public override string ToString() {
            return _modelNode.Id;
        }
    }
    public static class ModelApplicationBaseExtensions {
        public static void RemoveLayer(this ModelApplicationBase application, string id) {
            RefreshLayers(application, @base => @base.Id == id ? null : @base);
        }

        public static void ReplaceLayer(this ModelApplicationBase application, ModelApplicationBase layer) {
            RefreshLayers(application, @base => application.LastLayer.Id == layer.Id ? layer : @base);
        }

        static void RefreshLayers(ModelApplicationBase application, Func<ModelApplicationBase, ModelApplicationBase> func) {
            var modelApplicationBases = new List<ModelApplicationBase>();
            var lastLayer = application.LastLayer;
            ModelApplicationHelper.RemoveLayer(lastLayer);
            var afterSetup = application.LastLayer;
            ModelApplicationHelper.RemoveLayer(afterSetup);
            while (application.LastLayer.Id != "Unchanged Master Part") {
                ModelApplicationBase modelApplicationBase = application.LastLayer;
                modelApplicationBase = func.Invoke(modelApplicationBase);
                if (modelApplicationBase != null)
                    modelApplicationBases.Add(modelApplicationBase);
                ModelApplicationHelper.RemoveLayer(application.LastLayer);
            }
            modelApplicationBases.Reverse();
            foreach (var modelApplicationBase in modelApplicationBases) {
                ModelApplicationHelper.AddLayer(application, modelApplicationBase);
            }
            ModelApplicationHelper.AddLayer(application, afterSetup);
            ModelApplicationHelper.AddLayer(application, lastLayer);
        }

        public static void AddLayerBeforeLast(this ModelApplicationBase application, ModelApplicationBase layer) {
            ModelApplicationBase lastLayer = application.LastLayer;
            if (lastLayer.Id != "After Setup" && lastLayer.Id != "UserDiff")
                throw new ArgumentException("LastLayer.Id", lastLayer.Id);
            ModelApplicationHelper.RemoveLayer(application);
            ModelApplicationHelper.AddLayer(application, layer);
            ModelApplicationHelper.AddLayer(application, lastLayer);
        }

        public static List<ModelNodeWrapper> GetLayers(this ModelApplicationBase modelApplicationBase) {
            var propertyInfo = typeof(ModelNode).GetProperty("Layers", BindingFlags.Instance | BindingFlags.NonPublic);
            return ((List<ModelNode>)propertyInfo.GetValue(modelApplicationBase, null)).Select(node => new ModelNodeWrapper(node)).ToList();
        }

        public static void ReInitLayers(this ModelApplicationBase modelApplicationBase) {
            if (modelApplicationBase.Id == "Application") {
                var lastLayer = modelApplicationBase.LastLayer;
                while (lastLayer.Id != "Unchanged Master Part") {
                    ModelApplicationHelper.RemoveLayer(lastLayer);
                    lastLayer = modelApplicationBase.LastLayer;
                }
                var afterSetupLayer = modelApplicationBase.CreatorInstance.CreateModelApplication();
                afterSetupLayer.Id = "After Setup";
                ModelApplicationHelper.AddLayer(modelApplicationBase, afterSetupLayer);
            }
        }

        public static bool HasAspect(this ModelApplicationBase modelApplicationBase, string aspectName) {
            for (int i = 0; i < modelApplicationBase.AspectCount; i++) {
                if (modelApplicationBase.GetAspect(i) == aspectName)
                    return true;
            }
            return false;
        }
    }
}
