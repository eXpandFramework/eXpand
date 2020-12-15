using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.Dashboard.Services;
using Xpand.Extensions.XAF.Attributes;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Security;
using Xpand.XAF.Modules.CloneModelView;
using Xpand.Xpo;

namespace Xpand.ExpressApp.Dashboard.BusinessObjects {
    public interface IDashboardDefinition{
        bool DataViewService{ get; set; }
        RuleMode EditParameters{ get; set; }
        int Index { get; set; }
        string Name { get; set; }
        bool Active { get; set; }
        string Xml { get; set; }
        IList<ITypeWrapper> DashboardTypes { get; }
    }

    [ImageName("BO_DashboardDefinition")]
    [DefaultProperty("Name")]
    [DefaultClassOptions]
    [SecurityOperations("DashboardDefinitions", "DashboardOperation")]
    [NavigationItem("Reports")]
    [CloneModelView(CloneViewType.DetailView, DashboardViewerDetailView)]
    [CloneModelView(CloneViewType.DetailView, DashboardDesignerDetailView)]
    public class DashboardDefinition : XpandCustomObject, IDashboardDefinition,IDashboardData{
        public const string DashboardViewerDetailView = "DashboardDefinitionViewer_DetailView";
        public const string DashboardDesignerDetailView = "DashboardDesigner_DetailView";
        bool _active;
        BindingList<ITypeWrapper> _dashboardTypes;
        int _index;
        bool _dataViewService;
        [InvisibleInAllViews]
        public bool DataViewService{
            get => _dataViewService;
            set => SetPropertyValue(nameof(DataViewService), ref _dataViewService, value);
        }
        string _name;
        [Size(SizeAttribute.Unlimited)]
        [Persistent("TargetObjectTypes")]
        string _targetObjectTypes;
        IList<TypeWrapper> _types;

        public DashboardDefinition(Session session)
            : base(session) {
            _active = true;
        }

        RuleMode _editParameters;

        public RuleMode EditParameters{
            get => _editParameters;
            set => SetPropertyValue(nameof(EditParameters), ref _editParameters, value);
        }

        [Browsable(false)]
        internal IEnumerable<TypeWrapper> Types {
            get {
                return _types ??= XafTypesInfo.Instance.PersistentTypes
                    .Where(info => (info.IsVisible && info.IsPersistent) && (info.Type != null))
                    .Select(info => new TypeWrapper(info.Type))
                    .OrderBy(info => info.GetDefaultCaption())
                    .ToList();
            }
        }

        [Index(1)]
        public int Index{
            get => _index;
            set => SetPropertyValue("Index", ref _index, value);
        }

        [Index(0)]
        [RuleRequiredField]
        public string Name{
            get => _name;
            set => SetPropertyValue("Name", ref _name, value);
        }

        [Index(2)]
        public bool Active{
            get => _active;
            set => SetPropertyValue("Active", ref _active, value);
        }

        [Size(SizeAttribute.Unlimited)]
        [Delayed]
        [VisibleInDetailView(false)]
        [EditorAlias(EditorAliases.DashboardXMLEditor)]
        public string Xml{
            get => GetDelayedPropertyValue<string>();
            set => SetDelayedPropertyValue("Xml", value);
        }

        [DataSourceProperty("Types")]
        [ImmediatePostData]
        [VisibleInDetailView(false)]
        public IList<ITypeWrapper> DashboardTypes => GetBindingList();

        BindingList<ITypeWrapper> GetBindingList() {
            return _dashboardTypes ??= new BindingList<ITypeWrapper>();
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            SubscribeToListEvents();
        }


        protected override void OnLoaded() {
            base.OnLoaded();
            ReloadDashboardItems();
        }

        void UpdateTargetObjectTypes() {
            _targetObjectTypes = "<Types>\r\n";
            foreach (var resource in DashboardTypes.Distinct())
                _targetObjectTypes += $"<Value Type=\"{resource.Type.FullName}\"/>\r\n";
            _targetObjectTypes += "</Types>";
        }

        void ReloadDashboardItems() {
            UnsubscribeToListEvents();
            try {
                DashboardTypes.Clear();
                if (!String.IsNullOrEmpty(_targetObjectTypes)) {
                    var xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(_targetObjectTypes);
                    if (xmlDocument.DocumentElement != null)
                        foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
                            DashboardTypes.Add(Types.First(type => xmlNode.Attributes != null && type.Type == XafTypesInfo.Instance.FindTypeInfo(xmlNode.Attributes["Type"].Value).Type));
                }
            } finally {
                SubscribeToListEvents();
            }
        }

        void SubscribeToListEvents() {
            ((IBindingList)DashboardTypes).ListChanged += DashboardDefinition_ListChanged;
        }

        void UnsubscribeToListEvents() {
            ((IBindingList)DashboardTypes).ListChanged -= DashboardDefinition_ListChanged;
        }

        void DashboardDefinition_ListChanged(object sender, ListChangedEventArgs e) {
            if (e.ListChangedType == ListChangedType.ItemAdded || e.ListChangedType == ListChangedType.ItemDeleted || e.ListChangedType == ListChangedType.Reset) {
                UpdateTargetObjectTypes();
                OnChanged("TargetObjectTypes");
            }
        }

        string IDashboardData.Title{ get; set; }

        string IDashboardData.Content{
            get => Xml;
            set => Xml = value;
        }

        bool IDashboardData.SynchronizeTitle{ get; set; }
    }


}