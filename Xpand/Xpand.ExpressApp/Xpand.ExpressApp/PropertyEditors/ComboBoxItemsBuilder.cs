using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.PropertyEditors {
    public class ComboBoxItemsBuilder {

        PropertyEditor _propertyEditor;

        public static ComboBoxItemsBuilder Create() {
            return new ComboBoxItemsBuilder();
        }

        public ComboBoxItemsBuilder WithPropertyEditor(PropertyEditor propertyEditor) {
            _propertyEditor = propertyEditor;
            return this;
        }

        public void Build(Action<IEnumerable<string>, bool> itemsCalculated, Func<bool> itemsCalculating) {
            var dataSourcePropertyAttribute = _propertyEditor.MemberInfo.FindAttribute<DataSourcePropertyAttribute>();
            if (dataSourcePropertyAttribute != null)
                BuildFromDatasource(dataSourcePropertyAttribute, itemsCalculated, itemsCalculating);
            else
                GroupBuild(itemsCalculated);
        }

        void GroupBuild(Action<IEnumerable<string>, bool> itemsCalculated) {
            var xpView = new XPView(((ObjectSpace)_propertyEditor.View.ObjectSpace).Session, _propertyEditor.MemberInfo.GetOwnerInstance(_propertyEditor.CurrentObject).GetType());
            xpView.AddProperty(_propertyEditor.PropertyName, _propertyEditor.PropertyName, true);
            itemsCalculated.Invoke(xpView.OfType<ViewRecord>().Select(record => record[0]).OfType<string>(), false);
        }

        void BuildFromDatasource(DataSourcePropertyAttribute dataSourcePropertyAttribute, Action<IEnumerable<string>, bool> itemsCalculated, Func<bool> itemsCalculating) {
            CompositeView compositeView = _propertyEditor.View;
            if (compositeView != null) {
                compositeView.ObjectSpace.ObjectChanged += (sender, args) => {
                    var invoke = itemsCalculating.Invoke();
                    if (!invoke) {
                        var comboBoxItems = GetComboBoxItemsCore(dataSourcePropertyAttribute);
                        itemsCalculated.Invoke(comboBoxItems, true);
                    }
                };
                var b = itemsCalculating.Invoke();
                if (!b) {
                    var boxItems = GetComboBoxItemsCore(dataSourcePropertyAttribute);
                    itemsCalculated.Invoke(boxItems, false);
                }
            }
        }

        IEnumerable<string> GetComboBoxItemsCore(DataSourcePropertyAttribute dataSourcePropertyAttribute) {
            return ((IEnumerable<string>)_propertyEditor.View.ObjectTypeInfo.FindMember(dataSourcePropertyAttribute.DataSourceProperty).GetValue(_propertyEditor.CurrentObject));
        }

    }
}