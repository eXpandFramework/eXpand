using System;
using System.Collections.Generic;
using System.Linq;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.Logic.DomainLogic {
    
    public class ModelFrameTemplateContextDomainLogic {
        public static List<string> Get_FrameTemplateContexts(IModelFrameTemplateContext modelFrameTemplateContext) {
            return Enum.GetValues(typeof(FrameTemplateContext)).OfType<FrameTemplateContext>().Select(context => context.ToString()).ToList();
        }
    }
}