using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp.DC;
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

    public class StringLookupPropertyEditorVisibilityCalculator :EditorTypeVisibilityCalculator<IStringLookupPropertyEditor,IModelMemberViewItem>{
         
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
            else if (!string.IsNullOrEmpty(_propertyEditor.Model.PredefinedValues)) {
                itemsCalculated(PredefinedValuesEditorHelper.CreatePredefinedValuesFromString(_propertyEditor.Model.PredefinedValues), false);
            }
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
            void Handler(object sender, PropertyChangedEventArgs args) => BuildFromDatasourceCore(dataSourcePropertyAttribute, itemsCalculated, itemsCalculating, args.PropertyName);

            if (_propertyEditor.ObjectTypeInfo.IsPersistent) {
                ((IObjectSpaceHolder)_propertyEditor).ObjectSpace.ObjectChanged += (sender, args) => BuildFromDatasourceCore(dataSourcePropertyAttribute, itemsCalculated, itemsCalculating, args.PropertyName);
            } else {
                if (_propertyEditor.CurrentObject is INotifyPropertyChanged changed)
                    changed.PropertyChanged += Handler;
            }

            BuildFromDataSourceWhenCurrentObjectChanges(dataSourcePropertyAttribute, itemsCalculated, itemsCalculating, Handler);

            var b = itemsCalculating.Invoke();
            if (!b){
                var boxItems = GetComboBoxItemsCore(dataSourcePropertyAttribute);
                if (boxItems != null) itemsCalculated.Invoke(boxItems, false);
            }
        }


        void BuildFromDataSourceWhenCurrentObjectChanges(DataSourcePropertyAttribute dataSourcePropertyAttribute,
                                                         Action<IEnumerable<string>, bool> itemsCalculated, Func<bool> itemsCalculating,
                                                         PropertyChangedEventHandler propertyChangedEventHandler) {
            _propertyEditor.CurrentObjectChanged += (sender, args) => {
                if (!_propertyEditor.ObjectTypeInfo.IsPersistent && _propertyEditor.CurrentObject is INotifyPropertyChanged currentObject)
                    currentObject.PropertyChanged += propertyChangedEventHandler;
                BuildFromDatasourceCore(dataSourcePropertyAttribute, itemsCalculated, itemsCalculating, null);
            };

            _propertyEditor.CurrentObjectChanging += (sender, args) => {
                if (!_propertyEditor.ObjectTypeInfo.IsPersistent && _propertyEditor.CurrentObject is INotifyPropertyChanged currentObject)
                    currentObject.PropertyChanged -= propertyChangedEventHandler;
            };
        }

        void BuildFromDatasourceCore(DataSourcePropertyAttribute dataSourcePropertyAttribute, Action<IEnumerable<string>, bool> itemsCalculated, Func<bool> itemsCalculating, string propertyName) {
            if (_propertyEditor.PropertyName != propertyName) {
                var invoke = itemsCalculating.Invoke();
                if (!invoke){
                    var comboBoxItems = GetComboBoxItemsCore(dataSourcePropertyAttribute);
                    if (comboBoxItems != null) itemsCalculated.Invoke(comboBoxItems, true);
                }
            }
        }


        IEnumerable<string> GetComboBoxItemsCore(DataSourcePropertyAttribute dataSourcePropertyAttribute) {
            var objectTypeInfo = _propertyEditor.ObjectTypeInfo;
            if (_propertyEditor.MemberInfo is MemberPathInfo memberInfo) {
                objectTypeInfo = memberInfo.LastMember.Owner;
            }
            return ((IEnumerable<string>)objectTypeInfo.FindMember(dataSourcePropertyAttribute.DataSourceProperty).GetValue(_propertyEditor.CurrentObject));
        }

    }
}