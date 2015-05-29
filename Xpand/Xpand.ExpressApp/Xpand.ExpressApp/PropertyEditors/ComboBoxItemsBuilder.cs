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

        public void Build(Action<IEnumerable<string>, bool> itemsCalculated, Func<bool> itemsCalculating, string[] defaultValues=null){
            if (defaultValues == null)
                defaultValues = (_propertyEditor.Model.ModelMember.PredefinedValues + "").Split(';');
            var dataSourcePropertyAttribute = _propertyEditor.MemberInfo.FindAttribute<DataSourcePropertyAttribute>();
            if (dataSourcePropertyAttribute != null)
                BuildFromDatasource(dataSourcePropertyAttribute, defaultValues, itemsCalculated, itemsCalculating);
            else{
                var dataSourceCriteriaAttribute = _propertyEditor.MemberInfo.FindAttribute<DataSourceCriteriaAttribute>();
                GroupBuild(dataSourceCriteriaAttribute, defaultValues, itemsCalculated);
            }
        }

        void GroupBuild(DataSourceCriteriaAttribute dataSourceCriteriaAttribute, IEnumerable<string> defaultValues, Action<IEnumerable<string>, bool> itemsCalculated) {
            var xpView = new XPView(((XPObjectSpace) ((IObjectSpaceHolder) _propertyEditor).ObjectSpace).Session, _propertyEditor.ObjectTypeInfo.Type);
            if (dataSourceCriteriaAttribute != null)
                xpView.Criteria = DevExpress.Data.Filtering.CriteriaOperator.Parse(dataSourceCriteriaAttribute.DataSourceCriteria);
            var columnSortOrder = ((IModelMemberViewItemSortOrder)_propertyEditor.Model).SortingDirection;
            xpView.Sorting=new SortingCollection(new SortProperty(_propertyEditor.PropertyName,columnSortOrder));
            xpView.AddProperty(_propertyEditor.PropertyName, _propertyEditor.PropertyName, true);
            var list1 = xpView.OfType<ViewRecord>().Select(record => record[0]).Cast<string>().ToArray();
            itemsCalculated.Invoke(list1.Concat(defaultValues.Where(v => !list1.Contains(v))), false);
        }

        void BuildFromDatasource(DataSourcePropertyAttribute dataSourcePropertyAttribute, string[] defaultValues, 
                                                        Action<IEnumerable<string>, bool> itemsCalculated, Func<bool> itemsCalculating){
            PropertyChangedEventHandler propertyChangedEventHandler = (sender, args) => BuildFromDatasourceCore(dataSourcePropertyAttribute, defaultValues, itemsCalculated, itemsCalculating, args.PropertyName);

            if (_propertyEditor.ObjectTypeInfo.IsPersistent) {
                ((IObjectSpaceHolder)_propertyEditor).ObjectSpace.ObjectChanged += (sender, args) => BuildFromDatasourceCore(dataSourcePropertyAttribute, defaultValues, itemsCalculated, itemsCalculating, args.PropertyName);
            } else {
                var currentObject = _propertyEditor.CurrentObject as INotifyPropertyChanged;
                if (currentObject != null)
                    ((INotifyPropertyChanged)_propertyEditor.CurrentObject).PropertyChanged += propertyChangedEventHandler;
            }

            BuildFromDataSourceWhenCurrentObjectChanges(dataSourcePropertyAttribute, defaultValues, itemsCalculated, itemsCalculating, propertyChangedEventHandler);

            var b = itemsCalculating.Invoke();
            if (!b) {
                var boxItems = GetComboBoxItemsCore(dataSourcePropertyAttribute, defaultValues);
                itemsCalculated.Invoke(boxItems, false);
            }
        }


        void BuildFromDataSourceWhenCurrentObjectChanges(DataSourcePropertyAttribute dataSourcePropertyAttribute,
                                                         IEnumerable<string> defaultValues,
                                                         Action<IEnumerable<string>, bool> itemsCalculated, Func<bool> itemsCalculating,
                                                         PropertyChangedEventHandler propertyChangedEventHandler) {
            _propertyEditor.CurrentObjectChanged += (sender, args) => {
                var currentObject = _propertyEditor.CurrentObject as INotifyPropertyChanged;
                if (!_propertyEditor.ObjectTypeInfo.IsPersistent && currentObject != null)
                    currentObject.PropertyChanged += propertyChangedEventHandler;
                BuildFromDatasourceCore(dataSourcePropertyAttribute, defaultValues, itemsCalculated, itemsCalculating, null);
            };

            _propertyEditor.CurrentObjectChanging += (sender, args) => {
                var currentObject = _propertyEditor.CurrentObject as INotifyPropertyChanged;
                if (!_propertyEditor.ObjectTypeInfo.IsPersistent && currentObject != null)
                    currentObject.PropertyChanged -= propertyChangedEventHandler;
            };
        }

        void BuildFromDatasourceCore(DataSourcePropertyAttribute dataSourcePropertyAttribute,
                                                            IEnumerable<string> defaultValues,
                                                            Action<IEnumerable<string>, bool> itemsCalculated, 
                                                            Func<bool> itemsCalculating, string propertyName) {
            if (_propertyEditor.PropertyName != propertyName) {
                var invoke = itemsCalculating.Invoke();
                if (!invoke) {
                    var comboBoxItems = GetComboBoxItemsCore(dataSourcePropertyAttribute, defaultValues);
                    itemsCalculated.Invoke(comboBoxItems, true);
                }
            }
        }


        IEnumerable<string> GetComboBoxItemsCore(DataSourcePropertyAttribute dataSourcePropertyAttribute, IEnumerable<string> defaultValues){
            var retVal = ((IEnumerable<string>)_propertyEditor.ObjectTypeInfo.FindMember(dataSourcePropertyAttribute.DataSourceProperty).GetValue(_propertyEditor.CurrentObject)).ToArray();
            return retVal.Concat(defaultValues.Where(v => !retVal.Contains(v)));
        }

    }
}