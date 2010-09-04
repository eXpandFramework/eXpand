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


        }

    }
}