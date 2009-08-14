using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.ModelArtifactState.Attributes;
using eXpand.ExpressApp.ModelArtifactState.NodeWrappers;

namespace eXpand.ExpressApp.ModelArtifactState.Controllers{
    public partial class UpdateModelController : WindowController{
        public UpdateModelController(){
            InitializeComponent();
            RegisterActions(components);
        }

        private void CreateModelRulesFromClassAttributes(ModelArtifactStateNodeWrapper wrapper, ITypeInfo typeInfo){
            foreach (ArtifactStateRuleAttribute attribute in ArtifactStateRuleManager.FindAttributes(typeInfo)){
                wrapper.AddRule<ControllerStateRuleNodeWrapper>(attribute, typeInfo);
            }
        }

        private void CreateModelRulesFromMethodsAttributes(ModelArtifactStateNodeWrapper wrapper, ITypeInfo typeInfo){
            foreach (MethodInfo methodInfo in typeInfo.Type.GetMethods(ArtifactStateRuleManager.MethodRuleBindingFlags)){
                foreach (ArtifactStateRuleAttribute attribute in ArtifactStateRuleManager.FindAttributes(methodInfo)){
                    wrapper.AddRule<ControllerStateRuleNodeWrapper>(attribute, typeInfo);
                }
            }
        }

        public override void UpdateModel(Dictionary model){
            base.UpdateModel(model);

            ModelArtifactStateNodeWrapper wrapper = ModelArtifactStateModule.CreateModelWrapper(model);
            foreach (ITypeInfo typeInfo in XafTypesInfo.Instance.PersistentTypes){
                CreateModelRulesFromClassAttributes(wrapper, typeInfo);
                CreateModelRulesFromMethodsAttributes(wrapper, typeInfo);
            }


//            var applicatioNodeWrapper = new ApplicationNodeWrapper(model);
//            addRule<ConditionalControllerStateRuleNodeWrapper, ControllerStateRuleAttribute,ControllerStateRuleNodeWrapper>(
//                ConditionalControllerStateRuleNodeWrapper.NodeNameAttribute, applicatioNodeWrapper);
//            addRule<ConditionalActionStateRuleNodeWrapper, ActionStateRuleAttribute,ActionStateRuleNodeWrapper>(
//                ConditionalActionStateRuleNodeWrapper.NodeNameAttribute, applicatioNodeWrapper);
        }

//        private void addRule<TConditionalArtifactStateNodeWrapper, TArtifactStateRuleAttribute, TArtifactStateRuleNodeWrapper>(
//            string nodeName, ApplicationNodeWrapper applicationNodeWrapper)
//            where TConditionalArtifactStateNodeWrapper : ConditionalArtifactStateNodeWrapper
//            where TArtifactStateRuleNodeWrapper : ArtifactStateRuleNodeWrapper
//            where TArtifactStateRuleAttribute : ArtifactStateRuleAttribute
//        {
//            foreach (ClassInfoNodeWrapper clw in applicationNodeWrapper.BOModel.Classes)
//            {
//                DictionaryNode conditionalartifactStateNode = clw.Node.FindChildNode(nodeName);
//                if (conditionalartifactStateNode == null)
//                {
//                    var attributes = clw.ClassTypeInfo.FindAttributes<TArtifactStateRuleAttribute>();
//                    if (attributes.Count()>0)
//                    {
//                        conditionalartifactStateNode = clw.Node.AddChildNode(nodeName);
//                        var conditionalArtifactStateNodeWrapper =
//                            (TConditionalArtifactStateNodeWrapper)
//                            Activator.CreateInstance(typeof (TConditionalArtifactStateNodeWrapper),
//                                                     new object[] {conditionalartifactStateNode});
//                    
//                        foreach (
//                            TArtifactStateRuleAttribute attribute in
//                                attributes)
//                            conditionalArtifactStateNodeWrapper.AddRule<TArtifactStateRuleNodeWrapper>(attribute);
//                    }
//                }
//            }
//        }
    }
}