using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.ModelArtifactState.Attributes {
    public interface IStateRule : IModelRule{
        State State { get; set; }
    }
}