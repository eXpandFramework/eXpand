using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo.Metadata;
using Xpand.Utils.Helpers;

namespace Xpand.Persistent.Base.Xpo{
    public class AssemblyXpoTypeInfoSource:XpandXpoTypeInfoSource, IEntityStore{
        private readonly Assembly _assembly;
        private static readonly ReflectionDictionary _reflectionDictionary = new ReflectionDictionary();
        private static readonly Dictionary<Assembly, XpandXpoTypeInfoSource> _assemblyInstance = new Dictionary<Assembly, XpandXpoTypeInfoSource>();
        public AssemblyXpoTypeInfoSource(TypesInfo typesInfo, Assembly assembly) : this(typesInfo, assembly, Type.EmptyTypes) {
        }

        public AssemblyXpoTypeInfoSource(TypesInfo typesInfo, Assembly assembly, params Type[] types) : this(typesInfo, GetDictionary(types)) {
            _assembly = assembly;
        }

        private AssemblyXpoTypeInfoSource(TypesInfo typesInfo, XPDictionary xpDictionary) : base(typesInfo,xpDictionary){
            
        }

        private static ReflectionDictionary GetDictionary(Type[] types) {
            var reflectionDictionary = new ReflectionDictionary();
            reflectionDictionary.CollectClassInfos(types);
            return reflectionDictionary;
        }

        public static Dictionary<Assembly, XpandXpoTypeInfoSource> AssemblyInstance => _assemblyInstance;

        void IEntityStore.RegisterEntity(Type type) {
            if (((IEntityStore)this).CanRegister(type))
                RegisterEntity(type);
        }

        bool IEntityStore.CanRegister(Type type) {
            return CanRegister(type) && IsValidType(type);
        }

        public static void CreateSource(Assembly assembly) {
            var typesInfo = (TypesInfo)XafTypesInfo.Instance;
            var types = assembly.GetTypes().Where(type => _reflectionDictionary.CanGetClassInfoByType(type)).ToArray();
            var worldCreatorDBMapperTypeInfoSource = new AssemblyXpoTypeInfoSource(typesInfo, assembly, types);
            typesInfo.AddEntityStore(worldCreatorDBMapperTypeInfoSource);
            worldCreatorDBMapperTypeInfoSource.EntityTypes.Each(XafTypesInfo.Instance.RegisterEntity);
            _assemblyInstance[assembly] = worldCreatorDBMapperTypeInfoSource;
        }

        private bool IsValidType(Type type) {
            return type.Assembly == _assembly && _reflectionDictionary.CanGetClassInfoByType(type);
        }
    }
}