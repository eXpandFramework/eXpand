using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;

namespace Xpand.ExpressApp.Core
{
    
    public abstract class AttributeRegistrator
    {
        public abstract IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo);
    }
}
