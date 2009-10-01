namespace eXpand.ExpressApp.ModelArtifactState.Interfaces
{
    
    public interface IControllerStateRule:IArtifactStateRule
    {

        /// <summary>
        /// Type of controller to activate or not
        /// </summary>
        string ControllerType { get; set; }
    }
}