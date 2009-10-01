using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace eXpand.ExpressApp.ModelDifference.DataStore.Builders{
    public class ModelDifferenceObjectBuilder
    {
        private static void SetUp(ModelDifferenceObject modelDifferenceObject)
        {

            modelDifferenceObject.DateCreated = DateTime.Now;
            modelDifferenceObject.Name = "AutoCreated " + DateTime.Now;
            var dictionary = new Dictionary(new DictionaryNode(ApplicationNodeWrapper.NodeName), Schema.GetCommonSchema());
            modelDifferenceObject.Model=dictionary;
            
        }

        public static void SetUp(ModelDifferenceObject modelDifferenceObject, string applicationTypeName)
        {
            SetUp(modelDifferenceObject);
            
        }
    }
}