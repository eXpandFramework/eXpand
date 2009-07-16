using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp.HintModule
{
    public class HintCalculator : IDisposable
    {
        private HintAttribute attribute;
        private ViewType currentViewType;
        private object currentObject;
        private string currentAdditionalText;
        private void supportNotifyPropertyChanged_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((attribute != null) && (string.IsNullOrEmpty(e.PropertyName) || (e.PropertyName == attribute.HintPropertyName)))
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
            if (CurrentObject != null && attribute != null && !string.IsNullOrEmpty(attribute.HintPropertyName))
                AdditionalText = (string) ReflectionHelper.GetMemberValue(CurrentObject, attribute.HintPropertyName);
            else
                AdditionalText = "";
        }
        protected virtual void OnHintChanged()
        {
            if (HintChanged != null)
                HintChanged(this, EventArgs.Empty);
        }
        public HintCalculator(HintAttribute attribute)
        {
            this.attribute = attribute;
        }
        public HintAttribute Attribute
        {
            get { return attribute; }
            set
            {
                attribute = value;
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
                if ((attribute == null) || ((attribute.TargetViewType != ViewType.Any) && (currentViewType != attribute.TargetViewType)))
                {
                    return "";
                }
                string result = attribute.Hint;
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
            Attribute = null;
        }
        public event EventHandler<EventArgs> HintChanged;
    }
}