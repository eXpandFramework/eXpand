using System;
using System.Drawing;
using System.Xml.Linq;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl;

namespace eXpand.Persistent.BaseImpl
{
    [Serializable]
    public class ResourceImpl : IResource{
        private string caption;
        private string[] captionLines;
        private object id;
        private int oleColor;
        private int color;

        public ResourceImpl(XElement bindingElement){
            try{
                Id = new Guid(bindingElement.Attribute("Value").Value);
                //if ((bindingElement.Attribute("ResourceType")) != null) {
                //    ResourceTypeName = XafTypesInfo.Instance.FindTypeInfo(bindingElement.Attribute("ResourceType").Value).FullName;
                //}
            }
            catch (Exception e){
                throw new ArgumentException("Invalid resource binding element information", e);
            }
        }

        public ResourceImpl() {}

        public ResourceImpl(IResource resource){
            Id = resource.Id;
            Caption = resource.Caption;
            ResourceTypeName = ((BaseObject) resource).ClassInfo.FullName;
            OleColor = resource.OleColor;
        }

        public string ResourceTypeName { get; set; }

        public string[] CaptionLines{
            get { return captionLines ?? new[]{string.Empty}; }
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

        public Color Color {
            get { return (color == 0 ? Color.White : Color.FromArgb(color)); }
            set {
                color = value.ToArgb();
                OnChanged("Color");
            }
        }
        #endregion
        public XElement ToSchedulerXmlElement(){
            return new XElement("ResourceId"
                                , new XAttribute("Type", typeof (Guid).FullName)
                                , new XAttribute("Value", Id)
                                
                );
            //, new XAttribute("ResourceType", ResourceTypeName)
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