using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core.DictionaryHelpers;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Core.DictionaryHelpers
{
    public class ModuleRefNodeProvider : AttributeRefNodeProvider
    {
        public ModuleRefNodeProvider(string parameter)
            : base(parameter)
        {
        }

        protected override ReadOnlyDictionaryNodeCollection GetNodesCollectionInternal(DictionaryNode node, string attributeName)
        {
            var collectionInternal = new DictionaryNodeCollection();
            collectionInternal.AddRange(new ApplicationNodeWrapper(node.Dictionary.RootNode).Node.GetChildNode(ModuleController.Modules).ChildNodes);
            return collectionInternal;


        }
    }
}
