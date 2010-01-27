using System;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.PivotChart.Win.Core;
using System.Linq;

namespace eXpand.ExpressApp.PivotChart.Win {
    public sealed partial class PivotChartWinModule : ModuleBase {
        public PivotChartWinModule() {
            InitializeComponent();
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            foreach (var keyValuePair in PivotGridOptionMapper.Instance.Dictionary){
                CreateMembers(typesInfo,keyValuePair.Key,keyValuePair.Value);
            }
        }

        void CreateMembers(ITypesInfo typesInfo, Type optionsType, Type persistentType) {
            ITypeInfo typeInfo = typesInfo.FindTypeInfo(ReflectionHelper.GetType(persistentType.Name));
            foreach (PropertyInfo propertyInfo in optionsType.GetProperties().Where(info => info.GetSetMethod()!=null)) {
                typeInfo.CreateMember(propertyInfo.Name, propertyInfo.PropertyType);
            }
        }
    }
}