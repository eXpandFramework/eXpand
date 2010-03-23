using System;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.Logic.Conditional;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.ModelArtifactState.ArtifactState.Logic {
    public abstract class ArtifactStateRule : ConditionalLogicRule, IArtifactRule {
        readonly IArtifactRule artifactRule;

        protected Guid instanceGUID;
        protected MethodInfo methodInfo;

        protected ArtifactStateRule(IArtifactRule artifactRule) : base(artifactRule) {
            this.artifactRule = artifactRule;
        }

        public IArtifactRule ArtifactRule {
            get { return artifactRule; }
        }

        public string Module {
            get { return artifactRule.Module; }
        }


        public MethodInfo MethodInfo {
            get { return methodInfo; }
        }
        #region IArtifactRule Members
        string ILogicRule.ViewId {
            get { return artifactRule.ViewId; }
            set { artifactRule.ViewId = value; }
        }


        string ILogicRule.Description {
            get { return artifactRule.Description; }
            set { artifactRule.Description = value; }
        }


        ITypeInfo ILogicRule.TypeInfo {
            get { return artifactRule.TypeInfo; }
            set { artifactRule.TypeInfo = value; }
        }

        string ILogicRule.ID {
            get { return artifactRule.ID; }
            set { artifactRule.ID = value; }
        }

        Nesting ILogicRule.Nesting {
            get { return artifactRule.Nesting; }
            set { artifactRule.Nesting = value; }
        }


        string IArtifactRule.Module {
            get { return artifactRule.Module; }
            set { artifactRule.Module = value; }
        }
        #endregion
        public override string ToString() {
            return string.Format("{0}({1},{2},{3},{4},{4},{5}-{6})", base.ToString(), TypeInfo, ViewType,
                                 NormalCriteria,
                                 EmptyCriteria, MethodInfo, instanceGUID);
        }
    }
}