using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.InfoGenerators;
using DevExpress.ExpressApp.NodeWrappers;
using System.Linq;

namespace eXpand.ExpressApp.Core
{
    public static class ApplicationNodeWrapperExtensions
    {

        public static void Load(this ApplicationNodeWrapper wrapper, ITypeInfo typeInfo){
            Load(wrapper, XafTypesInfo.CastTypeInfoToType(typeInfo));
        }

        public static void Load(this ApplicationNodeWrapper wrapper, List<Type> types) {
            IEnumerable<ITypeInfo> typeInfos = types.Select(type1 => XafTypesInfo.CastTypeToTypeInfo(type1));
            loadBOModel(wrapper, typeInfos);
            generateViews(wrapper);
        }
        public static void Load(this ApplicationNodeWrapper wrapper, Type type) {
            loadBOModel(wrapper,new List<ITypeInfo>{XafTypesInfo.CastTypeToTypeInfo(type)});
            generateViews(wrapper);
        }

        private static void generateViews(ApplicationNodeWrapper wrapper) {
            var generator = new DefaultInfoGenerator();
            generator.Generate(wrapper);
        }

        private static void loadBOModel( ApplicationNodeWrapper wrapper, IEnumerable<ITypeInfo> domainComponents) {
            var loader = new XPObjectModelLoader();
            loader.Load(wrapper.BOModel, domainComponents,new List<IObjectModelCustomLoader>());
        }
    }
}
