using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace eXpand.ExpressApp.ModelDifference
{
    public class DictionaryCombiner
    {
        private readonly ModelDifferenceObject _modelDifferenceObject;
        private Dictionary _dictionary;

        public DictionaryCombiner(Dictionary dictionary){
            _dictionary = dictionary;
        }
        public DictionaryCombiner(ModelDifferenceObject modelDifferenceObject){
            _modelDifferenceObject = modelDifferenceObject;
        }

        public DictionaryCombiner(DictionaryNode dictionaryNode){
            _dictionary=new Dictionary(dictionaryNode);
        }

        public void CombineWith(Dictionary dictionary){
            foreach (var aspect in dictionary.Aspects){
                var node = new DictionaryXmlWriter().GetAspectXml(aspect,dictionary.RootNode);
                _dictionary.AddAspect(aspect,new DictionaryXmlReader().ReadFromString(node));
            }
        }

        public void CombineWith(ModelDifferenceObject modelDifferenceObject){

            if (_dictionary== null)
                _dictionary = modelDifferenceObject.Model;
            CombineWith(modelDifferenceObject.Model);
            if (_modelDifferenceObject != null) 
                _modelDifferenceObject.Model=_dictionary;
        }

        public void CombineWith(DictionaryNode dictionaryNode){
            CombineWith(new Dictionary(dictionaryNode));
        }

    }
}
