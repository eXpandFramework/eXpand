using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace SecurityDemo.Module {
    public class MemberLevelReferencedObject1 : SecurityDemoBaseObject {
        private MemberLevelSecurityObject owner;
        public MemberLevelReferencedObject1(Session session)
            : base(session) {
        }
        [Association("MemberLevelPermissionObject-ProtectedContentCollection")]
        public MemberLevelSecurityObject Owner {
            get {
                return owner;
            }
            set {
                SetPropertyValue("Owner", ref owner, value);
            }
        }
    }

    public class MemberLevelReferencedObject2 : SecurityDemoBaseObject {
        private MemberLevelSecurityObject owner;
        public MemberLevelReferencedObject2(Session session)
            : base(session) {
        }
        [Association("MemberLevelPermissionObject-ReadOnlyCollection")]
        public MemberLevelSecurityObject Owner {
            get {
                return owner;
            }
            set {
                SetPropertyValue("Owner", ref owner, value);
            }
        }
    }

    [Hint(Hints.MemberLevelSecurityObjectHint, ViewType.Any)]
    [NavigationItem(NavigationGroups.MemberLevelSecurity)]
    [ImageName("Demo_Security_MemberLevel")]
    [ModelDefault("Caption", "Member-Level Protected Object")]
    public class MemberLevelSecurityObject : SecurityDemoBaseObject {
        private string protectedContentProperty;
        private string readOnlyProperty;
        private string readWriteProperty;

        public MemberLevelSecurityObject(Session session)
            : base(session) {
        }
        public string ProtectedContentProperty {
            get {
                return protectedContentProperty;
            }
            set {
                SetPropertyValue("ProtectedContentProperty", ref protectedContentProperty, value);
            }
        }
        public string ReadOnlyProperty {
            get {
                return readOnlyProperty;
            }
            set {
                SetPropertyValue("ReadOnlyProperty", ref readOnlyProperty, value);
            }
        }
        public string ReadWriteProperty {
            get {
                return readWriteProperty;
            }
            set {
                SetPropertyValue("ReadWriteProperty", ref readWriteProperty, value);
            }
        }

        [Association("MemberLevelPermissionObject-ProtectedContentCollection")]
        public XPCollection<MemberLevelReferencedObject1> ProtectedContentCollection {
            get {
                return GetCollection<MemberLevelReferencedObject1>("ProtectedContentCollection");
            }
        }

        [Association("MemberLevelPermissionObject-ReadOnlyCollection")]
        public XPCollection<MemberLevelReferencedObject2> ReadOnlyCollection {
            get {
                return GetCollection<MemberLevelReferencedObject2>("ReadOnlyCollection");
            }
        }

    }

    [Hint(Hints.MemberByCriteriaLevelSecurityObjectHint, ViewType.Any)]
    [NavigationItem(NavigationGroups.MemberLevelSecurity)]
    [ImageName("Demo_Security_MemberByCriteria")]
    [ModelDefault("Caption", "Protect Member By Criteria Object")]
    public class MemberByCriteriaSecurityObject : SecurityDemoBaseObject {
        private string property1;
        private MemberLevelReferencedObject1 referenceProperty;
        public MemberByCriteriaSecurityObject(Session session)
            : base(session) {
        }
        public string Property1 {
            get { return property1; }
            set { SetPropertyValue("Property1", ref property1, value); }
        }
        public MemberLevelReferencedObject1 ReferenceProperty {
            get { return referenceProperty; }
            set { SetPropertyValue("ReferenceProperty", ref referenceProperty, value); }
        }
    }
}
