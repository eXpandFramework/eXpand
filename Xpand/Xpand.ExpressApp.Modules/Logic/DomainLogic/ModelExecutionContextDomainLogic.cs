using System;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.Logic.Model;

namespace Xpand.ExpressApp.Logic.DomainLogic
{
    [DomainLogic(typeof(IModelExecutionContext))]
    public class ModelExecutionContextDomainLogic
    {
        public static string Get_Name(IModelExecutionContext modelExecutionContext) {
            string value = modelExecutionContext.GetType().Name.Replace("Model","");
            return Enum.Parse(typeof(ExecutionContext),value).ToString();
        }
    }
}
