using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Core.DynamicModel;

namespace Xpand.ExpressApp.SystemModule {
    public class ModelExtenderPair {
        public ModelExtenderPair(Type interfaceType, Type extenderType) {
            InterfaceType = interfaceType;
            ExtenderType = extenderType;
        }

        public Type InterfaceType { get; set; }
        public Type ExtenderType { get; set; }
    }
    public abstract class OptionsController : ViewController, IModelExtender {


        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            foreach (var modelExtenderPair in GetModelExtenderPairs())
            {
                extenders.Add(modelExtenderPair.InterfaceType, modelExtenderPair.ExtenderType);
            }
            
            IEnumerable<DynamicModelType> dynamicModelTypes = GetDynamicModelTypes().ToList();
            foreach (var dynamicModelType in dynamicModelTypes)
            {
                extenders.Add(dynamicModelType.BaseType, dynamicModelType);
            }
        }

        protected abstract List<ModelExtenderPair> GetModelExtenderPairs();

        protected abstract IEnumerable<DynamicModelType> GetDynamicModelTypes();

    }
}