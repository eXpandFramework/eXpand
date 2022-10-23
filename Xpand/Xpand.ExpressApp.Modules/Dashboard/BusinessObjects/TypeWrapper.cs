using System;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.Dashboard.BusinessObjects {
    public interface ITypeWrapper {
        Type Type { get; }
        string GetDefaultCaption(ModelApplicationBase modelApplicationBase);
        string GetDefaultCaption();
    }

    [DefaultProperty("Caption")]
    [DomainComponent]
    public class TypeWrapper :  ITypeWrapper {
        public TypeWrapper( Type type){
            Type = type;
        }

        public Type Type { get; private set; }

        public string GetDefaultCaption(ModelApplicationBase modelApplicationBase){
            if (modelApplicationBase != null){
                var currentAspect = modelApplicationBase.CurrentAspect;
                modelApplicationBase.SetCurrentAspect("");
                var caption = modelApplicationBase.Application.BOModel[Type.FullName].Caption;
                modelApplicationBase.SetCurrentAspect(currentAspect);
                return caption;
            }
            return Type.FullName;
        }

        public string GetDefaultCaption() {
            var modelApplicationBase = ((ModelApplicationBase)CaptionHelper.ApplicationModel);
            return GetDefaultCaption(modelApplicationBase);
        }


        public override string ToString() {
            return GetDefaultCaption();
        }
    }
}