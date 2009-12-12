using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.InfoGenerators;
using DevExpress.ExpressApp.NodeWrappers;
using System.Linq;
using DevExpress.Xpo;

namespace eXpand.ExpressApp.Core
{
    public static class ApplicationNodeWrapperExtensions
    {
        public static ApplicationNodeWrapper Create( Type type) {
            var applicationNodeWrapper = new ApplicationNodeWrapper(new Dictionary(new DictionaryNode(ApplicationNodeWrapper.NodeName),Schema.GetCommonSchema()));
            Load(applicationNodeWrapper, GetHierarchryList(type));
            return applicationNodeWrapper;
        }

        static List<Type> GetHierarchryList(Type type) {
            if (type== null)
                return new List<Type>();
            var list = new List<Type> {type};
            Type baseType = type.BaseType;
            while (typeof (IXPSimpleObject).IsAssignableFrom(baseType) &&baseType.GetCustomAttributes(typeof (NonPersistentAttribute), false).Count() == 0) {
                list.Add(baseType);
                baseType = baseType.BaseType;
            }
            return list;
        }

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

        public static ApplicationNodeWrapper Create(Type[] args)
        {
            var applicationNodeWrapper = new ApplicationNodeWrapper(new Dictionary(new DictionaryNode(ApplicationNodeWrapper.NodeName), Schema.GetCommonSchema()));
            var hierarchryList =new List<Type>();
            foreach (var type in args) {
                hierarchryList.AddRange(GetHierarchryList(type));
            }
            Load(applicationNodeWrapper, hierarchryList.Distinct().ToList());
            return applicationNodeWrapper;
        }
    }
}
