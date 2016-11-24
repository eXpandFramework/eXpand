using System;
using System.ComponentModel;
using System.Dynamic;
using System.Windows;

namespace Xpand.Utils.Dynamic {
    public class DynamicElement : DynamicObject {
        readonly DependencyObject _depObj;

        public DynamicElement(DependencyObject depObj) {
            if (depObj == null)
                throw new ArgumentNullException("depObj");

            _depObj = depObj;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            Tuple<bool, object> output;

            if (_depObj.Dispatcher.CheckAccess()) {
                output = PerformGet(binder);
            } else {
                output = _depObj.Dispatcher.Invoke((Func<Tuple<bool, object>>)(() => PerformGet(binder))) as Tuple<bool, object>;
            }

            if (output != null) {
                result = output.Item2;
                return output.Item1;
            }
            throw new NotImplementedException();
        }

        Tuple<bool, object> PerformGet(GetMemberBinder binder) {
            bool success = false;
            object result = null;

            var propDesc = TypeDescriptor.GetProperties(_depObj)[binder.Name];
            if (propDesc != null) {
                result = propDesc.GetValue(_depObj);
                success = true;
            }

            return new Tuple<bool, object>(success, result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value) {
            bool success;
            if (_depObj.Dispatcher.CheckAccess()) {
                success = PerformSet(binder, value);
            } else {
                success = (bool)_depObj.Dispatcher.Invoke((Func<bool>)(() => PerformSet(binder, value)));
            }
            return success;
        }

        bool PerformSet(SetMemberBinder binder, object value) {
            bool success = false;
            var propDesc = TypeDescriptor.GetProperties(_depObj)[binder.Name];
            if (propDesc != null) {
                propDesc.SetValue(_depObj, value);
                success = true;
            }
            return success;
        }
    }
}
