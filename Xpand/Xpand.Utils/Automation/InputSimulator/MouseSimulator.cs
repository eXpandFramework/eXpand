using System;
using System.Drawing;
using System.Threading;
using Xpand.Utils.Win32;

namespace Xpand.Utils.Automation.InputSimulator{
    public class MouseSimulator : IMouseSimulator{
        private const int MouseWheelClickSize = 120;

        private readonly IInputSimulator _inputSimulator;

        private readonly IInputMessageDispatcher _messageDispatcher;

        public MouseSimulator(IInputSimulator inputSimulator){
            if (inputSimulator == null) throw new ArgumentNullException("inputSimulator");

            _inputSimulator = inputSimulator;
            _messageDispatcher = new WindowsInputMessageDispatcher();
        }

        internal MouseSimulator(IInputSimulator inputSimulator,
            IInputMessageDispatcher messageDispatcher){
            if (inputSimulator == null)
                throw new ArgumentNullException("inputSimulator");

            if (messageDispatcher == null)
                throw new InvalidOperationException(
                    string.Format(
                        "The {0} cannot operate with a null {1}. Please provide a valid {1} instance to use for dispatching {2} messages.",
                        typeof (MouseSimulator).Name, typeof (IInputMessageDispatcher).Name,
                        typeof (Win32Types.INPUT).Name));

            _inputSimulator = inputSimulator;
            _messageDispatcher = messageDispatcher;
        }

        public IKeyboardSimulator Keyboard{
            get { return _inputSimulator.Keyboard; }
        }

        public IMouseSimulator MoveMouseBy(int pixelDeltaX, int pixelDeltaY){
            Win32Types.INPUT[] inputList =
                new InputBuilder().AddRelativeMouseMovement(pixelDeltaX, pixelDeltaY).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        public IMouseSimulator MoveMouseTo(double absoluteX, double absoluteY){
            Win32Types.INPUT[] inputList =
                new InputBuilder().AddAbsoluteMouseMovement((int) Math.Truncate(absoluteX),
                    (int) Math.Truncate(absoluteY)).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }


        public IMouseSimulator DragAndDrop(Point start, Point end){
            MoveMouseTo(start.X, start.Y);
            LeftButtonClick();

            LeftButtonDown();


            MoveMouseTo(start.X, start.Y + 1);
            Sleep(1000);

            MoveMouseTo(end.X, end.Y);
            LeftButtonUp();
            return this;
        }

        public IMouseSimulator MoveMouseToPositionOnVirtualDesktop(double absoluteX, double absoluteY){
            Win32Types.INPUT[] inputList =
                new InputBuilder().AddAbsoluteMouseMovementOnVirtualDesktop((int) Math.Truncate(absoluteX),
                    (int) Math.Truncate(absoluteY)).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        public IMouseSimulator LeftButtonDown(){
            Win32Types.INPUT[] inputList = new InputBuilder().AddMouseButtonDown(MouseButton.LeftButton).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        public IMouseSimulator LeftButtonUp(){
            Win32Types.INPUT[] inputList = new InputBuilder().AddMouseButtonUp(MouseButton.LeftButton).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        public IMouseSimulator LeftButtonClick(){
            Win32Types.INPUT[] inputList = new InputBuilder().AddMouseButtonClick(MouseButton.LeftButton).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        public IMouseSimulator LeftButtonDoubleClick(){
            Win32Types.INPUT[] inputList =
                new InputBuilder().AddMouseButtonDoubleClick(MouseButton.LeftButton).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        public IMouseSimulator RightButtonDown(){
            Win32Types.INPUT[] inputList = new InputBuilder().AddMouseButtonDown(MouseButton.RightButton).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        public IMouseSimulator RightButtonUp(){
            Win32Types.INPUT[] inputList = new InputBuilder().AddMouseButtonUp(MouseButton.RightButton).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        public IMouseSimulator RightButtonClick(){
            Win32Types.INPUT[] inputList = new InputBuilder().AddMouseButtonClick(MouseButton.RightButton).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        public IMouseSimulator RightButtonDoubleClick(){
            Win32Types.INPUT[] inputList =
                new InputBuilder().AddMouseButtonDoubleClick(MouseButton.RightButton).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        public IMouseSimulator XButtonDown(int buttonId){
            Win32Types.INPUT[] inputList = new InputBuilder().AddMouseXButtonDown(buttonId).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        public IMouseSimulator XButtonUp(int buttonId){
            Win32Types.INPUT[] inputList = new InputBuilder().AddMouseXButtonUp(buttonId).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        public IMouseSimulator XButtonClick(int buttonId){
            Win32Types.INPUT[] inputList = new InputBuilder().AddMouseXButtonClick(buttonId).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        public IMouseSimulator XButtonDoubleClick(int buttonId){
            Win32Types.INPUT[] inputList = new InputBuilder().AddMouseXButtonDoubleClick(buttonId).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        public IMouseSimulator VerticalScroll(int scrollAmountInClicks){
            Win32Types.INPUT[] inputList =
                new InputBuilder().AddMouseVerticalWheelScroll(scrollAmountInClicks*MouseWheelClickSize).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        public IMouseSimulator HorizontalScroll(int scrollAmountInClicks){
            Win32Types.INPUT[] inputList =
                new InputBuilder().AddMouseHorizontalWheelScroll(scrollAmountInClicks*MouseWheelClickSize).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        public IMouseSimulator Sleep(int millsecondsTimeout){
            Thread.Sleep(millsecondsTimeout);
            return this;
        }

        public IMouseSimulator Sleep(TimeSpan timeout){
            Thread.Sleep(timeout);
            return this;
        }

        private void SendSimulatedInput(Win32Types.INPUT[] inputList){
            _messageDispatcher.DispatchInput(inputList);
        }
    }
}