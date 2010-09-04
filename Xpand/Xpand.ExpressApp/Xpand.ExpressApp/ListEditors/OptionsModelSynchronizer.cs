using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.ListEditors
{
    public abstract class OptionsModelSynchronizer<T,V,TModelOptionsType> : ModelSynchronizer<T, V> where V:IModelNode
    {
        protected OptionsModelSynchronizer(T control, V model)
            : base(control, model)
        {
        }

        IModelNode GetOptionNode(IModelNode modelListViewMainViewOptionsInterfaceType)
        {
            Type modelListViewMainViewOptionsType = GetModelOptionsType();
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


        internal static Type GetModelOptionsType() {
            ITypeInfo findTypeInfo = XafTypesInfo.Instance.FindTypeInfo(typeof(TModelOptionsType));
            return ReflectionHelper.FindTypeDescendants(findTypeInfo).Where(info => info.Type.IsInterface).Single().Type;
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
            object control = GetControl();
            var propertyInfos = control.GetType().GetProperties();
            MethodInfo getValueMethodInfo = typeof(IModelNode).GetMethod("GetValue");
            IModelNode optionsNode = GetOptionNode(modelListViewMainViewOptionsInterfaceType);
            DelegateValuesFromModelToControl(optionsNode, propertyInfos, getValueMethodInfo, control);
        }

        protected abstract object GetControl();

        public override void SynchronizeModel()
        {

        }
    }
}
