using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;

namespace Xpand.Persistent.Base.General {
    public abstract class AttributeRegistrator<T>:AttributeRegistrator where T:class{
        public Type Type{
            get { return typeof(T); }
        }
    }

    public abstract class AttributeRegistrator {
        public abstract IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo);
    }
}
