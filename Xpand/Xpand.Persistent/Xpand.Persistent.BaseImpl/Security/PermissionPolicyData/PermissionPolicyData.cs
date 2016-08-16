using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;
using Fasterflect;
using Xpand.Persistent.Base.General;
using Xpand.Xpo;
using IOperationPermissionProvider = Xpand.ExpressApp.Security.Permissions.IOperationPermissionProvider;

namespace Xpand.Persistent.BaseImpl.Security.PermissionPolicyData {
    [ImageName("BO_Security_Permission_Type")]
    public abstract class PermissionPolicyData : XpandCustomObject, IOperationPermissionProvider {
        IEnumerable<XPMemberInfo> _propertyInfos;


        protected PermissionPolicyData(Session session)
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
            return !string.IsNullOrEmpty(value?.ToString())
                       ? $"{propertyInfo.Name}: {value}" + ", "
                       : null;
        }

        void EnumerateProperties() {
            _propertyInfos = ClassInfo.Members.Where(info => AttributesMatch(info) && info.IsPersistent && InfoTypeMatch(info));
        }

        bool InfoTypeMatch(XPMemberInfo info) {
            return typeof(PermissionPolicyData).IsAssignableFrom(info.Owner.ClassType) && !(info is ServiceField);
        }

        bool AttributesMatch(XPMemberInfo info) {
            return info.FindAttributeInfo(typeof(NonPersistentAttribute)) == null && info.FindAttributeInfo(typeof(VisibleInListViewAttribute)) == null;
        }
        #region Implementation of IOperationPermissionProvider

        protected ITypeInfo GetPermissionTypeInfo(Type type){
            return XafTypesInfo.Instance.DomainSealedInfos(type).First(info => typeof(IOperationPermission).IsAssignableFrom(info.Type));
        }

        protected IList<IOperationPermission> GetOperationPermissions<TOperationPermission>() {
            var typeInfo = GetPermissionTypeInfo<TOperationPermission>();
            var modelCombinePermission = (TOperationPermission)typeInfo.Type.CreateInstance(this);
            return new[] { (IOperationPermission)modelCombinePermission };
        }

        protected ITypeInfo GetPermissionTypeInfo<T>() {
            return GetPermissionTypeInfo(typeof(T));
        }
        public abstract IList<IOperationPermission> GetPermissions();
        #endregion
        private XpandPermissionPolicyRole _role;

        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [Association("XpandPermissionPolicyRole-XpandPermissionDatas")]
        public XpandPermissionPolicyRole Role{
            get { return _role; }
            set { SetPropertyValue("Role", ref _role, value); }
        }
    }
}