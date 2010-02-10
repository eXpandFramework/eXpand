using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using eXpand.ExpressApp.AdditionalViewControlsProvider.NodeWrappers;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Controllers
{
    public abstract partial class AdditionalViewControlsProviderController : WindowController
    {
        private readonly LightDictionary<IAdditionalViewControlsProvider, object> controlProviders = new LightDictionary<IAdditionalViewControlsProvider, object>();
        
        
        private void DisposeControl(IAdditionalViewControlsProvider controlProvider)
        {
            object control = controlProviders[controlProvider];
            controlProviders[controlProvider] = null;
            if (control is IDisposable)
            {
                ((IDisposable)control).Dispose();
            }
        }
        protected void AddControls()
        {
            if (Window != null)
            {
                var viewSiteTemplate = Window.Template as IViewSiteTemplate;
                if (viewSiteTemplate == null)
                    return;
                object viewSiteControl = viewSiteTemplate.ViewSiteControl;
                if (viewSiteControl != null)
                    AddControls(viewSiteControl);
            }
        }

        public void AddControls(object viewSiteControl){
            foreach (IAdditionalViewControlsProvider controlProvider in controlProviders.Keys){
                DictionaryNode dictionaryNode = GetDictionaryNode(controlProvider.View);
                var additionalViewControlsRuleNodeWrapper = new AdditionalViewControlsRuleNodeWrapper(dictionaryNode);
                var calculator =
                    new AdditionalViewControlsProviderCalculator(additionalViewControlsRuleNodeWrapper);
                object control = createControl(controlProvider, calculator);
                AdditionalViewControlsProviderPosition position = decorateControl(controlProvider, calculator, control);
                if (control != null){
                    DisposeControl(controlProvider);
                    AddControlOnTemplate(viewSiteControl, control, position);
                    controlProviders[controlProvider] = control;
                }
            }
        }

        public static DictionaryNode GetDictionaryNode(View view) {
            ClassInfoNodeWrapper classInfoNodeWrappers =
                new ApplicationNodeWrapper(view.Info.Dictionary.RootNode).BOModel.Classes.Where(
                    wrapper => wrapper.ClassTypeInfo == view.ObjectTypeInfo).FirstOrDefault();
            return classInfoNodeWrappers.Node.GetChildNode(AdditionalViewControlsRulesNodeWrapper.NodeNameAttribute);
        }

        private AdditionalViewControlsProviderPosition decorateControl(IAdditionalViewControlsProvider controlProvider,
                                                                       AdditionalViewControlsProviderCalculator
                                                                           calculator, object control){
            AdditionalViewControlsProviderPosition position;
            if (calculator.ControlsRule.DecoratorType != null){
                var decorator =(AdditionalViewControlsProviderDecorator)
                    Activator.CreateInstance(calculator.ControlsRule.DecoratorType,new[] {controlProvider.View, control});
                position = decorator.Calculator.ControlsRule.AdditionalViewControlsProviderPosition;
            }
            else
                position = controlProvider.DecorateControl(control).Calculator.ControlsRule.AdditionalViewControlsProviderPosition;
            return position;
        }

        private object createControl(IAdditionalViewControlsProvider controlProvider, AdditionalViewControlsProviderCalculator calculator)
        {
            object control = null;
            if (calculator.ControlsRule.ControlType != null)
                control=Activator.CreateInstance(calculator.ControlsRule.ControlType);
            return control ?? (controlProvider.CreateControl());
        }


        protected void ClearControls()
        {
            foreach (IAdditionalViewControlsProvider controlProvider in controlProviders.Keys)
            {
                DisposeControl(controlProvider);
            }
        }

        protected abstract void AddControlOnTemplate(object viewSiteControl, object control, AdditionalViewControlsProviderPosition position);
        public virtual void Register(IAdditionalViewControlsProvider controlProvider,View view, Frame frame)
        {
            if (!(controlProviders.Keys.Contains(controlProvider)))
                controlProviders.Add(controlProvider, null);
            AddControls();
        }
        public virtual void Unregister(IAdditionalViewControlsProvider controlProvider)
        {
            DisposeControl(controlProvider);
            controlProviders.Remove(controlProvider);
        }
    }
}