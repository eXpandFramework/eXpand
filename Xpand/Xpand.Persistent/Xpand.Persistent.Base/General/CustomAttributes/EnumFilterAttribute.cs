using System;
using System.Collections;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.General.CustomAttributes {
    public interface IEnumPropertyEditor {
        
    }

    public static class EnumPropertyEditorExtensions {
        public static void SetupEnumPropertyDataSource<TControlItem>(this IMemberInfo memberInfo,object objectInstance,IObjectSpace objectSpace,
            TControlItem[] startitems, IList controlItems, Func<TControlItem, object> itemValueSelector) {
            
            var dataSourcePropertyAttribute = memberInfo.FindAttribute<DataSourcePropertyAttribute>();
            if (dataSourcePropertyAttribute != null && objectInstance != null){
                var noneItem = startitems.FirstOrDefault(item => itemValueSelector(item)==null);
                var member = memberInfo.Owner.FindMember(dataSourcePropertyAttribute.DataSourceProperty);
                var items = ((IEnumerable) member.GetValue(objectInstance)).Cast<object>()
                    .Select(o => {
                        var controlItem = startitems.First(item => $"{itemValueSelector(item)}" == $"{o}");
                        return controlItem;
                    }).ToArray();
                controlItems.Clear();
                if (noneItem != null) {
                    controlItems.Add(noneItem);
                }
                foreach (var item in items) {
                    controlItems.Add(item);
                }
            }

            foreach (var attribute in memberInfo.Owner.FindAttributes<EnumFilterAttribute>()){
                if (attribute.PropertyName == memberInfo.Name){
                    var isObjectFitForCriteria = attribute.Criteria == null? true
                        : objectSpace.IsObjectFitForCriteria(memberInfo.MemberType, objectInstance,
                            CriteriaOperator.Parse(attribute.Criteria));
                    if (isObjectFitForCriteria.HasValue && isObjectFitForCriteria.Value){
                        var items = dataSourcePropertyAttribute == null ? startitems : controlItems.Cast<TControlItem>().ToArray();
                        controlItems.Clear();
                        items = attribute.FilterMode == EnumFilterMode.Remove
                            ? items.Where(item => !attribute.Values.Contains(item)).ToArray()
                            : items.Where(item => attribute.Values.Contains(item)).ToArray();
                        foreach (var item in items) {
                            controlItems.Add(item);
                        }
                    }
                }
            }
        }

    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class EnumFilterAttribute : Attribute {
        public EnumFilterAttribute(string propertyName,EnumFilterMode filterMode, string criteria, params object[] values) {
            PropertyName = propertyName;
            FilterMode = filterMode;
            Criteria = criteria;
            Values = values;
        }
        public EnumFilterAttribute(string propertyName,EnumFilterMode filterMode,  params object[] values):this(propertyName,filterMode,null,values) {
            PropertyName = propertyName;
            Values = values;
        }

        public string PropertyName { get; }
        public EnumFilterMode FilterMode{ get; }
        public string Criteria{ get; }
        public object[] Values { get; }
    }

    public enum EnumFilterMode {
        Remove,
        Allow
    }
}