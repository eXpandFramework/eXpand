using System;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;

namespace eXpand.ExpressApp.HintModule.Controllers
{
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
            if (Window != null)
            {
                var viewSiteTemplate = Window.Template as IViewSiteTemplate;
                if (viewSiteTemplate == null)
                    return;
                if (viewSiteTemplate.ViewSiteControl != null)
                    foreach (IAdditionalInfoControlProvider controlProvider in controlProviders.Keys)
                    {
                        object control = controlProvider.CreateControl();
                        if (control != null)
                        {
                            AddInfoControlOnTemplate(viewSiteTemplate.ViewSiteControl, control);
                            OnControlAdded(new ControlEventArgs((Control) control));
                            controlProviders[controlProvider] = control;
                        }
                    }
            }
        }

        private void OnControlAdded(ControlEventArgs args)
        {
            if (ControlAdded!= null)
                ControlAdded(this,args);
        }

        protected void ClearControls()
        {
            foreach (IAdditionalInfoControlProvider controlProvider in controlProviders.Keys)
            {
                DisposeControl(controlProvider);
            }
        }
        public event EventHandler<ControlEventArgs> ControlAdded;
        protected abstract void AddInfoControlOnTemplate(object viewSiteControl, object control);
        public void Register(IAdditionalInfoControlProvider controlProvider)
        {
            controlProviders.Add(controlProvider, null);
            AddInfoControls();
        }
        public void Unregister(IAdditionalInfoControlProvider controlProvider)
        {
            DisposeControl(controlProvider);
            controlProviders.Remove(controlProvider);
        }
    }
}