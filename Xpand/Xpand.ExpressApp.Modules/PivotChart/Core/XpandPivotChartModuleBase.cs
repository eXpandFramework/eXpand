using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.Core;

namespace Xpand.ExpressApp.PivotChart.Core {
    public abstract class XpandPivotChartModuleBase : XpandModuleBase, ITypeInfoContainer {
        public override void AddGeneratorUpdaters(DevExpress.ExpressApp.Model.Core.ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(GetAnalysisPropertyEditorNodeUpdater());
        }

        protected abstract IModelNodesGeneratorUpdater GetAnalysisPropertyEditorNodeUpdater();

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            TypesInfo.AddTypes(GetAdditionalClasses(moduleManager));
        }
        public abstract TypesInfo TypesInfo { get; }


        void CreateMembers(ITypesInfo typesInfo, Type optionsType, Type persistentType) {
            ITypeInfo typeInfo = typesInfo.FindTypeInfo(ReflectionHelper.GetType(persistentType.Name));
            IEnumerable<PropertyInfo> propertyInfos = optionsType.GetProperties().Where(info => info.GetSetMethod() != null).Where(propertyInfo => typeInfo.FindMember(propertyInfo.Name) == null);
            foreach (PropertyInfo propertyInfo in propertyInfos) {
                OnCreateMember(typeInfo, propertyInfo.Name, propertyInfo.PropertyType);
            }
        }

        protected virtual IMemberInfo OnCreateMember(ITypeInfo typeInfo, string name, Type propertyType) {
            IMemberInfo memberInfo = typeInfo.CreateMember(name, propertyType);
            if (memberInfo.MemberType == typeof(Type))
                memberInfo.AddAttribute(new ValueConverterAttribute(typeof(TypeValueConverter)));
            return memberInfo;
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            if (Application == null)
                return;
            typesInfo.FindTypeInfo(TypesInfo.AnalysisType).AddAttribute(new DefaultPropertyAttribute("Name"));
            foreach (var keyValuePair in GetOptionsMapperDictionary()) {
                CreateMembers(typesInfo, keyValuePair.Key, keyValuePair.Value);
            }
        }

        protected abstract Dictionary<Type, Type> GetOptionsMapperDictionary();
    }

    public interface ITypeInfoContainer {
        TypesInfo TypesInfo { get; }
    }
}