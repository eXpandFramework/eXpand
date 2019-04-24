namespace Xpand.EasyTest.Commands.InputSimulator{
    public class LClickCommand:MouseClickCommand{
        public const string Name = "LClick";
        protected override string ClickMethodName(){
            return "LeftButtonClick";
        }
    }
}