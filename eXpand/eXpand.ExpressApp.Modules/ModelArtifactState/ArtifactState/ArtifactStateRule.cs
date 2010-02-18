using System;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.Logic;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.ModelArtifactState.ArtifactState {
    public abstract class ArtifactStateRule : ModelRule, IArtifactRule {
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
        string IModelRule.ViewId {
            get { return artifactRule.ViewId; }
            set { artifactRule.ViewId = value; }
        }


        string IModelRule.Description {
            get { return artifactRule.Description; }
            set { artifactRule.Description = value; }
        }


        ITypeInfo IModelRule.TypeInfo {
            get { return artifactRule.TypeInfo; }
            set { artifactRule.TypeInfo = value; }
        }

        string IModelRule.ID {
            get { return artifactRule.ID; }
            set { artifactRule.ID = value; }
        }

        Nesting IModelRule.Nesting {
            get { return artifactRule.Nesting; }
            set { artifactRule.Nesting = value; }
        }


        string IModelRule.NormalCriteria {
            get { return artifactRule.NormalCriteria; }
            set { artifactRule.NormalCriteria = value; }
        }


        string IModelRule.EmptyCriteria {
            get { return artifactRule.EmptyCriteria; }
            set { artifactRule.EmptyCriteria = value; }
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