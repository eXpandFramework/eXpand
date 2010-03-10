using System;
using DevExpress.ExpressApp.Security;

namespace eXpand.ExpressApp.MemberLevelSecurity {
    public class MemberAccessPermissionItem {
        string memberName;
        ObjectAccessModifier modifier;
        Type objectType;
        MemberOperation operation;

        public MemberAccessPermissionItem() {
        }

        public MemberAccessPermissionItem(MemberAccessPermissionItem source) {
            memberName = source.memberName;
            objectType = source.objectType;
            operation = source.operation;
            modifier = source.modifier;
        }

        public Type ObjectType {
            get { return objectType; }
            set { objectType = value; }
        }

        public string MemberName {
            get { return memberName; }
            set { memberName = value; }
        }

        public MemberOperation Operation {
            get { return operation; }
            set { operation = value; }
        }

        public ObjectAccessModifier Modifier {
            get { return modifier; }
            set { modifier = value; }
        }
    }
}