using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace eXpand.ExpressApp.MemberLevelSecurity {
    

    [NonPersistent]
    public class MemberAccessPermission : PermissionBase {
        readonly List<MemberAccessPermissionItem> items = new List<MemberAccessPermissionItem>();

        public MemberAccessPermission() {
        }

        public MemberAccessPermission(Type objectType, string memberName, MemberOperation operation)
            : this(objectType, memberName, operation, ObjectAccessModifier.Allow) {
        }

        public MemberAccessPermission(Type objectType, string memberName, MemberOperation operation,
                                      ObjectAccessModifier modifier) {
            ObjectType = objectType;
            MemberName = memberName;
            Operation = operation;
            Modifier = modifier;
        }

        [TypeConverter(typeof (PermissionTargetBusinessClassListConverter))]
        public Type ObjectType {
            get { return GetDesignModeItem().ObjectType; }
            set { GetDesignModeItem().ObjectType = value; }
        }

        public string MemberName {
            get { return GetDesignModeItem().MemberName; }
            set { GetDesignModeItem().MemberName = value; }
        }

        public MemberOperation Operation {
            get { return GetDesignModeItem().Operation; }
            set { GetDesignModeItem().Operation = value; }
        }

        public ObjectAccessModifier Modifier {
            get { return GetDesignModeItem().Modifier; }
            set { GetDesignModeItem().Modifier = value; }
        }

        public string Criteria {
            get { return GetDesignModeItem().Criteria; }
            set { GetDesignModeItem().Criteria=value; }
        }

        MemberAccessPermissionItem GetDesignModeItem() {
            if (items.Count > 1) {
                throw new InvalidOperationException();
            }
            if (items.Count == 0) {
                items.Add(new MemberAccessPermissionItem());
            }
            return items[0];
        }

        IEnumerable<MemberAccessPermissionItem> CloneItems() {
            return items.Select(item => new MemberAccessPermissionItem(item)).ToList();
        }

        public override IPermission Union(IPermission target) {
            var result = (MemberAccessPermission) Copy();
            result.items.AddRange(((MemberAccessPermission) target).CloneItems());
            return result;
        }

        public override bool IsSubsetOf(IPermission target) {
            if (base.IsSubsetOf(target)) {
                foreach (MemberAccessPermissionItem targetItem in ((MemberAccessPermission) target).items) {
                    if (targetItem.ObjectType == ObjectType
                        && targetItem.MemberName == MemberName
                        && targetItem.Operation == Operation) {
                        return targetItem.Modifier == Modifier;
                    }
                }
                return true;
            }
            return false;
        }

        public override SecurityElement ToXml() {
            SecurityElement result = base.ToXml();
            var itemElement = new SecurityElement("MemberAccessPermissionItem");
            itemElement.AddAttribute("Operation", Operation.ToString());
            itemElement.AddAttribute("ObjectType", (ObjectType != null) ? ObjectType.ToString() : "");
            itemElement.AddAttribute("Modifier", Modifier.ToString());
            itemElement.AddAttribute("MemberName", MemberName+"");
            itemElement.AddAttribute("Criteria", Criteria+"");
            result.AddChild(itemElement);
            return result;
        }

        public override void FromXml(SecurityElement element) {
            items.Clear();
            if (element.Children != null) {
                if (element.Children.Count != 1) {
                    throw new InvalidOperationException();
                }
                var childElement = (SecurityElement) element.Children[0];
                ObjectType = ReflectionHelper.FindType(childElement.Attributes["ObjectType"].ToString());
                Operation =
                    (MemberOperation)
                    Enum.Parse(typeof (MemberOperation), childElement.Attributes["Operation"].ToString());
                Modifier =
                    (ObjectAccessModifier)
                    Enum.Parse(typeof (ObjectAccessModifier), childElement.Attributes["Modifier"].ToString());
                MemberName = childElement.Attributes["MemberName"].ToString();
                Criteria = childElement.Attributes["Criteria"].ToString();
            }
        }

        public override string ToString() {
            return ((ObjectType != null) ? ObjectType.Name : "N/A") + "." + MemberName + " - " + Modifier + " " +Operation;
            //return base.ToString();
        }

        public override IPermission Copy() {
            var result = new MemberAccessPermission();
            result.items.AddRange(CloneItems());
            return result;
        }
    }
}