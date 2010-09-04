using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalEditorState;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.ConditionalControllerState.Logic;
using System.Linq;
using Xpand.Persistent.Base.ImportExport;

namespace Xpand.Persistent.BaseImpl.ImportExport
{
    [Custom("DefaultListViewMasterDetailMode", "ListViewAndDetailView")]
    [ControllerStateRule("ClassInfoGraphNode+NewObjectViewController", typeof(NewObjectViewController),  "1=1", "1=1", ControllerState.Disabled)]
    [ControllerStateRule("ClassInfoGraphNode+DeleteObjectsViewController", typeof(DeleteObjectsViewController),  "1=1", "1=1", ControllerState.Disabled)]
    [DefaultProperty("Name")]
    public class ClassInfoGraphNode : BaseObject, IClassInfoGraphNode
    {
        
        private string name;
        private SerializationConfiguration serializationConfiguration;
        private SerializationStrategy serializationStrategy;
        public ClassInfoGraphNode(Session session) : base(session) { }

        [EditorStateRule("RuleObjectCanNotBeKey", "Key", ViewType.Any)]
        public EditorState RuleObjectCanNotBeKey(out bool active) {
            active = XafTypesInfo.Instance.PersistentTypes.Where(info => info.Name == TypeName).Count()>0;
            return EditorState.Disabled ;
        }

        private NodeType _nodeType;
        [VisibleInListView(false)]
        [Custom("AllowEdit", "false")]
        public NodeType NodeType {
            get { return _nodeType; }
            set { SetPropertyValue("NodeType", ref _nodeType, value); }
        }
        [VisibleInDetailView(false)]
        [Custom("AllowEdit", "false")]
        public string Name
        {
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }

        [Association]
        public SerializationConfiguration SerializationConfiguration
        {
            get { return serializationConfiguration; }
            set { SetPropertyValue("SerializationConfiguration", ref serializationConfiguration, value); }
        }

        ISerializationConfiguration IClassInfoGraphNode.SerializationConfiguration
        {
            get { return SerializationConfiguration; }
            set { SerializationConfiguration = value as SerializationConfiguration; }
        }
        [VisibleInListView(false)]
        public SerializationStrategy SerializationStrategy
        {
            get { return serializationStrategy; }
            set { SetPropertyValue("SerializationStrategy", ref serializationStrategy, value); }
        }

        private bool _key;
        [VisibleInListView(false)]
        public bool Key {
            get { return _key; }
            set { SetPropertyValue("Key", ref _key, value); }
        }

        #region ITreeNode Members

        private string _typeName;

        [Browsable(false)]
        public string TypeName {
            get { return _typeName; }
            set { SetPropertyValue("TypeName", ref _typeName, value); }
        }
        #endregion
    }
}