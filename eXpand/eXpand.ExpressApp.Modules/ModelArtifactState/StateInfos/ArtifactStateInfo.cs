using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelArtifactState.StateRules;
using eXpand.ExpressApp.Security.Permissions;

namespace eXpand.ExpressApp.ModelArtifactState.StateInfos
{
    /// <summary>
    /// A helper class that is used to store the information about the artifact
    /// </summary>
    public sealed class ArtifactStateInfo
    {
        public ArtifactStateInfo(bool active, object obj, State state, ArtifactStateRule rule)
        {
            Active = active;
            objectCore = obj;
            this.state = state;
            this.rule = rule;
        }
        private readonly ArtifactStateRule rule;
        /// <summary>
        /// Represents a string that describes the current rule.
        /// </summary>
        public ArtifactStateRule Rule
        {
            get { return rule; }
        }
        private readonly object objectCore;
        /// <summary>
        /// Currently processed object in the View.
        /// </summary>
        public object Object
        {
            get
            {
                return objectCore;
            }
        }
        
        private State state = State.Default;
        /// <summary>
        /// Gets or sets the state of artifacts.
        /// </summary>
        public State State
        {
            get { return state; }
            set { state = value; }
        }

        /// <summary>
        /// Gets or sets whether the selected customization should be applied to the selected artifacts.
        /// </summary>
        public bool Active { get; set; }

        public bool InvertingCustomization { get; set; }

        public View View { get; set; }
    }
}