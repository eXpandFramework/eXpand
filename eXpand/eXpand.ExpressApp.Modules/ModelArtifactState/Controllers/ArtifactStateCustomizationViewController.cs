using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using eXpand.ExpressApp.ModelArtifactState.Interfaces;
using eXpand.ExpressApp.ModelArtifactState.StateInfos;
using eXpand.ExpressApp.ModelArtifactState.StateRules;
using eXpand.ExpressApp.Security.Permissions;

namespace eXpand.ExpressApp.ModelArtifactState.Controllers{
    public class ArtifactStateCustomizationViewController : ViewController, ISupportArtifactStateCustomization,
                                                            ISupportArtifactStateVisibilityCustomization, ISupportArtifactStateAccessibilityCustomization
    {
        private readonly LightDictionary<ISupportArtifactState, Type> providers = new LightDictionary<ISupportArtifactState, Type>();
        private bool isRefreshing;
        public const string ActiveObjectTypeHasRules = "ObjectTypeHasArtifactRules";
        #region ISupportArtifactStateCustomization Members
        public virtual bool IsReady{
            get { return Active.ResultValue && View != null && View.ObjectTypeInfo != null; }
        }

        public virtual void ForceCustomization(bool isReady, View view, bool invertCustomization){
            if (isReady){
                object currentObject = view.CurrentObject;
                foreach (ArtifactStateRule rule in ArtifactStateRuleManager.Instance[view.ObjectTypeInfo])
                    if (IsValidRule(rule, view))
                        ForceCustomizationCore(currentObject, rule, invertCustomization,view);
            }
            
        }

        public virtual void CustomizeArtifactState(ArtifactStateInfo info){
            var args = new ArtifactStateInfoCustomizingEventArgs(info, false);
            OnArtifactStateCustomizing(args);
            if (!args.Cancel){

                foreach (ISupportArtifactState supportArtifactState in providers.Keys){
                    if (providers[supportArtifactState].IsAssignableFrom(info.Rule.GetType()))
                    {
                        switch (info.State)
                        {
                            case State.Default:
                            case State.Hidden:{
                                if (supportArtifactState is ISupportArtifactStateVisibilityCustomization)
                                    ((ISupportArtifactStateVisibilityCustomization) supportArtifactState).CustomizeVisibility(info);
                                break;
                            }
                            case State.Disabled:{
                                if (supportArtifactState is ISupportArtifactStateAccessibilityCustomization)
                                    ((ISupportArtifactStateAccessibilityCustomization)supportArtifactState).CustomizeAccessibility(info);
                                break;
                            }
                            default:
                                break;
                        }

                    }
                }
            }
            OnArtifactStateStateCustomized(new ArtifactStateInfoCustomizedEventArgs(info));
        }

        public void Register<ArtifactStateRule>(ISupportArtifactState supportArtifactState)
        {
            if (!providers.Keys.Contains(supportArtifactState))
                providers.Add(supportArtifactState, typeof(ArtifactStateRule));
        }
        public void UnRegister(ISupportArtifactStateVisibilityCustomization supportArtifactStateVisibilityCustomization)
        {
            if (!providers.Keys.Contains(supportArtifactStateVisibilityCustomization))
                providers.Remove(supportArtifactStateVisibilityCustomization);
        }
        /// <summary>
        /// An event that can be used to be notified whenever artifact begin customizing.
        /// </summary>
        public event EventHandler<ArtifactStateInfoCustomizingEventArgs> ArtifactStateCustomizing;

        /// <summary>
        /// An event that can be used to be notified whenever artifact state has been customized.
        /// </summary>
        public event EventHandler<ArtifactStateInfoCustomizedEventArgs> ArtifactStateCustomized;
        #endregion
        #region ISupportArtifactStateVisibilityCustomization Members
        public virtual void CustomizeVisibility(ArtifactStateInfo artifactStateInfo){
        }
        #endregion

        protected override void OnViewChanging(View view){
            
            base.OnViewChanging(view);
            Active[ActiveObjectTypeHasRules] = ArtifactStateRuleManager.NeedsCustomization(view);
            ForceCustomization(Active[ActiveObjectTypeHasRules] && view != null && view.ObjectTypeInfo != null, view,false);
        }

        private void FrameOnViewChanging(object sender, EventArgs args){
            if (View != null) ResetCustomization(View);
        }

        private void ResetCustomization(View view){
            ForceCustomization(Active[ActiveObjectTypeHasRules] && view != null && view.ObjectTypeInfo != null, view,true);
        }

        protected override void OnFrameAssigned()
        {
            base.OnFrameAssigned();
            Frame.ViewChanging+=FrameOnViewChanging;
            Register<ArtifactStateRule>(this);
        }

        protected override void OnActivated(){
            base.OnActivated();
            if (IsReady){
                View.ObjectSpace.ObjectChanged += ObjectSpaceOnObjectChanged;
                View.CurrentObjectChanged+=ViewOnCurrentObjectChanged;
                View.ObjectSpace.Refreshing += ObjectSpace_Refreshing;
                View.ObjectSpace.Reloaded += ObjectSpace_Reloaded;
            }
        }

        protected override void OnDeactivating(){
            base.OnDeactivating();
            if (IsReady){
                View.ObjectSpace.ObjectChanged -= ObjectSpaceOnObjectChanged;
                View.CurrentObjectChanged -= ViewOnCurrentObjectChanged;
                View.ObjectSpace.Refreshing -= ObjectSpace_Refreshing;
                View.ObjectSpace.Reloaded -= ObjectSpace_Reloaded;
            }
        }

        private void ObjectSpace_Reloaded(object sender, EventArgs e){
            isRefreshing = false;
            ForceCustomization();
        }

        private void ObjectSpace_Refreshing(object sender, CancelEventArgs e){
            isRefreshing = true;
        }

        private void ViewOnCurrentObjectChanged(object sender, EventArgs args){
            if (!isRefreshing){
                ForceCustomization();
            }
        }

        private void ForceCustomization(){
            ForceCustomization(IsReady,View, false);
        }

        private void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs args){
            if (!String.IsNullOrEmpty(args.PropertyName)){
                ForceCustomization();
            }
        }

