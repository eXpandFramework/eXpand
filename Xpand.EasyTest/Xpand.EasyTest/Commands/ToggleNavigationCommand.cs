using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands {
    public class ToggleNavigationCommand:Command{
        private static bool _isToggled;
        public const string Name = "ToggleNavigation";

        protected override void InternalExecute(ICommandAdapter adapter){
            Toggle(adapter);
        }

        private static void Toggle(ICommandAdapter adapter){
            var actionCommand = new ActionCommand();
            actionCommand.Parameters.MainParameter = new MainParameter("Toggle Navigation");
            actionCommand.Parameters.ExtraParameter = new MainParameter("");
            actionCommand.Execute(adapter);
            _isToggled = !_isToggled;
        }


        public static void Reset(ICommandAdapter commandAdapter) {
            if (_isToggled)
                Toggle(commandAdapter);
        }

    }
}
