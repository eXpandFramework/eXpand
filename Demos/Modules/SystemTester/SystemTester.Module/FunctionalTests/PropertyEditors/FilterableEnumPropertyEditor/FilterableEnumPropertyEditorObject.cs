using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;

namespace SystemTester.Module.FunctionalTests.PropertyEditors.FilterableEnumPropertyEditor{
    [XpandNavigationItem("PropertyEditors/FilterableEnumPropertyEditorObject")]
    public class FilterableEnumPropertyEditorObject : BaseObject {
        private StatusColor? _filteredStatus;
        private bool _switch;

        public FilterableEnumPropertyEditorObject(Session session) : base(session) { }

        [DataSourceProperty("InstanceStatusDataSource")]
        [EditorAlias(EditorAliases.FilterableEnumPropertyEditor)]
        public StatusColor? FilteredStatus {
            get { return _filteredStatus; }
            set { SetPropertyValue("FilteredStatus", ref _filteredStatus, value); }
        }

        [ImmediatePostData]
        public bool Switch {
            get { return _switch; }
            set {
                SetPropertyValue("Switch", ref _switch, value);
                OnChanged("InstanceStatusDataSource");
            }
        }

        [Browsable(false)]
        public static IList<StatusColor?> StaticStatusDataSource {
            get {
                return new StatusColor?[] {
                    StatusColor.Red,
                    StatusColor.Green };
            }
        }

        [Browsable(false)]
        public IList<StatusColor?> InstanceStatusDataSource {
            get {
                return Switch ?
                    new StatusColor?[] {
                        StatusColor.Red,
                        StatusColor.Green } :
                    new StatusColor?[] {
                        StatusColor.White,
                        StatusColor.Blue };
            }
        }
    }

    public enum StatusColor{
        Red,
        Green,
        White,
        Blue
    }
}