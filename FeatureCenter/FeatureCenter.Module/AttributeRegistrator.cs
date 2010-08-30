using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;

namespace FeatureCenter.Module
{
    
    public abstract class AttributeRegistrator
    {
        public abstract IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo);
    }
}
