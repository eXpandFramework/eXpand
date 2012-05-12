using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Xpo.Metadata;

namespace Xpand.ExpressApp {
    public sealed class RunTimeMembersTypeDescriptor : CustomTypeDescriptor
    {
        private readonly Type _objectType;

        public RunTimeMembersTypeDescriptor(ICustomTypeDescriptor parent, Type objectType)
            : base(parent)
        {
            _objectType = objectType;
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            PropertyDescriptorCollection originalProperties = base.GetProperties();
            var newProperties = originalProperties.Cast<PropertyDescriptor>().ToList();

            var classInfo = XpandModuleBase.Dictiorary.GetClassInfo(_objectType);
            List<XPMemberInfo> runtimeMemberInfos =
                classInfo.OwnMembers.Where(
                    info => !newProperties.Select(descriptor => descriptor.Name).Contains(info.Name)&&!info.IsCollection&&info.IsPublic).ToList();
            newProperties.AddRange(runtimeMemberInfos.Select(memberInfo => TypeDescriptor.CreateProperty(_objectType, memberInfo.Name, memberInfo.MemberType, memberInfo.Attributes)));
            return new PropertyDescriptorCollection(newProperties.ToArray(), true);
        }
    }
}