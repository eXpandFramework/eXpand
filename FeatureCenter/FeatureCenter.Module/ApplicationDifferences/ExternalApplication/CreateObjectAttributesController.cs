using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using System.Linq;

namespace FeatureCenter.Module.ApplicationDifferences.ExternalApplication
{

    

    public class CreateObjectAttributesController : Module.CreateObjectAttributesController
    {
        protected override string GetTypeToDecorate() {
            return typeof(ModelDifferenceObject).FullName;
        }

        protected override DisplayFeatureModelAttribute GetDisplayFeatureModelAttribute() {
            return new DisplayFeatureModelAttribute("ExternalApplication_DetailView", "ExternalApplication");
        }

    }
}
