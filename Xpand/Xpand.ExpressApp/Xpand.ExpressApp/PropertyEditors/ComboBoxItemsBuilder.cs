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
            return new ComboBoxItemsBuilder() ;
        }

        public ComboBoxItemsBuilder WithPropertyEditor(PropertyEditor propertyEditor) {
            _propertyEditor = propertyEditor;
            return this;
        }

        public void Build(Action<IEnumerable<string>, bool> itemsCalculated) {
            var dataSourcePropertyAttribute = _propertyEditor.MemberInfo.FindAttribute<DataSourcePropertyAttribute>();
            if (dataSourcePropertyAttribute != null) {
                _propertyEditor.View.ObjectSpace.ObjectChanged += (sender, args) => {
                    var comboBoxItems = GetComboBoxItems(dataSourcePropertyAttribute);
                    itemsCalculated.Invoke(comboBoxItems,true);
                };
                itemsCalculated.Invoke(GetComboBoxItems(dataSourcePropertyAttribute),false);
            }
            var xpView = new XPView(((ObjectSpace)_propertyEditor.View.ObjectSpace).Session, _propertyEditor.MemberInfo.GetOwnerInstance(_propertyEditor.CurrentObject).GetType());
            xpView.AddProperty(_propertyEditor.PropertyName, _propertyEditor.PropertyName, true);
            itemsCalculated.Invoke(xpView.OfType<ViewRecord>().Select(record => record[0]).OfType<string>(),false);
        }
        IEnumerable<string> GetComboBoxItems(DataSourcePropertyAttribute dataSourcePropertyAttribute) {
            return ((IEnumerable<string>)_propertyEditor.View.ObjectTypeInfo.FindMember(dataSourcePropertyAttribute.DataSourceProperty).GetValue(_propertyEditor.CurrentObject));
        }

    }
}