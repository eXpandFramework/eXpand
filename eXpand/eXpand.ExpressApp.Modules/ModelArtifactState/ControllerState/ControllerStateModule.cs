using System.Linq;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.Logic;
using eXpand.ExpressApp.ModelArtifactState.ArtifactState.Logic;
using eXpand.ExpressApp.ModelArtifactState.ControllerState.Logic;

namespace eXpand.ExpressApp.ModelArtifactState.ControllerState {
    public class ControllerStateModule :ArtifactStateModule<IControllerStateRule> {
        public override string LogicRulesNodeAttributeName {
            get { return ControllerStateRulesNodeWrapper.NodeNameAttribute; }
        }

        public override string RootNodePath {
            get { return ModelArtifactStateModule.ModelArtifactStateAttributeName; }
        }
        protected override System.Collections.Generic.IEnumerable<IControllerStateRule> CollectRulesFromModelCore(LogicRulesNodeWrapper<IControllerStateRule> wrapper, DevExpress.ExpressApp.DC.ITypeInfo typeInfo) {
            var collectRulesFromModelCore = base.CollectRulesFromModelCore(wrapper, typeInfo).ToList();
            foreach (ControllerStateRule controllerStateRule in collectRulesFromModelCore) {
                var controllerType = ((IControllerStateRule)controllerStateRule).ControllerType;
                controllerStateRule.ControllerType = Application.Modules[0].ModuleManager.ControllersManager.CollectControllers(
                                        info => info.FullName == controllerType).
                                        Single().GetType();
            }

            return collectRulesFromModelCore;
        }

        protected override bool IsDefaultContext(ExecutionContext context) {
            return true;
        }
        public override Schema GetSchema() {
            Schema schema = base.GetSchema();
            DictionaryNode dictionaryNode = schema.RootNode.GetChildNode("Element", "Name",
                                                                         ModelArtifactStateModule.ModelArtifactStateAttributeName);
            DictionaryNode findChildNode = schema.RootNode.FindChildNode("Element", "Name", LogicRulesNodeAttributeName);
            schema.RootNode.RemoveChildNode(findChildNode);
            dictionaryNode.AddChildNode(findChildNode);
            return schema;
        }
        protected override void ModifySchemaAttributes(Core.DictionaryHelpers.AttibuteCreatedEventArgs args)
        {
            base.ModifySchemaAttributes(args);
            if (args.Attribute.IndexOf("ControllerType") > -1)
                args.AddTag(@"Required=""True"" RefNodeName=""/Application/ActionDesign/Controllers/*""");
        }

        public override string GetElementNodeName() {
            return ControllerStateRuleNodeWrapper.NodeNameAttribute;
        }
    }
}