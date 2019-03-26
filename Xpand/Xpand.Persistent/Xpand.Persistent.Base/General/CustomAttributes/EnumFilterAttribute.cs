using System;
using System.Collections;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.General.CustomAttributes {
    public interface IEnumPropertyEditor {
        
    }

    public static class EnumPropertyEditorExtensions {
        public static void SetupDataSource<TControlItem>(this IEnumPropertyEditor editor, TControlItem[] startitems, IList controlItems,Func<TControlItem,object> itemValueSelector){
            var propertyEditor = ((PropertyEditor) editor);
            var dataSourcePropertyAttribute = propertyEditor.MemberInfo.FindAttribute<DataSourcePropertyAttribute>();
            if (dataSourcePropertyAttribute != null && propertyEditor.CurrentObject != null){
                var member = propertyEditor.MemberInfo.Owner.FindMember(dataSourcePropertyAttribute.DataSourceProperty);
                var items = ((IEnumerable) member.GetValue(propertyEditor.CurrentObject)).Cast<object>()
                    .Select(o => startitems.First(item => $"{itemValueSelector(item)}" == $"{o}")).ToArray();
                controlItems.Clear();
                foreach (var item in items) {
                    controlItems.Add(item);
                }
            }

            foreach (var attribute in propertyEditor.ObjectTypeInfo.FindAttributes<EnumFilterAttribute>()){
                if (attribute.PropertyName == propertyEditor.PropertyName){
                    var isObjectFitForCriteria = attribute.Criteria == null? true
                        : propertyEditor.View.ObjectSpace.IsObjectFitForCriteria(propertyEditor.MemberInfo.MemberType, propertyEditor.CurrentObject,
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