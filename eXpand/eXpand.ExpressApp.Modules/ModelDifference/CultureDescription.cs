using System;
using System.Globalization;

namespace eXpand.ExpressApp.ModelDifference{
    public class CultureDescription
    {
        private readonly string name;
        private readonly string caption;
        public CultureDescription(CultureInfo culture)
        {
            name = culture.Name;
            caption = String.Concat(new[] { name, " (", culture.DisplayName, " - ", culture.NativeName, ")" });
        }
        public override string ToString()
        {
            return caption;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            var objCulture = obj as CultureDescription;
            if (objCulture != null)
            {
                return name == objCulture.Name;
            }
            return base.Equals(obj);
        }
        public string Caption
        {
            get { return caption; }
        }
        public string Name
        {
            get { return name; }
        }
    }
}