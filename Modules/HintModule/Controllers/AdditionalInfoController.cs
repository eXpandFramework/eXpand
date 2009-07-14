using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;

namespace eXpand.ExpressApp.SystemModule
{
    public interface IAdditionalInfoControlProvider
    {
        object CreateControl();
    }
    public abstract partial class AdditionalInfoController : WindowController
    {
        private readonly LightDictionary<IAdditionalInfoControlProvider, object> controlProviders = new LightDictionary<IAdditionalInfoControlProvider, object>();
        private void DisposeControl(IAdditionalInfoControlProvider controlProvider)
        {
            object control = controlProviders[controlProvider];
            controlProviders[controlProvider] = null;
            if (control is IDisposable)
            {
                ((IDisposable)control).Dispose();
            }
        }
        protected void AddInfoControls()
        {
            var viewSiteTemplate = Window.Template as IViewSiteTemplate;
            if (viewSiteTemplate == null)
            {
                return;
            }
            if (viewSiteTemplate.ViewSiteControl != null)
            {
                foreach (IAdditionalInfoControlProvider controlProvider in controlProviders.Keys)
                {
                    object control = controlProvider.CreateControl();
                    if (control != null)
                    {
                        AddInfoControlOnTemplate(viewSiteTemplate.ViewSiteControl, control);
                        controlProviders[controlProvider] = control;
                    }
                }
            }
        }
        protected void ClearControls()
        {
            foreach (IAdditionalInfoControlProvider controlProvider in controlProviders.Keys)
            {
                DisposeControl(controlProvider);
            }
        }
        protected abstract void AddInfoControlOnTemplate(object viewSiteControl, object control);
        public void Register(IAdditionalInfoControlProvider controlProvider)
        {
            controlProviders.Add(controlProvider, null);
        }
        public void Unregister(IAdditionalInfoControlProvider controlProvider)
        {
            DisposeControl(controlProvider);
            controlProviders.Remove(controlProvider);
        }
    }
}
