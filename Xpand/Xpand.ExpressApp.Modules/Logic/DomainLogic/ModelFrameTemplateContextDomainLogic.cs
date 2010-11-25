using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.Logic.Model;

namespace Xpand.ExpressApp.Logic.DomainLogic {
    [DomainLogic(typeof(IModelFrameTemplateContext))]
    public class ModelFrameTemplateContextDomainLogic {
        public static List<string> Get_FrameTemplateContexts(IModelFrameTemplateContext modelFrameTemplateContext) {
            return Enum.GetValues(typeof(FrameTemplateContext)).OfType<FrameTemplateContext>().Select(context => context.ToString()).ToList();
        }
    }
}