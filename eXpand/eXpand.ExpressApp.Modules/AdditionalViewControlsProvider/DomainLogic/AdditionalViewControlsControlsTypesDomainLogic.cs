using System;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.DomainLogic {
    public abstract class AdditionalViewControlsControlsTypesDomainLogic<TControl,TDecorator> where TDecorator:AdditionalViewControlsProviderDecorator{
        public static Type Get_ControlType(IModelNode modelNode){
            return typeof(TControl);
        }
        public static Type Get_DecoratorType(IModelNode modelNode){
            return typeof(TDecorator);
        }        
    }
}