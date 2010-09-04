using System;
using System.ComponentModel;

namespace Xpand.ExpressApp {
    public sealed class RuntimeMembersTypeDescriptionProvider : TypeDescriptionProvider
    {
        public RuntimeMembersTypeDescriptionProvider(TypeDescriptionProvider parent)
            : base(parent)
        {
        }

        
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance) {
            return new RunTimeMembersTypeDescriptor(base.GetTypeDescriptor(objectType, instance), objectType);
        }
    }
}