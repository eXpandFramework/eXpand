using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace eXpand.Persistent.Base.General{
    [Serializable]
    public class ResourceImpls : List<ResourceImpl>{
        public ResourceImpls(string element){
            PopulateFromXElement(XElement.Parse(element));
        }

        public ResourceImpls(IEnumerable<ResourceImpl> collection) : base(collection) {}
        public ResourceImpls() {}

        public XElement XmlElement{
            get { return CreateXElement(); }
            set { PopulateFromXElement(value); }
        }

        public XElement CreateXElement(){
            var element = new XElement("ResourceIds");
            foreach (ResourceImpl binding in this){
                element.Add(binding.ToSchedulerXmlElement());
            }
            return element;
        }

        private void PopulateFromXElement(XElement value){
            Clear();

            foreach (XElement element in value.Elements("ResourceId")){
                Add(new ResourceImpl(element));
            }
        }
    }
}