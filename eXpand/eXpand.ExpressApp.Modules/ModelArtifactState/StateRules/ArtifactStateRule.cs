using System;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.ModelArtifactState.Interfaces;
using eXpand.ExpressApp.Security.Interfaces;
using eXpand.ExpressApp.Security.Permissions;

namespace eXpand.ExpressApp.ModelArtifactState.StateRules
{
    public abstract class ArtifactStateRule : IArtifactStateRule
    {
        private readonly IArtifactStateRule artifactRule;
        
        protected Guid instanceGUID;
        protected MethodInfo methodInfo;

        protected ArtifactStateRule(IArtifactStateRule artifactRule)
        {
            this.artifactRule = artifactRule;
        }

        
        public ITypeInfo TypeInfo
        {
            get { return artifactRule.TypeInfo; }
        }
        public IArtifactStateRule ArtifactRule
        {
            get { return artifactRule; }
        }



        public string ViewId
        {
            get { return artifactRule.ViewId; }
        }
        string IStateRule.ViewId{
            get { return artifactRule.ViewId; }
            set { artifactRule.ViewId = value; }
        }


        public string Module
        {
            get { return artifactRule.Module; }
        }

        


        public Nesting Nesting
        {
            get { return artifactRule.Nesting; }
        }

        public State State
        {
            get { return artifactRule.State; }
        }

        
        string IStateRule.Description{
            get { return artifactRule.Description; }
            set { artifactRule.Description = value; }
        }

        
        ITypeInfo IStateRule.TypeInfo{
            get { return artifactRule.TypeInfo; }
            set { artifactRule.TypeInfo = value; }
        }

        public string NormalCriteria
        {
            get { return artifactRule.NormalCriteria; }
        }

        public string EmptyCriteria
        {
            get { return artifactRule.EmptyCriteria; }
        }

        public MethodInfo MethodInfo
        {
            get { return methodInfo; }
        }

        #region IArtifactRule Members
        
        string IStateRule.ID{
            get { return artifactRule.ID; }
            set { artifactRule.ID = value; }
        }


        
        public ViewType ViewType{
            get { return artifactRule.ViewType; }
            set { artifactRule.ViewType = value; }
        }

        
        Nesting IStateRule.Nesting{
            get { return artifactRule.Nesting; }
            set { artifactRule.Nesting = value; }
        }

        
        string IStateRule.NormalCriteria{
            get { return artifactRule.NormalCriteria; }
            set { artifactRule.NormalCriteria = value; }
        }


        
        string IStateRule.EmptyCriteria{
            get { return artifactRule.EmptyCriteria; }
            set { artifactRule.EmptyCriteria = value; }
        }


        
        State IStateRule.State{
            get { return artifactRule.State; }
            set { artifactRule.State = value; }
        }

        
        string IArtifactStateRule.Module{
            get { return artifactRule.Module; }
            set { artifactRule.Module = value; }
        }
        #endregion
        public override string ToString()
        {
            return string.Format("{0}({1},{2},{3},{4},{4},{5}-{6})", base.ToString(), TypeInfo, ViewType,
                                 NormalCriteria,
                                 EmptyCriteria, MethodInfo, instanceGUID);
        }

        public bool IsViewTargeted(ViewType targetViewType, Nesting targetViewNesting, string viewID)
        {
            return (ViewType == targetViewType || ViewType == ViewType.Any) &&
                   (Nesting == targetViewNesting || Nesting == Nesting.Any) &&
                   (ViewId == viewID || string.IsNullOrEmpty(ViewId));
        }

    }
}