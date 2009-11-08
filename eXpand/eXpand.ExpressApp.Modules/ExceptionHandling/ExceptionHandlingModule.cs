using System;
using System.Configuration;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace eXpand.ExpressApp.ExceptionHandling
{
    public abstract  partial class ExceptionHandlingModule : ModuleBase
    {
        public const string ExceptionHandling = "ExceptionHandling";
        protected ExceptionHandlingModule()
        {
            InitializeComponent();
        }

        
        protected void Log(Exception exception)
        {
            if ((ConfigurationManager.AppSettings[ExceptionHandling]+"").ToLower() == "true")
            {

                if (!(exception is ValidationException) && !(exception.InnerException != null && exception.InnerException is ValidationException))
                {
                    if (!System.Diagnostics.Debugger.IsAttached)
                    {
                        string asString = Tracing.Tracer.GetLastEntriesAsString();
                        asString = Regex.Replace(asString, @"\n", "<br>");
                        Logger.Write(asString);
                    }
                }
            }
        }

    }
}