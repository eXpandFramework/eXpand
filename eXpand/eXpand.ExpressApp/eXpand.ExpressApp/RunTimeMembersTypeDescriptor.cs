using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Xpo.Metadata;

namespace eXpand.ExpressApp {
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
            var newProperties = new List<PropertyDescriptor>();
            foreach (PropertyDescriptor pd in originalProperties)
                newProperties.Add(pd);

            List<XPMemberInfo> runtimeMemberInfos =
                XafTypesInfo.XpoTypeInfoSource.XPDictionary.GetClassInfo(_objectType).OwnMembers.Where(
                    info => !newProperties.Select(descriptor => descriptor.Name).Contains(info.Name)&&!info.IsCollection&&info.IsPublic).ToList();
            foreach (var memberInfo in runtimeMemberInfos) {
                var descriptor = TypeDescriptor.CreateProperty(_objectType, memberInfo.Name, memberInfo.MemberType,memberInfo.Attributes);
                newProperties.Add(descriptor);
            }
            return new PropertyDescriptorCollection(newProperties.ToArray(), true);
        }
    }
}