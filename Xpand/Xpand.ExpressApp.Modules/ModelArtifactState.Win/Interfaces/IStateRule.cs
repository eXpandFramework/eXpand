using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.ModelArtifactState.Interfaces {
    public interface IStateRule : IModelRule{
        State State { get; set; }
    }
}