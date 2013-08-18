using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.Persistent.Base.Logic {
    public interface ILogicInstaller {
        List<ExecutionContext> ExecutionContexts { get; }
        IModelLogicWrapper GetModelLogic(IModelApplication applicationModel);
        IModelLogicWrapper GetModelLogic();
    }

    public interface IModelLogicExecutionContextWrapper {
        IEnumerable<IModelExecutionContexts> ExecutionContextsGroup { get; set; }
    }

    public interface IModelLogicWrapper : IModelLogicExecutionContextWrapper {
        IEnumerable<IModelLogicRule> Rules { get; set; }
        IEnumerable<IModelActionExecutionContexts> ActionExecutionContextGroup { get; set; }
        IEnumerable<IModelViewContexts> ViewContextsGroup { get; set; }
        IEnumerable<IModelFrameTemplateContexts> FrameTemplateContextsGroup { get; set; }
        Type RuleType { get; set; }
    }

    public class ModelLogicWrapper : IModelLogicWrapper {
        public ModelLogicWrapper(IEnumerable<IModelLogicRule> rules, IModelLogicContexts logicContexts)
            : this(rules, logicContexts.ExecutionContextsGroup, logicContexts.ActionExecutionContextGroup,
                   logicContexts.ViewContextsGroup, logicContexts.FrameTemplateContextsGroup) {
        }

        public ModelLogicWrapper(IEnumerable<IModelLogicRule> rules, IEnumerable<IModelExecutionContexts> executionContextsGroup, IEnumerable<IModelViewContexts> viewContextsGroup, IEnumerable<IModelFrameTemplateContexts> frameTemplateContextsGroup) {
            Rules = rules;
            ExecutionContextsGroup = executionContextsGroup;
            ViewContextsGroup = viewContextsGroup;
            FrameTemplateContextsGroup = frameTemplateContextsGroup;
        }

        public ModelLogicWrapper(IEnumerable<IModelLogicRule> rules, IEnumerable<IModelExecutionContexts> executionContextsGroup)
            : this(rules, executionContextsGroup, null, null, null) {
        }

        public ModelLogicWrapper(IEnumerable<IModelLogicRule> rules, IEnumerable<IModelExecutionContexts> executionContextsGroup, IEnumerable<IModelActionExecutionContexts> actionExecutionContextGroup, IEnumerable<IModelViewContexts> viewContextsGroup, IEnumerable<IModelFrameTemplateContexts> frameTemplateContextsGroup) {
            Guard.ArgumentNotNull(rules,"rules");
            Rules = rules;
            ExecutionContextsGroup = executionContextsGroup??Enumerable.Empty<IModelExecutionContexts>();
            ActionExecutionContextGroup = actionExecutionContextGroup??Enumerable.Empty<IModelActionExecutionContexts>();
            ViewContextsGroup = viewContextsGroup??Enumerable.Empty<IModelViewContexts>();
            FrameTemplateContextsGroup = frameTemplateContextsGroup??Enumerable.Empty<IModelFrameTemplateContexts>();
        }

        public IEnumerable<IModelLogicRule> Rules { get; set; }

        public IEnumerable<IModelExecutionContexts> ExecutionContextsGroup { get; set; }
        public IEnumerable<IModelActionExecutionContexts> ActionExecutionContextGroup { get; set; }
        public IEnumerable<IModelViewContexts> ViewContextsGroup { get; set; }
        public IEnumerable<IModelFrameTemplateContexts> FrameTemplateContextsGroup { get; set; }

        Type IModelLogicWrapper.RuleType { get; set; }
    }

}