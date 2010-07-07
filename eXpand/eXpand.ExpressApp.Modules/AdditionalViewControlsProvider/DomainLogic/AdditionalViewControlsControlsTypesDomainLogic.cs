using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Model;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.DomainLogic {
    [DomainLogic(typeof(IModelAdditionalViewControlsRule))]
    public class AdditionalViewControlsControlsTypesDomainLogic
    {
        public static List<Type> Get_DecoratorTypes(IModelNode modelNode)
        {
            return GetDecorators().Select(info => info.Type).ToList();
        }
        static IEnumerable<ITypeInfo> GetDecorators()
        {
            ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(typeof(AdditionalViewControlsProviderDecorator));
            return ReflectionHelper.FindTypeDescendants(typeInfo);
        }
        public static List<Type> Get_ControlTypes(IModelNode modelNode) {
            IEnumerable<TypeDecorator[]> typeDecoratorsAttr = GetTypeDecoratorsAttr();
            TypeDecorator[] singleOrDefault = typeDecoratorsAttr.SingleOrDefault();
            return singleOrDefault != null ? singleOrDefault.SelectMany(typeDecorator => ReflectionHelper.FindTypeDescendants(XafTypesInfo.Instance.FindTypeInfo(typeDecorator.ControlType), true).Select(info => info.Type)).ToList() : new List<Type>();
        }

        static IEnumerable<TypeDecorator[]> GetTypeDecoratorsAttr() {
            IEnumerable<ITypeInfo> descendants = GetDecorators();
            return descendants.Select(descendant => descendant.Type.GetCustomAttributes(typeof(TypeDecorator), false)).Cast<TypeDecorator[]>();
        }
        public static Type Get_DecoratorType(IModelNode modelNode) {
            ITypeInfo decoratorType =
                GetDecorators().Where(info =>info.Type.GetCustomAttributes(typeof (TypeDecorator), true).OfType<TypeDecorator>().Where(
                        decorator => decorator.IsDefaultDecorator).Count() > 0).SingleOrDefault();
            return decoratorType != null ? decoratorType.Type : null;
        }

        public static Type Get_ControlType(IModelNode modelNode){
            TypeDecorator[] typeDecorators = GetTypeDecoratorsAttr().SingleOrDefault();
            return typeDecorators != null ? typeDecorators[0].DefaultType : null;
        }

    }
}