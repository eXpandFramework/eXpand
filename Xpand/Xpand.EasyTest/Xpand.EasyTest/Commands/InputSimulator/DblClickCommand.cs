namespace Xpand.EasyTest.Commands.InputSimulator{
    public class DblClickCommand:MouseClickCommand{
        public const string Name = "DblClick";

        protected override string ClickMethodName(){
            return "LeftButtonDoubleClick";
        }
    }
}