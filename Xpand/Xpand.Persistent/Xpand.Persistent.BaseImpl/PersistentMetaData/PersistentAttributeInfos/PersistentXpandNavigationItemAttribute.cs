using System.ComponentModel;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [InterfaceRegistrator(typeof(IPersistentNavigationItemAttribute))]
    [DefaultProperty("Path")]
    [System.ComponentModel.DisplayName("Xpand Navigation")]
    [CreateableItem(typeof(IPersistentClassInfo))]
    public class PersistentXpandNavigationItemAttribute : PersistentAttributeInfo, IPersistentNavigationItemAttribute {
        public PersistentXpandNavigationItemAttribute(Session session)
            : base(session) {
        }
        private string _viewId;
        public string ViewId {
            get { return _viewId; }
            set { SetPropertyValue("ViewId", ref _viewId, value); }
        }
        private string _objectKey;

        public string ObjectKey {
            get { return _objectKey; }
            set { SetPropertyValue("ObjectKey", ref _objectKey, value); }
        }
        private string _path;
        [RuleRequiredField]
        [Size(SizeAttribute.Unlimited)]
        public string Path {
            get { return _path; }
            set { SetPropertyValue("Path", ref _path, value); }
        }
        public override AttributeInfoAttribute Create() {
            var constructorInfo = typeof(XpandNavigationItemAttribute).GetConstructor(new[] { typeof(string), typeof(string), typeof(string), typeof(int) });
            return new AttributeInfoAttribute(constructorInfo, new object[] { Path, ViewId, ObjectKey });
        }
    }
}