using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.SystemModule;
using Xpand.ExpressApp.Attributes;

namespace Xpand.ExpressApp.SystemModule {
    public class NewObjectCreateGroupController : ViewController<ListView> {
        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.Disposing += FrameOnDisposing;
            Frame.GetController<NewObjectViewController>().CollectDescendantTypes += OnCollectDescendantTypes;
        }

        void FrameOnDisposing(object sender, EventArgs eventArgs) {
            Frame.GetController<NewObjectViewController>().CollectDescendantTypes -= OnCollectDescendantTypes;
        }

        void OnCollectDescendantTypes(object sender, CollectTypesEventArgs collectTypesEventArgs) {
            List<ITypeInfo> removeGroupedTypes = RemoveGroupedTypes(collectTypesEventArgs.Types).ToList();
            ChoiceActionItemCollection choiceActionItemCollection = Frame.GetController<NewObjectViewController>().NewObjectAction.Items;
            foreach (var removeGroupedType in removeGroupedTypes.ToList()) {
                string[] strings = removeGroupedType.FindAttribute<NewObjectCreateGroupAttribute>().GroupPath.Split('/');
                AddItem(choiceActionItemCollection, strings.ToList(), removeGroupedType);

            }
        }

        void AddItem(ChoiceActionItemCollection choiceActionItemCollection, List<string> strings, ITypeInfo groupedType) {
            string itemId = strings[0];
            ChoiceActionItem choiceActionItem = choiceActionItemCollection.FindItemByID(itemId);
            if (choiceActionItem == null) {
                choiceActionItem = new ChoiceActionItem(itemId, itemId, null);
                choiceActionItemCollection.Add(choiceActionItem);
                strings.RemoveAt(0);
                if (strings.Count == 0) {
                    AddItemCore(choiceActionItem, groupedType);
                    return;
                }
                AddItem(choiceActionItem.Items, strings, groupedType);
                return;
            }
            strings.RemoveAt(0);
            if (strings.Count == 0) {
                AddItemCore(choiceActionItem, groupedType);
                return;
            }

            AddItem(choiceActionItem.Items, strings, groupedType);
        }

        void AddItemCore(ChoiceActionItem choiceActionItem, ITypeInfo groupedType) {
            Type type = groupedType.Type;
            var modelClass = Application.Model.BOModel.GetClass(type);
            var actionItem = new ChoiceActionItem(type.Name, type) { Caption = modelClass.Caption,ImageName = modelClass.ImageName};
            choiceActionItem.Items.Add(actionItem);
        }

        IEnumerable<ITypeInfo> RemoveGroupedTypes(ICollection<Type> types) {
            var removeGroupedTypes = new List<ITypeInfo>();
            List<ITypeInfo> groupedInfos = types.Select(type => XafTypesInfo.Instance.FindTypeInfo(type)).Where(info => info.FindAttribute<NewObjectCreateGroupAttribute>() != null).ToList();
            foreach (var groupedInfo in groupedInfos) {
                types.Remove(groupedInfo.Type);
                removeGroupedTypes.Add(groupedInfo);
            }
            return removeGroupedTypes;
        }
    }
}