        protected virtual void OnArtifactStateCustomizing(ArtifactStateInfoCustomizingEventArgs args){
            if (ArtifactStateCustomizing != null){
                ArtifactStateCustomizing(this, args);
            }
        }

        protected virtual void OnArtifactStateStateCustomized(ArtifactStateInfoCustomizedEventArgs args){
            if (ArtifactStateCustomized != null){
                ArtifactStateCustomized(this, args);
            }
        }

        protected virtual bool IsValidRule(ArtifactStateRule rule, View view){
            return view != null &&(string.IsNullOrEmpty(rule.ViewId)||view.Id==rule.ViewId)&& view.ObjectTypeInfo != null &&
                   IsValidViewType(view, rule) && IsValidNestedType(rule,view)&&(rule.TypeInfo.IsAssignableFrom(view.ObjectTypeInfo));
        }

        private bool IsValidNestedType(ArtifactStateRule rule, View view){
            if (view is DetailView)
                return true;
            
            return (rule.Nesting == Nesting.Any || view.IsRoot);
        }

        private bool IsValidViewType(View view, ArtifactStateRule rule){
            return (rule.ViewType == ViewType.Any || (view is DetailView ? rule.ViewType == ViewType.DetailView : rule.ViewType==ViewType.ListView));
        }


        protected virtual void ForceCustomizationCore(object currentObject, ArtifactStateRule rule, bool invertCustomization, View view)
        {
            ArtifactStateInfo info = ArtifactStateRuleManager.CalculateArtifactStateInfo(currentObject, rule);
            if (info != null){
                info.InvertingCustomization = invertCustomization;
                info.View = view;
                if (invertCustomization)
                    info.Active = !info.Active;
                CustomizeArtifactState(info);
            }
        }


        public virtual void CustomizeAccessibility(ArtifactStateInfo info)
        {
            
        }
    }
}