using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Editors;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Helpers;
using System.Linq;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class NewObjectCollectCreatableItemTypesDataSource : ViewController<DetailView> {
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            View.GetItems<LookupPropertyEditor>().Each(editor => {
                var typesDataSourceAttribute = editor.MemberInfo.FindAttribute<NewObjectCollectCreatableItemTypesDataSourceAttribute>();
                if (typesDataSourceAttribute!=null)
                    editor.Control.Popup+=(sender, args) => {
                        var types =((IEnumerable<Type>)editor.MemberInfo.Owner.FindMember(typesDataSourceAttribute.PropertyName).GetValue(editor.CurrentObject));
                        var choiceActionItemCollection = editor.Control.Frame.GetController<NewObjectViewController>().NewObjectAction.Items;
                        choiceActionItemCollection.Clear();
                        var creatableItems = ((IModelApplicationCreatableItems)Application.Model).CreatableItems.Where(item => types.Contains(item.ModelClass.TypeInfo.Type));
                        creatableItems.Each(AddItems(choiceActionItemCollection));
                    };
            });
        }

        Action<IModelCreatableItem> AddItems(ChoiceActionItemCollection choiceActionItemCollection) {
            return creatableItem => choiceActionItemCollection.Add(CreateItem(creatableItem.ModelClass.TypeInfo.Type, creatableItem));
        }

        protected ChoiceActionItem CreateItem(Type type, IModelBaseChoiceActionItem info) {
            if (info == null) {
                var choiceActionItem = new ChoiceActionItem(type.Name, type);
                IModelClass modelClass = Application.Model.BOModel.GetClass(type);
                if (modelClass != null) {
                    choiceActionItem.Caption = modelClass.Caption;
                    choiceActionItem.ImageName = modelClass.ImageName;
                }
                return choiceActionItem;
            }
            return new ChoiceActionItem(info, type);
        }
    }
}
