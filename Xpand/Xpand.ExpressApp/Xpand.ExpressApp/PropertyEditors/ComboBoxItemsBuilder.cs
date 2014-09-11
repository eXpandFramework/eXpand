using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;

namespace Xpand.ExpressApp.PropertyEditors {
    [ModelAbstractClass]
    public interface IModelMemberViewItemSortOrder:IModelMemberViewItem{
        [Category(AttributeCategoryNameProvider.Xpand)]
        [ModelBrowsable(typeof(StringLookupPropertyEditorVisibilityCalculator))]
        SortingDirection SortingDirection { get; set; }
    }

    public class StringLookupPropertyEditorVisibilityCalculator :EditorTypeVisibilityCalculator<IStringLookupPropertyEditor>{
         
    }

    public class ComboBoxItemsBuilder {

        PropertyEditor _propertyEditor;

        public static ComboBoxItemsBuilder Create() {
            return new ComboBoxItemsBuilder();
        }

        public ComboBoxItemsBuilder WithPropertyEditor(PropertyEditor propertyEditor) {
            if (!(propertyEditor is IObjectSpaceHolder))
                throw new NotImplementedException(propertyEditor.GetType() + " must implement " + typeof(IObjectSpaceHolder));
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
            var xpView = new XPView(((XPObjectSpace) ((IObjectSpaceHolder) _propertyEditor).ObjectSpace).Session, _propertyEditor.ObjectTypeInfo.Type);
            var columnSortOrder = ((IModelMemberViewItemSortOrder)_propertyEditor.Model).SortingDirection;
            xpView.Sorting=new SortingCollection(new SortProperty(_propertyEditor.PropertyName,columnSortOrder));
            xpView.AddProperty(_propertyEditor.PropertyName, _propertyEditor.PropertyName, true);
            itemsCalculated.Invoke(xpView.OfType<ViewRecord>().Select(record => record[0]).OfType<string>(), false);
        }

        void BuildFromDatasource(DataSourcePropertyAttribute dataSourcePropertyAttribute, Action<IEnumerable<string>, bool> itemsCalculated, Func<bool> itemsCalculating) {
            PropertyChangedEventHandler propertyChangedEventHandler = (sender, args) => BuildFromDatasourceCore(dataSourcePropertyAttribute, itemsCalculated, itemsCalculating, args.PropertyName);

            if (_propertyEditor.ObjectTypeInfo.IsPersistent) {
                ((IObjectSpaceHolder)_propertyEditor).ObjectSpace.ObjectChanged += (sender, args) => BuildFromDatasourceCore(dataSourcePropertyAttribute, itemsCalculated, itemsCalculating, args.PropertyName);
            } else {
                var currentObject = _propertyEditor.CurrentObject as INotifyPropertyChanged;
                if (currentObject != null)
                    ((INotifyPropertyChanged)_propertyEditor.CurrentObject).PropertyChanged += propertyChangedEventHandler;
            }

            BuildFromDataSourceWhenCurrentObjectChanges(dataSourcePropertyAttribute, itemsCalculated, itemsCalculating, propertyChangedEventHandler);

            var b = itemsCalculating.Invoke();
            if (!b) {
                var boxItems = GetComboBoxItemsCore(dataSourcePropertyAttribute);
                itemsCalculated.Invoke(boxItems, false);
            }
        }


        void BuildFromDataSourceWhenCurrentObjectChanges(DataSourcePropertyAttribute dataSourcePropertyAttribute,
                                                         Action<IEnumerable<string>, bool> itemsCalculated, Func<bool> itemsCalculating,
                                                         PropertyChangedEventHandler propertyChangedEventHandler) {
            _propertyEditor.CurrentObjectChanged += (sender, args) => {
                var currentObject = _propertyEditor.CurrentObject as INotifyPropertyChanged;
                if (!_propertyEditor.ObjectTypeInfo.IsPersistent && currentObject != null)
                    currentObject.PropertyChanged += propertyChangedEventHandler;
                BuildFromDatasourceCore(dataSourcePropertyAttribute, itemsCalculated, itemsCalculating, null);
            };

            _propertyEditor.CurrentObjectChanging += (sender, args) => {
                var currentObject = _propertyEditor.CurrentObject as INotifyPropertyChanged;
                if (!_propertyEditor.ObjectTypeInfo.IsPersistent && currentObject != null)
                    currentObject.PropertyChanged -= propertyChangedEventHandler;
            };
        }

        void BuildFromDatasourceCore(DataSourcePropertyAttribute dataSourcePropertyAttribute, Action<IEnumerable<string>, bool> itemsCalculated, Func<bool> itemsCalculating, string propertyName) {
            if (_propertyEditor.PropertyName != propertyName) {
                var invoke = itemsCalculating.Invoke();
                if (!invoke) {
                    var comboBoxItems = GetComboBoxItemsCore(dataSourcePropertyAttribute);
                    itemsCalculated.Invoke(comboBoxItems, true);
                }
            }
        }


        IEnumerable<string> GetComboBoxItemsCore(DataSourcePropertyAttribute dataSourcePropertyAttribute) {
            return ((IEnumerable<string>)_propertyEditor.ObjectTypeInfo.FindMember(dataSourcePropertyAttribute.DataSourceProperty).GetValue(_propertyEditor.CurrentObject));
        }

    }
}