namespace eXpand.ExpressApp.ModelArtifactState.Interfaces
{
    public interface IControllerState
    {
//        bool ApplyToDerivedController { get; set; }

        /// <summary>
        /// Type of controller to activate or not
        /// </summary>
        string ControllerType { get; set; }
    }
}