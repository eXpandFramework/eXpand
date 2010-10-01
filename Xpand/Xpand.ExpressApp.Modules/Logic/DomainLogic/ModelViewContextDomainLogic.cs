using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.Logic.Model;

namespace Xpand.ExpressApp.Logic.DomainLogic {
    [DomainLogic(typeof(IModelViewContext))]
    public class ModelViewContextDomainLogic {
        public static List<string> Get_ViewContexts(IModelViewContext modelViewContext) {
            return modelViewContext.Application.Views.Select(view => view.Id).ToList();
        }
    }
}