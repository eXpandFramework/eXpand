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

        public void AddAspects(Dictionary dictionary){
            if (_dictionary== null)
                _dictionary=_modelDifferenceObject.Model;
            foreach (var aspect in dictionary.Aspects){
                var node = new DictionaryXmlWriter().GetAspectXml(aspect,dictionary.RootNode);
                if (string.IsNullOrEmpty(node))
                    node = "<Application/>";
                _dictionary.AddAspect(aspect,new DictionaryXmlReader().ReadFromString(node));
            }
            if (_modelDifferenceObject!= null)
                _modelDifferenceObject.Model=_dictionary;
        }

        public void AddAspects(ModelDifferenceObject modelDifferenceObject){

            if (_dictionary== null)
                _dictionary = modelDifferenceObject.Model;
            AddAspects(modelDifferenceObject.Model);
            if (_modelDifferenceObject != null) 
                _modelDifferenceObject.Model=_dictionary;
        }

        public void AddAspects(DictionaryNode dictionaryNode){
            AddAspects(new Dictionary(dictionaryNode));
        }

    }
}
