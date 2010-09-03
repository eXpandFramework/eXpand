using System;
using System.ComponentModel;
using System.Linq.Expressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.Core;

namespace eXpand.ExpressApp.SystemModule {
    public abstract class UpdateTypeConverterController<T, TReferenceConverter> : ViewController
        where T : class
        where TReferenceConverter : XpandReferenceConverter
    {
        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            ITypeInfo typeInfo = typesInfo.FindTypeInfo(typeof(T));
            typeInfo.AddAttribute(Expression(), new TypeConverterAttribute(typeof(TReferenceConverter)));
        }

        protected abstract Expression<Func<T, object>> Expression();
    }
}