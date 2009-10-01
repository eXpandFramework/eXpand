using System;
using System.Drawing;
using System.Xml.Linq;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;

namespace eXpand.Persistent.Base.General{
    [Serializable]
    public class ResourceImpl : IResource{
        private string caption;
        private string[] captionLines;
        private int color;
        private object id;

        public ResourceImpl(XElement bindingElement){
            try{
                XAttribute attribute = bindingElement.Attribute("Value");
                if (attribute != null) Id = new Guid(attribute.Value);
            }
            catch (Exception e){
                throw new ArgumentException("Invalid resource binding element information", e);
            }
        }

        public ResourceImpl() {}

        public ResourceImpl(IResource resource){
            Id = resource.Id;
            Caption = resource.Caption;
            ResourceTypeName = ((XPCustomObject) resource).ClassInfo.FullName;
            OleColor = resource.OleColor;
        }

        public string ResourceTypeName { get; set; }

        public string[] CaptionLines{
            get { return captionLines ?? new[]{string.Empty}; }
        }

        public Color Color{
            get { return (color == 0 ? Color.White : Color.FromArgb(color)); }
            set{
                color = value.ToArgb();
                OnChanged("Color");
            }
        }
        #region IResource Members
        public object Id{
            get { return id; }
            set{
                id = value;
                OnChanged("Id");
            }
        }

        public string Caption{
            get { return caption; }
            set{
                caption = value;
                OnChanged("Caption");
            }
        }

        public Int32 OleColor{
            get { return ColorTranslator.ToOle(Color.FromArgb(color)); }
            set { Color = ColorTranslator.FromOle(value); }
        }
        #endregion
        public XElement ToSchedulerXmlElement(){
            return new XElement("ResourceId"
                                , new XAttribute("Type", typeof (Guid).FullName)
                                , new XAttribute("Value", Id)
                );
        }

        protected void OnChanged(string propertyName){
            if (propertyName == "Caption"){
                captionLines = string.IsNullOrEmpty(Caption)
                                   ? new[]{string.Empty}
                                   : Caption.Split(new[]{"<br/>"}, StringSplitOptions.RemoveEmptyEntries);
            }
        }
    }
}