using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.MemberLevelSecurity {


    [NonPersistent]
    [DefaultProperty("Name")]
    public class MemberAccessPermission : PermissionBase {
        internal readonly MemberAccessItemList items = new MemberAccessItemList();


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

        public string Name {
            get { return ToString(); }
        }

        [TypeConverter(typeof(PermissionTargetBusinessClassListConverter))]
        public Type ObjectType {
            get { return GetDesignModeItem().ObjectType; }
            set { GetDesignModeItem().ObjectType = value; }
        }

        public string MemberName {
            get { return GetDesignModeItem().MemberName; }
            set { GetDesignModeItem().MemberName = value; }
        }
        [RuleValueComparison(null, DefaultContexts.Save, ValueComparisonType.NotEquals, MemberOperation.NotAssigned)]
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
            set { GetDesignModeItem().Criteria = value; }
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
            var result = (MemberAccessPermission)Copy();
            var memberAccessPermissionItems = ((MemberAccessPermission)target).CloneItems();
            foreach (var memberAccessPermissionItem in memberAccessPermissionItems) {
                result.items.Add(memberAccessPermissionItem);
            }

            return result;
        }

        public override bool IsSubsetOf(IPermission target) {
            if (base.IsSubsetOf(target)) {
                var memberAccessPermissionItem = items[0];
                foreach (MemberAccessPermissionItem targetItem in ((MemberAccessPermission)target).items) {
                    if (targetItem.ObjectType == memberAccessPermissionItem.ObjectType
                        && targetItem.MemberName == memberAccessPermissionItem.MemberName
                        && targetItem.Operation == memberAccessPermissionItem.Operation) {
                        return targetItem.Modifier == memberAccessPermissionItem.Modifier;
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
            itemElement.AddAttribute("MemberName", MemberName + "");
            itemElement.AddAttribute("Criteria", Criteria + "");
            result.AddChild(itemElement);
            return result;
        }

        public override void FromXml(SecurityElement element) {
            items.Clear();
            if (element.Children != null) {
                if (element.Children.Count != 1) {
                    throw new InvalidOperationException();
                }
                var childElement = (SecurityElement)element.Children[0];
                ObjectType = ReflectionHelper.FindType(childElement.Attributes["ObjectType"].ToString());
                Operation =
                    (MemberOperation)
                    Enum.Parse(typeof(MemberOperation), childElement.Attributes["Operation"].ToString());
                Modifier =
                    (ObjectAccessModifier)
                    Enum.Parse(typeof(ObjectAccessModifier), childElement.Attributes["Modifier"].ToString());
                MemberName = childElement.Attributes["MemberName"].ToString();
                Criteria = childElement.Attributes["Criteria"].ToString();
            }
        }

        public override string ToString() {
            var s = ((ObjectType != null) ? ObjectType.Name : "N/A") + "." + MemberName + " - " + Modifier + " " + Operation + " " + Criteria;
            return s;
        }

        public override IPermission Copy() {
            var result = new MemberAccessPermission();
            var memberAccessPermissionItems = CloneItems();
            foreach (var memberAccessPermissionItem in memberAccessPermissionItems) {
                result.items.Add(memberAccessPermissionItem);
            }
            return result;
        }


    }
    public class MemberAccessItemList : IEnumerable<MemberAccessPermissionItem> {
        internal List<MemberAccessPermissionItem> items = new List<MemberAccessPermissionItem>();
        public MemberAccessItemList() { }
        public MemberAccessItemList(MemberAccessItemList itemList) {
            Add(itemList);
        }
        public void Add(MemberAccessPermissionItem item) {
            if (item == null) {
                throw new ArgumentNullException();
            }
            items.Add(item);
        }
        public void Remove(MemberAccessPermissionItem item) {
            if (item != null)
                items.Remove(item);
        }
        public void Add(MemberAccessItemList itemList) {
            foreach (MemberAccessPermissionItem item in itemList) {
                Add(item);
            }
        }

        public MemberAccessPermissionItem FindAccessItem(Type objectType, ObjectAccess access) {
            return null;
            //            Type currentType = objectType;
            //            while (true)
            //            {
            //                bool needToCheckPersistentBaseObjectTypePermission = true;
            //                if (currentType != null && currentType.BaseType != null &&
            //                    PermissionTargetBusinessClassListConverter.PersistentBaseObjectType.IsAssignableFrom(currentType.BaseType))
            //                {
            //                    needToCheckPersistentBaseObjectTypePermission = false;
            //                }
            //                if (needToCheckPersistentBaseObjectTypePermission && PermissionTargetBusinessClassListConverter.PersistentBaseObjectType.IsAssignableFrom(currentType))
            //                {
            //                    ParticularAccessItem persistentBaseObjectItem = FindExactItem(PermissionTargetBusinessClassListConverter.PersistentBaseObjectType, access);
            //                    if (persistentBaseObjectItem != null)
            //                    {
            //                        return persistentBaseObjectItem;
            //                    }
            //                }
            //                ParticularAccessItem item = FindExactItem(currentType, access);
            //                if (item != null)
            //                {
            //                    return item;
            //                }
            //                if ((currentType != null) && currentType.IsInterface)
            //                {
            //                    currentType = typeof(object);
            //                }
            //                else
            //                {
            //                    if ((currentType == null) || (currentType == typeof(object)))
            //                    {
            //                        break;
            //                    }
            //                    currentType = currentType.BaseType;
            //                }
            //            }
            //            return null;
        }
        //        public void PackItems()
        //        {
        //            if (items.Count > 1)
        //            {
        //                var result = new ObjectAccessItemList();
        //                foreach (ParticularAccessItem item in items)
        //                {
        //                    ParticularAccessItem resultItem = result.FindExactItem(item.ObjectType, item.Access);
        //                    if (resultItem == null)
        //                    {
        //                        result.Add(item);
        //                    }
        //                    else
        //                    {
        //                        if (item.Modifier == ObjectAccessModifier.Deny)
        //                        {
        //                            resultItem.Modifier = ObjectAccessModifier.Deny;
        //                        }
        //                    }
        //                }
        //                items.Clear();
        //                foreach (ParticularAccessItem item in result.items)
        //                {
        //                    Add(item);
        //                }
        //            }
        //        }
        public bool IsEmpty {
            get { return items.Count == 0; }
        }
        public int Count {
            get { return items.Count; }
        }
        public void Clear() {
            items.Clear();
        }
        public MemberAccessPermissionItem this[int index] {
            get { return items[index]; }
        }
        IEnumerator<MemberAccessPermissionItem> IEnumerable<MemberAccessPermissionItem>.GetEnumerator() {
            return items.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return items.GetEnumerator();
        }
        public MemberAccessPermissionItem[] ToArray() {
            return items.ToArray();
        }
    }

}