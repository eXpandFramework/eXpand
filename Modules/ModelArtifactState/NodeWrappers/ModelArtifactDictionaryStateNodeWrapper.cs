using eXpand.ExpressApp.ModelArtifactState.Interfaces;
using eXpand.ExpressApp.Security.NodeWrappers;

namespace eXpand.ExpressApp.ModelArtifactState.NodeWrappers
{
    public abstract class ModelArtifactDictionaryStateNodeWrapper : DictionaryStateNodeWrapperBase,IModuleRule
    {
        public string Module
        {
            get { return DictionaryNode.GetAttributeValue("Module"); }
            set { DictionaryNode.SetAttribute("Module", value); }
        }

    }
}