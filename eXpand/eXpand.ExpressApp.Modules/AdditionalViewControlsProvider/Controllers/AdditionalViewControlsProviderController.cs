using System;
using DevExpress.ExpressApp;
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

        public void AddControls(object viewSiteControl)
        {
            foreach (IAdditionalViewControlsProvider controlProvider in controlProviders.Keys){
                var calculator =
                    new AdditionalViewControlsProviderCalculator(new AdditionalViewControlsRuleWrapper(controlProvider.View));
                object control = createControl(controlProvider, calculator);
                AdditionalViewControlsProviderPosition position = decorateControl(controlProvider, calculator, control);
                if (control != null){
                    DisposeControl(controlProvider);
                    AddControlOnTemplate(viewSiteControl, control, position);
                    controlProviders[controlProvider] = control;
                }
            }
        }

        private AdditionalViewControlsProviderPosition decorateControl(IAdditionalViewControlsProvider controlProvider,
                                                                       AdditionalViewControlsProviderCalculator
                                                                           calculator, object control)
        {
            AdditionalViewControlsProviderPosition position;
            if (calculator.AdditionalViewControlsRuleWrapper.DecoratorType != null)
            {
                var decorator =
                    (AdditionalViewControlsProviderDecorator)
                    Activator.CreateInstance(calculator.AdditionalViewControlsRuleWrapper.DecoratorType,
                                             new[] {controlProvider.View, control});
                position = decorator.Calculator.AdditionalViewControlsRuleWrapper.AdditionalViewControlsProviderPosition;
            }
            else
                position = controlProvider.DecorateControl(control).Calculator.AdditionalViewControlsRuleWrapper.AdditionalViewControlsProviderPosition;
            return position;
        }

        private object createControl(IAdditionalViewControlsProvider controlProvider, AdditionalViewControlsProviderCalculator calculator)
        {
            object control = null;
            if (calculator.AdditionalViewControlsRuleWrapper.ControlType != null)
                control=Activator.CreateInstance(calculator.AdditionalViewControlsRuleWrapper.ControlType);
            if (control== null)
                control = controlProvider.CreateControl();
            return control;
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