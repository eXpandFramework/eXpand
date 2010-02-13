using System;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.AdditionalViewControlsProvider.NodeWrappers;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider {
    public class AdditionalViewControlsProviderCalculator : IDisposable {
        IAdditionalViewControlsRule _controlsRule;
        readonly Type _objectType;
        string currentAdditionalText;
        object currentObject;

        public AdditionalViewControlsProviderCalculator(IAdditionalViewControlsRule controlsRule,Type objectType) {
            _controlsRule = controlsRule;
            _objectType = objectType;
        }

        public IAdditionalViewControlsRule ControlsRule {
            get { return _controlsRule; }
            set {
                _controlsRule = value;
                OnHintChanged();
            }
        }

//        public ViewType CurrentViewType {
//            get { return currentViewType; }
//            set {
//                currentViewType = value;
//                OnHintChanged();
//            }
//        }

        public object CurrentObject {
            get { return currentObject; }
            set {
                RemovePropertyChangedHandler(currentObject);
                currentObject = value;
                AddPropertyChangedHandler(currentObject);
                UpdateAdditionalText();
            }
        }

        public string AdditionalText {
            get { return currentAdditionalText; }
            set {
                currentAdditionalText = value;
                OnHintChanged();
            }
        }

        public string Hint {
            get {
                if ((_controlsRule == null)) {
                    return "";
                }
                string result = _controlsRule.Message;
                if (!string.IsNullOrEmpty(AdditionalText)) {
                    if (!string.IsNullOrEmpty(result)) {
                        result += "\r\n---\r\n";
                    }
                    result += AdditionalText;
                }
                return result;
            }
        }
        #region IDisposable Members
        public void Dispose() {
            CurrentObject = null;
            ControlsRule = null;
        }
        #endregion
        void supportNotifyPropertyChanged_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if ((_controlsRule != null) &&
                (string.IsNullOrEmpty(e.PropertyName) || (e.PropertyName == _controlsRule.MessagePropertyName))) {
                UpdateAdditionalText();
            }
        }

        void RemovePropertyChangedHandler(object obj) {
            var supportNotifyPropertyChanged = obj as INotifyPropertyChanged;
            if (supportNotifyPropertyChanged != null) {
                supportNotifyPropertyChanged.PropertyChanged -= supportNotifyPropertyChanged_PropertyChanged;
            }
        }

        void AddPropertyChangedHandler(object obj) {
            var supportNotifyPropertyChanged = obj as INotifyPropertyChanged;
            if (supportNotifyPropertyChanged != null)
                supportNotifyPropertyChanged.PropertyChanged += supportNotifyPropertyChanged_PropertyChanged;
        }

        void UpdateAdditionalText() {
            if (_controlsRule != null &&!string.IsNullOrEmpty(_controlsRule.MessagePropertyName)){
                AdditionalText = CurrentObject != null
                                     ? (string)ReflectionHelper.GetMemberValue(CurrentObject, _controlsRule.MessagePropertyName)
                                     : _objectType.GetProperty(_controlsRule.MessagePropertyName).GetValue(null, null)as string;
            }
            else
                AdditionalText = "";
        }

        protected virtual void OnHintChanged() {
            if (HintChanged != null)
                HintChanged(this, EventArgs.Empty);
        }

        public event EventHandler<EventArgs> HintChanged;
    }
}