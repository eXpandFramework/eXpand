using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.AdditionalViewControlsProvider.NodeWrappers;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider
{
    public class AdditionalViewControlsProviderCalculator : IDisposable
    {
        private IAdditionalViewControlsRule _controlsRule;
        private ViewType currentViewType;
        private object currentObject;
        private string currentAdditionalText;
        private void supportNotifyPropertyChanged_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((_controlsRule != null) && (string.IsNullOrEmpty(e.PropertyName) || (e.PropertyName == _controlsRule.MessagePropertyName)))
            {
                UpdateAdditionalText();
            }
        }
        private void RemovePropertyChangedHandler(object obj)
        {
            var supportNotifyPropertyChanged = obj as INotifyPropertyChanged;
            if (supportNotifyPropertyChanged != null)
            {
                supportNotifyPropertyChanged.PropertyChanged -= supportNotifyPropertyChanged_PropertyChanged;
            }
        }
        private void AddPropertyChangedHandler(object obj)
        {
            var supportNotifyPropertyChanged = obj as INotifyPropertyChanged;
            if (supportNotifyPropertyChanged != null)
                supportNotifyPropertyChanged.PropertyChanged +=supportNotifyPropertyChanged_PropertyChanged;
        }
        private void UpdateAdditionalText()
        {
            if (CurrentObject != null && _controlsRule != null && !string.IsNullOrEmpty(_controlsRule.MessagePropertyName))
                AdditionalText = (string) ReflectionHelper.GetMemberValue(CurrentObject, _controlsRule.MessagePropertyName);
            else
                AdditionalText = "";
        }
        protected virtual void OnHintChanged()
        {
            if (HintChanged != null)
                HintChanged(this, EventArgs.Empty);
        }
        public AdditionalViewControlsProviderCalculator(IAdditionalViewControlsRule controlsRule)
        {
            _controlsRule = controlsRule;
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
        public string Hint
        {
            get
            {
                if ((_controlsRule == null) ||
                    ((_controlsRule.ViewType != ViewType.Any) &&
                     (currentViewType != _controlsRule.ViewType)))
                {
                    return "";
                }
                string result = _controlsRule.Message;
                if (!string.IsNullOrEmpty(AdditionalText))
                {
                    if (!string.IsNullOrEmpty(result))
                    {
                        result += "\r\n---\r\n";
                    }
                    result += AdditionalText;
                }
                return result;
            }
        }
        public void Dispose()
        {
            CurrentObject = null;
            ControlsRule = null;
        }
        public event EventHandler<EventArgs> HintChanged;
    }
}