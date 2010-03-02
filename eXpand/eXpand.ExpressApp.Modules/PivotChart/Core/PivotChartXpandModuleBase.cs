using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.PivotChart.Core {
    public abstract class PivotChartXpandModuleBase : ModuleBase,ITypeInfoContainer {
        public override void UpdateModel(Dictionary model)
        {
            base.UpdateModel(model);
            IAnalysisInfo analysisInfo = null;
            string propertyName = analysisInfo.GetPropertyName(x=>x.Self);
            var analysisInfoNodeWrappers = new ApplicationNodeWrapper(model).BOModel.Classes.Where(wrapper => typeof(IAnalysisInfo).IsAssignableFrom(wrapper.ClassTypeInfo.Type)).SelectMany(nodeWrapper => nodeWrapper.Properties).Where(infoNodeWrapper => infoNodeWrapper.Name == propertyName);
            foreach (var propertyInfoNodeWrapper in analysisInfoNodeWrappers) {
                propertyInfoNodeWrapper.PropertyEditorType = GetPropertyEditorType().FullName;
            }
            
        }
        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            if (Application != null)
                TypesInfo.AddTypes(Application.Modules.SelectMany(@base => @base.AdditionalBusinessClasses));
        }

        public abstract TypesInfo TypesInfo{ get;}

        protected abstract Type GetPropertyEditorType();
        void CreateMembers(ITypesInfo typesInfo, Type optionsType, Type persistentType)
        {
            ITypeInfo typeInfo = typesInfo.FindTypeInfo(ReflectionHelper.GetType(persistentType.Name));
            foreach (PropertyInfo propertyInfo in optionsType.GetProperties().Where(info => info.GetSetMethod() != null)){
                if (typeInfo.FindMember(propertyInfo.Name)== null)
                    OnCreateMember(typeInfo, propertyInfo.Name, propertyInfo.PropertyType);
            }
        }

        protected virtual IMemberInfo OnCreateMember(ITypeInfo typeInfo, string name, Type propertyType) {
            return typeInfo.CreateMember(name, propertyType);
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            base.CustomizeTypesInfo(typesInfo);
            if (Application == null)
                return;
            typesInfo.FindTypeInfo(TypesInfo.AnalysisType).AddAttribute(new DefaultPropertyAttribute("Name"));
            foreach (var keyValuePair in GetOptionsMapperDictionary()){
                CreateMembers(typesInfo, keyValuePair.Key, keyValuePair.Value);
            }
        }

        protected abstract System.Collections.Generic.Dictionary<Type,Type> GetOptionsMapperDictionary();
    }

    public interface ITypeInfoContainer {
        TypesInfo TypesInfo { get; }
    }
}