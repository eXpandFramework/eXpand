using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.DC;
using System.ComponentModel;

namespace DevExpress.ExpressApp.Demos {
    public interface IHintProvider {
        string Hint { get; }
    }

    public interface IHintProviderEx : IHintProvider {
        ViewType TargetViewType { get; }
        string HintPropertyName { get; }
    }

	[Flags]
	public enum HintTargets { ListView, DetailVew }

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class HintAttribute : Attribute, IHintProviderEx {
        private string hint;
        private ViewType targetViewType;
        private string hintPropertyName;
        public HintAttribute(string hint, ViewType targetViewType)
            : this(hint, targetViewType, "") {
        }
        public HintAttribute(string hint, ViewType targetViewType, string hintPropertyName) {
            this.hint = hint;
            this.targetViewType = targetViewType;
            this.hintPropertyName = hintPropertyName;
        }
        public HintAttribute(string hint)
            : this(hint, ViewType.ListView) {
        }

        public string Hint {
            get { return hint; }
        }
        public ViewType TargetViewType {
            get { return targetViewType; }
        }
        public string HintPropertyName {
            get { return hintPropertyName; }
        }
        public bool IsTargetView(View view) {
            return
                TargetViewType == ViewType.Any
                ||
                ((view is DetailView) && (TargetViewType == ViewType.DetailView))
                ||
                ((view is ListView) && (TargetViewType == ViewType.ListView));
        }
    }

	public class HintCalculator : IDisposable {
		private IHintProvider hintProvider;
		private ViewType currentViewType;
		private object currentObject;
		private string currentAdditionalText;
		private void supportNotifyPropertyChanged_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if((hintProvider != null) && (hintProvider is IHintProviderEx) && (string.IsNullOrEmpty(e.PropertyName) || (e.PropertyName == ((IHintProviderEx)hintProvider).HintPropertyName))) {
                UpdateAdditionalText();
			}
		}
		private void RemovePropertyChangedHandler(object obj) {
			INotifyPropertyChanged supportNotifyPropertyChanged = obj as INotifyPropertyChanged;
			if(supportNotifyPropertyChanged != null) {
				supportNotifyPropertyChanged.PropertyChanged -= new PropertyChangedEventHandler(supportNotifyPropertyChanged_PropertyChanged);
			}
		}
		private void AddPropertyChangedHandler(object obj) {
			INotifyPropertyChanged supportNotifyPropertyChanged = obj as INotifyPropertyChanged;
			if(supportNotifyPropertyChanged != null) {
				supportNotifyPropertyChanged.PropertyChanged += new PropertyChangedEventHandler(supportNotifyPropertyChanged_PropertyChanged);
			}
		}
		private void UpdateAdditionalText() {
            if(CurrentObject != null && hintProvider != null && hintProvider is IHintProviderEx && !string.IsNullOrEmpty(((IHintProviderEx)hintProvider).HintPropertyName)) {
                AdditionalText = (string)ReflectionHelper.GetMemberValue(CurrentObject, ((IHintProviderEx)hintProvider).HintPropertyName);
			}
			else {
				AdditionalText = "";
			}
		}
		protected virtual void OnHintChanged() {
			if(HintChanged != null) {
				HintChanged(this, EventArgs.Empty);
			}
		}
        public HintCalculator(IHintProvider hintProvider) {
            this.hintProvider = hintProvider;
        }
        public IHintProvider HintProvider  {
            get { return hintProvider; }
            set {
                hintProvider = value;
                OnHintChanged();
            }
        }

		public ViewType CurrentViewType {
			get { return currentViewType; }
			set {
				currentViewType = value;
				OnHintChanged();
			}
		}
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
				if((hintProvider == null) || (hintProvider is IHintProviderEx && ((((IHintProviderEx)hintProvider).TargetViewType != ViewType.Any) && (currentViewType != ((IHintProviderEx)hintProvider).TargetViewType)))) {
					return "";
				}
				string result = hintProvider.Hint;
				if(!string.IsNullOrEmpty(AdditionalText)) {
					if(!string.IsNullOrEmpty(result)) {
						result += "\r\n---\r\n";
					}
					result += AdditionalText;
				}
				return result;
			}
		}
		public void Dispose() {
			CurrentObject = null;
			HintProvider = null;
		}
		public event EventHandler<EventArgs> HintChanged;
	}

}
