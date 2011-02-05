using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Model;
class NotAvaliableInThisPlatform {

}
namespace Xpand.ExpressApp.AdditionalViewControlsProvider.DomainLogic {
    [DomainLogic(typeof(IModelAdditionalViewControlsRule))]
    public class ModelAdditionalViewControlsRuleDomainLogic {
        public static List<Type> Get_DecoratorTypes(IModelAdditionalViewControlsRule modelAdditionalViewControlsRule) {
            List<Type> decoratorTypes = GetDecorators().Select(info => info.Type).ToList();
            if (decoratorTypes.Count == 0)
                decoratorTypes.Add(typeof(NotAvaliableInThisPlatform));
            return decoratorTypes;
        }

        public static IEnumerable<ITypeInfo> GetDecorators() {
            ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(typeof(AdditionalViewControlsProviderDecorator));
            return ReflectionHelper.FindTypeDescendants(typeInfo);
        }
        public static List<Type> Get_ControlTypes(IModelAdditionalViewControlsRule modelAdditionalViewControlsRule) {
            TypeDecorator decorator = GetTypeDecorator(modelAdditionalViewControlsRule.Position);
            return decorator != null ? FindTypeDescendants(decorator).Select(info => info.Type).ToList()
                       : new List<Type> { typeof(NotAvaliableInThisPlatform) };
        }

        static IEnumerable<ITypeInfo> FindTypeDescendants(TypeDecorator decorator) {
            return ReflectionHelper.FindTypeDescendants(XafTypesInfo.Instance.FindTypeInfo(decorator.ControlType), true).Where(info => info.FindAttribute<AdditionalViewControlAttribute>() != null);
        }

        public static TypeDecorator GetTypeDecorator(Position position) {
            IEnumerable<TypeDecorator> typeDecorators = GetTypeDecorators();
            return typeDecorators.Where(PredicatePosition(position)).FirstOrDefault();
        }

        static Func<TypeDecorator, bool> PredicatePosition(Position position) {
            return decorator => decorator.Position == Position.DetailViewItem ? decorator.Position == position : true;
        }

        static IEnumerable<TypeDecorator> GetTypeDecorators() {
            IEnumerable<ITypeInfo> descendants = GetDecorators();
            return descendants.Select(TypeDecoratorAttributes()).Cast<TypeDecorator[]>().SelectMany(decorators => decorators);
        }


        static Func<ITypeInfo, object[]> TypeDecoratorAttributes() {
            return descendant => descendant.Type.GetCustomAttributes(typeof(TypeDecorator), false);
        }
    }
}