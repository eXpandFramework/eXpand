using System.Collections.Generic;
using System.Linq;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.Logic.DomainLogic {
    public class ModelViewContextDomainLogic {
        public static List<string> Get_ViewContexts(IModelViewContext modelViewContext) {
            return modelViewContext.Application.Views.Select(view => view.Id).ToList();
        }
    }
}