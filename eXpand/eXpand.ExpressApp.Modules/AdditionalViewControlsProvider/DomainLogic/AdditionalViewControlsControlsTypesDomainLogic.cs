using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.DomainLogic {
    public abstract class AdditionalViewControlsControlsTypesDomainLogic<TControl,TDecorator> where TDecorator:AdditionalViewControlsProviderDecorator{
        public static Type Get_ControlType(IModelNode modelNode){
            return typeof(TControl);
        }
        public static List<Type> Get_ControlTypes(IModelNode modelNode) {
            IEnumerable<ITypeInfo> descendants = GetDecorators();
            IEnumerable<Type> enumerable =descendants.Select(descendant => descendant.Type.GetCustomAttributes(typeof (TypeDecorator), false)).
                    Cast<TypeDecorator>().Select(decorator => decorator.ControlType);
            return enumerable.ToList();
        }

        static IEnumerable<ITypeInfo> GetDecorators() {
            ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(typeof(AdditionalViewControlsProviderDecorator));
            return ReflectionHelper.FindTypeDescendants(typeInfo);
        }

        public static Type Get_DecoratorType(IModelNode modelNode){
            return typeof(TDecorator);
        }
        public static List<Type> Get_DecoratorTypes(IModelNode modelNode)
        {
            return GetDecorators().Select(info => info.Type).ToList();
        }        
    }
}