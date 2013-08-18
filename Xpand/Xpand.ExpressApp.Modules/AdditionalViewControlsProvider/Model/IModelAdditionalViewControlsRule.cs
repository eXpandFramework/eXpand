using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Model {
    [ModelInterfaceImplementor(typeof(IContextAdditionalViewControlsRule), "Attribute")]
    public interface IModelAdditionalViewControlsRule : IContextAdditionalViewControlsRule, IModelConditionalLogicRule<IAdditionalViewControlsRule> {
        [Browsable(false)]
        IEnumerable<Type> ControlTypes { get; }
        [Browsable(false)]
        IEnumerable<Type> DecoratorTypes { get; }        
    }
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
            TypeDecorator decorator = GetTypeDecorator(modelAdditionalViewControlsRule.DecoratorType, modelAdditionalViewControlsRule.Position);
            return decorator != null ? FindTypeDescendants(decorator).Select(info => info.Type).ToList()
                       : new List<Type> { typeof(NotAvaliableInThisPlatform) };
        }

        static IEnumerable<ITypeInfo> FindTypeDescendants(TypeDecorator decorator) {
            return ReflectionHelper.FindTypeDescendants(XafTypesInfo.Instance.FindTypeInfo(decorator.ControlType), true).Where(info => info.Implements<IAdditionalViewControl>());
        }

        public static TypeDecorator GetTypeDecorator(Type decorator, Position position) {
            IEnumerable<TypeDecorator> typeDecorators = GetTypeDecorators(new List<ITypeInfo> { XafTypesInfo.CastTypeToTypeInfo(decorator) });
            return typeDecorators.Where(PredicatePosition(position)).FirstOrDefault();
        }

        static Func<TypeDecorator, bool> PredicatePosition(Position position) {
            return decorator => decorator.Position != Position.DetailViewItem || decorator.Position == position;
        }

        static IEnumerable<TypeDecorator> GetTypeDecorators(IEnumerable<ITypeInfo> descenants) {
            return descenants.Select(TypeDecoratorAttributes()).Cast<TypeDecorator[]>().SelectMany(decorators => decorators);
        }


        static Func<ITypeInfo, object[]> TypeDecoratorAttributes() {
            return descendant => descendant.Type.GetCustomAttributes(typeof(TypeDecorator), false);
        }
    }

}