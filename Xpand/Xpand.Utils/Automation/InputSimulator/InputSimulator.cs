namespace Xpand.Utils.Automation.InputSimulator{
    public class InputSimulator : IInputSimulator{
        private readonly IInputDeviceStateAdaptor _inputDeviceState;
        private readonly IKeyboardSimulator _keyboardSimulator;
        private readonly IMouseSimulator _mouseSimulator;

        public InputSimulator(IKeyboardSimulator keyboardSimulator,
            IMouseSimulator mouseSimulator, IInputDeviceStateAdaptor inputDeviceStateAdaptor) {
            _keyboardSimulator = keyboardSimulator;
            _mouseSimulator = mouseSimulator;
            _inputDeviceState = inputDeviceStateAdaptor;
        }

        public InputSimulator(){
            _keyboardSimulator = new KeyboardSimulator(this);
            _mouseSimulator = new MouseSimulator(this);
            _inputDeviceState = new WindowsInputDeviceStateAdaptor();
        }

        public IKeyboardSimulator Keyboard{
            get { return _keyboardSimulator; }
        }

        public IMouseSimulator Mouse{
            get { return _mouseSimulator; }
        }

        public IInputDeviceStateAdaptor InputDeviceState {
            get { return _inputDeviceState; }
        }
    }
}