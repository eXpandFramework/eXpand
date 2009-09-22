using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core.DictionaryHelpers;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.NodeWrappers;
using System.Linq;

namespace eXpand.ExpressApp.Core.DictionaryHelpers
{
    public class ViewIdRefNodeProvider : AttributeRefNodeProvider
    {
        private readonly string viewType;
        private string classNameAttrPath;
        private readonly string classAssociatedCollections;
        private readonly ExpressionParamsParser paramsParser;

        public ViewIdRefNodeProvider(string param) : base(param)
        {
            paramsParser = new ExpressionParamsParser(param);
            viewType = paramsParser.GetParamValue("ViewType", "Any");
            classNameAttrPath = paramsParser.GetParamValue("ClassName", "");
            classAssociatedCollections = paramsParser.GetParamValue("Relations", "All");
        }

        protected override ReadOnlyDictionaryNodeCollection GetNodesCollectionInternal(DictionaryNode node, string attributeName)
        {
            var collectionInternal = new DictionaryNodeCollection();
            if (viewType=="All")
            {
                ICollection<BaseViewInfoNodeWrapper> collections = new ApplicationNodeWrapper(node.Dictionary.RootNode).Views.Items;
                GetViews(collectionInternal, collections);
            }
            else if (viewType=="All|ListView")
            {
                IEnumerable<BaseViewInfoNodeWrapper> collections =
                    new ApplicationNodeWrapper(node.Dictionary.RootNode).Views.Items.Where(
                        wrapper => wrapper is ListViewInfoNodeWrapper);
                GetViews(collectionInternal, collections);
            }
            else if (viewType=="All|DetailView")
            {
                IEnumerable<BaseViewInfoNodeWrapper> collections =
                    new ApplicationNodeWrapper(node.Dictionary.RootNode).Views.Items.Where(
                        wrapper => wrapper is DetailViewInfoNodeWrapper);
                GetViews(collectionInternal, collections);
            }
            else if (classNameAttrPath!= null&&classAssociatedCollections=="All")
            {
                string className = paramsParser.GetAttributeValueByPath(node, classNameAttrPath);
                BOModelNodeWrapper boModelNodeWrapper = new ApplicationNodeWrapper(node.Dictionary.RootNode).BOModel;
                IEnumerable<string> associatedCollection = GetAssociatedCollections(className, boModelNodeWrapper).Select(info => info.Name);
                var classInfoNodeWrappers = boModelNodeWrapper.Classes.Where(PropertiesAre(associatedCollection));
                collectionInternal.Clear();
                foreach (var classInfoNodeWrapper in classInfoNodeWrappers)
                    foreach (var dictionaryNode in GetDictionaryNodes(associatedCollection, classInfoNodeWrapper))
                        collectionInternal.Add(dictionaryNode);
            }
            return collectionInternal;
        }

        private IEnumerable<DictionaryNode> GetDictionaryNodes(IEnumerable<string> associatedCollection, ClassInfoNodeWrapper classInfoNodeWrapper)
        {
            return classInfoNodeWrapper.AllProperties.Where(wrapper => associatedCollection.Contains(wrapper.Name))
                .Select(wrapper => wrapper.Node);
        }

        private Func<ClassInfoNodeWrapper, bool> PropertiesAre(IEnumerable<string> associatedCollection)
        {
            return wrapper => wrapper.AllProperties.Any(
                                  nodeWrapper => associatedCollection.Any(info => nodeWrapper.Name == info));
        }


        private IEnumerable<IMemberInfo> GetAssociatedCollections(string className, BOModelNodeWrapper boModelNodeWrapper)
        {
            return boModelNodeWrapper.FindClassByName(className).
                ClassTypeInfo.Members.Where(info => info.IsAssociation && info.IsList);
        }

        private void GetViews(DictionaryNodeCollection collectionInternal, IEnumerable<BaseViewInfoNodeWrapper> collections)
        {
            collectionInternal.Clear();
                
            foreach (var nodeCollection in collections)
                collectionInternal.Add(nodeCollection.Node);
        }
    }
}