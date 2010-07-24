using System;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace eXpand.ExpressApp.ModelDifference.DataStore.Builders{
    public class ModelDifferenceObjectBuilder
    {
        private static void SetUp(ModelDifferenceObject modelDifferenceObject)
        {
            modelDifferenceObject.DateCreated = DateTime.Now;
            modelDifferenceObject.Name = "AutoCreated " + DateTime.Now;
        }

        public static void SetUp(ModelDifferenceObject modelDifferenceObject, string applicationTypeName)
        {
            SetUp(modelDifferenceObject);
        }
    }
}