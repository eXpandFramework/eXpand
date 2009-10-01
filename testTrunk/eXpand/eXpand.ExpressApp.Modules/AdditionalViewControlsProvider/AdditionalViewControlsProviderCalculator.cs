using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider
{
    public class AdditionalViewControlsProviderCalculator : IDisposable
    {
        private AdditionalViewControlsWrapper additionalViewControlsWrapper;
        private ViewType currentViewType;
        private object currentObject;
        private string currentAdditionalText;
        private void supportNotifyPropertyChanged_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((additionalViewControlsWrapper != null) && (string.IsNullOrEmpty(e.PropertyName) || (e.PropertyName == additionalViewControlsWrapper.MessagePropertyName)))
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
            if (CurrentObject != null && additionalViewControlsWrapper != null && !string.IsNullOrEmpty(additionalViewControlsWrapper.MessagePropertyName))
                AdditionalText = (string) ReflectionHelper.GetMemberValue(CurrentObject, additionalViewControlsWrapper.MessagePropertyName);
            else
                AdditionalText = "";
        }
        protected virtual void OnHintChanged()
        {
            if (HintChanged != null)
                HintChanged(this, EventArgs.Empty);
        }
        public AdditionalViewControlsProviderCalculator(AdditionalViewControlsWrapper additionalViewControlsWrapper)
        {
            this.additionalViewControlsWrapper = additionalViewControlsWrapper;
        }
        public AdditionalViewControlsWrapper AdditionalViewControlsWrapper
        {
            get { return additionalViewControlsWrapper; }
            set
            {
                additionalViewControlsWrapper = value;
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
                if ((additionalViewControlsWrapper == null) ||
                    ((additionalViewControlsWrapper.ViewType != ViewType.Any) &&
                     (currentViewType != additionalViewControlsWrapper.ViewType)))
                {
                    return "";
                }
                string result = additionalViewControlsWrapper.Message;
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
            AdditionalViewControlsWrapper = null;
        }
        public event EventHandler<EventArgs> HintChanged;
    }
}