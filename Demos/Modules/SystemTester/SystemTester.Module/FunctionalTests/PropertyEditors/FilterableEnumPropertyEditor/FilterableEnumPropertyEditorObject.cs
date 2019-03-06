using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.CustomAttributes;

namespace SystemTester.Module.FunctionalTests.PropertyEditors.FilterableEnumPropertyEditor {
    [XpandNavigationItem("PropertyEditors/FilterableEnumPropertyEditorObject")]
    [EnumFilter(nameof(FilteredStatus),EnumFilterMode.Remove, StatusColor.Blue)]
    public class FilterableEnumPropertyEditorObject : BaseObject {
        private StatusColor? _filteredStatus;
        private bool _switch;

        public FilterableEnumPropertyEditorObject(Session session) : base(session) {
        }

        [DataSourceProperty("InstanceStatusDataSource")]

        public StatusColor? FilteredStatus {
            get => _filteredStatus;
            set => SetPropertyValue("FilteredStatus", ref _filteredStatus, value);
        }

        [ImmediatePostData]
        public bool Switch {
            get => _switch;
            set {
                SetPropertyValue("Switch", ref _switch, value);
                OnChanged("InstanceStatusDataSource");
            }
        }

        [Browsable(false)]
        public static IList<StatusColor?> StaticStatusDataSource =>
            new StatusColor?[] {
                StatusColor.Red,
                StatusColor.Green
            };

        [Browsable(false)]
        public IList<StatusColor?> InstanceStatusDataSource =>
            Switch
                ? new StatusColor?[] {
                    StatusColor.Red,
                    StatusColor.Green
                }
                : new StatusColor?[] {
                    StatusColor.White,
                    StatusColor.Blue
                };
    }

    public enum StatusColor {
        Red,
        Green,
        White,
        Blue
    }
}