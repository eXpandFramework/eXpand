//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using DevExpress.ExpressApp;
//using DevExpress.ExpressApp.DC;
//using DevExpress.Xpo.Metadata;
//using Xpand.Persistent.Base.General;
//using Xpand.Utils.Helpers;
//
//namespace Xpand.ExpressApp.WorldCreator.System {
//    public class WorldCreatorAssemblyTypeInfoSource : XpandXpoTypeInfoSource, IEntityStore {
//        private readonly Assembly _assembly;
//        private static readonly ReflectionDictionary _reflectionDictionary = new ReflectionDictionary();
//        private static readonly Dictionary<Assembly, WorldCreatorAssemblyTypeInfoSource> _instance = new Dictionary<Assembly, WorldCreatorAssemblyTypeInfoSource>();
//
//        public WorldCreatorAssemblyTypeInfoSource(TypesInfo typesInfo, Assembly assembly) : this(typesInfo, assembly, Type.EmptyTypes) {
//        }
//
//        public WorldCreatorAssemblyTypeInfoSource(TypesInfo typesInfo, Assembly assembly, params Type[] types) : this(typesInfo, GetDictionary(types)) {
//            _assembly = assembly;
//        }
//
//        private static ReflectionDictionary GetDictionary(Type[] types) {
//            var reflectionDictionary = new ReflectionDictionary();
//            reflectionDictionary.CollectClassInfos(types);
//            return reflectionDictionary;
//        }
//
//        public WorldCreatorAssemblyTypeInfoSource(TypesInfo typesInfo, XPDictionary dictionary) : base(typesInfo, dictionary) {
//        }
//
//        public static Dictionary<Assembly, WorldCreatorAssemblyTypeInfoSource> Instance => _instance;
//
//        void IEntityStore.RegisterEntity(Type type) {
//            if (((IEntityStore)this).CanRegister(type))
//                RegisterEntity(type);
//        }
//
//        bool IEntityStore.CanRegister(Type type) {
//            return CanRegister(type) && IsValidType(type);
//        }
//
//        public static void CreateSource(Assembly assembly) {
//            var typesInfo = (TypesInfo)XafTypesInfo.Instance;
//            var types = assembly.GetTypes().Where(type => _reflectionDictionary.CanGetClassInfoByType(type)).ToArray();
//            var worldCreatorDBMapperTypeInfoSource = new WorldCreatorAssemblyTypeInfoSource(typesInfo, assembly, types);
//            typesInfo.AddEntityStore(worldCreatorDBMapperTypeInfoSource);
//            worldCreatorDBMapperTypeInfoSource.EntityTypes.Each(XafTypesInfo.Instance.RegisterEntity);
//            _instance[assembly] = worldCreatorDBMapperTypeInfoSource;
//        }
//
//        private bool IsValidType(Type type) {
//            return type.Assembly == _assembly && _reflectionDictionary.CanGetClassInfoByType(type);
//        }
//    }
//}
