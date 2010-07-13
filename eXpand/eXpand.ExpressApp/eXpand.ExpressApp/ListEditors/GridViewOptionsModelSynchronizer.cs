using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.ListEditors
{
    public class GridViewOptionsModelSynchronizer : ModelSynchronizer<object, IModelListView>
    {
        public GridViewOptionsModelSynchronizer(object control, IModelListView model)
            : base(control, model)
        {
        }

        IModelNode GetOptionNode(IModelNode modelListViewMainViewOptionsInterfaceType)
        {
            Type modelListViewMainViewOptionsType = GetModelListViewMainViewOptionsType();
            IModelNode modelNode;
            for (int i = 0; i < modelListViewMainViewOptionsInterfaceType.NodeCount; i++)
            {
                modelNode = modelListViewMainViewOptionsInterfaceType.GetNode(i);
                var id = modelNode.GetValue<string>("Id");
                if (id == modelListViewMainViewOptionsType.GetProperties()[0].Name)
                {
                    return modelNode;
                }
            }
            return null;
        }

        void DelegateValuesFromModelToControl(IModelNode optionsNode, IEnumerable<PropertyInfo> propertyInfos, MethodInfo getValueMethodInfo, object control)
        {
            for (int i = 0; i < optionsNode.NodeCount; i++)
            {
                var modelNode = optionsNode.GetNode(i);
                var id = modelNode.GetValue<string>("Id");
                PropertyInfo propertyInfo = propertyInfos.FirstOrDefault(info => info.Name == id);
                if (propertyInfo != null)
                {
                    object value = propertyInfo.GetValue(control, null);
                    var properties = propertyInfo.PropertyType.GetProperties().Where(info => info.GetSetMethod() != null);
                    foreach (PropertyInfo property in properties)
                    {
                        PropertyInfo info = modelNode.GetType().GetProperty(property.Name);
                        if (info != null)
                        {
                            MethodInfo genericMethod = getValueMethodInfo.MakeGenericMethod(info.PropertyType);
                            object invoke = genericMethod.Invoke(modelNode, new object[] { property.Name });
                            if (invoke != null)
                                property.SetValue(value, invoke, null);
                        }
                    }
                }
            }
        }

        protected override void ApplyModelCore()
        {
            var modelListViewMainViewOptionsInterfaceType = ((IModelNode)Model);
            var propertyInfos = Control.GetType().GetProperties();
            MethodInfo getValueMethodInfo = typeof(IModelNode).GetMethod("GetValue");
            IModelNode optionsNode = GetOptionNode(modelListViewMainViewOptionsInterfaceType);
            DelegateValuesFromModelToControl(optionsNode, propertyInfos, getValueMethodInfo, Control);
        }
        internal static Type GetModelListViewMainViewOptionsType()
        {
            ITypeInfo findTypeInfo = XafTypesInfo.Instance.FindTypeInfo(typeof(IModelListViewMainViewOptionsBase));
            return ReflectionHelper.FindTypeDescendants(findTypeInfo).Single().Type;
        }

        public override void SynchronizeModel()
        {

        }
    }
}
