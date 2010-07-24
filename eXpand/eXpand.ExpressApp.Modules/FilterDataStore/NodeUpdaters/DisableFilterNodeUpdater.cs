using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.FilterDataStore.Model;
using eXpand.ExpressApp.FilterDataStore.NodeGenerators;

namespace eXpand.ExpressApp.FilterDataStore.NodeUpdaters {
    public abstract class DisableFilterNodeUpdater : ModelNodesGeneratorUpdater<ModelDisabledDataStoreFiltersNodesGenerator> {
        public override void UpdateNode(ModelNode node) {
            if (!GetTypes().Contains(((IModelClass)node.Parent).TypeInfo.Type)){
                var modelDisabledDataStoreFilter = node.AddNode<IModelDisabledDataStoreFilter>();
                modelDisabledDataStoreFilter.Name = GetFilterProviderName();
            }

        }

        protected abstract List<Type> GetTypes();

        protected abstract string GetFilterProviderName();
    }
}