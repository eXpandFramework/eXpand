using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp;

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
    [Custom("Caption", "Member-Level Protected Object")]
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
}
