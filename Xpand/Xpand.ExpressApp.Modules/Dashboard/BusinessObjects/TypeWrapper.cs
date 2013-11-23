using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.Dashboard.BusinessObjects {
    public interface ITypeWrapper {
        string Caption { get; }
        Type Type { get; }
    }

    [DefaultProperty("Caption")]
    [NonPersistent]
    public class TypeWrapper :  ITypeWrapper {
        public TypeWrapper( Type type){
            Type = type;
        }

        public Type Type { get; private set; }

        public String Caption {
            get { return CaptionHelper.GetClassCaption(Type.FullName); }
        }

        public override string ToString() {
            return Caption;
        }
    }
}