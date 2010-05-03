using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.Attributes;
using eXpand.ExpressApp.Core.DictionaryHelpers;

namespace eXpand.ExpressApp.Win.SystemModule {
    public class WinNewObjectViewController : DevExpress.ExpressApp.Win.SystemModule.WinNewObjectViewController {
        public const string ClassTypeToInstantiate = "ClassTypeToInstantiate";
        protected override void UpdateActionState() {
            base.UpdateActionState();
            foreach (ITypeInfo typeInfo in currentViewTypes) {
                var b = typeInfo.FindAttribute<CanInstantiate>() != null;
                if (b) {
                    String diagnosticInfo;
                    if (!CanCreateAndEdit(typeInfo.Type, out diagnosticInfo) ) {
                        if (NewObjectAction.Items.Count > 0)
                            NewObjectAction.Items[0].BeginGroup = true;
                        DictionaryNode findChildNode = Application.Info.FindChildNode("CreatableItems").GetChildNode(
                            "Item", "ClassName", typeInfo.FullName);
                        if (findChildNode != null)
                            NewObjectAction.Items.Insert(0, CreateItem(typeInfo.Type, findChildNode));
                    }
                }
            }
            var type = ReflectionHelper.FindType(View.Info.GetAttributeValue(ClassTypeToInstantiate));
            if (type != null)
            {
                DictionaryNode findChildNode = Application.Info.FindChildNode("CreatableItems").GetChildNode(
                            "Item", "ClassName", type.FullName);
                if (findChildNode != null)
                {
                    if (NewObjectAction.Items.Count>0)
                        NewObjectAction.Items.RemoveAt(0);
                    NewObjectAction.Items.Insert(0, CreateItem(type, findChildNode));
                }
            }
        }
        public override Schema GetSchema()
        {
            var schema = base.GetSchema();
            var schemaBuilder = new SchemaBuilder();
            schema.CombineWith(new Schema(schemaBuilder.Inject(@"<Attribute Name=""" + ClassTypeToInstantiate + @""" RefNodeName=""/Application/BOModel/Class""/>", ModelElement.ListView)));
            schema.CombineWith(new Schema(schemaBuilder.Inject(@"<Attribute Name=""" + ClassTypeToInstantiate + @""" RefNodeName=""/Application/BOModel/Class""/>", ModelElement.DetailView)));
            return schema;
        }   

    }
}