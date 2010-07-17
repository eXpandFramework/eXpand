using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.Logic.TypeConverters {
    public class StringToModelMemberConverter : TypeConverter
    {
        readonly IModelApplication _modelApplication;

        public StringToModelMemberConverter(IModelApplication modelApplication)
        {
            _modelApplication = modelApplication;
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            return _modelApplication.BOModel.GetClass(destinationType).AllMembers.Where(member => member.Name == value.ToString()).Single();
        }
    }
}