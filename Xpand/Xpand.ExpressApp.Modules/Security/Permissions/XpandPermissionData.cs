using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;
using Xpand.ExpressApp.Security.Core;
using Xpand.Xpo;

namespace Xpand.ExpressApp.Security.Permissions {
    public interface IOperationPermissionProvider {
        IList<IOperationPermission> GetPermissions();
    }
    [ImageName("BO_Security_Permission_Type")]
    public abstract class XpandPermissionData : XpandCustomObject, IOperationPermissionProvider {
        IEnumerable<XPMemberInfo> _propertyInfos;


        protected XpandPermissionData(Session session)
            : base(session) {
            EnumerateProperties();
        }

        public override string ToString() {
            return Permission;
        }

        [VisibleInDetailView(false)]
        public string Permission {
            get {
                return string.Format("{1}= {0}", GetPermissionInfoCaption(), string.Join(", ", GetPermissions().Select(item => item.Operation).ToArray()));
            }
        }
        protected virtual string GetPermissionInfoCaption() {
            return _propertyInfos.Aggregate<XPMemberInfo, string>(null, (current, propertyInfo)
                => current + GetCaption(propertyInfo)).TrimEnd(", ".ToCharArray());
        }

        string GetCaption(XPMemberInfo propertyInfo) {
            var value = propertyInfo.GetValue(this);
            return value != null && !string.IsNullOrEmpty(value.ToString())
                       ? string.Format("{0}: {1}", propertyInfo.Name, value) + ", "
                       : null;
        }

        void EnumerateProperties() {
            _propertyInfos = ClassInfo.Members.Where(info => AttributesMatch(info) && info.IsPersistent && InfoTypeMatch(info));
        }

        bool InfoTypeMatch(XPMemberInfo info) {
            return typeof(XpandPermissionData).IsAssignableFrom(info.Owner.ClassType) && !(info is ServiceField);
        }

        bool AttributesMatch(XPMemberInfo info) {
            return info.FindAttributeInfo(typeof(NonPersistentAttribute)) == null && info.FindAttributeInfo(typeof(VisibleInListViewAttribute)) == null;
        }
        #region Implementation of IOperationPermissionProvider
        public abstract IList<IOperationPermission> GetPermissions();

        #endregion
        private XpandRole _role;
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [Association("XpandRole-XpandPermissionDatas")]
        public XpandRole Role {
            get {
                return _role;
            }
            set {
                SetPropertyValue("Role", ref _role, value);
            }
        }
    }
}