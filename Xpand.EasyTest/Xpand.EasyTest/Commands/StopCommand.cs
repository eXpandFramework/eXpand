using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest.Commands{
    public class StopCommand : Command {
        public const string Name = "Stop";
        protected override void InternalExecute(ICommandAdapter adapter){
            var value = Parameters.MainParameter.Value;
            if (value == ScreenCaptureCommand.Name){
                ScreenCaptureCommand.Stop();
            }
        }
    }
}