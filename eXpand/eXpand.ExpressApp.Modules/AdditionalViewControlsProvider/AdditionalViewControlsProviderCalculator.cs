using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.AdditionalViewControlsProvider.NodeWrappers;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider
{
    public class AdditionalViewControlsProviderCalculator : IDisposable
    {
        private AdditionalViewControlsRuleWrapper _additionalViewControlsRuleWrapper;
        private ViewType currentViewType;
        private object currentObject;
        private string currentAdditionalText;
        private void supportNotifyPropertyChanged_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((_additionalViewControlsRuleWrapper != null) && (string.IsNullOrEmpty(e.PropertyName) || (e.PropertyName == _additionalViewControlsRuleWrapper.MessagePropertyName)))
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
            if (CurrentObject != null && _additionalViewControlsRuleWrapper != null && !string.IsNullOrEmpty(_additionalViewControlsRuleWrapper.MessagePropertyName))
                AdditionalText = (string) ReflectionHelper.GetMemberValue(CurrentObject, _additionalViewControlsRuleWrapper.MessagePropertyName);
            else
                AdditionalText = "";
        }
        protected virtual void OnHintChanged()
        {
            if (HintChanged != null)
                HintChanged(this, EventArgs.Empty);
        }
        public AdditionalViewControlsProviderCalculator(AdditionalViewControlsRuleWrapper _additionalViewControlsRuleWrapper)
        {
            this._additionalViewControlsRuleWrapper = _additionalViewControlsRuleWrapper;
        }
        public AdditionalViewControlsRuleWrapper AdditionalViewControlsRuleWrapper
        {
            get { return _additionalViewControlsRuleWrapper; }
            set
            {
                _additionalViewControlsRuleWrapper = value;
                OnHintChanged();
            }
        }
        public ViewType CurrentViewType
        {
            get { return currentViewType; }
            set
            {
                currentViewType = value;
                OnHintChanged();
            }
        }
        public object CurrentObject
        {
            get { return currentObject; }
            set
            {
                RemovePropertyChangedHandler(currentObject);
                currentObject = value;
                AddPropertyChangedHandler(currentObject);
                UpdateAdditionalText();
            }
        }
        public string AdditionalText
        {
            get { return currentAdditionalText; }
            set
            {
                currentAdditionalText = value;
                OnHintChanged();
            }
        }
        public string Hint
        {
            get
            {
                if ((_additionalViewControlsRuleWrapper == null) ||
                    ((_additionalViewControlsRuleWrapper.ViewType != ViewType.Any) &&
                     (currentViewType != _additionalViewControlsRuleWrapper.ViewType)))
                {
                    return "";
                }
                string result = _additionalViewControlsRuleWrapper.Message;
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
            AdditionalViewControlsRuleWrapper = null;
        }
        public event EventHandler<EventArgs> HintChanged;
    }
}