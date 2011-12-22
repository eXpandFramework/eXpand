using System;

using DevExpress.Xpo;

using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;

namespace SecurityDemo.Module {
	public class Hints {
        public const string FullAccessObjectHint =
            @"Objects in this View are fully accessible by any user.";
        public const string ProtectedContentObjectHint =
            @"Objects in this View are inaccessible for the ""John"" user.";
        public const string ReadOnlyObjectHint =
            @"Objects in this View are read-only for the ""John"" user.";
        public const string IrremovableObjectHint =
            @"Objects in this View cannot be removed by the ""John"" user.";
        public const string UncreatableObjectHint =
            @"Objects in this View cannot be created by the ""John"" user.";
		
        public const string MemberLevelSecurityObjectHint =
            @"This View demonstrates an object that is partially accessible for the ""John"" user. Some object members are read-only or totally inaccessible for him.";

        public const string ObjectLevelSecurityObject =
            @"This View demonstrates objects of the same type that have various access levels for the ""John"" user. There are read-only, irremovable, inaccessible and fully accessible objects.";

		public const string LogonWindowHeaderHint =
@"This application demonstrates the Complex Security Strategy in action. There are two predefined users in this demo. ""Sam"" is an administrator who has a full access to demo objects, as well as an option to change permissions for other users. ""John"" is a user with certain predefined restrictions.";
	}

    public static class NavigationGroups {
        public const string ClassLevelSecurity = "Class-level security";
        public const string MemberLevelSecurity = "Member-level security";
        public const string ObjectLevelSecurity = "Object-level security";
    }
}
