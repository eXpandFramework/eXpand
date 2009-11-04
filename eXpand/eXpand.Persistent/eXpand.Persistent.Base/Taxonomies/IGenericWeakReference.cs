using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Xpo;

namespace eXpand.Persistent.Base.Taxonomies{
    public interface IGenericWeakReference<TObjectType> where TObjectType : IXPObject {
        string AssociatedObjectName { get; set; }
        TObjectType Owner { get; }
        void EvaluatePropertyValues();
    }

    public interface IGenericWeakReference {
        string AssociatedObjectName { get; set; }
        IXPObject Owner { get; }
        void EvaluatePropertyValues();
    }
}