using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.Dashboard.BusinessObjects {
    public interface ITypeWrapper {
        string Caption { get; }
        Type Type { get; }
        string GetDefaultCaption();
    }

    [DefaultProperty("Caption")]
    [NonPersistent]
    public class TypeWrapper :  ITypeWrapper {
        public TypeWrapper( Type type){
            Type = type;
        }

        public Type Type { get; private set; }

        public string GetDefaultCaption() {
            var modelApplicationBase = ((ModelApplicationBase)CaptionHelper.ApplicationModel);
            var currentAspect = modelApplicationBase.CurrentAspect;
            modelApplicationBase.SetCurrentAspect("");
            var caption = Caption;
            modelApplicationBase.SetCurrentAspect(currentAspect);
            return caption;
        }

        public String Caption {
            get { return CaptionHelper.GetClassCaption(Type.FullName); }
        }

        public override string ToString() {
            return Caption;
        }
    }
}