namespace Xpand.EasyTest.Commands.InputSimulator{
    public class RClickCommand:MouseClickCommand{
        public const string Name = "RClick";
        protected override string ClickMethodName(){
            return "LeftButtonClick";
        }
    }
}