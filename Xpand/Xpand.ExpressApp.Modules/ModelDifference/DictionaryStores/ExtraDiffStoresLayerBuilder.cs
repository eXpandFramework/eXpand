﻿using System.Collections.Generic;
using System.Linq;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.ModelDifference;

namespace Xpand.ExpressApp.ModelDifference.DictionaryStores {
    internal class ExtraDiffStoresLayerBuilder{
        public void AddLayers(Dictionary<string, ModelDifferenceObjectInfo> modelDifferenceObjectInfos, List<ModelApplicationFromStreamStoreBase> extraDiffsStore){
            foreach (var modelDifferenceObjectInfo in modelDifferenceObjectInfos){
                var layer = modelDifferenceObjectInfo.Value.Model;
                IEnumerable<ModelApplicationFromStreamStoreBase> extraDiffStores =extraDiffsStore.Where(extraDiffStore => layer.Id == extraDiffStore.Name);
                foreach (ModelApplicationFromStreamStoreBase extraDiffStore in extraDiffStores){
                    extraDiffStore.Load(layer);
                    Tracing.Tracer.LogVerboseValue("Name", extraDiffStore.Name);
                    modelDifferenceObjectInfo.Value.ModelDifferenceObject.CreateAspects(layer);
                }
            }
        }
    }
}