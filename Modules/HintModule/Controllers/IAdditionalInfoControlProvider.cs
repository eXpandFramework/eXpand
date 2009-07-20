using DevExpress.ExpressApp;
using System;

namespace eXpand.ExpressApp.HintModule.Controllers
{
    public interface IAdditionalInfoControlProvider
    {
        object CreateControl();
        View View { get; }
    }
}