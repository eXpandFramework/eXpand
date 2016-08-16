using System;
using System.ComponentModel;
using System.Reflection;
using DevExpress.ExpressApp;
using Xpand.Persistent.Base.AdditionalViewControls;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider {
    public class AdditionalViewControlsProviderCalculator : IDisposable {
        IAdditionalViewControlsRule _controlsRule;
        readonly Type _objectType;
        private string _currentAdditionalText;
        private object _currentObject;

        public AdditionalViewControlsProviderCalculator(IAdditionalViewControlsRule controlsRule, Type objectType) {
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

        public object CurrentObject {
            get { return _currentObject; }
            set {
                RemovePropertyChangedHandler(_currentObject);
                _currentObject = value;
                AddPropertyChangedHandler(_currentObject);
                UpdateAdditionalText();
            }
        }

        public string AdditionalText {
            get { return _currentAdditionalText; }
            set {
                _currentAdditionalText = value;
                OnHintChanged();
            }
        }

        public string Hint {
            get {
                if ((_controlsRule == null)) {
                    return "";
                }
                string result = _controlsRule.MessageProperty == null ? _controlsRule.Message : null;
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
                ((e.PropertyName == _controlsRule.MessageProperty) && e.PropertyName != null)) {
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
            if (!string.IsNullOrEmpty(_controlsRule?.MessageProperty)) {
                if (CurrentObject != null) {
                    var memberInfo = XafTypesInfo.Instance.FindTypeInfo(CurrentObject.GetType()).FindMember(_controlsRule.MessageProperty);
                    if (memberInfo != null) {
                        if (!string.IsNullOrEmpty(_controlsRule.Message))
                            memberInfo.SetValue(CurrentObject, _controlsRule.Message);
                        AdditionalText = memberInfo.GetValue(CurrentObject) as string;
                    } else
                        AdditionalText = null;
                } else {
                    PropertyInfo propertyInfo = _objectType.GetProperty(_controlsRule.MessageProperty, BindingFlags.Static);
                    if (propertyInfo != null)
                        AdditionalText = propertyInfo.GetValue(null, null) as string;
                }
            } else
                AdditionalText = "";
        }

        protected virtual void OnHintChanged(){
            HintChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<EventArgs> HintChanged;
    }
}