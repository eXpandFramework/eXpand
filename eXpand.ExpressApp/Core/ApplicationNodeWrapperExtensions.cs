using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.InfoGenerators;
using DevExpress.ExpressApp.NodeWrappers;

namespace eXpand.ExpressApp.Core
{
    public static class ApplicationNodeWrapperExtensions
    {

        public static void Load(this ApplicationNodeWrapper wrapper, ITypeInfo typeInfo){
            Load(wrapper, XafTypesInfo.CastTypeInfoToType(typeInfo));
        }

        public static void Load(this ApplicationNodeWrapper wrapper, Type type)
        {
            ITypeInfo roleAspectTypeInfo = XafTypesInfo.CastTypeToTypeInfo(type);
            var loader = new XPObjectModelLoader();
            loader.Load(wrapper.BOModel, 
                                           new List<ITypeInfo> { roleAspectTypeInfo },
                                           new List<IObjectModelCustomLoader>());
            var generator = new DefaultInfoGenerator();
            generator.Generate(wrapper);
        }

    }
}
